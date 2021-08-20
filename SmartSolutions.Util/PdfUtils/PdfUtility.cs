using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartSolutions.Util.PdfUtils
{
    public class PdfUtility
    {
        private Document document;
        public  string ExportToPDF(System.Data.DataTable DT, string title)
        {
            if (DT == null) return null;
            try
            {
                var stream = new MemoryStream();
                // Create PDF Writer
                PdfWriter writer = new PdfWriter(stream);

                //Initialize PDF document
                PdfDocument pdf = new PdfDocument(writer);
                // Creates a PDF document
                document = new Document(pdf, PageSize.A4);
                document.SetMargins(0, 0, 15, 5);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { DT.Columns.Count }));
                table.SetTextAlignment(alignment: TextAlignment.CENTER)
                    .SetHorizontalAlignment(horizontalAlignment: HorizontalAlignment.CENTER);
                document.Add(table);

                for (int i = 0; i < DT.Columns.Count; i++)
                {
                    var cellName = $"cell {i}";
                    Cell cellname = new Cell(i, DT.Columns.Count).Add(new Paragraph($"{i}"))
                        .SetFont(font)
                        .SetFontSize(13)
                        .SetTextAlignment(TextAlignment.CENTER);
                    table.AddHeaderCell(cellName);
                }
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    for (int j = 1; j < DT.Columns.Count; j++)
                    {
                        table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(DT.Columns[j].ToString())));
                    }
                }
              
                document.Add(table);
                document.Close();
                //BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(DATA), StandardCharsets.UTF_8));

                //// Creates a PdfPTable with column count of the table equal to no of columns of the gridview or gridview datasource.
                //iTextSharp.text.pdf.PdfPTable mainTable = new iTextSharp.text.pdf.PdfPTable(noOfColumns);

                //// Sets the first 4 rows of the table as the header rows which will be repeated in all the pages.
                //mainTable.HeaderRows = 4;

                //// Creates a PdfPTable with 2 columns to hold the header in the exported PDF.
                //iTextSharp.text.pdf.PdfPTable headerTable = new iTextSharp.text.pdf.PdfPTable(2);

                //// Creates a phrase to hold the application name at the left hand side of the header.
                //Phrase phApplicationName = new Phrase("Total Month Sales", FontFactory.GetFont("Arial", ApplicationNameSize, iTextSharp.text.Font.NORMAL));

                //// Creates a PdfPCell which accepts a phrase as a parameter.
                //PdfPCell clApplicationName = new PdfPCell(phApplicationName);
                //// Sets the border of the cell to zero.
                //clApplicationName.Border = PdfPCell.NO_BORDER;
                //// Sets the Horizontal Alignment of the PdfPCell to left.
                //clApplicationName.HorizontalAlignment = Element.ALIGN_LEFT;

                //// Creates a phrase to show the current date at the right hand side of the header.
                //Phrase phDate = new Phrase(DateTime.Now.Date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", ApplicationNameSize, iTextSharp.text.Font.NORMAL));

                //// Creates a PdfPCell which accepts the date phrase as a parameter.
                //PdfPCell clDate = new PdfPCell(phDate);
                //// Sets the Horizontal Alignment of the PdfPCell to right.
                //clDate.HorizontalAlignment = Element.ALIGN_RIGHT;
                //// Sets the border of the cell to zero.
                //clDate.Border = PdfPCell.NO_BORDER;

                //// Adds the cell which holds the application name to the headerTable.
                //headerTable.AddCell(clApplicationName);
                //// Adds the cell which holds the date to the headerTable.
                //headerTable.AddCell(clDate);
                //// Sets the border of the headerTable to zero.
                //headerTable.DefaultCell.Border = PdfPCell.NO_BORDER;

                //// Creates a PdfPCell that accepts the headerTable as a parameter and then adds that cell to the main PdfPTable.
                //PdfPCell cellHeader = new PdfPCell(headerTable);
                //cellHeader.Border = PdfPCell.NO_BORDER;
                //// Sets the column span of the header cell to noOfColumns.
                //cellHeader.Colspan = noOfColumns;
                //// Adds the above header cell to the table.
                //mainTable.AddCell(cellHeader);

                //// Creates a phrase which holds the file name.
                //Phrase phHeader = new Phrase("Sales for " + SelectedMonth, FontFactory.GetFont("Arial", ReportNameSize, iTextSharp.text.Font.BOLD));
                //PdfPCell clHeader = new PdfPCell(phHeader);
                //clHeader.Colspan = noOfColumns;
                //clHeader.Border = PdfPCell.NO_BORDER;
                //clHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                //mainTable.AddCell(clHeader);

                //// Creates a phrase for a new line.
                //Phrase phSpace = new Phrase("\n");
                //PdfPCell clSpace = new PdfPCell(phSpace);
                //clSpace.Border = PdfPCell.NO_BORDER;
                //clSpace.Colspan = noOfColumns;
                //mainTable.AddCell(clSpace);

                //// Sets the gridview column names as table headers.

                //mainTable.AddCell(new Phrase("Seller", FontFactory.GetFont("Arial", HeaderTextSize, iTextSharp.text.Font.BOLD)));
                //mainTable.AddCell(new Phrase("Product", FontFactory.GetFont("Arial", HeaderTextSize, iTextSharp.text.Font.BOLD)));
                //mainTable.AddCell(new Phrase("Qty", FontFactory.GetFont("Arial", HeaderTextSize, iTextSharp.text.Font.BOLD)));
                //mainTable.AddCell(new Phrase("Total", FontFactory.GetFont("Arial", HeaderTextSize, iTextSharp.text.Font.BOLD)));

                //// Reads the gridview rows and adds them to the mainTable
                //foreach (var item in AllSales)
                //{

                //    {
                //        mainTable.AddCell(new Phrase(item.Seller, FontFactory.GetFont("Arial", ReportTextSize, iTextSharp.text.Font.NORMAL)));
                //        mainTable.AddCell(new Phrase(item.OrderItems.Name, FontFactory.GetFont("Arial", ReportTextSize, iTextSharp.text.Font.NORMAL)));
                //        mainTable.AddCell(new Phrase(item.OrderItems.Quantity.ToString(), FontFactory.GetFont("Arial", ReportTextSize, iTextSharp.text.Font.NORMAL)));
                //        mainTable.AddCell(new Phrase(item.ItemTotal.ToString(), FontFactory.GetFont("Arial", ReportTextSize, iTextSharp.text.Font.NORMAL)));

                //    }
                //    // Tells the mainTable to complete the row even if any cell is left incomplete.
                //    mainTable.CompleteRow();
                //}


                //// Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                //PdfWriter wri = PdfWriter.GetInstance(document, new System.IO.FileStream("SalesReport.pdf", FileMode.Create));

                //document.Open();//Open Document to write


                //Paragraph paragraph = new Paragraph("data Exported From DataGridview!");

                //document.Add(mainTable);
                ////doc.Add(t1);
                //document.Close(); //Close document
                ////Ope the pdf file just created
                //System.Diagnostics.Process.Start(@"SalesReport.pdf");
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return title;
        }
    }
}
