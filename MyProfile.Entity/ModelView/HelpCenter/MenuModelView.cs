using System.Collections.Generic;
using System.Linq;

namespace MyProfile.Entity.ModelView.HelpCenter
{
    public class MenuModelView
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        /// <summary>
        /// Filtered on client side
        /// </summary>
        public bool IsShow { get; set; } = true;
        public IEnumerable<MenuArticleModelView> Articles { get; set; }
    }

    public class MenuArticleModelView
    {
        public int ID { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// Filtered on client side
        /// </summary>
        public bool IsShow { get; set; } = true;
        public string KeyWords { get; set; }
        public string Link { get; set; }
        public int CountViews { get; set; }
    }
}
