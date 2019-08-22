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

namespace Monika.Rpy
{
    class BinaryFormat2Rpy : IConverter<BinaryFormat, Rpy>
    {
        TextReader Reader;
        Rpy Result;
        string Variable;

        public Rpy Convert(BinaryFormat source)
        {
            Result = new Rpy();
            Reader = new TextReader(source.Stream, Encoding.UTF8);

            //Skip the first line
            Reader.ReadLine();

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

                        ReadTextToTranslate();
                        Result.Variables.Add(Variable);
                    }
                    Variable = "";
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

            //Translated text
            text = Reader.ReadLine();
            if (text.Remove(0, text.Length - 1).Equals("\"")) Result.TranslatedText.Add(text.Remove(text.Length - 1));
            else
            {
                int final_position = text.IndexOf("\" ");
                if(final_position == 0) Result.TranslatedText.Add(text.Remove(final_position));
                else Result.TranslatedText.Add(text.Remove(final_position));
            }
        }
    }
}
