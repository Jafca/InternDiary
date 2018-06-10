using InternDiary.Data;
using InternDiary.Models.Database;
using InternDiary.ViewModels.SkillVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace InternDiary.Controllers
{
    [Authorize]
    public class SkillController : BaseController
    {
        private DataAccess _data;

        public SkillController()
        {
            _data = new DataAccess();
        }

        public SkillController(DataAccess dataAccess)
        {
            _data = dataAccess;
        }

        public ActionResult Index()
        {
            var skillsFrequency = new List<int>();
            var skills = _data.SkillService.GetSkillsByUserAlphabetically(_userId);
            foreach (var skill in skills)
            {
                skillsFrequency.Add(_data.EntrySkillService.CountEntrySkillsBySkillId(skill.Id));
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
            if (_data.SkillService.GetSkillsByText(skill.Text).Any())
                ModelState.AddModelError("Text", "A skill with that name already exists");

            if (ModelState.IsValid)
            {
                skill.AuthorId = _userId;
                _data.SkillService.AddSkill(skill);
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
            var skill = _data.SkillService.GetSkillById(id.Value);
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
            var duplicates = _data.SkillService.GetSkillsByText(skill.Text);
            if (duplicates.Any(s => s.Id != skill.Id))
                ModelState.AddModelError("Text", "A skill with that name already exists");

            if (ModelState.IsValid)
            {
                _data.SkillService.UpdateSkill(skill);
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
            var skill = _data.SkillService.GetSkillById(id.Value);
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
            var currentEntrySkills = _data.EntrySkillService.GetEntrySkillsBySkillId(id);
            _data.EntrySkillService.DeleteEntrySkills(currentEntrySkills);

            _data.SkillService.DeleteSkillById(id);

            return RedirectToAction("Index");
        }
    }
}
