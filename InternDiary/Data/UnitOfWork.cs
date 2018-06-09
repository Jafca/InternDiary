using InternDiary.Models;
using InternDiary.Models.Database;
using System;

namespace InternDiary.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private EFRepository<Entry> _entryRepository;
        private EFRepository<Skill> _skillRepository;
        private EFRepository<EntrySkill> _entrySkillRepository;

        public IRepository<Entry> EntryRepository
        {
            get
            {
                if (_entryRepository == null)
                    _entryRepository = new EFRepository<Entry>(_db);
                return _entryRepository;
            }
        }

        public IRepository<Skill> SkillRepository
        {
            get
            {
                if (_skillRepository == null)
                    _skillRepository = new EFRepository<Skill>(_db);
                return _skillRepository;
            }
        }

        public IRepository<EntrySkill> EntrySkillRepository
        {
            get
            {
                if (_entrySkillRepository == null)
                    _entrySkillRepository = new EFRepository<EntrySkill>(_db);
                return _entrySkillRepository;
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}