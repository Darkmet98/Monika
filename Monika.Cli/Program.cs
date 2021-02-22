using System;
using System.IO;

namespace Monika
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Monika — A easy toolkit for RenPy games for fantranslations by Darkmet98.\nThanks to Pleonex for Yarhl libraries.\nVersion: 1.4");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                Info();
                return;
            }
            switch (args[0])
            {
                case "-export":
                    if (File.Exists(args[1]))
                    {
                        Converter.ExportSingleRpy(args[1]);
                    }
                    break;
                case "-import":
                    if (File.Exists(args[1]))
                    {
                        Converter.ImportRpy(args[1]);
                    }
                    break;
                case "-fix_import":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                       Converter.ImportRpyFix(args[1], args[2]);
                    }
                    break;
                /*case "-port":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                       Port(args[1], args[2]);
                    }
                    break;*/
            }
        }

        /*public static void Port(string po, string lua)
        {
            Console.WriteLine("\n\nWARNING, THIS FUNCTION IS ON ALPHA STAGE, DO NOT USE FOR NOW!!!!\n\n");
            // 1
            Node nodo = NodeFactory.FromFile(po); // Po
            nodo.Transform<Po2Binary, BinaryFormat, Po>();

            // 2
            Po2Lua importer = new Po2Lua
            {
                LuaFile = File.ReadAllLines(lua)
            };

            Node nodoDat = nodo.Transform(importer);

            //3
            Console.WriteLine("Importing " + po + "...");
            string file = po.Remove(po.Length - 3);
            nodoDat.Stream.WriteTo(file + "_new.lua");
        }*/

        public static void Info()
        {
            Console.WriteLine("\nUsage: Monika <-export/-import> \"File1\" \"File2\" ");
            Console.WriteLine("Export Rpy to Po: Monika -export \"script-ch0.rpy\" ");
            Console.WriteLine("Export Po to Rpy: Monika -import \"script-ch0.po\"");
            Console.WriteLine("Fix Po import if the translation program (Like PoEdit) broke the Po: Monika -fix_import \"script-ch0.po\" \"script-ch0.rpy\"");
            Console.WriteLine("Port Po to Luke DDLC's Lua file: Monika -port \"script-ch0.po\" \"script-ch0.lua\"");
        }
    }
}
