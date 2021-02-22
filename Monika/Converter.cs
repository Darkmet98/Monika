using System;
using System.IO;
using System.Threading.Tasks;
using Monika.Rpy;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Monika
{
    public class Converter
    {
        public static void ExportSingleRpy(string rpy)
        {
            var node = NodeFactory.FromFile(rpy); // BinaryFormat
            ExportRpy(node, rpy);
        }

        public static (bool, string) ExportRpyFolder(string rpy, string mapFile)
        {
            var nodes = NodeFactory.FromDirectory(rpy, "*.rpy", "rpys", true);

            try
            {
                Parallel.For(0, nodes.Children.Count, i =>
                {
                    var child = nodes.Children[i];
                    ExportRpy(child, rpy + Path.DirectorySeparatorChar + child.Name, mapFile);
                });
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
            
            return (true, string.Empty);
        }

        private static void ExportRpy(Node rpy, string path, string mapFile = "")
        {
            var rpyConverter = new BinaryFormat2Rpy();
            if (!string.IsNullOrWhiteSpace(mapFile))
                rpyConverter.CharactersFile = mapFile;
            
            rpy.TransformWith(rpyConverter).TransformWith(new Rpy2Po());

            Console.WriteLine("Exporting " + rpy + "...");
            var file = path.Remove(path.Length - 4);
            rpy.TransformWith(new Po2Binary()).Stream.WriteTo(file + ".po");
        }

        public static void ImportRpy(string po)
        {
            // 1
            var nodo = NodeFactory.FromFile(po); // Po
            nodo.TransformWith(new Binary2Po()).TransformWith(new Po2Rpy()).TransformWith(new Rpy2BinaryFormat());

            Console.WriteLine("Importing " + po + "...");
            var file = po.Remove(po.Length - 3);
            nodo.Stream.WriteTo(file + "_new.rpy");
        }

        public static void ImportRpyFix(string po, string rpy)
        {
            // 1
            var nodoPo = NodeFactory.FromFile(po).TransformWith(new Binary2Po()); // Po
            var nodoOr = NodeFactory.FromFile(rpy); // BinaryFormat


            // 2
            var textConverter = new BinaryFormat2Rpy
            {

                PoFix = nodoPo.GetFormatAs<Po>()

            };
            nodoOr.TransformWith(textConverter);

            // 3
            IConverter<Rpy.Rpy, BinaryFormat> rpyConverter = new Rpy2BinaryFormat();
            nodoOr.TransformWith(new Rpy2BinaryFormat());
            //3
            Console.WriteLine("Importing " + po + "...");
            string file = po.Remove(po.Length - 3);
            nodoOr.Stream.WriteTo(file + "_new.rpy");
        }
    }
}
