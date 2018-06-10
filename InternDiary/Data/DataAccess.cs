using InternDiary.Models;

namespace InternDiary.Data
{
    public class DataAccess
    {
        private readonly ApplicationDbContext db;
        public virtual IEntryService EntryService { get; set; }
        public virtual IEntrySkillService EntrySkillService { get; set; }
        public virtual ISkillService SkillService { get; set; }

        public DataAccess()
        {
            db = new ApplicationDbContext();
            EntryService = new EntryService(db);
            EntrySkillService = new EntrySkillService(db);
            SkillService = new SkillService(db);
        }

        public DataAccess(IEntryService entryService, IEntrySkillService entrySkillService, ISkillService skillService)
        {
            EntryService = entryService;
            EntrySkillService = entrySkillService;
            SkillService = skillService;
        }
    }
}