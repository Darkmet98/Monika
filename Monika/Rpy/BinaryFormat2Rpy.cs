using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Monika.Exceptions;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;
using TextReader = Yarhl.IO.TextReader;

namespace Monika.Rpy
{
    public class BinaryFormat2Rpy : IConverter<BinaryFormat, Rpy>
    {
        private TextReader reader;
        private Rpy result;
        private string variable;
        public Po PoFix { get; set; }

        public string CharactersFile { get; set; } =
            $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Characters.map";
        int Count { get; set; }
        private Dictionary<string, string> Map { get; set; }

        public BinaryFormat2Rpy() {
            Map = new Dictionary<string, string>();
        }
        
        public Rpy Convert(BinaryFormat source)
        {
            //Check if the dictionary exist
            if (File.Exists(CharactersFile))
                GenerateFontMap(CharactersFile);
            
            result = new Rpy();
            reader = new TextReader(source.Stream, Encoding.UTF8);

            //Check if a Rpy file
            var check = reader.ReadLine();
            if (!check.Contains("# TODO: Translation updated at "))
                throw new NotRpyFile();

            //Initialize the Count
            Count = 0;
            do
            {
                if (!string.IsNullOrEmpty(reader.PeekLine()))
                {
                    if (reader.PeekLine().Contains("strings") && !result.IsSelection)
                    {
                        result.IsSelection = true;
                        //Skip the white line
                        reader.ReadLine();
                        reader.ReadLine();
                    }

                    if (result.IsSelection)
                    {
                        result.Variables.Add(reader.ReadLine());
                        result.Names.Add("Selection");
                        reader.ReadToToken("\"");
                        ReadTextToTranslateD();
                    }
                    else
                    {
                        //Read the two variables
                        variable = reader.ReadLine() + "|" + reader.ReadLine() + "|";
                        reader.ReadLine();
                        reader.ReadToToken("#");
                        var line = reader.ReadLine();
                        ReadTextToTranslate(line, false);
                        ReadTextToTranslate(reader.ReadLine(), true);
                        result.Variables.Add(variable);
                        /*string name = reader.ReadToToken("\"");
                        variable += name;
                        result.Names.Add(CheckName(name));
                        ReadTextToTranslate();
                        ;*/
                    }
                    variable = "";
                    Count++;
                }
                else reader.ReadLine();
            }
            while (!reader.Stream.EndOfStream);

            return result;
        }


        private void ReadTextToTranslate(string line, bool translationZone)
        {
            var split = line.Replace("\\\"", "{QUOTES}").Split('\"');

            if (split.Length == 3)
            {
                if (!translationZone)
                {
                    variable += split[0];
                    result.Names.Add(CheckName(split[0]));
                    result.OriginalText.Add(split[1].Replace("{QUOTES}", "\\\""));
                    variable += "|" + split[2];
                }
                else
                    result.TranslatedText.Add(PoFix == null ? split[1].Replace("{QUOTES}", "\\\"") : PoFix.Entries[Count].Translated);

            }
            else
            {
                if (!translationZone)
                {
                    variable += split[0] + "NAMEREPLACING";
                    result.Names.Add("NAME REPLACING");
                    result.OriginalText.Add("{CHARA=" + split[1] + "}" + split[3].Replace("{QUOTES}", "\\\""));
                    variable += "|" + split[4];
                }
                else
                    result.TranslatedText.Add(PoFix == null ? split[3].Replace("{QUOTES}", "\\\"") : PoFix.Entries[Count].Translated);

            }
        }

        private void ReadTextToTranslateD()
        {
            //Original text
            string text = reader.ReadLine();
            if(text.Remove(0,text.Length - 1).Equals("\"")) result.OriginalText.Add(text.Remove(text.Length-1));
            else
            {
                int finalPosition = text.IndexOf("\" ");
                result.OriginalText.Add(text.Remove(finalPosition));
                variable += "|" + text.Remove(0, finalPosition+1);
            }

            //Skip the white line and the first values
            reader.ReadToToken("\"");


            if(PoFix == null)
            {
                //Translated text
                text = reader.ReadLine();
                if (text.Remove(0, text.Length - 1).Equals("\"")) result.TranslatedText.Add(text.Remove(text.Length - 1));
                else
                {
                    int finalPosition = text.IndexOf("\" ");
                    if (finalPosition == 0) result.TranslatedText.Add(text.Remove(finalPosition));
                    else result.TranslatedText.Add(text.Remove(finalPosition));
                }
            }
            else
            {
                reader.ReadLine();
                result.TranslatedText.Add(PoFix.Entries[Count].Translated);
            }
        }

        private string CheckName(string value)
        {
            if (Map.Count != 0)
            {
                foreach (var replace in Map) {
                    if (value.Contains(replace.Key)) return replace.Value;
                }

                return "Narrator";
            }
            else
                return "No name";
        }
        
        private void GenerateFontMap(string file)
        {
            try
            {
                var dictionary = File.ReadAllLines(file);
                foreach (string line in dictionary)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var lineFields = line.Split('=');
                    Map.Add(lineFields[0], lineFields[1]);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"The dictionary is wrong, please, check the wiki and fix it.\n{e.Message}");
            }
        }
    }
}
