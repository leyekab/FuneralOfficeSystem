using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Client Client { get; set; } = default!;

        [BindProperty]
        public bool IsPopup { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Client == null)
            {
                return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    ? new JsonResult(new { success = false })
                    : Page() as IActionResult;
            }

            ModelState.Clear();

            try
            {
                _context.Clients.Add(Client);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return new JsonResult(new { success = true });
                    }
                    return IsPopup
                        ? Content("<script>window.close();</script>", "text/html")
                        : RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError("", "Δεν ήταν δυνατή η αποθήκευση του εντολέα.");
                    return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                        ? new JsonResult(new { success = false })
                        : Page() as IActionResult;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Προέκυψε σφάλμα: {ex.Message}");
                return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    ? new JsonResult(new { success = false })
                    : Page() as IActionResult;
            }
        }
    }
}