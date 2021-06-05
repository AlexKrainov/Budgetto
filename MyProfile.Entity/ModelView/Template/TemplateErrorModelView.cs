using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.TemplateModelView
{
    public class TemplateErrorModelView
    {
        public bool IsOk { get; set; } = true;
        public TemplateViewModel Template { get; set; }
        public string Href { get; set; }


        public bool NameAlreadyExist { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
