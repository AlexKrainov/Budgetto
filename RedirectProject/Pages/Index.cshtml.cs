using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RedirectProject.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            if (this.HttpContext.Request.Host.Value.Contains("app.budgetto.ru"))
            {
                Response.Redirect("https://app.budgetto.org");
            }
            else
            {
                Response.Redirect("https://budgetto.org");
            }
        }
    }
}
