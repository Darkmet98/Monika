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
    class Rpy2BinaryFormat : IConverter<Rpy, BinaryFormat>
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
                if (source.Names[i] != "Selection") Writer.Write(source.WriteText(i));
                else Writer.Write(source.WriteSelection(i));
            }

            return Binary;
        }


        private void GenerateFile()
        {
            //Generate the exported file
            Binary = new BinaryFormat();
            Writer = new TextWriter(Binary.Stream, Encoding.UTF8);

            //Add BOM
            byte[] bom = Writer.Encoding.GetPreamble();
            Writer.Stream.Write(bom, 0, bom.Length);
        }
    }
}
