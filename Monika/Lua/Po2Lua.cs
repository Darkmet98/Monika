// Copyright (C) 2020 Pedro Garau Martínez
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
using TextWriter = Yarhl.IO.TextWriter;

// ReSharper disable IdentifierTypo

namespace Monika.Lua
{
    class Po2Lua : IConverter<Po, BinaryFormat>
    {
        public string[] LuaFile { get; set; }
        private string[] TranslatedTextAdapted { get; set; }
        private string[] OriginalTextAdapted { get; set; }
        private Po Po { get; set; }

        public BinaryFormat Convert(Po source)
        {
            
            Po = source;
            GeneratePortedText();
            PortTranslation();

            return GenerateBinary();
        }


        private void GeneratePortedText()
        {
            TranslatedTextAdapted = new string[Po.Entries.Count];
            OriginalTextAdapted = new string[Po.Entries.Count];

            for (int i = 0; i < Po.Entries.Count; i++)
            {
                OriginalTextAdapted[i] = ReplaceText(DeleteUnnecesaryThings(Po.Entries[i].Original), _variables);
                TranslatedTextAdapted[i] = ReplaceText(DeleteUnnecesaryThings(Po.Entries[i].Translated), _variables);
            }
        }

        private void PortTranslation()
        {
            var start = 0;
            for (int i = 0; i < OriginalTextAdapted.Length; i++)
            {
                for (int j = start; j < LuaFile.Length; j++)
                {
                    var checkText = ReplaceText(LuaFile[j], _fixVariables);
                    if (checkText.Contains(OriginalTextAdapted[i]))
                    {
                        LuaFile[j] = "\t\tcw(\'" + ReturnName(Po.Entries[i].ExtractedComments) + "\', \"" + TranslatedTextAdapted[i] + "\")";
                        //start = j+1;
                        break;
                    }
                }
            }
        }

        private string ReplaceText(string line, Dictionary<string,string> dic)
        {
            string result = line;
            foreach (var replace in dic)
            {
                result = result.Replace(replace.Key, replace.Value);
            }
            return result;
        }

        private BinaryFormat GenerateBinary()
        {
            var result = new TextWriter(new DataStream(), Encoding.UTF8) {NewLine = "\r\n"};
            foreach (var text in LuaFile)
            {
             result.WriteLine(text);   
            }
            return new BinaryFormat(result.Stream);
        }

        private string ReturnName(string name)
        {
            return name switch
            {
                "Sayori" => "s",
                "Protagonist" => //Old version
                "mc",
                "Main Character" => "mc",
                "Yuri" => "y",
                "Narrator" => "bl",
                "Natsuki" => "n",
                "Monika" => "m",
                _ => "???"
            };
        }

        private string DeleteUnnecesaryThings(string line)
        {
            return line.Replace("{i}", "").Replace("{/i}", "");
        }

        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>()
        {
            {"[player]", "\" .. player .. \""  },
            {"[ch2_winner]", "\"..ch2_winner..\""},
            {"%%", "%"},
        };

        private readonly Dictionary<string, string> _fixVariables = new Dictionary<string, string>()
        {
            {"\\'", "\'"  },
            {"' .. player .. '", "\" .. player .. \"" },
            {"\" .. player .. \'", "\" .. player .. \"" },
            {"\"..player..\"", "\" .. player .. \"" },
            {"'..player..'", "\" .. player .. \"" },
            {"'.. player .. '", "\" .. player .. \"" },
            {",player .. '", ",\" .. player .. \""  },
            {",player..\"", ",\" .. player .. \""  }
        };
    }
}
