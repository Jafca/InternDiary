﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternDiary.Models.Database
{
    [Table("Entries")]
    public class Entry : BaseModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string Title { get; set; }

        [Range(1,5)]
        public int Rating { get; set; }

        public string Content { get; set; }

        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}