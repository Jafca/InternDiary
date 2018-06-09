using InternDiary.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternDiary.Data
{
    public class EntryService : IEntryService
    {
        private IUnitOfWork _unitOfWork = new UnitOfWork();

        public Entry GetEntryById(Guid id)
        {
            return _unitOfWork.EntryRepository.Get(e => e.Id == id).FirstOrDefault();
        }

        public List<Entry> GetEntriesByUser(string userId)
        {
            return _unitOfWork.EntryRepository.Get(e => e.AuthorId == userId).ToList();
        }

        public List<Entry> GetEntriesByUserAndDate(string userId, DateTime date)
        {
            return _unitOfWork.EntryRepository.Get(e => e.AuthorId == userId && e.Date == date).ToList();
        }

        public List<Entry> GetEntriesByUserOrderByDateDesc(string userId)
        {
            // OrderBy is outside of Get() because LINQ to Entities only supports casting EDM primitive or enumeration types, not DateTime
            return _unitOfWork.EntryRepository.Get(e => e.AuthorId == userId).OrderBy(e => e.Date).ToList();
        }

        public void AddEntry(Entry entry)
        {
            _unitOfWork.EntryRepository.Add(entry);
            _unitOfWork.Save();
        }

        public void UpdateEntry(Entry entry)
        {
            _unitOfWork.EntryRepository.Alter(entry);
            _unitOfWork.Save();
        }

        public void DeleteEntryById(Guid id)
        {
            var entry = GetEntryById(id);
            _unitOfWork.EntryRepository.Remove(entry);
            _unitOfWork.Save();
        }
    }
}