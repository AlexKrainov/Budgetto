using System;

namespace MyProfile.Entity.ModelView.Progress
{
    public class ProgressItemViewModel
    {
        public string CssClass { get; set; }
        public string Tooltip { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public DateTime? DateComplete { get; set; }
        public string OnClick { get; set; }
        public string Href { get; set; }
    }
}
