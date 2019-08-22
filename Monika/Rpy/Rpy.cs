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
using Yarhl.FileFormat;

namespace Monika.Rpy
{
    class Rpy : Format
    {
        public string Date => "# TODO: Translation updated at " + Convert.ToDateTime(DateTime.Now.ToString("yyyy/M/dd ") + DateTime.Now.ToString("hh:mm:ss")).ToString("yyyy-MM-dd HH:mm") + "\r\n\r\n";
        public bool IsSelection { get; set; }
        public string Language { get; set; }
        public List<string> OriginalText { get; set; }
        public List<string> TranslatedText { get; set; }
        public List<string> Variables { get; set; }
        public List<string> Names { get; set; }

        public Rpy()
        {
            OriginalText = new List<string>();
            TranslatedText = new List<string>();
            Variables = new List<string>();
            Names = new List<string>();
        }

        public string WriteText(int i)
        {
            string result = "";
            string[] var = Variables[i].Split('|');

            result += var[0] + "\r\n" + var[1] + "\r\n\r\n";

            if(var.Length == 4)
            {
                result += "    #" + var[2] + "\"" + FixString(OriginalText[i]) + "\""+ var[3] + "\r\n";

                result += "   " + var[2] + "\"" + CheckTranslation(i) + "\"" + var[3] + "\r\n\r\n";
            }
            else
            {
                result += "    #" + var[2] + "\"" + FixString(OriginalText[i]) + "\"\r\n";

                result += "   " + var[2] + "\"" + CheckTranslation(i) + "\"\r\n\r\n";
            }

            return result;
        }

        public string WriteSelection(int i)
        {
            string result = "";
            if(!IsSelection)
            {
                IsSelection = true;
                //Por ahora voy a hacerlo para DDLC, luego lo hago universal
                result += "translate Spanish strings:\r\n\r\n";
            }
            result += Variables[i] + "\r\n";
            result += "    old \"" + FixString(OriginalText[i]) + "\"\r\n";
            result += "    new \"" + CheckTranslation(i) + "\"\r\n\r\n";

            return result;
        }

        private string CheckTranslation(int i)
        {
            if (!TranslatedText[i].Equals("NULL")) return FixString(TranslatedText[i]);
            else return "";
        }

        private string FixString(string text)
        {
            return text.Replace("\n", "\\n").Replace("\\\\", "\\");
        }
    }
}
