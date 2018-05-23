using System;
using System.IO;
using System.Linq;
using Xceed.Words.NET;
using Word = Microsoft.Office.Interop.Word;

namespace InternDiary.Controllers
{
    public class FileController : BaseController
    {
        public void EntriesToDocx()
        {
            var filepath = Server.MapPath("/UserFiles/" + _userId + ".docx");
            WriteEntriesToDocx(filepath);

            SendAndDeleteFile(filepath, ".docx");
        }

        public void EntriesToPdf()
        {
            var filepath = Server.MapPath("/UserFiles/" + _userId + ".docx");
            WriteEntriesToDocx(filepath);

            var pdfFilePath = Server.MapPath("/UserFiles/" + _userId + ".pdf");
            Convert(filepath, pdfFilePath, Word.WdSaveFormat.wdFormatPDF);

            SendAndDeleteFile(pdfFilePath, ".pdf");
        }

        public void WriteEntriesToDocx(string filepath, bool includeSkills = true)
        {
            var doc = DocX.Create(filepath);
            doc.InsertParagraph("Intern Diary\n", false, new Formatting
            {
                Bold = true,
                Size = 20,
                UnderlineStyle = UnderlineStyle.singleLine
            }).Alignment = Alignment.center;

            var entries = db.Entries.Where(e => e.AuthorId == _userId).OrderBy(e => e.Date).ToList();
            foreach (var entry in entries)
            {
                doc.InsertParagraph(entry.Date.ToShortDateString()).Alignment = Alignment.right;

                var rating = String.Join(" ", (new String('★', entry.Rating) + new String('☆', 5 - entry.Rating)).ToCharArray());
                doc.InsertParagraph($"{entry.Title}\t{rating}", false, new Formatting
                {
                    Bold = true
                });

                if (includeSkills)
                {
                    var skillsLearnt = string.Join(", ", db.Skills
                        .Where(s => db.EntrySkills
                                .Where(e => e.EntryId == entry.Id)
                                .Select(e => e.SkillId)
                                .Contains(s.Id))
                        .OrderBy(s => s.Text)
                        .Select(s => s.Text).ToArray());

                    if (string.IsNullOrEmpty(skillsLearnt))
                        skillsLearnt = "None";

                    doc.InsertParagraph($"Skills I learnt: {skillsLearnt}");
                }

                doc.InsertParagraph($"{entry.Content}\n");
            }


            if (includeSkills)
            {
                var skillsFreq = db.Skills
                    .Where(s => s.AuthorId == _userId)
                    .GroupJoin(db.EntrySkills, s => s.Id, es => es.SkillId, (s, es) => new { s, es })
                    .Select(f => new { skillText = f.s.Text, skillCount = f.es.Count() })
                    .OrderByDescending(f => f.skillCount)
                    .ThenBy(f => f.skillText).ToList();

                var t = doc.AddTable(skillsFreq.Count + 1, 2);
                t.Alignment = Alignment.center;
                t.Design = TableDesign.ColorfulList;
                t.Rows[0].Cells[0].Paragraphs.First().Append("Skill Text");
                t.Rows[0].Cells[1].Paragraphs.First().Append("Frequency");

                for (int i = 0; i < skillsFreq.Count; i++)
                {
                    t.Rows[i + 1].Cells[0].Paragraphs.First().Append(skillsFreq[i].skillText);
                    t.Rows[i + 1].Cells[1].Paragraphs.First().Append(skillsFreq[i].skillCount.ToString());
                }
                doc.InsertTable(t);
            }

            doc.Save();
        }

        /// <summary>
        /// Source: http://cathalscorner.blogspot.co.uk/2009/10/converting-docx-into-doc-pdf-html.html
        /// Please note: You must have the Microsoft Office 2007 Add-in: Microsoft Save as PDF or XPS installed
        /// http://www.microsoft.com/downloads/details.aspx?FamilyId=4D951911-3E7E-4AE6-B059-A2E79ED87041&displaylang=en
        /// </summary>
        public static void Convert(string input, string output, Word.WdSaveFormat format)
        {
            Word._Application oWord = new Word.Application
            {
                Visible = false // Make this instance of word invisible (Can still see it in the taskmgr).
            };

            // Interop requires objects.
            object oMissing = System.Reflection.Missing.Value;
            object isVisible = true;
            object readOnly = false;
            object oInput = input;
            object oOutput = output;
            object oFormat = format;

            // Load a document into our instance of word.exe
            Word._Document oDoc = oWord.Documents.Open(ref oInput, ref oMissing, ref readOnly, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref isVisible, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            // Make this document the active document.
            oDoc.Activate();

            // Save this document in Word 2003 format.
            oDoc.SaveAs(ref oOutput, ref oFormat, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            oDoc.Close();

            // Always close Word.exe.
            oWord.Quit(ref oMissing, ref oMissing, ref oMissing);

            System.IO.File.Delete(input);
        }

        public void SendAndDeleteFile(string filepath, string extension)
        {
            FileInfo file = new FileInfo(filepath);
            if (file.Exists)
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + $"InternDiary{extension}");
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.Flush();
                Response.TransmitFile(file.FullName);
                Response.End();
            }

            System.IO.File.Delete(filepath);
        }
    }
}