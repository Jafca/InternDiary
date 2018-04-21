using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternDiary.Models.Database
{
    [Table("EntrySkills")]
    public class EntrySkill : BaseModel
    {
        public Guid EntryId { get; set; }

        public Guid SkillId { get; set; }

        [ForeignKey("EntryId")]
        public virtual Entry Entry { get; set; }

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; }
    }
}