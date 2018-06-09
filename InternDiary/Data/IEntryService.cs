using InternDiary.Models.Database;
using System;
using System.Collections.Generic;

namespace InternDiary.Data
{
    public interface IEntryService
    {
        Entry GetEntryById(Guid id);
        List<Entry> GetEntriesByUser(string userId);
        List<Entry> GetEntriesByUserAndDate(string userId, DateTime date);
        List<Entry> GetEntriesByUserOrderByDateDesc(string userId);
        void AddEntry(Entry entry);
        void UpdateEntry(Entry entry);
        void DeleteEntryById(Guid id);
    }
}