// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of Monika.
//
// Monika is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Monika is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Monika. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Text;
using Monika.Lua;
using Monika.Rpy;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;
using TextReader = Yarhl.IO.TextReader;

namespace Monika
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Monika — A easy toolkit for RenPy games for fantranslations by Darkmet98.\nThanks to Pleonex for Yarhl libraries.\nVersion: 1.3");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
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
                        ExportRpy(args[1]);
                    }
                    break;
                case "-import":
                    if (File.Exists(args[1]))
                    {
                        ImportRpy(args[1]);
                    }
                    break;
                case "-fix_import":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                       ImportRpyFix(args[1], args[2]);
                    }
                    break;
                case "-port":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                       Port(args[1], args[2]);
                    }
                    break;
            }
        }

        public static void ExportRpy(string rpy)
        {
            // 1
            var nodo = NodeFactory.FromFile(rpy); // BinaryFormat

            // 2
            IConverter<BinaryFormat, Rpy.Rpy> textConverter = new BinaryFormat2Rpy();
            var nodoRpy = nodo.Transform(textConverter);

            // 3
            IConverter<Rpy.Rpy, Po> poConverter = new Rpy2Po();
            var nodoPo = nodoRpy.Transform(poConverter);

            //4
            Console.WriteLine("Exporting " + rpy + "...");
            var file = rpy.Remove(rpy.Length - 4);
            nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".po");
        }

        public static void ImportRpy(string po)
        {
            // 1
            Node nodo = NodeFactory.FromFile(po); // Po
            nodo.Transform<Po2Binary, BinaryFormat, Po>();

            // 2
            IConverter<Po, Rpy.Rpy> poConverter = new Po2Rpy();
            Node nodoRpy = nodo.Transform(poConverter);

            // 3
            IConverter<Rpy.Rpy, BinaryFormat> rpyConverter = new Rpy2BinaryFormat();
            Node nodoFile = nodoRpy.Transform(rpyConverter);
            //3
            Console.WriteLine("Importing " + po + "...");
            string file = po.Remove(po.Length - 3);
            nodoFile.Stream.WriteTo(file + "_new.rpy");
        }

        public static void ImportRpyFix(string po, string rpy)
        {
            // 1
            Node nodoPo = NodeFactory.FromFile(po); // Po
            nodoPo.Transform<Po2Binary, BinaryFormat, Po>();

            Node nodoOr = NodeFactory.FromFile(rpy); // BinaryFormat


            // 2
            IConverter<BinaryFormat, Rpy.Rpy> textConverter = new BinaryFormat2Rpy {

                PoFix = nodoPo.GetFormatAs<Po>()

            };
            Node nodoRpy = nodoOr.Transform(textConverter);

            // 3
            IConverter<Rpy.Rpy, BinaryFormat> rpyConverter = new Rpy2BinaryFormat();
            Node nodoFile = nodoRpy.Transform(rpyConverter);
            //3
            Console.WriteLine("Importing " + po + "...");
            string file = po.Remove(po.Length - 3);
            nodoFile.Stream.WriteTo(file + "_new.rpy");
        }

        public static void Port(string po, string lua)
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
        }

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
