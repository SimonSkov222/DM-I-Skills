using System;
using System.Collections.ObjectModel;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DM_Skills.Scripts
{
    class Print : PdfPageEventHelper
    {
        Font fontTxt = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL);

        private float[] width = new float[] { 2f, 2f, 1f, 1f, 1f };
        private int _pageNum = 1;
        public int pageNum { set { pageNum = _pageNum; } }
        
        PdfContentByte cb;
        PdfTemplate template;
        BaseFont bf = null;
        DateTime PrintTime = DateTime.Now;
        private float titleHeight;


        public void CreatePDF(string filename, ObservableCollection<Models.TableModelN> models)
        {
            PdfPTable pdfDoc = new PdfPTable(1);
            
            pdfDoc.DefaultCell.Border = 0;
            pdfDoc.SetWidthPercentage(new float[] { PageSize.A4.Width -50 }, PageSize.A4);

            foreach (var i in models)
            {
                PdfPTable data = new PdfPTable(5);

                data.KeepTogether = true;
                data.DefaultCell.Phrase = new Phrase { Font = fontTxt };
                data.DefaultCell.Border = 2;
                data.SetWidths(width);

                data.AddCell(WriteData(i.Persons));
                data.AddCell(WriteData(i.School.Name + "\n" + i.Team.Class));
                
                data.AddCell(WriteData(i.Location.Name, Element.ALIGN_CENTER));
                data.AddCell(WriteData(i.Team.Date    , Element.ALIGN_CENTER));
                data.AddCell(WriteData(i.Team.Time    , Element.ALIGN_CENTER));

                pdfDoc.AddCell(data);

            }

            try
            {
                Document doc = new Document(PageSize.A4, 0f, 0f, 40f, 40f);
                var fs = new FileStream(filename, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);           
            
                writer.PageEvent = this;

                doc.Open();
                doc.Add(pdfDoc);
                doc.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Error!");
            }
            
            

        }

        private PdfPTable WriteData(ObservableCollection<Models.PersonModel> persons)
        {
            PdfPTable tabPerson = new PdfPTable(1);

            tabPerson.DefaultCell.Border = 0;
            tabPerson.DefaultCell.Phrase = new Phrase { Font = fontTxt };
            
            foreach (var person in persons)
            {
                var cell = WriteData(person.Name);
                cell.Border = 0;
                tabPerson.AddCell(cell);
            }

            return tabPerson;
        }

        private PdfPCell WriteData(string text, int horizontal = Element.ALIGN_LEFT)
        {
            return new PdfPCell(new Phrase(text, fontTxt))
                    {
                        Border = 2,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = horizontal
            };
        }




        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Font fontT = new Font(Font.FontFamily.HELVETICA, 20f, Font.BOLD);
            PdfPCell title = new PdfPCell(new Phrase("DM i Skills", fontT)) { Border = 0 };

            Font fontH = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD);
            PdfPTable header = new PdfPTable(5);

            header.TotalWidth = PageSize.A4.Width-50;

            header.DefaultCell.Border = 2;
            header.SetWidths(width);

            if (writer.CurrentPageNumber == 1) {
                title.Colspan = 5;
                header.AddCell(title);
            }

            header.AddCell(new PdfPCell(new Phrase("Deltagere", fontH)) { Border = 2 });
            header.AddCell(new PdfPCell(new Phrase("Skole og klasse", fontH)) { Border = 2 });
            header.AddCell(new PdfPCell(new Phrase("Lokation", fontH)) { Border = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            header.AddCell(new PdfPCell(new Phrase("Dato", fontH)) { Border = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            header.AddCell(new PdfPCell(new Phrase("Tid", fontH)) { Border = 2, HorizontalAlignment = Element.ALIGN_CENTER });

            float height = 0;
            if (writer.CurrentPageNumber == 1)
            {
                titleHeight = header.CalculateHeights();
            }
            else {
                height = titleHeight - header.CalculateHeights();
            }

            header.WriteSelectedRows(0, -1, 25, PageSize.A4.Height - height, writer.DirectContent);
        }
        

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (Exception){}
        }
        
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);


            string text = string.Format("Side {0} af ", writer.PageNumber);
            float len = bf.GetWidthPoint(text, 8);
            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetRight(70), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(template, pageSize.GetRight(70) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Printet den " + PrintTime.ToString(),
            pageSize.GetLeft(40),
            pageSize.GetBottom(30), 0);
            cb.EndText();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();
        }



    }
}
