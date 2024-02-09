using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Code1stUsersRoles.Pages
{
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<IdentityUser>? Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // User deleted successfully
                return RedirectToPage("/Users/UsersModel");
            }

            // Failed to delete user, handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Re-populate the Users list before returning the page
            Users = await _userManager.Users.ToListAsync();
            return Page();
        }



    }

}
