using System;
using Xamarin.Forms;
using System.IO;

[assembly: Dependency(typeof(Avantxa.Droid.Renderers.SaveAndroid))]
namespace Avantxa.Droid.Renderers
{
    public class SaveAndroid : ISave
    {
        public string Save(MemoryStream stream)
        {
            string root = null;
            string fileName = "PDFAvantxa.pdf";
            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
            }
            Java.IO.File myDir = new Java.IO.File(root + "/Syncfusion");
            myDir.Mkdir();
            Java.IO.File file = new Java.IO.File(myDir, fileName);
            string filePath = file.Path;
            if (file.Exists()) file.Delete();
            Java.IO.FileOutputStream outs = new Java.IO.FileOutputStream(file);
            outs.Write(stream.ToArray());
            _ = file.Path;
            outs.Flush();
            outs.Close();
            return filePath;
        }
    }
}
