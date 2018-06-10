using InternDiary.Models.Database;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace InternDiary.ViewModels.EntryVM
{
    public class EntryCreateViewModel
    {
        public Entry Entry { get; set; }
        public List<SelectListItem> SavedSkills { get; set; }
        [DisplayName("Skills I learnt")]
        public string[] SkillsLearntText { get; set; }
    }
}