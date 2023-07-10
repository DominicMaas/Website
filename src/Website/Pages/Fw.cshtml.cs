﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Common;

namespace Website.Pages;

public class FwModel : PageModel
{
    private readonly DatabaseContext _databaseContext;

    public string? RequestedUri { get; set; }

    public string? Message { get; set; }

    public bool IsSuccess { get; set; }

    public FwModel(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task OnGetAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            RequestedUri = string.Empty;
            Message = "This link does not exist";
            IsSuccess = false;
            return;
        }

        var link = await _databaseContext.ShortLinks.FindAsync(id);

        if (link == null || string.IsNullOrEmpty(link.RedirectLink))
        {
            // This URI does not exist
            RequestedUri = string.Empty;
            Message = "This link does not exist";
            IsSuccess = false;
            return;
        }

        RequestedUri = link.RedirectLink;
        IsSuccess = true;
    }
}
