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

using System.IO;
using NUnit.Framework;
using Monika;
using Monika.Exceptions;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace MonikaTest
{
    public class Tests
    {
        private string[] ExampleText =
        {
            "This human feeds me, i should be a god stand with legs in litter box, but poop outside.",
            "Get scared by doggo also cucumerro open the door, let me out, let me out, let me-out, let me-aow, let meaow, meaow!",
            "for climb leg sniff all the things lick the \\\"plastic bag\\\" fall asleep on the washing machine good now the other hand, too.",
            "Plays league of legends eat all the power cords.",
            "Curl into a furry donut make it to the carpet before i vomit mmmmmm.",
            "Example1",
            "Example2"

        };

        private string[] ExampleCharacters =
        {
            "Mika",
            "Mika",
            "Fivi",
            "Fivi",
            "Narrator",
            "Selection",
            "Selection"
        };
        
        [Test, Order(1)]
        public void TestExportFile()
        {
            Program.ExportRpy("Example.rpy");
            Assert.IsTrue(File.Exists("Example.po"));
        }

        [Test, Order(2)]
        public void TestPoContent()
        {
            var poFile = new DataStream("Example.po", FileOpenMode.Read);
            var poBinary = new BinaryFormat(poFile);
            var po = poBinary.ConvertTo<Po>();

            for (var i = 0; i < 7; i++)
            {
                Assert.AreEqual(ExampleText[i],po.Entries[i].Original);
                Assert.AreEqual(i + "|" + ExampleCharacters[i],po.Entries[i].Context);
                Assert.AreEqual(ExampleCharacters[i],po.Entries[i].ExtractedComments);
            }
                
            
            Assert.AreEqual("Small kitty warm kitty little balls of fur" +
                            " meowwww and lick face hiss at owner, pee a lot," +
                            " and meow repeatedly scratch at fence purrrrrr" +
                            " eat muffins and poutine until owner comes back.",po.Entries[3].Translated);

        }
        
        [Test, Order(3)]
        public void TestNotRpyFile()
        {
            Assert.Throws<NotRpyFile>( () => { Program.ExportRpy("Example2.rpy"); });
        }
        
        [Test, Order(4)]
        public void TestImportRpy()
        {
            Program.ImportRpy("Example.po");
            Assert.IsTrue(File.Exists("Example_new.rpy"));
        }

        [Test, Order(5)]
        public void TestRpyImported()
        {
            string[] or = File.ReadAllLines("Example.rpy");
            string[] mod = File.ReadAllLines("Example_new.rpy");

            for (int i = 1; i < or.Length; i++)
            {
                Assert.AreEqual(or[i], mod[i]);
            }
        }
    }
}