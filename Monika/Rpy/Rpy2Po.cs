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
    class Rpy2Po : IConverter<Rpy, Po>
    {
        Po po { get; set; }

        public Rpy2Po()
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            po = new Po
            {
                //Por ahora me voy a centrar en hacerlo para DDLC, luego lo hago universal
                Header = new PoHeader("A renpy game", "anyemail@gmail.com", currentCulture.Name)
                {
                    LanguageTeam = "AnyTeam"
                }
            };
        }

        public Po Convert(Rpy source)
        {
            for(int i = 0; i < source.OriginalText.Count; i++)
            {
                PoEntry entry = new PoEntry();
                entry.Context = i.ToString() + "|" + source.Names[i];
                entry.Reference = source.Variables[i];
                entry.Original = source.OriginalText[i];
                if (!String.IsNullOrEmpty(source.TranslatedText[i])) entry.Translated = source.TranslatedText[i];
                entry.ExtractedComments = source.Names[i];
                //Exclusivo para DDLC, luego lo haré configurable
                //entry.Flags = "game-doki, max-length:191";
                po.Add(entry);
            }

            return po;
        }
    }
}
