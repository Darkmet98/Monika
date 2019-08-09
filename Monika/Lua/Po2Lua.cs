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

using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Monika.Lua
{
    class Po2Lua : IConverter<Po, BinaryFormat>
    {
        public TextReader LuaFile { get; set; }

        public BinaryFormat Convert(Po source)
        {
            var result = new BinaryFormat();
            var Writer = new TextWriter(result.Stream, Encoding.UTF8);
            int i = 0;
            string TextFinal = "";
            do
            {
                if (CheckIfText(LuaFile.PeekLine()) && i < source.Entries.Count && !(source.Entries[i].ExtractedComments.Equals("System text or election")
                    || source.Entries[i].ExtractedComments.Equals("Selection")))
                {

                    TextFinal += "\t\tcw(\'" + ReturnName(source.Entries[i].ExtractedComments) + "\', \"" + DeleteUnnecesaryThings(ReplaceText(source.Entries[i].Translated)) + "\")\n";
                    i++;
                    LuaFile.ReadLine();
                }
                else TextFinal += LuaFile.ReadLine() + "\n";
            }
            while (!LuaFile.Stream.EndOfStream);

            for(i = 0; i < source.Entries.Count; i++ )
            {
                if (source.Entries[i].ExtractedComments.Equals("System text or election")|| source.Entries[i].ExtractedComments.Equals("Selection")) {
                    TextFinal = TextFinal.Replace(DeleteUnnecesaryThings(ReplaceText(source.Entries[i].Original)), DeleteUnnecesaryThings(ReplaceText(source.Entries[i].Translated)));
                }
            }

            Writer.Write(TextFinal);

            /*FileText = ReplaceText(LuaFile.ReadToEnd(), true);
            FileText = DeleteUnnecesaryThings(FileText, true);

            for (int i = 0; i < source.Entries.Count; i++)
            {
                string OriginalText = DeleteUnnecesaryThings(source.Entries[i].Original, false);
                if (!String.IsNullOrEmpty(source.Entries[i].Translated))
                FileText = FileText.Replace(OriginalText, DeleteUnnecesaryThings(ReplaceText(source.Entries[i].Translated, false), false));
            }

            Writer.Write(FileText);*/
            return result;
        }

        private string ReplaceText(string line)
        {
            string result = line;
            foreach (var replace in Variables)
            {
                result = result.Replace(replace.Value, replace.Key);
            }
            return result;
        }

        private bool CheckIfText(string line)
        {
            if (line.Contains("cw(")) return true;
            else if (line.Contains("y \"")) return true;
            else if (line.Contains("n \"")) return true;
            else if (line.Contains("mc \"")) return true;
            else if (line.Contains("bl \"")) return true;
            else if (line.Contains("s \"")) return true;
            else if (line.Contains("m \"")) return true;
            else return false;
        }

        private string ReturnName(string name)
        {
            switch (name)
            {
                case "Sayori":
                    return "s";
                case "Protagonist": //Old version
                case "Main Character":
                    return "mc";
                case "Yuri":
                    return "y";
                case "Narrator":
                    return "bl";
                case "Natsuki":
                    return "n";
                case "Monika":
                    return "m";
                default:
                    return "???";
            }
        }

        private string DeleteUnnecesaryThings(string line)
        {
            return line.Replace("{i}", "").Replace("{/i}", "");
        }

        private Dictionary<string, string> Variables = new Dictionary<string, string>()
        {
            {"\"..player..\"", "[player]" },
            {"\"..ch2_winner..\"", "[ch2_winner]"},
            {"\\'", "'"},
            {"%", "%%"},
        };

        /*private string DeleteUnnecesaryThings(string line, bool Original)
        {
            if (!Original)
                return line.Replace("{i}", "").Replace("{/i}", "");
            else
                return line.Replace("  ", " ").Replace("''", "'");
        }

        private Dictionary<string, string> Variables = new Dictionary<string, string>()
        {
            {"\"..player..\"", "[player]" },
            {"\" .. player .. \"", "[player]" },
            {"\" .. player .. \'", "[player]" },
            {"\' .. player .. \"", "[player]" },
            {"\' .. player .. \'", "[player]" },
            {"\"..ch2_winner..\"", "[ch2_winner]"},
            {"\\'", "'"},
            {"%", "%%"},
        };*/
    }
}
