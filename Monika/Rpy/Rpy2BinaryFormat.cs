using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Monika.Rpy
{
    public class Rpy2BinaryFormat : IConverter<Rpy, BinaryFormat>
    {
        private BinaryFormat Binary;
        private TextWriter Writer;

        public BinaryFormat Convert(Rpy source)
        {
            //Generate the file
            GenerateFile();

            Writer.Write(source.Date);

            for(int i = 0; i < source.OriginalText.Count; i++)
            {
                Writer.Write(source.Names[i] != "Selection" ? source.WriteText(i) : source.WriteSelection(i));
            }

            return Binary;
        }


        private void GenerateFile()
        {
            //Generate the exported file
            Binary = new BinaryFormat();
            Writer = new TextWriter(Binary.Stream, Encoding.UTF8);

            //Add BOM
            var bom = Writer.Encoding.GetPreamble();
            Writer.Stream.Write(bom, 0, bom.Length);
        }
    }
}
