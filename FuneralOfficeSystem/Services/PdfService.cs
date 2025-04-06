using FuneralOfficeSystem.Data;
using FuneralOfficeSystem.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Layout.Borders;

namespace FuneralOfficeSystem.Services
{
    public class PdfService
    {
        private readonly ApplicationDbContext _context;

        public PdfService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Προσθήκη της μεθόδου GenerateSamplePdf
        public byte[] GenerateSamplePdf()
        {
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Ορισμός γραμματοσειρών
                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Τίτλος
                Paragraph title = new Paragraph("ΔΕΙΓΜΑ PDF ΕΓΓΡΑΦΟΥ")
                    .SetFont(titleFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER);
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Περιεχόμενο
                Paragraph content = new Paragraph()
                    .Add(new Text("Αυτό είναι ένα δοκιμαστικό PDF αρχείο για το Funeral Office System.\n\n")
                        .SetFont(normalFont)
                        .SetFontSize(12))
                    .Add(new Text("Το PDF αυτό δημιουργήθηκε χρησιμοποιώντας τη βιβλιοθήκη iText7.\n\n")
                        .SetFont(normalFont)
                        .SetFontSize(12))
                    .Add(new Text($"Ημερομηνία και ώρα δημιουργίας: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(content);

                // Προσθήκη πίνακα παραδείγματος
                document.Add(new Paragraph("\n\n"));
                document.Add(new Paragraph("Παράδειγμα Πίνακα")
                    .SetFont(titleFont)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER));

                Table table = new Table(3)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Επικεφαλίδες πίνακα
                table.AddCell(new Cell().Add(new Paragraph("Α/Α")
                    .SetFont(titleFont)
                    .SetFontSize(10))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph("Περιγραφή")
                    .SetFont(titleFont)
                    .SetFontSize(10))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph("Ποσό")
                    .SetFont(titleFont)
                    .SetFontSize(10))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER));

