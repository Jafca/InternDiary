using System;

namespace InternDiary.Models
{
    public class FullCalendarEvent
    {
        public string title;
        public bool allDay = true;
        public DateTime start;
        public string url;
        public string color;
        public string textColor;
    }
}