using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;
using RCHub.Models.Entities;
using RCHub.Services;
using static AspNet.Security.OAuth.Discord.DiscordAuthenticationConstants;

namespace RCHub.Pages.Account
{
    [Authorize(AuthenticationSchemes = "Discord")]
    public class SigninModel : PageModel
    {
        private readonly AppUserService _appUserService;
        private readonly DiscordBotService _discordBotService;

        public SigninModel(AppUserService appUserService, DiscordBotService discordBotService)
        {
            _appUserService = appUserService;
            _discordBotService = discordBotService;
        }


        public async Task<IActionResult> OnGet()
        {
            var auth = await HttpContext.AuthenticateAsync("Discord");
            var discordId = auth?.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Missing authentication information.");

            var member = await _discordBotService.GetServerMemberAsync(ulong.Parse(discordId)) ?? throw new Exception("Missing member information.");

            var claims = new List<Claim>
            {
                new Claim("Id", discordId),
                new Claim(ClaimTypes.Name, member.Username),
            };

            List<ulong> adminRoles = new List<ulong>() { 1005876575121707048 };
            if (member.Roles.Any(r => adminRoles.Contains(r.Id)))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties() { AllowRefresh = true, IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Index");
        }


        private async Task<AppUser?> GetOrCreateAppUser()
        {



            //var user = await _appUserService.FindAsync(discordId);

            //if (user == null)
            //{
            //    user = new AppUser() { Username = discordId, DiscordId = ulong.Parse(discordId) };

            //    _appUserService.Add(user);
            //    await _appUserService.SaveChangesAsync();
            //}

            return null;
        }

    }
}
