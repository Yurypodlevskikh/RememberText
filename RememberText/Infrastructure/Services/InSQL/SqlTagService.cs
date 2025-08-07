using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using RememberText.DAL.Context;
using RememberText.Data;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.Models;
using RememberText.RTTools;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlTagService : IRTTagService
    {
        private readonly RememberTextDbContext _db;
        private readonly IRTTagAssignmentService _tagAssignmentService;
        private readonly IRTNormalizedTagService _normTagService;
        private readonly IRTLanguageService _languageService;
        public SqlTagService(RememberTextDbContext db, 
            IRTTagAssignmentService tagAssignmentService,
            IRTNormalizedTagService normTagService,
            IRTLanguageService languageService)
        {
            _db = db;
            _tagAssignmentService = tagAssignmentService;
            _normTagService = normTagService;
            _languageService = languageService;
        }

        /// <summary>
        /// Create Tag
        /// </summary>
        /// <param name="normTagId">Normalized Tag Id</param>
        /// <param name="langSubtag">Language Subtag with upper first letter (Sv)</param>
        /// <param name="tagName">Tag Name</param>
        /// <returns>Identifier of the new Tag</returns>
        public async Task<int?> CreateTagAsync(int normTagId, string langSubtag, string tagName)
        {
            int? result = null;
            using(var command = _db.Database.GetDbConnection().CreateCommand())
            {
                await _db.Database.OpenConnectionAsync();

                command.CommandText = "dbo.spCreateTag";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@langSubtag", langSubtag));
                command.Parameters.Add(new SqlParameter("@tagName", tagName));
                command.Parameters.Add(new SqlParameter("@normTagId", normTagId));

                object scalResult = await command.ExecuteScalarAsync();

                if(int.TryParse(scalResult?.ToString(), out int scalInt))
                {
                    result = scalInt;
                }
            }

            return result;
        }

        public async Task<List<TagTopicTextGeneral>> GetTagsByTopicIdAndLangCode(int topicId, string langCode)
        {
            langCode.ToUpperFirstLangTwoChars(out string langSubtag);

            string sqlExpression = $@"SELECT et.Id, et.TagName, et.NormalizedTagId, nt.NormalizedTagName, l.LangCode, etx.TopicId, eta.TextId 
                                    FROM {langSubtag}Tags et
                                    JOIN {langSubtag}TagAssignments eta ON et.Id = eta.TagId
                                    JOIN {langSubtag}Texts etx ON eta.TextId = etx.Id
                                    JOIN NormalizedTags nt ON nt.Id = et.NormalizedTagId
                                    JOIN Languages l ON l.Id = etx.LanguageId
                                    WHERE etx.TopicId = {topicId}";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TagTopicTextGeneral>(_db, sqlExpression);
        }

        public async Task<List<TagViewModel>> GetTagsByLangCodeAndTextId(string langCode, int textId)
        {
            langCode.ToUpperFirstLangTwoChars(out string langSubtag);
            
            return await RawSqlQueryHelper.SpDynamicGetTableData<TagViewModel>(_db, langSubtag, textId, "dbo.spGetTagsByTextIdAndLangSubtag");
        }

        public async Task<List<TagAndTaggedTexts>> GetTagsByLangCodeAndUserId(string langCode, string userId)
        {
            if (string.IsNullOrEmpty(langCode) || string.IsNullOrEmpty(userId))
                return null;

            SqlExpressions.GetTextTableName(langCode, out string tableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tableName))
                return null;

            langCode.ToUpperFirstLangTwoChars(out string langSubtag);

            string sqlExpression = $@"SELECT tg.Id, tg.TagName, COUNT(tg.TagName) AS TaggedTexts, l.LangCode FROM {langSubtag}Tags tg
                                    JOIN {langSubtag}TagAssignments ta ON ta.TagId = tg.Id
                                    JOIN {langSubtag}Texts txt ON txt.Id = ta.TextId
                                    JOIN Topics tp ON tp.Id = txt.TopicId
                                    JOIN Languages l ON l.Id = txt.LanguageId
                                    WHERE tp.UserId = '{userId}'
                                    GROUP BY tg.TagName, tg.Id, l.LangCode";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TagAndTaggedTexts>(_db, sqlExpression);
        }

        public async Task<List<TagAndTaggedTexts>> GetPublishedTagsByLangCode(string langCode)
        {
            if (string.IsNullOrEmpty(langCode))
                return null;

            SqlExpressions.GetTagAssignmentTableName(langCode, out string tagAssignmentTableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tagAssignmentTableName))
                return null;

            SqlExpressions.GetTagTableName(langCode, out string tagTableName);

            SqlExpressions.GetTextTableName(langCode, out string textTableName);

            string sqlExpression = $@"SELECT tg.Id, tg.TagName, COUNT(DISTINCT txt.TopicId) AS TaggedTopics, l.LangCode FROM {tagTableName} tg
                                    JOIN {tagAssignmentTableName} ta ON ta.TagId = tg.Id
                                    JOIN {textTableName} txt ON txt.Id = ta.TextId
                                    JOIN Topics tp ON tp.Id = txt.TopicId
                                    JOIN Languages l ON l.Id = txt.LanguageId
                                    WHERE tp.PublicText = 1 AND l.LangCode = '{langCode}'
                                    GROUP BY tg.TagName, tg.Id, l.LangCode";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TagAndTaggedTexts>(_db, sqlExpression);
        }

        public async Task<List<TagAndTaggedTexts>> GetUsersTagsByUsersLangCode(string langCode, string userId)
        {
            if (string.IsNullOrEmpty(langCode))
                return null;

            if (string.IsNullOrEmpty(userId))
                return null;

            SqlExpressions.GetTagAssignmentTableName(langCode, out string tagAssignmentTableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tagAssignmentTableName))
                return null;

            SqlExpressions.GetTagTableName(langCode, out string tagTableName);

            SqlExpressions.GetTextTableName(langCode, out string textTableName);

            string sqlExpression = $@"SELECT tg.Id, tg.TagName, COUNT(DISTINCT txt.TopicId) AS TaggedTopics, l.LangCode FROM {tagTableName} tg
                                    JOIN {tagAssignmentTableName} ta ON ta.TagId = tg.Id
                                    JOIN {textTableName} txt ON txt.Id = ta.TextId
                                    JOIN Topics tp ON tp.Id = txt.TopicId
                                    JOIN Languages l ON l.Id = txt.LanguageId
                                    WHERE tp.UserId = '{userId}' AND l.LangCode = '{langCode}'
                                    GROUP BY tg.TagName, tg.Id, l.LangCode";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TagAndTaggedTexts>(_db, sqlExpression);
        }

        public async Task<List<TagAndTaggedTexts>> GetPublishedTagsExceptUsersByUsersLangCode(string langCode, string userId)
        {
            if (string.IsNullOrEmpty(langCode))
                return null;

            if (string.IsNullOrEmpty(userId))
                return null;

            SqlExpressions.GetTagAssignmentTableName(langCode, out string tagAssignmentTableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tagAssignmentTableName))
                return null;

            SqlExpressions.GetTagTableName(langCode, out string tagTableName);

            SqlExpressions.GetTextTableName(langCode, out string textTableName);

            string sqlExpression = $@"SELECT tg.Id, tg.TagName, COUNT(DISTINCT txt.TopicId) AS TaggedTopics, l.LangCode FROM {tagTableName} tg
                                    JOIN {tagAssignmentTableName} ta ON ta.TagId = tg.Id
                                    JOIN {textTableName} txt ON txt.Id = ta.TextId
                                    JOIN Topics tp ON tp.Id = txt.TopicId
                                    JOIN Languages l ON l.Id = txt.LanguageId
                                    WHERE tp.PublicText = 1 AND tp.UserId != '{userId}' AND l.LangCode = '{langCode}'
                                    GROUP BY tg.TagName, tg.Id, l.LangCode";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TagAndTaggedTexts>(_db, sqlExpression);
        }

        public async Task<List<TagViewModel>> GetOfferTagsExceptTextId(string tagPart, string langCode, int? textId)
        {
            if (string.IsNullOrEmpty(langCode) || string.IsNullOrEmpty(tagPart) || textId == null)
                return null;

            SqlExpressions.GetTagAssignmentTableName(langCode, out string tagAssignmentTableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tagAssignmentTableName))
                return null;

            SqlExpressions.GetTagTableName(langCode, out string tagTableName);

            SqlExpressions.GetTextTableName(langCode, out string textTableName);

            var entities = new List<TagViewModel>();

            string sqlExpression = $@"SELECT DISTINCT tg.Id, tg.TagName, l.LangCode FROM NormalizedTags ntg
                                    JOIN {tagTableName} tg ON tg.NormalizedTagId = ntg.Id
                                    JOIN {tagAssignmentTableName} ta ON ta.TagId = tg.Id
                                    JOIN {textTableName} txt ON txt.Id = ta.TextId
                                    JOIN Languages l ON l.Id = txt.LanguageId
                                    WHERE ntg.NormalizedTagName LIKE @tagPart + '%' AND txt.Id != @textId";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlExpression;
                command.CommandType = CommandType.Text;
                SqlParameter TagPart = new SqlParameter("@tagPart", SqlDbType.NVarChar);
                SqlParameter TextId = new SqlParameter("@textId", SqlDbType.Int);
                TagPart.Value = tagPart;
                TextId.Value = textId;
                command.Parameters.Add(TagPart);
                command.Parameters.Add(TextId);

                await _db.Database.OpenConnectionAsync();
                using(var dbResult = await command.ExecuteReaderAsync())
                {
                    if(dbResult.HasRows)
                    {
                        while(await dbResult.ReadAsync())
                        {
                            var entity = new TagViewModel();
                            var entityProperties = entity.GetType().GetProperties();

                            for(int i = 0; i < entityProperties.Length; i++)
                            {
                                if(!string.IsNullOrEmpty(dbResult[entityProperties[i].Name].ToString()))
                                {
                                    entity.GetType().GetProperty(entityProperties[i].Name).SetValue(entity, dbResult[entityProperties[i].Name]);
                                }
                            }

                            entities.Add(entity);
                        }
                    }
                }
            }

            return entities;
        }

        public async Task<int?> GetTagId(string langSubtag, string tagName)
        {
            int? result = null;
            using(var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @$"SELECT Id FROM {langSubtag}Tags
                    WHERE TagName = @tagName";

                command.CommandType = CommandType.Text;
                SqlParameter TagName = new SqlParameter("@tagName", SqlDbType.NVarChar);
                TagName.Value = tagName;
                command.Parameters.Add(TagName);

                await _db.Database.OpenConnectionAsync();
                object scalarTagId = await command.ExecuteScalarAsync();

                if (int.TryParse(scalarTagId?.ToString(), out int existsTagId))
                {
                    result = existsTagId;
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the tag is linked to other users' projects.
        /// </summary>
        /// <param name="langSubtag">Common language cod: En</param>
        /// <param name="userId">User Id</param>
        /// <param name="tagId">Tag Id</param>
        /// <returns>If it connected then true, otherwise false.</returns>
        public async Task<bool> TagConnectedToOtherUsersByTagId(string langSubtag, string userId, int tagId)
        {
            string sqlExpression = $@"SELECT TOP 1 TagId 
                                    FROM {langSubtag}TagAssignments ta
                                    JOIN {langSubtag}Texts txt ON txt.Id = ta.TextId
                                    JOIN Topics tp ON tp.Id = txt.TopicId
                                    WHERE ta.TagId = {tagId} AND tp.UserId != '{userId}'";
            int result = await RawSqlQueryHelper.RawScalarSqlQuery(_db, sqlExpression);
            return result > 0;
        }

        public async Task<bool> TagConnectedToOtherUsersByNormTagId(string langSubtag, string userId, int normTagId)
        {
            string sqlExpression = $@"SELECT TOP 1 t.Id 
                                    FROM {langSubtag}Tags t 
                                    JOIN {langSubtag}TagAssignments ta ON ta.TagId = t.Id
                                    JOIN {langSubtag}Texts txt ON txt.Id = ta.TextId
                                    JOIN Topics tp ON tp.Id = txt.TopicId
                                    WHERE t.NormalizedTagId = {normTagId} AND tp.UserId != '{userId}'";
            int result = await RawSqlQueryHelper.RawScalarSqlQuery(_db, sqlExpression);
            return result > 0;
        }

        public async Task<bool> TagConnected(string langSubtag, int tagId)
        {
            string sqlExpression = @$"SELECT TOP 1 TagId 
                                    FROM {langSubtag}TagAssignments
                                    WHERE TagId = {tagId}";
            int result = await RawSqlQueryHelper.RawScalarSqlQuery(_db, sqlExpression);
            return result > 0;
        }

        public async Task<ResponseFromRawQuery> DisableTagFromProject(int textId, int tagId, string langSubtag)
        {
            string sqlExpression = $"DELETE FROM {langSubtag}TagAssignments WHERE textId = {textId} AND TagId = {tagId}";
            return await RawSqlQueryHelper.RawNonQuery(_db, sqlExpression);
        }

        public async Task<string> DisconnectTag(string langCode, int? tagId, int? textId)
        {
            // The response is filled in in case of an error.
            string response = "";

            if (tagId == null)
                return null;

            // Checking the culture in the database
            langCode.ToUpperFirstLangTwoChars(out string langSubtag);
            if (string.IsNullOrEmpty(langSubtag))
                return null;

            // Delete tag only from this text
            ResponseFromRawQuery deletetagAssignmentResult = await DisableTagFromProject((int)textId, (int)tagId, langSubtag);
            response = deletetagAssignmentResult.RespStr;

            if (!await TagConnected(langSubtag, (int)tagId))
            {
                string responseOfDeletingTag = await DeleteTag(langSubtag, (int)tagId);
            }

            return response;
        }

        public async Task<string> DeleteTag(string langSubtag, int tagId)
        {
            string response = "";

            // Checking the existence of the Tag in the database
            List<TagGeneral> tag = await RawSqlQueryHelper.SpDynamicGetTableData<TagGeneral>(_db, langSubtag, (int)tagId, "dbo.spGetTagByTagId");

            if (tag.Count == 0)
                return null;

            int normTagId = tag.First().NormalizedTagId;
            string tagTableName = $"{langSubtag}Tags";

            // Delete the Tag from database
            int deleteTagResult = await RawSqlQueryHelper.SpDeleteRowById(_db, tagTableName, (int)tagId);

            // Stored procedure return -1(success) and more then (wrong)
            if (deleteTagResult == -1)
            {
                response = "The tag has been removed.";
                // If the Normalized Tag checks if it is in the database 
                int? ifRefToNormTagExists = await RawSqlQueryHelper.SpIfRowExists(_db, tagTableName, "NormalizedTagId", normTagId.ToString());
                if (ifRefToNormTagExists != null && ifRefToNormTagExists == 0)
                {
                    // Checking the ezistence of the reference to Normalized Tag in other cultures
                    bool normTagInSeveralCulture = false;
                    var primLangs = await _languageService.PrimaryLanguagesAsync();
                    foreach (var lang in primLangs.Select(x => x.LangCode))
                    {
                        // Checking the culture in the database
                        lang.ToUpperFirstLangTwoChars(out string otherLangSubtag);
                        if (!string.IsNullOrEmpty(otherLangSubtag))
                        {
                            if (otherLangSubtag == langSubtag)
                            {
                                string otherTagTableName = $"{otherLangSubtag}Tags";
                                int? OtherRefToNormTagExists = await RawSqlQueryHelper.SpIfRowExists(_db, otherTagTableName, "NormalizedTagId", normTagId.ToString());
                                if (OtherRefToNormTagExists != null && OtherRefToNormTagExists > 0)
                                {
                                    normTagInSeveralCulture = true;
                                    break;
                                }
                            }
                        }

                    }
                    if (!normTagInSeveralCulture)
                    {
                        // Remove Normalized Tag. If response is empty then removing is success.
                        string respNormTag = await _normTagService.DeleteNormTag(normTagId);
                        response += $" {respNormTag}";
                    }
                }
                else if (ifRefToNormTagExists == null || ifRefToNormTagExists < 0)
                {
                    // Error
                    response += " But there is wrong to find the Normalized Tag.";
                }
            }

            return response;
        }

        public async Task<ResponseFromRawQuery> DeleteMultipleTags(string langSubtag, string ids)
        {
            string sqlExpression = $@"DELETE FROM {langSubtag}Tags WHERE Id IN ({ids})";
            return await RawSqlQueryHelper.RawNonQuery(_db, sqlExpression);
        }
    }
}
