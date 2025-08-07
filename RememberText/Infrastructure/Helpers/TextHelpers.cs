using RememberText.Domain.Entities;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RememberText.Infrastructure.Helpers
{
    public static class TextHelpers
    {
        public static string DefSentence = "...";
        public static string RTSeparator = "§br§";
        public static string DefSentenceWithSeparator = DefSentence + RTSeparator;
        public static int UsersTextOnlyCount(string text)
        {
            if (text.Contains(DefSentenceWithSeparator))
            {
                text = text.Replace(DefSentenceWithSeparator, "");
            }
            if (text.Contains(RTSeparator))
            {
                text = text.Replace(RTSeparator, "");
            }
            return text.Count();
        }

        public static string CreateBlockOfTextFields(int rows)
        {
            string text = "";
            for(int i = 0; i < rows; i++)
            {
                text += DefSentenceWithSeparator;
            }
            return text;
        }

        public static IEnumerable<TopicsWithLangFlagsViewModel> SortTopics(this IEnumerable<TopicsWithLangFlagsViewModel> topics, string sortParam)
        {
            switch (sortParam)
            {
                case "title_desc":
                    return topics.OrderByDescending(x => x.TopicTitle).ToList();
                case "public":
                    return topics.OrderByDescending(x => x.PublicText).ThenBy(x => x.TopicTitle).ToList();
                case "public_desc":
                    return topics.OrderBy(x => x.PublicText).ThenBy(x => x.TopicTitle).ToList();
                case "source":
                    return topics.OrderBy(x => x.SourceLang).ThenBy(x => x.TopicTitle).ToList();
                case "source_desc":
                    return topics.OrderByDescending(x => x.SourceLang).ThenBy(x => x.TopicTitle).ToList(); ;
                case "target":
                    return topics.OrderBy(x => x.TargetLang).ThenBy(x => x.TopicTitle).ToList();
                case "target_desc":
                    return topics.OrderByDescending(x => x.TargetLang).ThenBy(x => x.TopicTitle).ToList();
                case "created":
                    return topics.OrderBy(x => x.CreatedDateTime).ThenBy(x => x.TopicTitle).ToList();
                case "created_desc":
                    return topics.OrderByDescending(x => x.CreatedDateTime).ThenBy(x => x.TopicTitle).ToList();
                case "updated":
                    return topics.OrderBy(x => x.UpdatedDateTime).ThenBy(x => x.TopicTitle).ToList();
                case "updated_desc":
                    return topics.OrderByDescending(x => x.UpdatedDateTime).ThenBy(x => x.TopicTitle).ToList();
                default:
                    return topics.OrderBy(x => x.TopicTitle).ToList();
            }
        }

        public static void ToUpperFirstLangTwoChars(this string langCode, out string langChars)
        {
            if (langCode.Contains("-"))
            {
                string[] langArr = langCode.Split('-');
                langCode = langArr[0];
            }

            langChars = char.ToUpper(langCode[0]) + langCode.Substring(1);
        }

        public static void MakeAdditionalLines(int numberOfLines, out string additionalLines)
        {
            int i = 0;
            additionalLines = "";
            while (i < numberOfLines)
            {
                additionalLines += TextHelpers.DefSentence;
                i++;
                if (i < numberOfLines)
                {
                    additionalLines += TextHelpers.RTSeparator;
                }
            }
        }

        public static List<SourceTargetSentence> FillSentenceList(string text)
        {
            List<SourceTargetSentence> sourceTargetSentence = new List<SourceTargetSentence>();

            if (!string.IsNullOrWhiteSpace(text))
            {
                string[] sentences = text.Split(new string[] { RTSeparator }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < sentences.Length; i++)
                {
                    bool breaking = false;
                    string sentence;

                    if (sentences[i].Contains(RTSeparator))
                    {
                        // If the string has a break characters "§br§", remove them
                        sentence = sentences[i].Replace(RTSeparator, "");
                        breaking = true;
                    }
                    else
                    {
                        sentence = sentences[i];
                    }

                    sourceTargetSentence.Add(new SourceTargetSentence
                    {
                        SentenceIndex = i,
                        Sentence = sentence.Trim(),
                        Breaking = breaking
                    });
                }
            }

            return sourceTargetSentence;
        }
    }
}
