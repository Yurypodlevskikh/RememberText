using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RememberText.DAL.Context;
using RememberText.Data;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.Models;
using RememberText.RTTools;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlTextService : IRTTextService
    {
        private readonly RememberTextDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRTLanguageService _languageService;
        private readonly IRTTopicService _topicService;
        private readonly IConfiguration _config;

        public SqlTextService(RememberTextDbContext db, 
            UserManager<User> userManager, 
            IHttpContextAccessor contextAccessor,
            IRTLanguageService languageService,
            IRTTopicService topicService,
            IConfiguration config)
        {
            _db = db;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _languageService = languageService;
            _topicService = topicService;
            _config = config;
        }

        #region Get Text
        public async Task<List<TextContentWithId>> GetTextContentByLangCodeAndTopicId(string langCode, int topicId)
        {
            SqlExpressions.GetTextTableName(langCode, out string tableName);
            if (await RawSqlQueryHelper.SpIfTableExists(_db, tableName))
            {
                string query = @$"SELECT Id, TextContent FROM {tableName}
                              WHERE TopicId = {topicId}";

                return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TextContentWithId>(_db, query);
            }

            return null;
        }

        public async Task<string> GetTextContentByTableNameAndTextId(string tableName, int textId)
        {

            string query = @$"SELECT TextContent FROM {tableName}
                              WHERE Id = {textId}";
            List<string> result = await RawSqlQueryHelper.RawGetFieldStrs(_db, query);
            return result?.First();
        }
        #endregion Get Text

        public async Task<ResponseFromRawQuery> CreateTextAsync(int topicId, string langCode, string topicTitle, int rowsToAdd = 0)
        {
            SqlExpressions.GetTextTableName(langCode, out string tableName);

            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tableName))
            {
                return null;
            }

            ClaimsPrincipal user = _contextAccessor.HttpContext.User;
            User currUser = await _userManager.GetUserAsync(user);

            string text = topicTitle + TextHelpers.RTSeparator;
            if(rowsToAdd > 0)
            {
                text = text += TextHelpers.CreateBlockOfTextFields(rowsToAdd);
            }

            string sqlExpression = @$"INSERT INTO {tableName} 
                (TextContent, AuthorId, AuthorNickname, AuthorEmail, CreatedDateTime, LanguageId, TopicId) 
                VALUES (@textContent, @authorId, @authorNickname, @authorEmail, @createdDateTime, 
                @languageId, @topicId)";

            int languageId = _db.Languages.FirstOrDefault(x => x.LangCode == langCode).Id;

            var response = new ResponseFromRawQuery();
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sqlExpression, connection);
                SqlParameter txtParam = new SqlParameter("@textContent", text);
                cmd.Parameters.Add(txtParam);
                SqlParameter authorIdParam = new SqlParameter("@authorId", currUser.Id);
                cmd.Parameters.Add(authorIdParam);
                SqlParameter authorNickParam = new SqlParameter("@authorNickname", currUser.Nickname);
                cmd.Parameters.Add(authorNickParam);
                SqlParameter authorEmailParam = new SqlParameter("@authorEmail", currUser.Email);
                cmd.Parameters.Add(authorEmailParam);
                SqlParameter dateTimeParam = new SqlParameter("@createdDateTime", DateTime.Now);
                cmd.Parameters.Add(dateTimeParam);
                SqlParameter langIdParam = new SqlParameter("@languageId", languageId);
                cmd.Parameters.Add(langIdParam);
                SqlParameter topicIdParam = new SqlParameter("@topicId", topicId);
                cmd.Parameters.Add(topicIdParam);

                int result = cmd.ExecuteNonQuery();
                
                if(result > 0)
                {
                    response.RespInt = result;
                    response.RespStr = "The task was completed successfully!";
                }
                else
                {
                    response.RespInt = 0;
                    response.RespStr = "Changes failed!";
                }
            }

            return response;
        }

        public async Task<ResponseFromRawQuery> EditSentenceAsync(EditSentence model, int? RestOfTextVolume = null)
        {
            SqlExpressions.GetTextTableName(model.TargetLang, out string tableName);
            if (await RawSqlQueryHelper.SpIfTableExists(_db, tableName))
            {
                string textContent = await GetTextContentByTableNameAndTextId(tableName, model.TargetTextId);

                if (!string.IsNullOrEmpty(textContent))
                {
                    string[] sentences = textContent.Split(TextHelpers.RTSeparator, StringSplitOptions.RemoveEmptyEntries);

                    // Check if the index is correct
                    if(sentences.Length > model.TargetTextIndex)
                    {
                        // Check of the amount of memory allows you to save the sentence
                        if (RestOfTextVolume != null)
                        {
                            int editableSentenceLength = sentences[model.TargetTextIndex].Length;
                            int sentenceLength = model.Sentence.Length;

                            if (editableSentenceLength < sentenceLength)
                            {
                                int amountToAdd = sentenceLength - editableSentenceLength;
                                if (amountToAdd > RestOfTextVolume)
                                {
                                    return null;
                                }
                            }
                        }

                        sentences[model.TargetTextIndex] = !string.IsNullOrEmpty(model.Sentence) ? model.Sentence :
                            sentences.Length == model.TargetTextIndex ? TextHelpers.DefSentence : TextHelpers.DefSentenceWithSeparator;

                        string newTextContent = string.Join(TextHelpers.RTSeparator, sentences);

                        return await RawSqlQueryHelper.RawUpdateTextContent(_db, newTextContent, tableName, model.TargetTextId);
                    }
                }
            }

            return null;
        }

        public async Task<ResponseFromRawQuery> DeleteSentenceAsync(UpdateSentenceFormText projecttext)
        {
            ResponseFromRawQuery commonResponse = new ResponseFromRawQuery();

            if (projecttext != null)
            {
                if(projecttext.SourceText != null && projecttext.SourceText.Count() > 0)
                {
                    projecttext.SourceText.RemoveAt(projecttext.SentenceIndex);
                    string sourceTextContent = "";

                    int iteration = 0;
                    foreach(var sText in projecttext.SourceText)
                    {
                        sourceTextContent += sText.Sentence;
                        iteration++;
                        if(iteration < projecttext.SourceText.Count())
                        {
                            sourceTextContent += TextHelpers.RTSeparator;
                        }
                    }

                    SqlExpressions.GetTextTableName(projecttext.SourceLang, out string sTableName);
                    if (await RawSqlQueryHelper.SpIfTableExists(_db, sTableName))
                    {
                        var sResponse = await RawSqlQueryHelper.RawUpdateTextContent(_db, sourceTextContent, sTableName, projecttext.SourceTextId);
                        if (sResponse != null)
                        {
                            commonResponse.RespInt = sResponse.RespInt;
                            commonResponse.RespStr = "1. " + sResponse.RespStr;
                        }
                    }
                }

                if(projecttext.TargetText != null && projecttext.TargetText.Count() > 0)
                {
                    projecttext.TargetText.RemoveAt(projecttext.SentenceIndex);
                    string targetTextContent = "";
                    int iteration = 0;
                    foreach(var tText in projecttext.TargetText)
                    {
                        targetTextContent += tText.Sentence;
                        iteration++;
                        if (iteration < projecttext.TargetText.Count())
                        {
                            targetTextContent += TextHelpers.RTSeparator;
                        }
                    }

                    SqlExpressions.GetTextTableName(projecttext.TargetLang, out string tTableName);
                    if (await RawSqlQueryHelper.SpIfTableExists(_db, tTableName))
                    {
                        var sResponse = await RawSqlQueryHelper.RawUpdateTextContent(_db, targetTextContent, tTableName, (int)projecttext.TargetTextId);
                        if (sResponse != null)
                        {
                            commonResponse.RespInt = commonResponse.RespInt + sResponse.RespInt;
                            commonResponse.RespStr = commonResponse.RespStr + " 2. " + sResponse.RespStr;
                        }
                    }
                }
            }

            return commonResponse;
        }

        public async Task<bool> ExistsText(int textId, string langCode)
        {
            bool exists = false;
            SqlExpressions.GetTextTableName(langCode, out string textTableName);
            int? ifTextExists = await RawSqlQueryHelper.SpIfRowExists(_db, textTableName, "Id", textId.ToString());
            if(ifTextExists != null && ifTextExists > 0)
            {
                exists = true;
            }

            return exists;
        }

        public async Task<int> CalculateUserTopicsTextContentAsync(IEnumerable<TopicsViewModel> topics)
        {
            int amountOfText = 0;

            if (topics != null && topics.Count() > 0)
            {
                foreach (var topic in topics)
                {
                    amountOfText += topic.TopicTitle.Length;

                    if (topic.SourceLang == topic.TargetLang)
                    {
                        List<TextContentWithId> textContents = await GetTextContentByLangCodeAndTopicId(topic.SourceLang, topic.TopicId);
                        if (textContents != null && textContents.Count() > 0)
                        {
                            foreach (var textContent in textContents)
                            {
                                amountOfText += TextHelpers.UsersTextOnlyCount(textContent.TextContent);
                            }
                        }
                    }
                    else
                    {
                        List<TextContentWithId> sourceTextContent = await GetTextContentByLangCodeAndTopicId(topic.SourceLang, topic.TopicId);
                        if (sourceTextContent != null && sourceTextContent.Count() > 0)
                        {
                            amountOfText += TextHelpers.UsersTextOnlyCount(sourceTextContent.First().TextContent);
                        }

                        if (!string.IsNullOrEmpty(topic.TargetLang))
                        {
                            List<TextContentWithId> targetTextContent = await GetTextContentByLangCodeAndTopicId(topic.TargetLang, topic.TopicId);
                            if (targetTextContent != null && targetTextContent.Count() > 0)
                            {
                                amountOfText += TextHelpers.UsersTextOnlyCount(targetTextContent.First().TextContent);
                            }
                        }
                    }
                }
            }

            return amountOfText;
        }
    }
}
