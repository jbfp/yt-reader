using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace YouTubeReader
{
    public static class TextHelpers
    {
        private static readonly Regex _numberRegex = new Regex(
            "(?<prefix>[-,;\\.] *)\\d+\\.$",
            RegexOptions.Compiled);

        public static IReadOnlyList<string> ToSentences(this IEnumerable<string> texts)
        {
            var sentences = new List<string>();
            var text = string.Join(' ', texts).Trim();

            while (text.Length > 0 && text.IndexOf('.') >= 0)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '.')
                    {
                        var match = _numberRegex.Match(text.Substring(0, i + 1));

                        if (match.Success)
                        {
                            i = match.Index + match.Groups[1].Length - 1;
                        }
                        else if (i < text.Length - 1)
                        {
                            var next = text[i + 1];

                            if (next == '"' || next == '”' || next == ')' || next == ']')
                            {
                                i = i + 1;
                            }
                        }

                        var j = Math.Min(text.Length, i + 1);
                        var run = text.Substring(0, j).Trim();

                        if (run.Length > 1)
                        {
                            sentences.Add(run);
                        }

                        text = text.Substring(j);
                        break;
                    }
                }
            }

            return sentences.AsReadOnly();
        }

        public static IEnumerable<string> ToParagraphs(this IReadOnlyList<string> texts)
        {
            if (texts.Count == 0)
            {
                yield break;
            }

            // Compute seed for RNG that determines the number of sentences per paragraph
            var seed = new HashCode();

            foreach (var text in texts)
            {
                seed.Add(text);
            }

            var rng = new Random(seed.ToHashCode());

            // Use a queue to track unused texts
            var q = new Queue<string>(texts);

            while (q.Count > 0)
            {
                // Take 1-5 (or the remaining) texts from queue
                var n = Math.Min(q.Count, rng.Next(1, 6));
                var paragraph = new string[n];

                for (int i = 0; i < n; i++)
                {
                    paragraph[i] = q.Dequeue();
                }

                yield return string.Join("  ", paragraph);
            }
        }
    }
}