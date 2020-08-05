using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Feedback
{
    public class FeedbackCreateModelView
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int Priority { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; } = "New";
        public int MoodID { get; set; }
        public List<Image> Images { get; set; } = new List<Image>();

    }

    public class Image
    {
        public int ID { get; set; }
        public string ImageBase64 { get; set; }
    }
}
