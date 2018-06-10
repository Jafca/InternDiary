using InternDiary.Data;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Xceed.Words.NET;
using Word = Microsoft.Office.Interop.Word;

namespace InternDiary.Controllers
{
    public class FileController : BaseController
    {
        private DataAccess _data = new DataAccess();

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

        public void WriteEntriesToDocx(string filepath, bool includeSkillsTable = true)
        {
            var doc = DocX.Create(filepath);
            doc.InsertParagraph("Intern Diary\n", false, new Formatting
            {
                Bold = true,
                Size = 20,
                UnderlineStyle = UnderlineStyle.singleLine
            }).Alignment = Alignment.center;

            var entries = _data.EntryService.GetEntriesByUserOrderByDateDesc(_userId);

            var headerFormatting = new Formatting { FontColor = Color.White, Bold = true };
            var borderColour = new Border { Color = Color.LightSkyBlue };

            var entryTable = doc.AddTable(entries.Count + 1, 3);
            entryTable.Alignment = Alignment.center;
            entryTable.Design = TableDesign.TableNormal;
            entryTable.SetWidths(new float[] { 50, 450, 100 });

            entryTable.Rows[0].Cells[0].Paragraphs.First().Append("Date", headerFormatting);
            entryTable.Rows[0].Cells[1].Paragraphs.First().Append("Entry", headerFormatting);
            entryTable.Rows[0].Cells[2].Paragraphs.First().Append("Skills I learnt", headerFormatting);

            entryTable.Rows[0].Cells[0].Shading = Color.LightSkyBlue;
            entryTable.Rows[0].Cells[1].Shading = Color.LightSkyBlue;
            entryTable.Rows[0].Cells[2].Shading = Color.LightSkyBlue;

            entryTable.Rows[0].Cells.First().SetBorder(TableCellBorderType.Left, borderColour);
            entryTable.Rows[0].Cells.Last().SetBorder(TableCellBorderType.Right, borderColour);

            for (int i = 0; i < entries.Count; i++)
            {
                var rating = String.Join(" ", (new String('★', entries[i].Rating) + new String('☆', 5 - entries[i].Rating)).ToCharArray());
                entryTable.Rows[i + 1].Cells[0].Paragraphs.First().Append($"{entries[i].Date:dd/MM/yyyy}\n{rating}");
                entryTable.Rows[i + 1].Cells[0].SetBorder(TableCellBorderType.Bottom, borderColour);

                entryTable.Rows[i + 1].Cells[1].Paragraphs.First().Append(entries[i].Title).Bold().Append($"\n{entries[i].Content}");
                entryTable.Rows[i + 1].Cells[1].SetBorder(TableCellBorderType.Bottom, borderColour);

                var id = entries[i].Id;
                var skillsLearnt = _data.SkillService.GetSkillsLearntByEntryId(id);

                if (string.IsNullOrEmpty(skillsLearnt))
                    skillsLearnt = "None";

                entryTable.Rows[i + 1].Cells[2].Paragraphs.First().Append(skillsLearnt);
                entryTable.Rows[i + 1].Cells[2].SetBorder(TableCellBorderType.Bottom, borderColour);

                entryTable.Rows[i + 1].Cells.First().SetBorder(TableCellBorderType.Left, borderColour);
                entryTable.Rows[i + 1].Cells.Last().SetBorder(TableCellBorderType.Right, borderColour);
            }
            doc.InsertTable(entryTable);

            doc.InsertParagraph();

            if (includeSkillsTable)
            {
                var skillsFreq = _data.SkillService.GetSkillsFrequencyByUser(_userId);

                var t = doc.AddTable(skillsFreq.Count + 1, 2);
                t.Alignment = Alignment.center;
                t.Design = TableDesign.TableNormal;

                t.Rows[0].Cells[0].Paragraphs.First().Append("Skills", headerFormatting);
                t.Rows[0].Cells[1].Paragraphs.First().Append("Frequency", headerFormatting);

                t.Rows[0].Cells[0].Shading = Color.LightSkyBlue;
                t.Rows[0].Cells[1].Shading = Color.LightSkyBlue;

                t.Rows[0].Cells.First().SetBorder(TableCellBorderType.Left, borderColour);
                t.Rows[0].Cells.Last().SetBorder(TableCellBorderType.Right, borderColour);

                for (int i = 0; i < skillsFreq.Count; i++)
                {
                    t.Rows[i + 1].Cells[0].Paragraphs.First().Append(skillsFreq[i].Key);
                    t.Rows[i + 1].Cells[0].SetBorder(TableCellBorderType.Bottom, borderColour);

                    t.Rows[i + 1].Cells[1].Paragraphs.First().Append(skillsFreq[i].Value.ToString());
                    t.Rows[i + 1].Cells[1].SetBorder(TableCellBorderType.Bottom, borderColour);

                    t.Rows[i + 1].Cells.First().SetBorder(TableCellBorderType.Left, borderColour);
                    t.Rows[i + 1].Cells.Last().SetBorder(TableCellBorderType.Right, borderColour);
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