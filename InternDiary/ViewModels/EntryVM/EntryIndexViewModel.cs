using InternDiary.Models.Database;
using System.Collections.Generic;
using System.ComponentModel;

namespace InternDiary.ViewModels.EntryVM
{
    public class EntryIndexViewModel
    {
        public List<Entry> Entries { get; set; }
        [DisplayName("No. of Skills learnt")]
        public List<int> SkillsLearntCount { get; set; }
    }
}