using System.Collections.Generic;

namespace MyProfile.Entity.ModelView.Template
{
    public class PreparedTemplatesViewModel : IPreparedModel
    {
        public List<PreparedTemplateViewModel> Periods { get; set; } = new List<PreparedTemplateViewModel>();
    }

    public class PreparedTemplateViewModel
    {
        public int ID { get; set; }
        public int PeriodTypeID { get; set; }
        public string PeriodTypeName { get; set; }
        public bool IsSelected { get; set; }
        public List<TemplateViewModel> Templates { get; set; } = new List<TemplateViewModel>();
    }
}
