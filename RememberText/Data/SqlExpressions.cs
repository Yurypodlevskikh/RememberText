using RememberText.Infrastructure.Helpers;

namespace RememberText.Data
{
    public class SqlExpressions
    {
        public static void GetTextTableName(string langCode, out string textTableName)
        {
            langCode.ToUpperFirstLangTwoChars(out string langSubtag);
            textTableName = $"{langSubtag}Texts";
        }
        public static void GetTagAssignmentTableName(string langCode, out string textTableName)
        {
            langCode.ToUpperFirstLangTwoChars(out string langSubtag);
            textTableName = $"{langSubtag}TagAssignments";
        }
        public static void GetTagTableName(string langCode, out string textTableName)
        {
            langCode.ToUpperFirstLangTwoChars(out string langSubtag);
            textTableName = $"{langSubtag}Tags";
        }
        public static string GetTextsByTableNameAndTopicId(string tableName, int topicId)
        {
            return $"SELECT * FROM {tableName} WHERE TopicId = {topicId}";
        }
    }
}
