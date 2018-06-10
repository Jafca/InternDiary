using InternDiary.Models;
using InternDiary.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternDiary.Data
{
    public class EntryService : IEntryService
    {
        private IRepository<Entry> _entryRepo;

        public EntryService(ApplicationDbContext db)
        {
            _entryRepo = new EFRepository<Entry>(db);
        }

        public EntryService(IRepository<Entry> entryRepo)
        {
            _entryRepo = entryRepo;
        }

        public Entry GetEntryById(Guid id)
        {
            return _entryRepo.Get(e => e.Id == id, null).FirstOrDefault();
        }

        public List<Entry> GetEntriesByUser(string userId)
        {
            return _entryRepo.Get(e => e.AuthorId == userId, null).ToList();
        }

        public List<Entry> GetEntriesByUserAndDate(string userId, DateTime date)
        {
            return _entryRepo.Get(e => e.AuthorId == userId && e.Date == date, null).ToList();
        }

        public List<Entry> GetEntriesByUserOrderByDateDesc(string userId)
        {
            // OrderBy is outside of Get() because LINQ to Entities only supports casting EDM primitive or enumeration types, not DateTime
            return _entryRepo.Get(e => e.AuthorId == userId, null).OrderBy(e => e.Date).ToList();
        }

        public void AddEntry(Entry entry)
        {
            _entryRepo.Add(entry);
            _entryRepo.Save();
        }

        public void UpdateEntry(Entry entry)
        {
            _entryRepo.Alter(entry);
            _entryRepo.Save();
        }

        public void DeleteEntryById(Guid id)
        {
            var entry = GetEntryById(id);
            _entryRepo.Remove(entry);
            _entryRepo.Save();
        }
    }
}