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
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Monika
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Monika — A easy toolkit for RenPy games for fantranslations by Darkmet98.\nThanks to Pleonex for Yarhl libraries.\nVersion: 1.1");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("\nUsage: Monika <-export/-import>");
                Console.WriteLine("Export Rpy to Po: Monika -export \"script-ch0.rpy\"");
                Console.WriteLine("Export Po to Rpy: Monika -import \"script-ch0.po\"");
                Console.WriteLine("Fix Po import if the translation program (Like PoEdit) broke the Po: Monika -fix_import \"script-ch0.po\" \"script-ch0.rpy\"");
                Console.WriteLine("Port Po to Luke DDLC's Lua file: Monika -port \"script-ch0.po\" \"script-ch0.lua\"");
                return;
            }
            switch (args[0])
            {
                case "-export":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        IConverter<BinaryFormat, Rpy.Rpy> TextConverter = new Rpy.BinaryFormat2Rpy { };
                        Node nodoRpy = nodo.Transform(TextConverter);

                        // 3
                        IConverter<Rpy.Rpy, Po> PoConverter = new Rpy.Rpy2Po { };
                        Node nodoPo = nodoRpy.Transform(PoConverter);

                        //4
                        Console.WriteLine("Exporting " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".po");
                    }
                    break;
                case "-import":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po
                        nodo.Transform<Po2Binary, BinaryFormat, Po>();

                        // 2
                        IConverter<Po, Rpy.Rpy> PoConverter = new Rpy.Po2Rpy { };
                        Node nodoRpy = nodo.Transform(PoConverter);

                        // 3
                        IConverter<Rpy.Rpy, BinaryFormat> RpyConverter = new Rpy.Rpy2BinaryFormat { };
                        Node nodoFile = nodoRpy.Transform(RpyConverter);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 3);
                        nodoFile.Stream.WriteTo(file + "_new.rpy");
                    }
                    break;
                case "-fix_import":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                        // 1
                        Node nodoPo = NodeFactory.FromFile(args[1]); // Po
                        nodoPo.Transform<Po2Binary, BinaryFormat, Po>();

                        Node nodoOr = NodeFactory.FromFile(args[2]); // BinaryFormat


                        // 2
                        IConverter<BinaryFormat, Rpy.Rpy> TextConverter = new Rpy.BinaryFormat2Rpy {

                            PoFix = nodoPo.GetFormatAs<Po>()

                        };
                        Node nodoRpy = nodoOr.Transform(TextConverter);

                        // 3
                        IConverter<Rpy.Rpy, BinaryFormat> RpyConverter = new Rpy.Rpy2BinaryFormat { };
                        Node nodoFile = nodoRpy.Transform(RpyConverter);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 3);
                        nodoFile.Stream.WriteTo(file + "_new.rpy");
                    }
                    break;
                case "-port":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po
                        nodo.Transform<Po2Binary, BinaryFormat, Po>();

                        // 2
                        Lua.Po2Lua importer = new Lua.Po2Lua
                        {
                            LuaFile = new Yarhl.IO.TextReader(new DataStream(args[2], FileOpenMode.Read), Encoding.UTF8)
                        };

                        Node nodoDat = nodo.Transform(importer);

                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoDat.Stream.WriteTo(file + "_new.lua");
                    }
                    break;
            }
        }
    }
}
