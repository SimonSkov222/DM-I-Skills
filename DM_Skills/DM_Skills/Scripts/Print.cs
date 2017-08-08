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

        private int _pageNum = 1;
        public int pageNum { set { pageNum = _pageNum; } }

        public void Test(ObservableCollection<Models.TableModelN> models)
        {
            var width = new float[] { 2f, 2f, 1f, 1f, 1f };

            Font fontH = new Font(Font.FontFamily.TIMES_ROMAN, 20f, Font.BOLD);
            Font fontTxt = new Font(Font.FontFamily.TIMES_ROMAN, 14, Font.NORMAL);

            PdfPTable pdfDoc = new PdfPTable(1);
            PdfPTable header = new PdfPTable(5);


            PdfPCell title = new PdfPCell(new Phrase("DM i Skills", fontH));

            pdfDoc.DefaultCell.Border = 0;
            title.Border = 0;

            pdfDoc.SetWidthPercentage(new float[] { PageSize.A4.Width -50 }, PageSize.A4);

            

          

            header.DefaultCell.Border = 2;
            header.SetWidths(width);
            
            header.AddCell("Dektagere");
            header.AddCell("Skole og klasse");
            header.AddCell("Lokation");
            header.AddCell("Dato");
            header.AddCell("Tid");







            

            //table2.AddCell("enghaveskolen");
            //table2.AddCell("Oluwayemisi Winston\nOluwayemisi Winston\nOluwayemisi Winston");
            //table2.AddCell("Ballerup");
            //table2.AddCell("02/04/2017");
            //table2.AddCell("12:11:10");

            //table2.AddCell("enghaveskolen");
            //table2.AddCell("Oluwayemisi Winston");
            //table2.AddCell("Ballerup");
            //table2.AddCell("02/04/2017");
            //table2.AddCell("12:11:10");

            pdfDoc.AddCell(title);
            pdfDoc.AddCell(header);

            foreach (var i in models)
            {
                PdfPTable data = new PdfPTable(5);
                PdfPTable tabPerson = new PdfPTable(1);
                tabPerson.DefaultCell.Border = 0;

                data.DefaultCell.Border = 2;
                data.SetWidths(width);

                foreach (var person in i.Persons)
                {
                    tabPerson.AddCell(person.Name);
                }
                data.AddCell(i.School.Name + "\n" + i.Team.Class);
                data.AddCell(tabPerson);
                data.AddCell(i.Location.Name);
                data.AddCell(i.Team.Date);
                data.AddCell(i.Team.Time);

                pdfDoc.AddCell(data);

            }






            Document doc = new Document(PageSize.A4, 0f, 0f, 20f, 40f);
            var fs = new System.IO.FileStream(@"C:\Users\kide\Desktop\DMiSkills.pdf", System.IO.FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);







            
            writer.PageEvent = this;


            doc.Open();

            pdfDoc.KeepTogether = true;
            for (int i = 0; i < 100; i++)
            {
                doc.Add(pdfDoc);

            }
            doc.Add(pdfDoc);


            doc.Close();
            

        }





        PdfContentByte cb;
        PdfTemplate template;
        BaseFont bf = null;
        DateTime PrintTime = DateTime.Now;
        
       
        private Font _FooterFont;
        public Font FooterFont
        {
            get { return _FooterFont; }
            set { _FooterFont = value; }
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
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
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
