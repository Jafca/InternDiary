﻿using InternDiary.Models;
using InternDiary.Models.Database;
using InternDiary.ViewModels.EntryVM;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace InternDiary.Controllers
{
    [Authorize]
    public class EntryController : BaseController
    {
        public ActionResult Index()
        {
            var entries = db.Entries.Where(e => e.AuthorId == _userId).OrderByDescending(e => e.Date).ToList();
            var skillsLearntCount = new List<int>();

            foreach (var entry in entries)
            {
                skillsLearntCount.Add(db.EntrySkills.Count(es => es.EntryId == entry.Id));
            }

            var vm = new EntryIndexViewModel
            {
                Entries = entries,
                SkillsLearntCount = skillsLearntCount
            };

            return View(vm);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            if (entry.AuthorId != _userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var skillsLearnt = string.Join(", ", db.Skills
                .Where(s => db.EntrySkills
                    .Where(e => e.EntryId == id)
                    .Select(e => e.SkillId)
                    .Contains(s.Id))
                .OrderBy(s => s.Text)
                .Select(s => s.Text).ToArray());

            var vm = new EntryDetailsViewModel
            {
                Entry = entry,
                SkillsLearnt = skillsLearnt
            };
            return View(vm);
        }

        public ActionResult Create()
        {
            var savedSkills = db.Skills
                .Where(s => s.AuthorId == _userId)
                .OrderBy(s => s.Text)
                .Select(s => new SelectListItem
                {
                    Text = s.Text,
                    Value = s.Text
                });

            var vm = new EntryCreateViewModel
            {
                Entry = new Entry { Date = DateTime.Now },
                SavedSkills = savedSkills
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EntryCreateViewModel vm)
        {
            var entries = db.Entries.Where(e => e.AuthorId == _userId).ToList();
            if (entries.Any(e => e.Date == vm.Entry.Date))
                ModelState.AddModelError("Entry.Date", "An Entry already exists for this date.");

            if (ModelState.IsValid)
            {
                vm.Entry.AuthorId = _userId;
                db.Entries.Add(vm.Entry);

                var skills = db.Skills.Where(s => s.AuthorId == _userId);
                foreach (var skillText in vm.SkillsLearntText ?? Enumerable.Empty<string>())
                {
                    var skill = skills.FirstOrDefault(s => s.Text == skillText);
                    if (skill is null)
                    {
                        skill = new Skill { Text = skillText, AuthorId = _userId };
                        db.Skills.Add(skill);
                    }

                    db.EntrySkills.Add(new EntrySkill { EntryId = vm.Entry.Id, SkillId = skill.Id });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            vm.SavedSkills = db.Skills
                .Where(s => s.AuthorId == _userId)
                .OrderBy(s => s.Text)
                .Select(s => new SelectListItem
                {
                    Text = s.Text,
                    Value = s.Text
                });
            return View(vm);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            if (entry.AuthorId != _userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var savedSkills = db.Skills
                .Where(s => s.AuthorId == _userId)
                .OrderBy(s => s.Text)
                .Select(s => new SelectListItem
                {
                    Text = s.Text,
                    Value = s.Text
                }).ToList();

            var currentEntrySkills = db.EntrySkills.Where(es => es.EntryId == entry.Id).ToList();
            foreach (var entrySkill in currentEntrySkills)
            {
                var skill = db.Skills.Find(entrySkill.SkillId);
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
            var currentEntrySkills = db.EntrySkills.Where(es => es.EntryId == vm.Entry.Id).ToList();

            var duplicates = db.Entries.Where(e => e.AuthorId == _userId && e.Date == vm.Entry.Date);
            if (duplicates.Any(e => e.Id != vm.Entry.Id))
                ModelState.AddModelError("Entry.Date", "An Entry already exists for this date.");

            if (ModelState.IsValid)
            {
                db.Entry(vm.Entry).State = EntityState.Modified;

                db.EntrySkills.RemoveRange(currentEntrySkills);

                var skills = db.Skills.Where(s => s.AuthorId == _userId);
                foreach (var skillText in vm.SkillsLearntText ?? Enumerable.Empty<string>())
                {
                    var skill = skills.FirstOrDefault(s => s.Text == skillText);
                    if (skill is null)
                    {
                        skill = new Skill { Text = skillText, AuthorId = _userId };
                        db.Skills.Add(skill);
                    }

                    db.EntrySkills.Add(new EntrySkill { EntryId = vm.Entry.Id, SkillId = skill.Id });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            vm.SavedSkills = db.Skills
                .Where(s => s.AuthorId == _userId)
                .OrderBy(s => s.Text)
                .Select(s => new SelectListItem
                {
                    Text = s.Text,
                    Value = s.Text
                }).ToList();

            foreach (var entrySkill in currentEntrySkills)
            {
                var skill = db.Skills.Find(entrySkill.SkillId);
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
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            if (entry.AuthorId != _userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var skillsLearnt = string.Join(", ", db.Skills.
                Where(s => db.EntrySkills
                    .Where(e => e.EntryId == id)
                    .Select(e => e.SkillId)
                    .Contains(s.Id))
                .OrderBy(s => s.Text)
                .Select(s => s.Text).ToArray());

            var vm = new EntryDetailsViewModel
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
            Entry entry = db.Entries.Find(id);
            db.Entries.Remove(entry);

            var currentEntrySkills = db.EntrySkills.Where(es => es.EntryId == id);
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
