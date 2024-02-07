using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Code1stUsersRoles.Pages.Roles;

public class EditRole : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public EditRole(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [BindProperty]
    public string? RoleId { get; set; }

    [BindProperty]
    public string? RoleName { get; set; }

    public void OnGet(string roleId)
    {
        var role = _roleManager.FindByIdAsync(roleId).Result;
        if (role != null)
        {
            RoleId = role.Id;
            RoleName = role.Name;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (RoleId != null)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role != null)
            {
                role.Name = RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToPage("Index");
                }
            }
        }

        return Page();
    }
}