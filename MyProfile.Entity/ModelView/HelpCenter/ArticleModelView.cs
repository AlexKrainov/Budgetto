using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.HelpCenter
{
    public class ArticleModelView
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string AreaName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        public string UserName { get; set; }
        public string ImageLink { get; set; }
        public int Views { get; set; }
    }
}
