using InternDiary.Models;
using InternDiary.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternDiary.Data
{
    public class EntrySkillService : IEntrySkillService
    {
        private IRepository<EntrySkill> entrySkillRepo;

        public EntrySkillService(ApplicationDbContext db)
        {
            entrySkillRepo = new EFRepository<EntrySkill>(db);
        }

        public List<EntrySkill> GetEntrySkillsByEntryId(Guid id)
        {
            return entrySkillRepo.Get(es => es.EntryId == id, null).ToList();
        }

        public List<EntrySkill> GetEntrySkillsBySkillId(Guid id)
        {
            return entrySkillRepo.Get(es => es.SkillId == id, null).ToList();
        }

        public void AddEntrySkill(EntrySkill entrySkill)
        {
            entrySkillRepo.Add(entrySkill);
            entrySkillRepo.Save();
        }

        public void DeleteEntrySkills(List<EntrySkill> entrySkills)
        {
            foreach (var entrySkill in entrySkills)
                entrySkillRepo.Remove(entrySkill);
            entrySkillRepo.Save();
        }

        public int CountEntrySkillsByEntryId(Guid id)
        {
            return GetEntrySkillsByEntryId(id).Count;
        }

        public int CountEntrySkillsBySkillId(Guid id)
        {
            return entrySkillRepo.Get(es => es.SkillId == id, null).Count();
        }
    }
}