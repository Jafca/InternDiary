using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InternDiary.Models;
using InternDiary.Models.Database;
using InternDiary.ViewModels.SkillVM;
using Microsoft.AspNet.Identity;

namespace InternDiary.Controllers
{
    [Authorize]
    public class SkillController : BaseController
    {
        public ActionResult Index()
        {
            var skills = db.Skills.Where(s => s.AuthorId == _userId).OrderBy(s => s.Text).ToList();
            var skillsFrequency = new List<int>();

            foreach (var skill in skills)
            {
                skillsFrequency.Add(db.EntrySkills.Count(es => es.SkillId == skill.Id));
            }

            var vm = new SkillIndexViewModel
            {
                Skills = skills,
                SkillsFrequency = skillsFrequency
            };

            return View(vm);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,AuthorId")] Skill skill)
        {
            if (db.Skills.Any(s => s.Text == skill.Text))
                ModelState.AddModelError("Text", "A skill with that name already exists");

            if (ModelState.IsValid)
            {
                skill.AuthorId = _userId;
                db.Skills.Add(skill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(skill);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            if (skill.AuthorId != _userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,AuthorId")] Skill skill)
        {
            var duplicates = db.Skills.Where(s => s.Text == skill.Text);
            if (duplicates.Any(s => s.Id != skill.Id))
                ModelState.AddModelError("Text", "A skill with that name already exists");

            if (ModelState.IsValid)
            {
                db.Entry(skill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skill);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skill skill = db.Skills.Find(id);
            if (skill == null)
            {
                return HttpNotFound();
            }
            if (skill.AuthorId != _userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(skill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Skill skill = db.Skills.Find(id);
            db.Skills.Remove(skill);

            var currentEntrySkills = db.EntrySkills.Where(es => es.SkillId == id);
            db.EntrySkills.RemoveRange(currentEntrySkills);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
