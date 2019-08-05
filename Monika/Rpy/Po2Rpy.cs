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
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace Monika.Rpy
{
    class Po2Rpy : IConverter<Po, Rpy>
    {
        Rpy Result;

        public Rpy Convert(Po source)
        {
            Result = new Rpy();

            foreach (var entry in source.Entries)
            {
                Result.OriginalText.Add(entry.Original);

                Result.Variables.Add(entry.Reference);

                Result.Names.Add(entry.ExtractedComments);

                if (!String.IsNullOrEmpty(entry.Translated)) Result.TranslatedText.Add(entry.Translated);

                else Result.TranslatedText.Add("NULL");

            }

            return Result;
        }

    }
}
