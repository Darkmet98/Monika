using System;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace Monika.Rpy
{
    public class Po2Rpy : IConverter<Po, Rpy>
    { public Rpy Convert(Po source)
        {
            var result = new Rpy();

            foreach (var entry in source.Entries)
            {
                result.OriginalText.Add((entry.Original!= "<!empty>")?entry.Original:string.Empty);

                result.Variables.Add(entry.Reference);

                result.Names.Add(entry.ExtractedComments);

                if (!string.IsNullOrEmpty(entry.Translated))
                    result.TranslatedText.Add((entry.Translated != "<!empty>")?entry.Translated:string.Empty);

                else result.TranslatedText.Add("NULL");

            }
            return result;
        }

    }
}
