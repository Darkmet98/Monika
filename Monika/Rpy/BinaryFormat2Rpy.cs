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

using System.Text;
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

        public Rpy Convert(BinaryFormat source)
        {
            Result = new Rpy();
            Reader = new TextReader(source.Stream, Encoding.UTF8);

            //Skip the first line
            Reader.ReadLine();

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

                        //Esto es temporal, luego añado un diccionario con nombres y demas, por ahora voy a centrarme nada mas que en ddlc
                        if (name.Contains("s")) Result.Names.Add("Sayori");
                        else if(name.Contains("mc")) Result.Names.Add("Main Character");
                        else if (name.Contains("m")) Result.Names.Add("Monika");
                        else if (name.Contains("y")) Result.Names.Add("Yuri");
                        else if (name.Contains("n")) Result.Names.Add("Natsuki");
                        else Result.Names.Add("Narrator");

                        //Temporal, solo para el juego este de Sakura Sadist
                        /*if (name.Contains("m")) Result.Names.Add("Mamiko");
                        else if(name.Contains("a")) Result.Names.Add("Azusa");
                        else if (name.Contains("ma")) Result.Names.Add("Mari");
                        else if (name.Contains("marilyn")) Result.Names.Add("Marilyn");
                        else if (name.Contains("marilyn_")) Result.Names.Add("Marilyn Mari");
                        else if (name.Contains("w")) Result.Names.Add("???");
                        else if (name.Contains("tm")) Result.Names.Add("Twintailed maid");
                        else if (name.Contains("c")) Result.Names.Add("Customer #1");
                        else if (name.Contains("cc")) Result.Names.Add("Customer #2");
                        else if (name.Contains("ccc")) Result.Names.Add("Customer #3");
                        else if (name.Contains("s")) Result.Names.Add("Professor Shibata");
                        else if (name.Contains("dad")) Result.Names.Add("Dad");
                        else if (name.Contains("mom")) Result.Names.Add("Mom");
                        else if (name.Contains("k")) Result.Names.Add("Kotoko");
                        else if (name.Contains("maid")) Result.Names.Add("Maid");
                        else if (name.Contains("li")) Result.Names.Add("Librarian");
                        else Result.Names.Add("Narrator");*/

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
                int final_position = text.IndexOf("\" ");
                Result.OriginalText.Add(text.Remove(final_position));
                Variable += "|" + text.Remove(0, final_position+1);
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
                    int final_position = text.IndexOf("\" ");
                    if (final_position == 0) Result.TranslatedText.Add(text.Remove(final_position));
                    else Result.TranslatedText.Add(text.Remove(final_position));
                }
            }
            else
            {
                Reader.ReadLine();
                Result.TranslatedText.Add(PoFix.Entries[Count].Translated);
            }
        }
    }
}
