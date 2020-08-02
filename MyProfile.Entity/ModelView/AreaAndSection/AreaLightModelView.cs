using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.AreaAndSection
{
    public class AreaLightModelView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<SectionLightModelView> BudgetSections { get; set; }
    }

    public class SectionLightModelView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int BudgetAreaID { get; set; }
        public string BudgetAreaname { get; set; }
        public string CssBackground { get; set; }
        public int? SectionTypeID { get; set; }
    }
}
