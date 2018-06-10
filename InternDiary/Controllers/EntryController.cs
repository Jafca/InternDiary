using InternDiary.Data;
using InternDiary.Models;
using InternDiary.Models.Database;
using InternDiary.ViewModels.EntryVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace InternDiary.Controllers
{
    [Authorize]
    public class EntryController : BaseController
    {
        private DataAccess _data;

        public EntryController()
        {
            _data = new DataAccess();
        }

        public EntryController(DataAccess dataAccess)
        {
            _data = dataAccess;
        }

        public ActionResult Create(string date)
        {
            var entryDate = DateTime.TryParse(date, out DateTime parsedDate) ? parsedDate : DateTime.Now.Date;
            var entries = _data.EntryService.GetEntriesByUser(_userId);

            var existingEntry = entries.FirstOrDefault(e => e.Date == entryDate);
            if (existingEntry != null)
                return RedirectToAction("Edit", new { id = existingEntry.Id });

            var savedSkills = _data.SkillService.GetSelectListOfSkillsByUserAlphabetically(_userId);
            var vm = new EntryCreateViewModel
            {
                Entry = new Entry { Date = entryDate },
                SavedSkills = savedSkills
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EntryCreateViewModel vm)
        {
            var entries = _data.EntryService.GetEntriesByUser(_userId);
            if (entries.Any(e => e.Date == vm.Entry.Date))
                ModelState.AddModelError("Entry.Date", "An Entry already exists for this date.");

            if (ModelState.IsValid)
            {
                vm.Entry.AuthorId = _userId;
                _data.EntryService.AddEntry(vm.Entry);

                var skills = _data.SkillService.GetSkillsByUserAlphabetically(_userId);
                foreach (var skillText in vm.SkillsLearntText ?? Enumerable.Empty<string>())
                {
                    var skill = skills.FirstOrDefault(s => s.Text == skillText);
                    if (skill is null)
                    {
                        skill = new Skill { Text = skillText, AuthorId = _userId };
                        _data.SkillService.AddSkill(skill);
                    }

                    _data.EntrySkillService.AddEntrySkill(new EntrySkill { EntryId = vm.Entry.Id, SkillId = skill.Id });
                }

                return RedirectToAction("Index", "Home");
            }

            vm.SavedSkills = _data.SkillService.GetSelectListOfSkillsByUserAlphabetically(_userId);
            return View(vm);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entry = _data.EntryService.GetEntryById(id.Value);
            if (entry == null)
            {
                return HttpNotFound();
            }
            if (entry.AuthorId != _userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var savedSkills = _data.SkillService.GetSelectListOfSkillsByUserAlphabetically(_userId);

            var currentEntrySkills = _data.EntrySkillService.GetEntrySkillsByEntryId(entry.Id);
            foreach (var entrySkill in currentEntrySkills)
            {
                var skill = _data.SkillService.GetSkillById(entrySkill.SkillId);
                savedSkills.FirstOrDefault(s => s.Value == skill.Text).Selected = true;
            }

            var vm = new EntryCreateViewModel
            {
                Entry = entry,
                SavedSkills = savedSkills
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EntryCreateViewModel vm)
        {
            var currentEntrySkills = _data.EntrySkillService.GetEntrySkillsByEntryId(vm.Entry.Id);

            var duplicates = _data.EntryService.GetEntriesByUserAndDate(_userId, vm.Entry.Date);
            if (duplicates.Any(e => e.Id != vm.Entry.Id))
                ModelState.AddModelError("Entry.Date", "An Entry already exists for this date.");

            if (ModelState.IsValid)
            {
                _data.EntryService.UpdateEntry(vm.Entry);

                _data.EntrySkillService.DeleteEntrySkills(currentEntrySkills);

                var skills = _data.SkillService.GetSkillsByUserAlphabetically(_userId);
                foreach (var skillText in vm.SkillsLearntText ?? Enumerable.Empty<string>())
                {
                    var skill = skills.FirstOrDefault(s => s.Text == skillText);
                    if (skill is null)
                    {
                        skill = new Skill { Text = skillText, AuthorId = _userId };
                        _data.SkillService.AddSkill(skill);
                    }

                    _data.EntrySkillService.AddEntrySkill(new EntrySkill { EntryId = vm.Entry.Id, SkillId = skill.Id });
                }

                return RedirectToAction("Index", "Home");
            }

            vm.SavedSkills = _data.SkillService.GetSelectListOfSkillsByUserAlphabetically(_userId);

            foreach (var entrySkill in currentEntrySkills)
            {
                var skill = _data.SkillService.GetSkillById(entrySkill.SkillId);
                vm.SavedSkills.FirstOrDefault(s => s.Value == skill.Text).Selected = true;
            }
            return View(vm);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entry = _data.EntryService.GetEntryById(id.Value);
            if (entry == null)
            {
                return HttpNotFound();
            }
            if (entry.AuthorId != _userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var skillsLearnt = _data.SkillService.GetSkillsLearntByEntryId(id.Value);

            var vm = new EntryDeleteViewModel
            {
                Entry = entry,
                SkillsLearnt = skillsLearnt
            };
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var currentEntrySkills = _data.EntrySkillService.GetEntrySkillsByEntryId(id);
            _data.EntrySkillService.DeleteEntrySkills(currentEntrySkills);

            _data.EntryService.DeleteEntryById(id);

            return RedirectToAction("Index", "Home");
        }

        public JsonResult GetCalendarEntries()
        {
            var entries = _data.EntryService.GetEntriesByUser(_userId);

            var events = new List<FullCalendarEvent>();
            foreach (var entry in entries)
            {
                var skillsLearntCount = _data.EntrySkillService.CountEntrySkillsByEntryId(entry.Id);

                var ratingColor = "BLACK";
                switch (entry.Rating)
                {
                    case 1:
                        ratingColor = "CRIMSON";
                        break;
                    case 2:
                        ratingColor = "DARKORANGE";
                        break;
                    case 3:
                        ratingColor = "GOLD";
                        break;
                    case 4:
                        ratingColor = "STEELBLUE";
                        break;
                    case 5:
                        ratingColor = "LIMEGREEN";
                        break;
                }

                events.Add(new FullCalendarEvent
                {
                    title = $"{entry.Title}\n[Skills Count: {skillsLearntCount}]",
                    start = entry.Date.AddDays(1),
                    url = $"/Entry/Edit/{entry.Id}",
                    color = ratingColor,
                    textColor = "WHITE"
                });
            }
            return Json(events);
        }
    }
}
