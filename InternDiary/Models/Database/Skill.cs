using System.ComponentModel.DataAnnotations.Schema;

namespace InternDiary.Models.Database
{
    [Table("Skills")]
    public class Skill : BaseModel
    {
        public string Text { get; set; }

        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}