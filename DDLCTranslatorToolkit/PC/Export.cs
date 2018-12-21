using System;

namespace DDLCTranslatorToolkit.PC
{
    public class Export
    {
        public void Exports(string file)
        {
            Console.WriteLine("Exporting " + file + "...");
            string[] textfile = System.IO.File.ReadAllLines(file);
            foreach (string line in textfile)
            {
                Values.check = "";
                if (line != "" && Values.strings == false) { Values.check = line.Substring(0, 2); }
                else if (line != "" && Values.strings == true) { Values.check = line.Substring(0, 5); }
                if (line == "translate Spanish strings:")
                {
                    Values.strings = true;
                    Values.check = line.Substring(0, 5);
                }
                if (Values.check == "# " && Values.strings == false)
                {
                    Values.check = "";
                    Values.check = line.Substring(0, 6);
                    if (Values.check != "# TODO")
                    Values.line.Add(line);
                }
                else if (Values.strings == true && Values.check == "    #")
                {
                    Values.line.Add(line);
                }
                else if (Values.strings == false && Values.check == "tr")
                {
                    Values.values.Add(line);
                }
                else if (Values.strings == true && Values.check == "    o")
                {
                    Values.check = "";
                    Values.check = line.Substring(0, 5);
                    string[] lineFields = line.Split('"');
                    foreach (string part in lineFields)
                    {
                        if (lineFields[1] == part)
                        {
                            Values.control.Add(lineFields[0]);
                            Values.text.Add(lineFields[1]);
                        }

                    }
                    Values.values.Add("Selection");
                }
                else if (Values.strings == false && Values.check == "  ")
                {
                    Values.check = "";
                    Values.check = line.Substring(0, 5);
                    if (Values.check == "    #")
                    {
                        string[] lineFields = line.Split('"');
                        foreach (string part in lineFields)
                        {
                            if (lineFields[1] == part)
                            {
                                Values.control.Add(lineFields[0]);
                                Values.text.Add(lineFields[1]);
                            }
                            
                        }
                    }
                }
            }
            Common.Po(file, Values.text, Values.values, Values.line, Values.control);
        }
    }
}
