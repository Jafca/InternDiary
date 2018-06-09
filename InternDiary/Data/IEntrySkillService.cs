using InternDiary.Models.Database;
using System;
using System.Collections.Generic;

namespace InternDiary.Data
{
    public interface IEntrySkillService
    {
        List<EntrySkill> GetEntrySkillsByEntryId(Guid id);
        List<EntrySkill> GetEntrySkillsBySkillId(Guid id);
        void AddEntrySkill(EntrySkill entrySkill);
        void DeleteEntrySkills(List<EntrySkill> entrySkills);
        int CountEntrySkillsByEntryId(Guid id);
        int CountEntrySkillsBySkillId(Guid id);
    }
}