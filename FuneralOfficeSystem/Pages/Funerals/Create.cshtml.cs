using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;

namespace FuneralOfficeSystem.Pages.Funerals
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
        public Funeral Funeral { get; set; } = default!;

        public void OnGet()
        {
            ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Received Funeral Data:");
            _logger.LogInformation($"DeceasedId: {Funeral.DeceasedId}");
            _logger.LogInformation($"ClientId: {Funeral.ClientId}");
            _logger.LogInformation($"ChurchId: {Funeral.ChurchId}");
            _logger.LogInformation($"BurialPlaceId: {Funeral.BurialPlaceId}");
            _logger.LogInformation($"FuneralOfficeId: {Funeral.FuneralOfficeId}");
            _logger.LogInformation($"FuneralDate: {Funeral.FuneralDate}");
            _logger.LogInformation($"CeremonyTime: {Funeral.CeremonyTime}");
            _logger.LogInformation($"IsFinalBill: {Funeral.IsFinalBill}");

            // Καθαρίζουμε τα validation errors για τα navigation properties
            ModelState.Remove("Funeral.Deceased");
            ModelState.Remove("Funeral.Client");
            ModelState.Remove("Funeral.Church");
            ModelState.Remove("Funeral.BurialPlace");
            ModelState.Remove("Funeral.FuneralOffice");

            // Φορτώνουμε και ελέγχουμε τα σχετικά entities
            var deceased = await _context.Deceaseds.FindAsync(Funeral.DeceasedId);
            if (deceased == null)
            {
                ModelState.AddModelError("Funeral.DeceasedId", "Ο αποβιώσας δε βρέθηκε.");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            var client = await _context.Clients.FindAsync(Funeral.ClientId);
            if (client == null)
            {
                ModelState.AddModelError("Funeral.ClientId", "Ο εντολέας δε βρέθηκε.");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            var church = await _context.Churches.FindAsync(Funeral.ChurchId);
            if (church == null)
            {
                ModelState.AddModelError("Funeral.ChurchId", "Η εκκλησία δε βρέθηκε.");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            var burialPlace = await _context.BurialPlaces.FindAsync(Funeral.BurialPlaceId);
            if (burialPlace == null)
            {
                ModelState.AddModelError("Funeral.BurialPlaceId", "Ο τόπος ταφής δε βρέθηκε.");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            var funeralOffice = await _context.FuneralOffices.FindAsync(Funeral.FuneralOfficeId);
            if (funeralOffice == null)
            {
                ModelState.AddModelError("Funeral.FuneralOfficeId", "Το γραφείο τελετών δε βρέθηκε.");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            // Αφού έχουμε επιβεβαιώσει ότι όλα τα entities υπάρχουν, τα αναθέτουμε
            Funeral.Deceased = deceased;
            Funeral.Client = client;
            Funeral.Church = church;
            Funeral.BurialPlace = burialPlace;
            Funeral.FuneralOffice = funeralOffice;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is not valid");
                foreach (var modelState in ModelState)
                {
                    var key = modelState.Key;
                    var errors = modelState.Value.Errors;
                    foreach (var error in errors)
                    {
                        _logger.LogError($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }

            try
            {
                _context.Funerals.Add(Funeral);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving funeral");
                ModelState.AddModelError("", $"Προέκυψε σφάλμα κατά την αποθήκευση: {ex.Message}");
                ViewData["FuneralOfficeId"] = new SelectList(_context.FuneralOffices.OrderBy(f => f.Name), "Id", "Name");
                return Page();
            }
        }
    }
}