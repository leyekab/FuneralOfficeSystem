using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuneralOfficeSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FuneralOfficeSystem.Pages.Admin
{
    public class UserManagementModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            SearchString = string.Empty; // Αρχικοποίηση του SearchString
        }

        public class UserViewModel
        {
            public required string Id { get; set; } // Προσθήκη required
            public required string UserName { get; set; }
            public string? Email { get; set; }
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public List<string> Roles { get; set; } = new List<string>();
        }

        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public string SearchString { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1, string? searchString = null)
        {
            SearchString = searchString ?? string.Empty;
            CurrentPage = pageNumber < 1 ? 1 : pageNumber;

            var usersQuery = _userManager.Users.AsQueryable();

            // Εφαρμογή φίλτρου αναζήτησης αν υπάρχει
            if (!string.IsNullOrEmpty(SearchString))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.UserName != null && u.UserName.Contains(SearchString)) ||
                    (u.Email != null && u.Email.Contains(SearchString)) ||
                    (u.FirstName != null && u.FirstName.Contains(SearchString)) ||
                    (u.LastName != null && u.LastName.Contains(SearchString)));
            }

            var totalUsers = await usersQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalUsers / (double)PageSize);

            // Φόρτωση χρηστών με σελιδοποίηση
            var users = await usersQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Δημιουργία του view model
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                Users.Add(new UserViewModel
                {
                    Id = user.Id ?? string.Empty, // Null check για το Id
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Ο χρήστης διαγράφηκε επιτυχώς.";
            }
            else
            {
                TempData["ErrorMessage"] = "Σφάλμα κατά τη διαγραφή του χρήστη: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToPage();
        }
    }
}