                // Δεδομένα πίνακα
                for (int i = 1; i <= 5; i++)
                {
                    table.AddCell(new Cell().Add(new Paragraph(i.ToString())
                        .SetFont(normalFont)
                        .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"Στοιχείο #{i}")
                        .SetFont(normalFont)
                        .SetFontSize(10)));
                    table.AddCell(new Cell().Add(new Paragraph($"{i * 100:C}")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.RIGHT));
                }

                document.Add(table);

                // Υπογραφή
                document.Add(new Paragraph("\n\n\n"));
                Paragraph signature = new Paragraph("Funeral Office System")
                    .SetFont(normalFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.RIGHT);
                document.Add(signature);

                document.Close();
                return ms.ToArray();
            }
        }

        // Προσθήκη της μεθόδου GenerateFuneralReport
        public byte[] GenerateFuneralReport(int funeralId)
        {
            using (var ms = new MemoryStream())
            {
                // Ανάκτηση δεδομένων κηδείας από τη βάση δεδομένων
                var funeral = _context.Funerals
                    .Include(f => f.FuneralOffice)
                    .Include(f => f.Deceased)
                    .Include(f => f.Client)
                    .Include(f => f.FuneralProducts)
                        .ThenInclude(fp => fp.Product)
                    .Include(f => f.FuneralServices)
                        .ThenInclude(fs => fs.Service)
                    .FirstOrDefault(f => f.Id == funeralId);

                // Δημιουργία του PDF
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                if (funeral == null)
                {
                    // Αν δεν βρεθεί η κηδεία, δημιουργήστε ένα PDF σφάλματος
                    Paragraph errorTitle = new Paragraph($"Σφάλμα: Η κηδεία με ID {funeralId} δεν βρέθηκε")
                        .SetFontSize(16)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontColor(ColorConstants.RED);
                    document.Add(errorTitle);

                    document.Close();
                    return ms.ToArray();
                }

                // Ορισμός γραμματοσειρών
                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Τίτλος
                Paragraph title = new Paragraph($"ΑΝΑΦΟΡΑ ΚΗΔΕΙΑΣ #{funeral.Id}")
                    .SetFont(titleFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER);
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Στοιχεία γραφείου
                Paragraph officeInfo = new Paragraph()
                    .Add(new Text($"Γραφείο Τελετών: {funeral.FuneralOffice.Name}\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Διεύθυνση: {funeral.FuneralOffice.Address}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Τηλέφωνο: {funeral.FuneralOffice.Phone}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Ημερομηνία αναφοράς: {DateTime.Now:dd/MM/yyyy}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(officeInfo);
                document.Add(new Paragraph("\n"));

                // Στοιχεία κηδείας
                Paragraph funeralInfo = new Paragraph()
                    .Add(new Text("ΣΤΟΙΧΕΙΑ ΚΗΔΕΙΑΣ\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Ημερομηνία Κηδείας: {funeral.FuneralDate:dd/MM/yyyy}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Εκκλησία: {funeral.Church}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Ώρα Τέλεσης: {funeral.CeremonyTime}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Τόπος Ταφής: {funeral.BurialPlace}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(funeralInfo);
                document.Add(new Paragraph("\n"));

                // Στοιχεία αποβιώσαντα
                Paragraph deceasedInfo = new Paragraph()
                    .Add(new Text("ΣΤΟΙΧΕΙΑ ΑΠΟΒΙΩΣΑΝΤΑ\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Ονοματεπώνυμο: {funeral.Deceased.FullName}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"ΑΜΚΑ: {funeral.Deceased.AMKA}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"ΑΦΜ: {funeral.Deceased.AFM}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Ημερομηνία Θανάτου: {funeral.Deceased.DeathDate:dd/MM/yyyy}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(deceasedInfo);
                document.Add(new Paragraph("\n"));

                // Στοιχεία εντολέα
                Paragraph clientInfo = new Paragraph()
                    .Add(new Text("ΣΤΟΙΧΕΙΑ ΕΝΤΟΛΕΑ\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Ονοματεπώνυμο: {funeral.Client.FullName}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"ΑΦΜ: {funeral.Client.AFM}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Διεύθυνση: {funeral.Client.Address}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Τηλέφωνο: {funeral.Client.Phone}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Συγγένεια: {funeral.Client.RelationshipToDeceased}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(clientInfo);
                document.Add(new Paragraph("\n"));

                // Σύνοψη οικονομικών
                document.Add(new Paragraph("ΟΙΚΟΝΟΜΙΚΗ ΣΥΝΟΨΗ")
                    .SetFont(headerFont)
                    .SetFontSize(12));

                decimal productTotal = funeral.FuneralProducts.Sum(fp => fp.TotalPrice);
                decimal serviceTotal = funeral.FuneralServices.Sum(fs => fs.Price);
                decimal totalAmount = productTotal + serviceTotal;

                Table summaryTable = new Table(2)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                summaryTable.AddCell(new Cell().Add(new Paragraph("Σύνολο Προϊόντων")
                    .SetFont(normalFont)
                    .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.LEFT));
                summaryTable.AddCell(new Cell().Add(new Paragraph($"{productTotal:C}")
                    .SetFont(normalFont)
                    .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.RIGHT));

                summaryTable.AddCell(new Cell().Add(new Paragraph("Σύνολο Υπηρεσιών")
                    .SetFont(normalFont)
                    .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.LEFT));
                summaryTable.AddCell(new Cell().Add(new Paragraph($"{serviceTotal:C}")
                    .SetFont(normalFont)
                    .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.RIGHT));

                summaryTable.AddCell(new Cell().Add(new Paragraph("ΓΕΝΙΚΟ ΣΥΝΟΛΟ")
                    .SetFont(headerFont)
                    .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetBorderTop(new SolidBorder(1)));
                summaryTable.AddCell(new Cell().Add(new Paragraph($"{totalAmount:C}")
                    .SetFont(headerFont)
                    .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBorderTop(new SolidBorder(1)));

                document.Add(summaryTable);

                document.Add(new Paragraph("\n\n"));
                Paragraph footerInfo = new Paragraph($"Αναφορά που δημιουργήθηκε στις {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                    .SetFont(normalFont)
                    .SetFontSize(8)
                    .SetTextAlignment(TextAlignment.CENTER);
                document.Add(footerInfo);

                document.Close();
                return ms.ToArray();
            }
        }

        public async Task<byte[]> GenerateFuneralInvoicePdf(int funeralId)
        {
            var funeral = await _context.Funerals
                .Include(f => f.FuneralOffice)
                .Include(f => f.Deceased)
                .Include(f => f.Client)
                .Include(f => f.FuneralProducts)
                    .ThenInclude(fp => fp.Product)
                .Include(f => f.FuneralServices)
                    .ThenInclude(fs => fs.Service)
                .FirstOrDefaultAsync(f => f.Id == funeralId);

            if (funeral == null)
            {
                return null;
            }

            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Ορισμός γραμματοσειρών
                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Τίτλος
                Paragraph title = new Paragraph("ΛΟΓΑΡΙΑΣΜΟΣ ΚΗΔΕΙΑΣ")
                    .SetFont(titleFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER);
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Στοιχεία γραφείου
                Paragraph officeInfo = new Paragraph()
                    .Add(new Text($"Γραφείο Τελετών: {funeral.FuneralOffice.Name}\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Διεύθυνση: {funeral.FuneralOffice.Address}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Τηλέφωνο: {funeral.FuneralOffice.Phone}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Ημερομηνία: {DateTime.Now:dd/MM/yyyy}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(officeInfo);
                document.Add(new Paragraph("\n"));

                // Στοιχεία κηδείας
                Paragraph funeralInfo = new Paragraph()
                    .Add(new Text("ΣΤΟΙΧΕΙΑ ΚΗΔΕΙΑΣ\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Ημερομηνία Κηδείας: {funeral.FuneralDate:dd/MM/yyyy}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Εκκλησία: {funeral.Church}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Ώρα Τέλεσης: {funeral.CeremonyTime}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Τόπος Ταφής: {funeral.BurialPlace}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(funeralInfo);
                document.Add(new Paragraph("\n"));

                // Στοιχεία αποβιώσαντα
                Paragraph deceasedInfo = new Paragraph()
                    .Add(new Text("ΣΤΟΙΧΕΙΑ ΑΠΟΒΙΩΣΑΝΤΑ\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Ονοματεπώνυμο: {funeral.Deceased.FullName}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"ΑΜΚΑ: {funeral.Deceased.AMKA}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"ΑΦΜ: {funeral.Deceased.AFM}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Ημερομηνία Θανάτου: {funeral.Deceased.DeathDate:dd/MM/yyyy}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(deceasedInfo);
                document.Add(new Paragraph("\n"));

                // Στοιχεία εντολέα
                Paragraph clientInfo = new Paragraph()
                    .Add(new Text("ΣΤΟΙΧΕΙΑ ΕΝΤΟΛΕΑ\n")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .Add(new Text($"Ονοματεπώνυμο: {funeral.Client.FullName}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"ΑΦΜ: {funeral.Client.AFM}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Διεύθυνση: {funeral.Client.Address}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Τηλέφωνο: {funeral.Client.Phone}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .Add(new Text($"Συγγένεια: {funeral.Client.RelationshipToDeceased}\n")
                        .SetFont(normalFont)
                        .SetFontSize(10));
                document.Add(clientInfo);
                document.Add(new Paragraph("\n"));

                // Πίνακας προϊόντων
                if (funeral.FuneralProducts != null && funeral.FuneralProducts.Any())
                {
                    document.Add(new Paragraph("ΠΡΟΪΟΝΤΑ")
                        .SetFont(headerFont)
                        .SetFontSize(12));

                    Table productsTable = new Table(4)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    // Ορισμός πλάτους στηλών
                    productsTable.SetWidth(UnitValue.CreatePercentValue(100));
                    productsTable.AddCell(new Cell().Add(new Paragraph("Περιγραφή")
                        .SetFont(headerFont)
                        .SetFontSize(10))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER));
                    productsTable.AddCell(new Cell().Add(new Paragraph("Ποσότητα")
                        .SetFont(headerFont)
                        .SetFontSize(10))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER));
                    productsTable.AddCell(new Cell().Add(new Paragraph("Τιμή")
                        .SetFont(headerFont)
                        .SetFontSize(10))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER));
                    productsTable.AddCell(new Cell().Add(new Paragraph("Σύνολο")
                        .SetFont(headerFont)
                        .SetFontSize(10))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER));

                    // Δεδομένα
                    decimal productTotal = 0;
                    foreach (var product in funeral.FuneralProducts)
                    {
                        productsTable.AddCell(new Cell().Add(new Paragraph(product.Product.Name)
                            .SetFont(normalFont)
                            .SetFontSize(10)));
                        productsTable.AddCell(new Cell().Add(new Paragraph(product.Quantity.ToString())
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.CENTER)));
                        productsTable.AddCell(new Cell().Add(new Paragraph($"{product.Price:C}")
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.RIGHT)));
                        productsTable.AddCell(new Cell().Add(new Paragraph($"{product.TotalPrice:C}")
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.RIGHT)));
                        productTotal += product.TotalPrice;
                    }

                    // Σύνολο προϊόντων
                    Cell totalProductLabelCell = new Cell(1, 3)
                        .Add(new Paragraph("Σύνολο Προϊόντων")
                            .SetFont(headerFont)
                            .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.RIGHT);
                    productsTable.AddCell(totalProductLabelCell);

                    Cell totalProductValueCell = new Cell()
                        .Add(new Paragraph($"{productTotal:C}")
                            .SetFont(headerFont)
                            .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.RIGHT);
                    productsTable.AddCell(totalProductValueCell);

                    document.Add(productsTable);
                    document.Add(new Paragraph("\n"));
                }

                // Πίνακας υπηρεσιών
                if (funeral.FuneralServices != null && funeral.FuneralServices.Any())
                {
                    document.Add(new Paragraph("ΥΠΗΡΕΣΙΕΣ")
                        .SetFont(headerFont)
                        .SetFontSize(12));

                    Table servicesTable = new Table(2)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    // Επικεφαλίδες
                    servicesTable.AddCell(new Cell().Add(new Paragraph("Περιγραφή")
                        .SetFont(headerFont)
                        .SetFontSize(10))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER));
                    servicesTable.AddCell(new Cell().Add(new Paragraph("Τιμή")
                        .SetFont(headerFont)
                        .SetFontSize(10))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetTextAlignment(TextAlignment.CENTER));

                    // Δεδομένα
                    decimal serviceTotal = 0;
                    foreach (var service in funeral.FuneralServices)
                    {
                        servicesTable.AddCell(new Cell().Add(new Paragraph(service.Service.Name)
                            .SetFont(normalFont)
                            .SetFontSize(10)));
                        servicesTable.AddCell(new Cell().Add(new Paragraph($"{service.Price:C}")
                            .SetFont(normalFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.RIGHT)));
                        serviceTotal += service.Price;
                    }

                    // Σύνολο υπηρεσιών
                    Cell totalServiceLabelCell = new Cell()
                        .Add(new Paragraph("Σύνολο Υπηρεσιών")
                            .SetFont(headerFont)
                            .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.RIGHT);
                    servicesTable.AddCell(totalServiceLabelCell);

                    Cell totalServiceValueCell = new Cell()
                        .Add(new Paragraph($"{serviceTotal:C}")
                            .SetFont(headerFont)
                            .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.RIGHT);
                    servicesTable.AddCell(totalServiceValueCell);

                    document.Add(servicesTable);
                    document.Add(new Paragraph("\n"));
                }

                // Γενικό σύνολο
                decimal totalAmount = funeral.FuneralProducts.Sum(fp => fp.Price * fp.Quantity) +
                                      funeral.FuneralServices.Sum(fs => fs.Price);

                Table totalTable = new Table(2)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                Cell totalLabelCell = new Cell()
                    .Add(new Paragraph("ΓΕΝΙΚΟ ΣΥΝΟΛΟ")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .SetBorderTop(new SolidBorder(1))
                    .SetBorderLeft(Border.NO_BORDER)
                    .SetBorderRight(Border.NO_BORDER)
                    .SetBorderBottom(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT);

                totalTable.AddCell(totalLabelCell);

                Cell totalValueCell = new Cell()
                    .Add(new Paragraph($"{totalAmount:C}")
                        .SetFont(headerFont)
                        .SetFontSize(12))
                    .SetBorderTop(new SolidBorder(1))
                    .SetBorderLeft(Border.NO_BORDER)
                    .SetBorderRight(Border.NO_BORDER)
                    .SetBorderBottom(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT);

                totalTable.AddCell(totalValueCell);

                document.Add(totalTable);

                // Υπογραφές
                document.Add(new Paragraph("\n\n\n"));

                Table signaturesTable = new Table(2)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                signaturesTable.AddCell(new Cell()
                    .Add(new Paragraph("Ο Εντολέας")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.CENTER));

                signaturesTable.AddCell(new Cell()
                    .Add(new Paragraph("Για το Γραφείο Τελετών")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.CENTER));

                signaturesTable.AddCell(new Cell()
                    .Add(new Paragraph(" "))
                    .SetBorder(Border.NO_BORDER)
                    .SetHeight(40));

                signaturesTable.AddCell(new Cell()
                    .Add(new Paragraph(" "))
                    .SetBorder(Border.NO_BORDER)
                    .SetHeight(40));

                signaturesTable.AddCell(new Cell()
                    .Add(new Paragraph($"{funeral.Client.FullName}")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .SetBorderTop(new SolidBorder(1))
                    .SetBorderLeft(Border.NO_BORDER)
                    .SetBorderRight(Border.NO_BORDER)
                    .SetBorderBottom(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.CENTER));

                signaturesTable.AddCell(new Cell()
                    .Add(new Paragraph($"{funeral.FuneralOffice.Name}")
                        .SetFont(normalFont)
                        .SetFontSize(10))
                    .SetBorderTop(new SolidBorder(1))
                    .SetBorderLeft(Border.NO_BORDER)
                    .SetBorderRight(Border.NO_BORDER)
                    .SetBorderBottom(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(signaturesTable);

                document.Close();
                return ms.ToArray();
            }
        }
    }
}