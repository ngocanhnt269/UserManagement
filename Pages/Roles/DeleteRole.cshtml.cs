using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Code1stUsersRoles.Pages.Roles;
public class DeleteRole : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public DeleteRole(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IEnumerable<IdentityRole>? Roles { get; set; }

    [BindProperty]
    public string? RoleId { get; set; }

    public void OnGet()
    {
        Roles = _roleManager.Roles;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (RoleId != null)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToPage("Index");
                }
            }
        }

        // Handle error
        return Page();
    }
}