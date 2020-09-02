using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
    public class MenuItemViewModel
    {
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; } = "";
        public string Icon { get; set; }
        public bool IsLastBeforeLine { get; set; }
        public string ClassElement { get; set; } = "";
    }
}
