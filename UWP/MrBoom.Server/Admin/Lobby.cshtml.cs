// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MrBoom.Server.Admin
{
    public class LobbyModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public void OnGet()
        {
        }
    }
}
