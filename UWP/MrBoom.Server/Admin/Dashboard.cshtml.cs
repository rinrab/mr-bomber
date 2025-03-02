using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MrBoom.Server.Admin
{
    public class DashboardModel : PageModel
    {
        public readonly IMetrics Metrics;

        public DashboardModel(IMetrics metrics)
        {
            this.Metrics = metrics;
        }

        public void OnGet()
        {
        }
    }
}
