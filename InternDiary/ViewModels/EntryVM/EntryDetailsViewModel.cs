﻿using InternDiary.Models.Database;
using System.ComponentModel;

namespace InternDiary.ViewModels.EntryVM
{
    public class EntryDetailsViewModel
    {
        public Entry Entry { get; set; }
        [DisplayName("Skills I learnt")]
        public string SkillsLearnt { get; set; }
    }
}