using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;
using System.IO;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace DDLCTranslatorToolkit.PC
{
    class Import
    {
        public void Imports(string file)
        {
            Console.WriteLine("Importing " + file + "...");
            Po po = new BinaryFormat(new DataStream(file, FileOpenMode.Read)).ConvertTo<Po>(); //Flan code
            file = file.Remove(file.Length - 3);
            Yarhl.IO.TextWriter writer = new Yarhl.IO.TextWriter(new DataStream(file + ".rpy", FileOpenMode.Write));
            byte[] bom = writer.Encoding.GetPreamble();
            writer.Stream.Write(bom, 0, bom.Length);
            writer.NewLine = "\r\n";
            string date = DateTime.Now.ToString("yyyy/M/dd ");
            string time = DateTime.Now.ToString("hh:mm:ss");
            writer.WriteLine("# TODO: Translation updated at " + Convert.ToDateTime(date + time).ToString("yyyy-MM-dd HH:mm") + "\r\n");
            foreach (var entry in po.Entries)
            {
                string potext = string.IsNullOrEmpty(entry.Translated) ?
                    entry.Original : entry.Translated;
                entry.Original = entry.Original.Replace("\n", "\\n");
                potext = potext.Replace("\n", "\\n");
                if (entry.ExtractedComments == "Selection" && Values.strings == false)
                {
                    Values.strings = true;
                    writer.WriteLine("translate Spanish strings:\r\n");
                }
                if (entry.ExtractedComments != "Selection" && Values.strings == false)
                {
                    writer.WriteLine(entry.Context);
                    writer.WriteLine(entry.ExtractedComments + "\r\n");
                    //writer.WriteLine("    " + entry.Reference + " \"" + entry.Original + "\"");
                    writer.WriteLine(entry.Reference + "\"" + entry.Original + "\"");
                    if (entry.Reference != "    #") { writer.WriteLine("  " + entry.Reference.Substring(5) + " \"" + potext + "\"\r\n"); }
                    else if (entry.Reference == "    #") { writer.WriteLine("    " + "\"" + potext + "\"\r\n"); }
                }
                else if (entry.ExtractedComments == "Selection" && Values.strings == true)
                {
                    writer.WriteLine(entry.Context);
                    writer.WriteLine("    old \"" + entry.Original + "\"");
                    writer.WriteLine("    new \"" + potext + "\"\r\n");
                }
            }
        }
    }
}
