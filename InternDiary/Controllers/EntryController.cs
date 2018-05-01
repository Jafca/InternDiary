using InternDiary.Models;
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
    public class EntryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEnumerable<SelectListItem> _savedSkills;
        private string _userId;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _userId = User.Identity.GetUserId();
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            var entries = db.Entries.Where(e => e.AuthorId == _userId).ToList();
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
                        .Contains(s.Id)
                ).Select(s => s.Text).ToArray());

            var vm = new EntryDetailsViewModel
            {
                Entry = entry,
                SkillsLearnt = skillsLearnt
            };
            return View(vm);
        }

        public ActionResult Create()
        {
            _savedSkills = db.Skills
                    .Where(s => s.AuthorId == _userId)
                    .Select(a => new SelectListItem
                    {
                        Text = a.Text,
                        Value = a.Id.ToString()
                    });

            var vm = new EntryCreateViewModel
            {
                SavedSkills = _savedSkills
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EntryCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.Entry.AuthorId = _userId;
                db.Entries.Add(vm.Entry);

                foreach (var skillId in vm.SkillsLearntIds ?? Enumerable.Empty<Guid>())
                {
                    db.EntrySkills.Add(new EntrySkill { EntryId = vm.Entry.Id, SkillId = skillId });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            vm.SavedSkills = _savedSkills;
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

            _savedSkills = db.Skills
                    .Where(s => s.AuthorId == _userId)
                    .Select(a => new SelectListItem
                    {
                        Text = a.Text,
                        Value = a.Id.ToString()
                    }).ToList();

            var currentEntrySkills = db.EntrySkills.Where(es => es.EntryId == entry.Id);
            foreach (var entrySkill in currentEntrySkills)
            {
                _savedSkills.FirstOrDefault(s => s.Value == entrySkill.SkillId.ToString()).Selected = true;
            }

            var vm = new EntryCreateViewModel
            {
                Entry = entry,
                SavedSkills = _savedSkills
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EntryCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vm.Entry).State = EntityState.Modified;

                var currentEntrySkills = db.EntrySkills.Where(es => es.EntryId == vm.Entry.Id);
                db.EntrySkills.RemoveRange(currentEntrySkills);

                foreach (var skillId in vm.SkillsLearntIds ?? Enumerable.Empty<Guid>())
                {
                    db.EntrySkills.Add(new EntrySkill { EntryId = vm.Entry.Id, SkillId = skillId });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            vm.SavedSkills = _savedSkills;
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
                        .Contains(s.Id)
                ).Select(s => s.Text).ToArray());

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
