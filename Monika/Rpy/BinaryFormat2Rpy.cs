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
                        string variable = Reader.ReadLine() + "|" + Reader.ReadLine() + "|";
                        Reader.ReadLine();
                        Reader.ReadToToken("#");
                        string name = Reader.ReadToToken("\"");
                        variable += name;
                        Result.Variables.Add(variable);

                        //Esto es temporal, luego añado un diccionario con nombres y demas, por ahora voy a centrarme nada mas que en ddlc
                        if (name.Contains("s")) Result.Names.Add("Sayori");
                        else if(name.Contains("mc")) Result.Names.Add("Main Character");
                        else if (name.Contains("m")) Result.Names.Add("Monika");
                        else if (name.Contains("y")) Result.Names.Add("Yuri");
                        else if (name.Contains("n")) Result.Names.Add("Natsuki");
                        else Result.Names.Add("Narrator");

                        ReadTextToTranslate();
                    }

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
            Result.OriginalText.Add(text.Remove(text.Length-1));

            //Skip the white line and the first values
            Reader.ReadToToken("\"");

            //Translated text
            text = Reader.ReadLine();
            Result.TranslatedText.Add(text.Remove(text.Length - 1));
        }
    }
}
