using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace YouTubeReader
{
    public static class XmlHelpers
    {
        public static string? ExtractEnglishCaptionName(this XContainer xml)
        {
            return xml
                .Elements()
                .Where(e => e.Name == "track")
                .Where(e => e.Attribute("lang_code").Value == "en")
                .Select(e => e.Attribute("name").Value)
                .FirstOrDefault();
        }

        public static IReadOnlyList<string> ExtractCaptions(this XContainer xml)
        {
            return xml
                .Elements()
                .Where(e => e.Name == "text")
                .Select(e => e.Value.Trim())
                .Select(HttpUtility.HtmlDecode)
                .ToList()
                .AsReadOnly();
        }
    }
}