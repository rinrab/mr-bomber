// Copyright (c) Timofei Zhakov. All rights reserved.

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
