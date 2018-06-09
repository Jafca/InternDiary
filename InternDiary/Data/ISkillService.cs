using InternDiary.Models.Database;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InternDiary.Data
{
    public interface ISkillService
    {
        Skill GetSkillById(Guid id);
        List<Skill> GetSkillsByUserAlphabetically(string userId);
        List<SelectListItem> GetSelectListOfSkillsByUserAlphabetically(string userId);
        List<Skill> GetSkillsByText(string text);
        string GetSkillsLearntByEntryId(Guid id);
        List<KeyValuePair<string, int>> GetSkillsFrequencyByUser(string userId);
        void AddSkill(Skill skill);
        void UpdateSkill(Skill skill);
        void DeleteSkillById(Guid id);
    }
}