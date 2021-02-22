using System;
using System.Collections.Generic;
using Yarhl.FileFormat;

namespace Monika.Rpy
{
    public class Rpy : IFormat
    {
        public string Date => "# TODO: Translation updated at " + Convert.ToDateTime(DateTime.Now.ToString("yyyy/M/dd ") + DateTime.Now.ToString("hh:mm:ss")).ToString("yyyy-MM-dd HH:mm") + "\r\n\r\n";
        public bool IsSelection { get; set; }
        private string Language { get; set; }
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

        private string GetLanguage()
        {
            var result = Variables[0].Split(' ');
            return result[2];
        }
        
        public string WriteSelection(int i)
        {
            string result = "";
            if(!IsSelection)
            {
                IsSelection = true;
                Language = GetLanguage();
                result += "translate " + Language + " strings:\r\n\r\n";
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
