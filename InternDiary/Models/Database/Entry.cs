using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternDiary.Models.Database
{
    [Table("Entries")]
    public class Entry : BaseModel
    {
        public DateTime Date { get; set; }

        public string Title { get; set; }

        public int Rating { get; set; }

        public string Content { get; set; }
    }
}