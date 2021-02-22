using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;
using TextWriter = Yarhl.IO.TextWriter;

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
            switch (name)
            {
                case "Sayori":
                    return "s";
                case "Protagonist":
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
            return line.Replace("{i}", "").Replace("{/i}", "").Replace("{nw}", "").Replace("{fast}", "");
        }

        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>()
        {
            {"[player]", "\" .. player .. \""  },
            {"[ch2_winner]", "\"..ch2_winner..\""},
            {"[ch4_name]", "\"..savevalue..\""},
            {"[s_name]", "\"..s_name..\""},
            {"[currentuser]", "\"..currentuser..\""},
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
