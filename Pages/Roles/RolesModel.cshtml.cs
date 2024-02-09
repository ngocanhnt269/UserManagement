// RolesModel.cshtml.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Code1stUsersRoles.Pages;

public class RolesModel : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RolesModel(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public IEnumerable<IdentityRole>? Roles { get; set; }
    public IEnumerable<IdentityUser>? Admins { get; set; }
    public IEnumerable<IdentityUser>? Members { get; set; }

    public List<string> Errors { get; private set; } = new List<string>();
    public bool HasErrors => Errors.Count > 0;

    public IEnumerable<IdentityUser>? GetUsersInRole(string roleName)
    {
        var usersInRole = _userManager.GetUsersInRoleAsync(roleName).Result;
        return usersInRole;
    }

    public async Task<IEnumerable<IdentityUser>> GetUsersInRoleAsync(string roleName)
    {
        return await _userManager.GetUsersInRoleAsync(roleName);
    }

    public IEnumerable<IdentityUser>? GetAvailableUsers()
    {
        var allUsers = _userManager.Users.ToList();
        var usersInRoles = Roles?.Where(role => role.Name != null)
                                 .SelectMany(role => GetUsersInRoleAsync(role.Name!).Result)
                                 ?? Enumerable.Empty<IdentityUser>();
        var availableUsers = allUsers.Except(usersInRoles);
        return availableUsers;
    }

    public void OnGet()
    {
        // Fetch roles and convert to a list
        Roles = _roleManager.Roles.ToList();

        // Ensure Admins and Members are initialized
        Admins = _userManager.GetUsersInRoleAsync("Admin").Result ?? Enumerable.Empty<IdentityUser>();
        Members = _userManager.GetUsersInRoleAsync("Member").Result ?? Enumerable.Empty<IdentityUser>();
    }




    public async Task<IActionResult> OnPostCreateRoleAsync(string newRoleName)

    {
        if (await _roleManager.RoleExistsAsync(newRoleName))
        {
            ModelState.AddModelError(string.Empty, "Role already exists.");
            return Page();
        }
        if (string.IsNullOrEmpty(newRoleName))
        {
            ModelState.AddModelError(string.Empty, "Role name cannot be empty.");
            return Page();
        }


        var role = new IdentityRole { Name = newRoleName };
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            // Fetch roles again after creating a new role
            Roles = _roleManager.Roles.ToList();
            // Ensure Admins and Members are initialized
            Admins = await _userManager.GetUsersInRoleAsync("Admin") ?? Enumerable.Empty<IdentityUser>();
            Members = await _userManager.GetUsersInRoleAsync("Member") ?? Enumerable.Empty<IdentityUser>();
            return RedirectToPage();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        // Fetch roles again after encountering an error
        Roles = _roleManager.Roles.ToList();

        return Page();
    }



    public async Task<IActionResult> OnPostRemoveRoleAsync(string roleIdToRemove)
    {
        Errors = new List<string>();

        var role = await _roleManager.FindByIdAsync(roleIdToRemove);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToPage();
            }
            else
            {
                // Handle errors
                Errors.AddRange(result.Errors.Select(error => error.Description));
            }
        }
        else
        {
            Errors.Add("Role not found.");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddUserToRoleAsync(string userIdToAdd, string roleName)
    {
        if (string.IsNullOrEmpty(userIdToAdd) || string.IsNullOrEmpty(roleName))
        {
            ModelState.AddModelError(string.Empty, "User ID and Role name cannot be null or empty.");
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userIdToAdd);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            return Page();
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            ModelState.AddModelError(string.Empty, "Role does not exist.");
            return Page();
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            // Refresh roles and available users
            Roles = _roleManager.Roles.ToList();
            Admins = await _userManager.GetUsersInRoleAsync("Admin") ?? Enumerable.Empty<IdentityUser>();
            Members = await _userManager.GetUsersInRoleAsync("Member") ?? Enumerable.Empty<IdentityUser>();
            return RedirectToPage();
        }
        else
        {
            // Handle errors
            Errors.AddRange(result.Errors.Select(error => error.Description));
            return Page();
        }
    }
    public async Task<IActionResult> OnPostRemoveUserFromRoleAsync(string userIdToRemove, string roleNameToRemove)
    {
        if (string.IsNullOrEmpty(userIdToRemove) || string.IsNullOrEmpty(roleNameToRemove))
        {
            ModelState.AddModelError(string.Empty, "User ID and Role name cannot be null or empty.");
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userIdToRemove);
        if (user != null)
        {
            var userIsInRole = await _userManager.IsInRoleAsync(user, roleNameToRemove);

            if (userIsInRole)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleNameToRemove);
                if (result.Succeeded)
                {
                    // Refresh roles and available users
                    Roles = _roleManager.Roles.ToList();
                    Admins = await _userManager.GetUsersInRoleAsync("Admin") ?? Enumerable.Empty<IdentityUser>();
                    Members = await _userManager.GetUsersInRoleAsync("Member") ?? Enumerable.Empty<IdentityUser>();
                    return RedirectToPage();
                }
                else
                {
                    // Handle errors
                    Errors.AddRange(result.Errors.Select(error => error.Description));
                }
            }
            else
            {
                Errors.Add($"User is not in the {roleNameToRemove} role.");
            }
        }
        else
        {
            Errors.Add("User not found.");
        }

        // Refresh roles and available users
        Roles = _roleManager.Roles.ToList();
        Admins = await _userManager.GetUsersInRoleAsync("Admin") ?? Enumerable.Empty<IdentityUser>();
        Members = await _userManager.GetUsersInRoleAsync("Member") ?? Enumerable.Empty<IdentityUser>();

        return Page();
    }

}
