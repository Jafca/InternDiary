using System;
using System.ComponentModel.DataAnnotations;

namespace InternDiary.Models.Database
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}