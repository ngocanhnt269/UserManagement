// RemoveFromRole.cshtml.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Code1stUsersRoles.Pages.Users;
public class RemoveFromRole : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IEnumerable<IdentityUser>? Users { get; set; }
    public IEnumerable<IdentityRole>? Roles { get; set; }

    [BindProperty]
    public string? UserId { get; set; }

    [BindProperty]
    public string? RoleName { get; set; }

    public RemoveFromRole(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void OnGet()
    {
        Users = _userManager.Users;
        Roles = _roleManager.Roles;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(RoleName))
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var role = await _roleManager.FindByNameAsync(RoleName);

            if (user != null && role != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, RoleName);

                if (result.Succeeded)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
        }

        return Page();
    }
}
