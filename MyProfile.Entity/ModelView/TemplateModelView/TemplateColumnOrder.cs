using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.TemplateModelView
{
    public class TemplateColumnOrder
    {
        public List<ColumnOrder> ListOrder { get; set; }
        public int TemplateID { get; set; }
    }

    public class ColumnOrder
    {
        public int ID { get; set; }
        public int NewOrder { get; set; }
        public int OldOrder { get; set; }
    }
}
