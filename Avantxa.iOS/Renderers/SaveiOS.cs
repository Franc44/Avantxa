using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(Avantxa.iOS.Renderers.SaveiOS))]
namespace Avantxa.iOS.Renderers
{
    public class SaveiOS : ISave
    {
        public string Save(MemoryStream fileStream)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filepath = Path.Combine(path, "PDFAvantxa.pdf");

            FileStream outputFileStream = File.Open(filepath, FileMode.Create);
            fileStream.Position = 0;
            fileStream.CopyTo(outputFileStream);
            outputFileStream.Close();
            return filepath;

        }
    }
}
