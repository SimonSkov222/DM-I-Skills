using System;
using System.Collections.ObjectModel;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Threading.Tasks;



namespace DM_Skills.Scripts
{
    class Print
    {
        public void Test(ObservableCollection<Models.TableModelN> models)
        {
            PdfPTable table = new PdfPTable(5);
            PdfPTable table2 = new PdfPTable(4);
            table.DefaultCell.Border = 2;
            

            table2.AddCell("H");
            table2.AddCell("H");
            table2.AddCell("H");
            table2.AddCell("H");

            table2.AddCell("H");
            table2.AddCell("");
            table2.AddCell("");
            table2.AddCell("");



            table.AddCell("Skole");
            table.AddCell("Klasse");
            table.AddCell("Lokation");
            table.AddCell("Dato");
            table.AddCell("Tid");


            //table.AddCell(new PdfPCell(new Phrase("Cell text")) {  Border = 0});
            //table.AddCell(table2);

            Document doc = new Document();
            var fs = new System.IO.FileStream(@"C:\Users\kide\Desktop\DMiSkills.pdf", System.IO.FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();
            doc.Add(table);
            doc.Close();
        }

    }
}
