using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

namespace Website.Controllers;

public class AuthenticationController : Controller
{
    [HttpGet("~/signin")]
    public IActionResult SignIn()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/" }, MicrosoftAccountDefaults.AuthenticationScheme);
    }

    [HttpGet("~/signout")]
    [HttpPost("~/signout")]
    public IActionResult SignOutCurrentUser()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("~/.well-known/microsoft-identity-association")]
    public IActionResult MicrosoftIdentityAssociation()
    {
        return File("~/.well-known/microsoft-identity-association.json", "application/json");
    }
}