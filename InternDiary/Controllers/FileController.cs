using InternDiary.ViewModels.EntryVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xceed.Words.NET;

namespace InternDiary.Controllers
{
    public class FileController : BaseController
    {
        public void EntriesToDocx()
        {
            var filepath = Server.MapPath("/UserFiles/" + _userId + ".docx");
            WriteEntriesToDocx(filepath);

            FileInfo file = new FileInfo(filepath);
            if (file.Exists)
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "InternDiary.docx");
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.Flush();
                Response.TransmitFile(file.FullName);
                Response.End();
            }

            System.IO.File.Delete(filepath);
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
                                .Contains(s.Id)
                        ).Select(s => s.Text).ToArray());

                    if (string.IsNullOrEmpty(skillsLearnt))
                        skillsLearnt = "None";

                    doc.InsertParagraph($"Skills I learnt: {skillsLearnt}");
                }

                doc.InsertParagraph($"{entry.Content}\n");
            }

            doc.Save();
        }
    }
}