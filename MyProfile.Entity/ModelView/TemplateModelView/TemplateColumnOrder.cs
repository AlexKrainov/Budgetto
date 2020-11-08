using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.TemplateModelView
{
    public class TemplateColumnOrder
    {
        public List<int> NewColumnsOrder { get; set; }
        public List<int> OldColumnsOrder { get; set; }
        public int TemplateID { get; set; }
    }
}
