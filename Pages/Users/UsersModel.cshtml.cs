using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Code1stUsersRoles.Pages;

public class UsersModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    public UsersModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IEnumerable<IdentityUser>? Users { get; set; }

    public void OnGet()
    {
        Users = _userManager.Users;
    }
}
