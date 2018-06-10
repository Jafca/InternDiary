using InternDiary.Models;
using InternDiary.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace InternDiary.Data
{
    public class SkillService : ISkillService
    {
        private IRepository<Skill> skillRepo;
        private IRepository<EntrySkill> entrySkillRepo;

        public SkillService(ApplicationDbContext db)
        {
            skillRepo = new EFRepository<Skill>(db);
            entrySkillRepo = new EFRepository<EntrySkill>(db);
        }

        public Skill GetSkillById(Guid id)
        {
            return skillRepo.Get(s => s.Id == id, null).FirstOrDefault();
        }

        public List<Skill> GetSkillsByUserAlphabetically(string userId)
        {
            return skillRepo.Get(s => s.AuthorId == userId, s => s.Text).ToList();
        }

        public List<SelectListItem> GetSelectListOfSkillsByUserAlphabetically(string userId)
        {
            return skillRepo.Get(s => s.AuthorId == userId, s => s.Text)
                .Select(s => new SelectListItem
                {
                    Text = s.Text,
                    Value = s.Text
                })
                .ToList();
        }

        public List<Skill> GetSkillsByText(string text)
        {
            return skillRepo.Get(s => s.Text == text, null).ToList();
        }

        public string GetSkillsLearntByEntryId(Guid id)
        {
            var skillIdsFromEntrySkillId = entrySkillRepo.Get(es => es.EntryId == id, null).Select(es => es.SkillId);
            return string.Join(", ", skillRepo
                .Get(s => skillIdsFromEntrySkillId.Contains(s.Id), s => s.Text)
                .Select(s => s.Text)
                .ToArray());
        }

        public List<KeyValuePair<string, int>> GetSkillsFrequencyByUser(string userId)
        {
            var skills = GetSkillsByUserAlphabetically(userId);
            return skills.GroupJoin(entrySkillRepo.Get(null, null), s => s.Id, es => es.SkillId, (s, es) => new { s, es })
                .ToList()
                .Select(f => new KeyValuePair<string, int>(f.s.Text, f.es.Count()))
                    .OrderByDescending(f => f.Value)
                    .ThenBy(f => f.Key).ToList();
        }

        public void AddSkill(Skill skill)
        {
            skillRepo.Add(skill);
            skillRepo.Save();
        }

        public void UpdateSkill(Skill skill)
        {
            skillRepo.Alter(skill);
            skillRepo.Save();
        }

        public void DeleteSkillById(Guid id)
        {
            var skill = GetSkillById(id);
            skillRepo.Remove(skill);
            skillRepo.Save();
        }
    }
}