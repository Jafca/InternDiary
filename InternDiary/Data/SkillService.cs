using InternDiary.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace InternDiary.Data
{
    public class SkillService : ISkillService
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();

        public Skill GetSkillById(Guid id)
        {
            return _unitOfWork.SkillRepository.Get(s => s.Id == id).FirstOrDefault();
        }

        public List<Skill> GetSkillsByUserAlphabetically(string userId)
        {
            return _unitOfWork.SkillRepository.Get(s => s.AuthorId == userId, s => s.Text).ToList();
        }

        public List<SelectListItem> GetSelectListOfSkillsByUserAlphabetically(string userId)
        {
            return _unitOfWork.SkillRepository.Get(s => s.AuthorId == userId, s => s.Text)
                .Select(s => new SelectListItem
                {
                    Text = s.Text,
                    Value = s.Text
                })
                .ToList();
        }

        public List<Skill> GetSkillsByText(string text)
        {
            return _unitOfWork.SkillRepository.Get(s => s.Text == text).ToList();
        }

        public string GetSkillsLearntByEntryId(Guid id)
        {
            var skillIdsFromEntrySkillId = _unitOfWork.EntrySkillRepository.Get(es => es.EntryId == id).Select(es => es.SkillId);
            return string.Join(", ", _unitOfWork.SkillRepository
                .Get(s => skillIdsFromEntrySkillId.Contains(s.Id), s => s.Text)
                .Select(s => s.Text)
                .ToArray());
        }

        public List<KeyValuePair<string, int>> GetSkillsFrequencyByUser(string userId)
        {
            var skills = GetSkillsByUserAlphabetically(userId);
            return skills.GroupJoin(_unitOfWork.EntrySkillRepository.GetAll(), s => s.Id, es => es.SkillId, (s, es) => new { s, es })
                .ToList()
                .Select(f => new KeyValuePair<string, int>(f.s.Text, f.es.Count()))
                    .OrderByDescending(f => f.Value)
                    .ThenBy(f => f.Key).ToList();
        }

        public void AddSkill(Skill skill)
        {
            _unitOfWork.SkillRepository.Add(skill);
            _unitOfWork.Save();
        }

        public void UpdateSkill(Skill skill)
        {
            _unitOfWork.SkillRepository.Alter(skill);
            _unitOfWork.Save();
        }

        public void DeleteSkillById(Guid id)
        {
            var skill = GetSkillById(id);
            _unitOfWork.SkillRepository.Remove(skill);
            _unitOfWork.Save();
        }
    }
}