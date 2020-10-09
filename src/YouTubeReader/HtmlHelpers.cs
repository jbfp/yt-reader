using HtmlAgilityPack;

namespace YouTubeReader
{
    public static class HtmlHelpers
    {
        public static string? ExtractTitle(this HtmlDocument html)
        {
            var titleNode = html.DocumentNode.SelectSingleNode("//head/title");

            if (titleNode != null)
            {
                var title = titleNode.InnerText;
                var idx = title.LastIndexOf(" - YouTube");

                if (idx == -1)
                {
                    return title;
                }

                return title.Remove(idx);
            }

            return null;
        }
    }
}