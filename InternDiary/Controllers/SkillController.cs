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
        private IEntrySkillService _entrySkillService = new EntrySkillService();
        private ISkillService _skillService = new SkillService();

        public ActionResult Index()
        {
            var skillsFrequency = new List<int>();
            var skills = _skillService.GetSkillsByUserAlphabetically(_userId);
            foreach (var skill in skills)
            {
                skillsFrequency.Add(_entrySkillService.CountEntrySkillsBySkillId(skill.Id));
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
            if (_skillService.GetSkillsByText(skill.Text).Any())
                ModelState.AddModelError("Text", "A skill with that name already exists");

            if (ModelState.IsValid)
            {
                skill.AuthorId = _userId;
                _skillService.AddSkill(skill);
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
            var skill = _skillService.GetSkillById(id.Value);
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
            var duplicates = _skillService.GetSkillsByText(skill.Text);
            if (duplicates.Any(s => s.Id != skill.Id))
                ModelState.AddModelError("Text", "A skill with that name already exists");

            if (ModelState.IsValid)
            {
                _skillService.UpdateSkill(skill);
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
            var skill = _skillService.GetSkillById(id.Value);
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
            var currentEntrySkills = _entrySkillService.GetEntrySkillsBySkillId(id);
            _entrySkillService.DeleteEntrySkills(currentEntrySkills);

            _skillService.DeleteSkillById(id);

            return RedirectToAction("Index");
        }
    }
}
