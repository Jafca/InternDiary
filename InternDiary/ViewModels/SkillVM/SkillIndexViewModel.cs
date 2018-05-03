using InternDiary.Models.Database;
using System.Collections.Generic;
using System.ComponentModel;

namespace InternDiary.ViewModels.SkillVM
{
    public class SkillIndexViewModel
    {
        public List<Skill> Skills { get; set; }
        [DisplayName("No. of Times Learnt")]
        public List<int> SkillsFrequency { get; set; }
    }
}