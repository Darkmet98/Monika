using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace Monika.Rpy
{
    public class Rpy2Po : IConverter<Rpy, Po>
    {
        private Po po;

        public Rpy2Po()
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            po = new Po
            {
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
                var entry = new PoEntry
                {
                    Context = i + "|" + source.Names[i],
                    Reference = source.Variables[i],
                    Original = (!string.IsNullOrEmpty(source.OriginalText[i])) ? source.OriginalText[i] : "<!empty>"
                };
                if (!string.IsNullOrEmpty(source.TranslatedText[i]))
                    entry.Translated = source.TranslatedText[i];
                entry.ExtractedComments = source.Names[i];
                po.Add(entry);
            }

            return po;
        }
    }
}
