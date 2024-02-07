using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Code1stUsersRoles.Pages.Users;

[Authorize(Roles = "Admin")]
public class AddToRole : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IEnumerable<IdentityUser>? Users { get; set; }
    public IEnumerable<IdentityRole>? Roles { get; set; }

    [BindProperty]
    public string? UserId { get; set; }

    [BindProperty]
    public string? RoleName { get; set; }

    public AddToRole(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
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
        var user = await _userManager.FindByIdAsync(UserId ?? string.Empty);

        if (user != null)
        {
            if (!string.IsNullOrEmpty(RoleName))
            {
                var role = await _roleManager.FindByNameAsync(RoleName);

                if (role != null && role.Name != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToPage("Index");
                    }
                }
            }
        }
        return Page();
    }
}