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
using System.Collections.Generic;
using System.Text;
using Monika.Exceptions;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Monika.Rpy
{
    class BinaryFormat2Rpy : IConverter<BinaryFormat, Rpy>
    {
        TextReader Reader { get; set; }
        Rpy Result { get; set; }
        string Variable { get; set; }
        public Po PoFix { get; set; }
        int Count { get; set; }
        private Dictionary<string, string> Map { get; set; }

        public BinaryFormat2Rpy() {
            Map = new Dictionary<string, string>();
        }
        
        public Rpy Convert(BinaryFormat source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("Characters.map")) GenerateFontMap("Characters.map");
            
            Result = new Rpy();
            Reader = new TextReader(source.Stream, Encoding.UTF8);

            //Check if a Rpy file
            var check = Reader.ReadLine();
            if (!check.Contains("# TODO: Translation updated at ")) throw new NotRpyFile();

                //Initialize the Count
            Count = 0;
            do
            {
                if (!string.IsNullOrEmpty(Reader.PeekLine()))
                {
                    if (Reader.PeekLine().Contains("strings") && !Result.IsSelection)
                    {
                        Result.IsSelection = true;
                        //Skip the white line
                        Reader.ReadLine();
                        Reader.ReadLine();
                    }

                    if (Result.IsSelection)
                    {
                        Result.Variables.Add(Reader.ReadLine());
                        Result.Names.Add("Selection");
                        Reader.ReadToToken("\"");
                        ReadTextToTranslate();
                    }
                    else
                    {
                        //Read the two variables
                        Variable = Reader.ReadLine() + "|" + Reader.ReadLine() + "|";
                        Reader.ReadLine();
                        Reader.ReadToToken("#");
                        string name = Reader.ReadToToken("\"");
                        Variable += name;
                        Result.Names.Add(CheckName(name));
                        ReadTextToTranslate();
                        Result.Variables.Add(Variable);
                    }
                    Variable = "";
                    Count++;
                }
                else Reader.ReadLine();
            }
            while (!Reader.Stream.EndOfStream);

            return Result;
        }

        private void ReadTextToTranslate()
        {
            //Original text
            string text = Reader.ReadLine();
            if(text.Remove(0,text.Length - 1).Equals("\"")) Result.OriginalText.Add(text.Remove(text.Length-1));
            else
            {
                int finalPosition = text.IndexOf("\" ");
                Result.OriginalText.Add(text.Remove(finalPosition));
                Variable += "|" + text.Remove(0, finalPosition+1);
            }

            //Skip the white line and the first values
            Reader.ReadToToken("\"");


            if(PoFix == null)
            {
                //Translated text
                text = Reader.ReadLine();
                if (text.Remove(0, text.Length - 1).Equals("\"")) Result.TranslatedText.Add(text.Remove(text.Length - 1));
                else
                {
                    int finalPosition = text.IndexOf("\" ");
                    if (finalPosition == 0) Result.TranslatedText.Add(text.Remove(finalPosition));
                    else Result.TranslatedText.Add(text.Remove(finalPosition));
                }
            }
            else
            {
                Reader.ReadLine();
                Result.TranslatedText.Add(PoFix.Entries[Count].Translated);
            }
        }

        private string CheckName(string value)
        {
            if (Map.Count != 0)
            {
                foreach (var replace in Map) {
                    if (value.Contains(replace.Key)) return replace.Value;
                }

                return "Narrator";
            }
            else
                return "No name";
        }
        
        private void GenerateFontMap(string file)
        {
            try
            {
                string[] dictionary = System.IO.File.ReadAllLines(file);
                foreach (string line in dictionary)
                {
                    string[] lineFields = line.Split('=');
                    Map.Add(lineFields[0], lineFields[1]);
                }
            }
            catch (Exception e)
            {
                Console.Beep();
                Console.WriteLine("The dictionary is wrong, please, check the wiki and fix it.");
                Console.WriteLine(e);
                Environment.Exit(-1);
            }
        }
    }
}
