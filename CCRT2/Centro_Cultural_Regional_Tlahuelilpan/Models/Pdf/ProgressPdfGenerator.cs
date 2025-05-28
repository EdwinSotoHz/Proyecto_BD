using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Centro_Cultural_Regional_Tlahuelilpan.Models.DBCRUDCORE;

namespace Centro_Cultural_Regional_Tlahuelilpan.Models.Pdf
{
    public class ProgressPdfGenerator
    {
        private readonly List<VistaProgresoAlumno> _data;

        public ProgressPdfGenerator(List<VistaProgresoAlumno> data)
        {
            _data = data;
        }

        public byte[] GeneratePdf()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 20, 20, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                // Pie de página personalizado
                writer.PageEvent = new PdfFooter("Centro Cultural Regional Tlahuelilpan • Hidalgo, México");

                document.Open();

                AddInstitutionHeader(document);
                AddTitle(document, "PROGRESO DE ALUMNOS");
                AddTable(document);

                document.Close();
                return ms.ToArray();
            }
        }

        private void AddInstitutionHeader(Document document)
        {
            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.Black);
            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.DarkGray);

            PdfPTable headerTable = new PdfPTable(1) { WidthPercentage = 100 };
            headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

            headerTable.AddCell(new PdfPCell(new Phrase("Centro Cultural Regional Tlahuelilpan", boldFont)) { Border = Rectangle.NO_BORDER });
            headerTable.AddCell(new PdfPCell(new Phrase("Dirección de Educación y Cultura", normalFont)) { Border = Rectangle.NO_BORDER });
            headerTable.AddCell(new PdfPCell(new Phrase("Av. Universidad s/n, Centro, CP 42780", normalFont)) { Border = Rectangle.NO_BORDER });
            headerTable.AddCell(new PdfPCell(new Phrase("Tels.: (763) 786 00 10, cel. (773) 124 69 53", normalFont)) { Border = Rectangle.NO_BORDER });

            document.Add(headerTable);
        }

        private void AddTitle(Document document, string title)
        {
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.Black);
            Paragraph p = new Paragraph(title, titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 15f };
            document.Add(p);
            document.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy}", FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.Gray)) { Alignment = Element.ALIGN_RIGHT });
        }

        private void AddTable(Document document)
        {
            PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 1, 2, 2, 1, 1 });

            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.White);
            BaseColor headerBg = new BaseColor(83, 109, 254); // Azul principal

            table.AddCell(new PdfPCell(new Phrase("Progreso ID", headerFont)) { BackgroundColor = headerBg });
            table.AddCell(new PdfPCell(new Phrase("Nombre Completo", headerFont)) { BackgroundColor = headerBg });
            table.AddCell(new PdfPCell(new Phrase("Estado", headerFont)) { BackgroundColor = headerBg });
            table.AddCell(new PdfPCell(new Phrase("Calificación", headerFont)) { BackgroundColor = headerBg });
            table.AddCell(new PdfPCell(new Phrase("Asistencia (%)", headerFont)) { BackgroundColor = headerBg });

            Font contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.Black);

            foreach (var item in _data)
            {
                table.AddCell(new PdfPCell(new Phrase(item.ProgresoId.ToString(), contentFont)));
                table.AddCell(new PdfPCell(new Phrase(item.NombreCompleto ?? "-", contentFont)));
                table.AddCell(new PdfPCell(new Phrase(item.Estado ?? "-", contentFont)));
                table.AddCell(new PdfPCell(new Phrase(item.Calificacion?.ToString("F2") ?? "-", contentFont)));
                table.AddCell(new PdfPCell(new Phrase(item.Asistencia?.ToString("F2") ?? "-", contentFont)));
            }

            document.Add(table);
        }

        private class PdfFooter : PdfPageEventHelper
        {
            private readonly string _footerText;

            public PdfFooter(string footerText) => _footerText = footerText;

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPTable footer = new PdfPTable(1)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true
                };

                PdfPCell cell = new PdfPCell(new Phrase(_footerText, FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.Gray)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 10f
                };

                footer.AddCell(cell);
                footer.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - 10, writer.DirectContent);
            }
        }
    }
}