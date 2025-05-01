using FuneralOfficeSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FuneralOfficeSystem.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                using var context = new ApplicationDbContext(
                    serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

                // Ορισμός των ρόλων
                string[] roles = { "SuperAdmin", "Admin", "Employer", "Employee" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                        logger.LogInformation($"Created role: {role}");
                    }
                }

                try
                {
                    // Δημιουργία του SuperAdmin
                    var superAdminEmail = "superadmin@example.com";
                    var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);

                    if (superAdminUser == null)
                    {
                        superAdminUser = new ApplicationUser
                        {
                            UserName = "SuperAdmin",
                            Email = superAdminEmail,
                            EmailConfirmed = true,
                            FirstName = "Super",
                            LastName = "Administrator",
                            CreatedAt = DateTime.UtcNow,
                            IsEnabled = true
                        };

                        var result = await userManager.CreateAsync(superAdminUser, "SuperAdmin@123");

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                            logger.LogInformation("SuperAdmin user created successfully");
                        }
                        else
                        {
                            logger.LogError($"Failed to create SuperAdmin user: {string.Join(", ", result.Errors)}");
                        }
                    }

                    // Δημιουργία του Admin
                    var adminEmail = "admin@example.com";
                    var adminUser = await userManager.FindByEmailAsync(adminEmail);

                    if (adminUser == null)
                    {
                        adminUser = new ApplicationUser
                        {
                            UserName = "Admin",
                            Email = adminEmail,
                            EmailConfirmed = true,
                            FirstName = "System",
                            LastName = "Administrator",
                            CreatedAt = DateTime.UtcNow,
                            IsEnabled = true
                        };

                        var result = await userManager.CreateAsync(adminUser, "Admin@123");

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                            logger.LogInformation("Admin user created successfully");
                        }
                    }

                    // Δημιουργία του Employer (Εργοδότης)
                    var employerEmail = "employer@example.com";
                    var employerUser = await userManager.FindByEmailAsync(employerEmail);

                    if (employerUser == null)
                    {
                        employerUser = new ApplicationUser
                        {
                            UserName = "Employer",
                            Email = employerEmail,
                            EmailConfirmed = true,
                            FirstName = "Employer",
                            LastName = "Employer",
                            CreatedAt = DateTime.UtcNow,
                            IsEnabled = true
                        };

                        var result = await userManager.CreateAsync(employerUser, "Employer@123");

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(employerUser, "Employer");
                            logger.LogInformation("Employer user created successfully");
                        }
                    }

                    // Δημιουργία του Employee (Υπάλληλος)
                    var employeeEmail = "employee@example.com";
                    var employeeUser = await userManager.FindByEmailAsync(employeeEmail);

                    if (employeeUser == null)
                    {
                        employeeUser = new ApplicationUser
                        {
                            UserName = "Employee",
                            Email = employeeEmail,
                            EmailConfirmed = true,
                            FirstName = "Employee",
                            LastName = "Employee",
                            CreatedAt = DateTime.UtcNow,
                            IsEnabled = true
                        };

                        var result = await userManager.CreateAsync(employeeUser, "Employee@123");

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(employeeUser, "Employee");
                            logger.LogInformation("Employee user created successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error during user creation: {ex.Message}");
                    throw;
                }

                try
                {
                    // Προσθήκη κοιμητηρίων αν δεν υπάρχουν
                    if (!context.BurialPlaces.Any())
                    {
                        var burialPlaces = new List<BurialPlace>
                        {
                            new BurialPlace
                            {
                                Name = "Κοιμητήριο Βασιλικού",
                                Address = "Εντός του Κοιμητηρίου ευρίσκεται ο Ναός του Αγίου Παντελεήμωνος",
                                Phone = "",
                                IsEnabled = true
                            },
                            new BurialPlace
                            {
                                Name = "Εβραϊκό Νεκροταφείο",
                                Address = "Messapion ke Anapafseos, Χαλκίδα 341 00",
                                Phone = "6971949816",
                                IsEnabled = true
                            },
                            new BurialPlace
                            {
                                Name = "Νέο Κοιμητήριο Χαλκίδας",
                                Address = "Χαλκίδα 341 00",
                                Phone = "2221040911",
                                IsEnabled = true
                            }
                        };

                        context.BurialPlaces.AddRange(burialPlaces);
                        await context.SaveChangesAsync();
                        logger.LogInformation("Burial places seeded successfully");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error during burial places seeding: {ex.Message}");
                    throw;
                }

                try
                {
                    // Προσθήκη εκκλησιών αν δεν υπάρχουν
                    if (!context.Churches.Any())
                    {
                        var churches = new List<Church>
                        {
                            new Church
                            {
                                Name = "Ιερός Ναός Ευαγγελισμού της Θεοτόκου",
                                Address = "Apollonos ke Elefinoros, Chalkida 341 00",
                                Phone = "2221024550",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Αγίου Δημητρίου",
                                Address = "Αντωνίου, Μητροπόλεως &, Χαλκίδα 341 00",
                                Phone = "2221023279",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Αγίας Μαρίνας",
                                Address = "Δημ. Πολιορκητού 24, Χαλκίδα 341 00",
                                Phone = "",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Παναγίας Οδηγήτριας Μέσα Παναγίτσα",
                                Address = "Γιαννίτση 3, Χαλκίδα 341 00",
                                Phone = "2221029619",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Αγίας Βαρβάρας Χαλκίδας",
                                Address = "Χαλκίδα 341 00",
                                Phone = "",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Εκκλησία Άγιος Πετρος και Παύλος",
                                Address = "Κρήτης 10, Χαλκίδα 341 00",
                                Phone = "2221074824",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Ευβοέων Αγίων (Άγιοι Ευβοείς)",
                                Address = "Λεωφ. Γεωρ. Παπανδρέου 89, Χαλκίδα 341 00",
                                Phone = "2221084600",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Παμμεγίστων Ταξιαρχών",
                                Address = "Mitropolitou Vasiliou ke Delagrammatika, Chalkida 341 00",
                                Phone = "2221024828",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Αγίου Νικολάου",
                                Address = "Πλ. Αγίου Νικολάου, Χαλκίδα 341 00",
                                Phone = "2221024815",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Αγίων Κωνσταντίνου και Ελένης",
                                Address = "Πετρογιάννη 111, Χαλκίδα 341 00",
                                Phone = "2221087731",
                                IsEnabled = true
                            },
                            new Church
                            {
                                Name = "Ιερός Ναός Αγίου Σπυρίδωνος & Αγίου Γεωργίου",
                                Address = "Χαλκίδα 341 00",
                                Phone = "",
                                IsEnabled = true
                            }
                        };

                        context.Churches.AddRange(churches);
                        await context.SaveChangesAsync();
                        logger.LogInformation("Churches seeded successfully");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error during churches seeding: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError($"An error occurred during database seeding: {ex.Message}");
                throw;
            }
        }
    }
}