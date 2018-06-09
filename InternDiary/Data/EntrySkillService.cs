using InternDiary.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternDiary.Data
{
    public class EntrySkillService : IEntrySkillService
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();

        public List<EntrySkill> GetEntrySkillsByEntryId(Guid id)
        {
            return _unitOfWork.EntrySkillRepository.Get(es => es.EntryId == id).ToList();
        }

        public List<EntrySkill> GetEntrySkillsBySkillId(Guid id)
        {
            return _unitOfWork.EntrySkillRepository.Get(es => es.SkillId == id).ToList();
        }

        public void AddEntrySkill(EntrySkill entrySkill)
        {
            _unitOfWork.EntrySkillRepository.Add(entrySkill);
            _unitOfWork.Save();
        }

        public void DeleteEntrySkills(List<EntrySkill> entrySkills)
        {
            foreach (var entrySkill in entrySkills)
                _unitOfWork.EntrySkillRepository.Remove(entrySkill);
            _unitOfWork.Save();
        }

        public int CountEntrySkillsByEntryId(Guid id)
        {
            return GetEntrySkillsByEntryId(id).Count;
        }

        public int CountEntrySkillsBySkillId(Guid id)
        {
            return _unitOfWork.EntrySkillRepository.Get(es => es.SkillId == id).Count();
        }
    }
}