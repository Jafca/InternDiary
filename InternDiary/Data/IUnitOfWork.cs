using InternDiary.Models.Database;

namespace InternDiary.Data
{
    public interface IUnitOfWork
    {
        IRepository<Entry> EntryRepository { get; }
        IRepository<Skill> SkillRepository { get; }
        IRepository<EntrySkill> EntrySkillRepository { get; }

        void Save();
        void Dispose();
    }
}