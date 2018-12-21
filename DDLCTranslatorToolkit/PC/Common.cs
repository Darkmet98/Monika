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
    public class Common
    {
        static public void Po(string file, List<string> text, List<string> values, List<string> line, List<string> control)
        {
            Po po = new Po
            {
                Header = new PoHeader("Doki Doki Literature Club", "glowtranslations@gmail.com", "es")
                {
                    LanguageTeam = "GlowTranslations",
                }
            };
            for (int i = 0; i < text.Count; i++)
            {
                PoEntry entry = new PoEntry();
                entry.Context = line[i];
                entry.Reference = control[i];
                entry.Original = text[i];
                entry.ExtractedComments = values[i];
                entry.Flags = "game-doki, max-length:191";
                po.Add(entry);
            }
            file = file.Remove(file.Length - 3);
            po.ConvertTo<BinaryFormat>().Stream.WriteTo(file + "po");
        }
    }
}
