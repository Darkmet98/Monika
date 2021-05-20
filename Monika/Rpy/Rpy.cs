using System;
using System.Collections.Generic;
using Yarhl.FileFormat;

namespace Monika.Rpy
{
    public class Rpy : IFormat
    {
        public string Date => "# TODO: Translation updated at " + Convert.ToDateTime(DateTime.Now.ToString("yyyy/M/dd ") + DateTime.Now.ToString("hh:mm:ss")).ToString("yyyy-MM-dd HH:mm") + "\r\n\r\n";
        public bool IsSelection { get; set; }
        public string Language { get; set; }
        public List<string> OriginalText { get; }
        public List<string> TranslatedText { get; }
        public List<string> Variables { get; }
        public List<string> Names { get; }

        public Rpy()
        {
            OriginalText = new List<string>();
            TranslatedText = new List<string>();
            Variables = new List<string>();
            Names = new List<string>();
        }

        private string ori;
        private string tra;
        private string[] variables;

        public string WriteText(int i)
        {
            var result = "";
            ori = FixString(OriginalText[i], true);
            tra = CheckTranslation(i);
            variables = SubstractName(Variables[i]).Split('|');

            result += variables[0] + "\r\n" + variables[1] + "\r\n\r\n";

            if(variables.Length == 4)
            {
                result += "    #" + variables[2] + "\"" + ori + "\""+ variables[3] + "\r\n";

                result += "   " + variables[2] + "\"" + tra + "\"" + variables[3] + "\r\n\r\n";
            }
            else
            {
                result += "    #" + variables[2] + "\"" + FixString(OriginalText[i], true) + "\"\r\n";

                result += "   " + variables[2] + "\"" + CheckTranslation(i) + "\"\r\n\r\n";
            }

            return result;
        }

        public string WriteSelection(int i)
        {
            string result = "";
            if(!IsSelection)
            {
                IsSelection = true;
                result += "translate " + Language + " strings:\r\n\r\n";
            }
            result += Variables[i] + "\r\n";
            result += "    old \"" + FixString(OriginalText[i], true) + "\"\r\n";
            result += "    new \"" + CheckTranslation(i) + "\"\r\n\r\n";

            return result;
        }

        private string CheckTranslation(int i)
        {
            return !TranslatedText[i].Equals("NULL") ? FixString(TranslatedText[i], false) : string.Empty;
        }

        private string FixString(string text, bool oriText)
        {

            var result = text.Replace("\n", "\\n");
            if (!oriText && text.Contains("\\\""))
                return result;

            return oriText ? result : result.Replace("\"", "\\\"");
        }

        private string SubstractName(string variable)
        {
            if (!variable.Contains("NAMEREPLACING"))
                return variable;

            if (!string.IsNullOrWhiteSpace(tra) && !tra.Contains("{CHARA="))
                throw new Exception("You need to place the {CHARA=TEXT} Tag on the translated box.");

            var posEnd = ori.IndexOf("}", StringComparison.InvariantCulture);
            var name = ori.Substring(0, posEnd).Replace("{CHARA=", "").Replace("}", "");
            ori = ori.Substring(posEnd+1);

            if (!string.IsNullOrWhiteSpace(tra))
            {
                posEnd = tra.IndexOf("}", StringComparison.InvariantCulture);
                name = tra.Substring(0, posEnd).Replace("{CHARA=", "").Replace("}", "");
                tra = tra.Substring(posEnd+1);
            }

            return variable.Replace("NAMEREPLACING", $"\"{name}\" ");
        }
    }
}
