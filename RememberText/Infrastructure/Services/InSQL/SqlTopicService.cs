using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Data;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.Models;
using RememberText.RTTools;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlTopicService : IRTTopicService
    {
        private readonly RememberTextDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRTTagService _tagService;
        private readonly IRTLanguageService _langService;
        private readonly IRTTextCopyrightService _textCopyrightService;

        public SqlTopicService(
            RememberTextDbContext db,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            IRTTagService tagService,
            IRTLanguageService langService,
            IRTTextCopyrightService textCopyrightService)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _tagService = tagService;
            _langService = langService;
            _textCopyrightService = textCopyrightService;
        }

        public async Task<List<TopicsViewModel>> GetAllPublishedTopics()
        {
            string sqlExpression = $@"SELECT tp.Id AS TopicId, tp.TopicTitle, tp.SourceLang, 
                                    tp.TargetLang, tp.CreatedDateTime, tp.UpdatedDateTime, 
                                    tp.PublicText, u.Nickname AS AuthorNickname, tp.BanText AS BanProject 
                                    FROM Topics tp
                                    JOIN AspNetUsers u ON u.Id = tp.UserId
                                    WHERE tp.PublicText = 1";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TopicsViewModel>(_db, sqlExpression);
        }

        public async Task<List<TopicsViewModel>> GetAllTopicsByLoggedInUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            string sqlExpression = $@"SELECT tp.Id AS TopicId, tp.TopicTitle, tp.SourceLang, 
                                    tp.TargetLang, tp.CreatedDateTime, tp.UpdatedDateTime, 
                                    tp.PublicText, u.Nickname AS AuthorNickname, tp.BanText AS BanProject 
                                    FROM Topics tp
                                    JOIN AspNetUsers u ON u.Id = tp.UserId
                                    WHERE tp.UserId = '{userId}'";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TopicsViewModel>(_db, sqlExpression);
        }

        public async Task<List<Topic>> GetOnlyTopicsByUserId(string userId, bool? published = null)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            if(published == null)
            {
                return await _db.Topics.Where(x => x.UserId == userId).ToListAsync();
            }
            else
            {
                return await _db.Topics.Where(x => x.UserId == userId && x.PublicText == published).ToListAsync();
            }
        }

        public async Task<List<Topic>> GetAllTopicsByCopyrightId(int copyrightId) =>
            await _db.Topics.Where(t => t.CopyrightId == copyrightId).ToListAsync();

        public async Task<List<TopicsViewModel>> GetUsersTopicsByTagId(int tagId, string langCode, string userId)
        {
            if (string.IsNullOrEmpty(langCode) || string.IsNullOrEmpty(userId))
                return null;

            SqlExpressions.GetTagAssignmentTableName(langCode, out string tagAssignmentTableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tagAssignmentTableName))
                return null;

            SqlExpressions.GetTagTableName(langCode, out string tagTableName);

            SqlExpressions.GetTextTableName(langCode, out string textTableName);

            string sqlExpression = $@"SELECT tp.Id AS TopicId, tp.TopicTitle, tp.SourceLang, 
                                    tp.TargetLang, tp.CreatedDateTime, tp.UpdatedDateTime, 
                                    tp.PublicText, u.Nickname AS AuthorNickname, tp.BanText AS BanProject 
                                    FROM Topics tp
                                    JOIN {textTableName} txt ON txt.TopicId = tp.Id
                                    JOIN {tagAssignmentTableName} ta ON ta.TextId = txt.Id
                                    JOIN {tagTableName} tg ON tg.Id = ta.TagId
                                    JOIN AspNetUsers u ON u.Id = tp.UserId
                                    WHERE tp.UserId = '{userId}' AND tg.Id = {tagId}";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TopicsViewModel>(_db, sqlExpression);
        }
        /// <summary>
        /// Provides a Topic with attached texts. Anonimous user gets published topics. 
        /// Registered user receives own or published topics.
        /// </summary>
        /// <param name="id">Topic Id</param>
        /// <param name="HowToDisplay">Normal or vice versa</param>
        /// <returns>Topic with attached texts.</returns>
        public async Task<ProjectWithText> GetTopicWithTextById(int? id, string HowToDisplay = "")
        {
            if(id != null)
            {
                Topic topic = await _db.Topics.Include(c => c.TextCopyright).FirstOrDefaultAsync(x => x.Id == id);

                if(topic != null)
                {
                    var projectText = new ProjectWithText
                    {
                        ProjectAuthorId = topic.UserId,
                        TopicId = topic.Id,
                        TopicTitle = topic.TopicTitle,
                        SourceLang = topic.SourceLang,
                        TargetLang = topic.TargetLang,
                        HowToDisplay = HowToDisplay,
                        Copyright = topic.TextCopyright
                    };

                    if(topic.PublicText == true)
                    {
                        await LoadRelatedText(projectText);
                        return projectText;
                    }
                    else
                    {
                        ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;
                        User currentUser = await _userManager.GetUserAsync(user);

                        if(user != null)
                        {
                            if (topic.UserId == currentUser.Id || await _userManager.IsInRoleAsync(currentUser, "Administrators") || await _userManager.IsInRoleAsync(currentUser, "GenAdministrator"))
                            {
                                await LoadRelatedText(projectText);
                                return projectText;
                            }
                        }
                    }    
                }
            }
            
            return null;
        }

        public async Task<List<TopicsViewModel>> GetPublishedTopicsByTagIdAndLangCode(int tagId, string langCode)
        {
            if (string.IsNullOrEmpty(langCode))
                return null;

            SqlExpressions.GetTagAssignmentTableName(langCode, out string tagAssignmentTableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tagAssignmentTableName))
                return null;

            SqlExpressions.GetTagTableName(langCode, out string tagTableName);

            SqlExpressions.GetTextTableName(langCode, out string textTableName);

            string sqlExpression = $@"SELECT tp.Id AS TopicId, tp.TopicTitle, tp.SourceLang, 
                                    tp.TargetLang, tp.CreatedDateTime, tp.UpdatedDateTime, 
                                    tp.PublicText, u.Nickname AS AuthorNickname, tp.BanText AS BanProject 
                                    FROM Topics tp
                                    JOIN {textTableName} txt ON txt.TopicId = tp.Id
                                    JOIN {tagAssignmentTableName} ta ON ta.TextId = txt.Id
                                    JOIN {tagTableName} tg ON tg.Id = ta.TagId
                                    JOIN AspNetUsers u ON u.Id = tp.UserId
                                    WHERE tp.PublicText = 1 AND tg.Id = {tagId}";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TopicsViewModel>(_db, sqlExpression);
        }

        public async Task<List<TopicsViewModel>> GetPublishedTopicsByTagIdExceptUsersTopics(int tagId, string langCode, string userId)
        {
            if (string.IsNullOrEmpty(langCode) || string.IsNullOrEmpty(userId))
                return null;

            SqlExpressions.GetTagAssignmentTableName(langCode, out string tagAssignmentTableName);
            if (!await RawSqlQueryHelper.SpIfTableExists(_db, tagAssignmentTableName))
                return null;

            SqlExpressions.GetTagTableName(langCode, out string tagTableName);

            SqlExpressions.GetTextTableName(langCode, out string textTableName);

            string sqlExpression = $@"SELECT tp.Id AS TopicId, tp.TopicTitle, tp.SourceLang, 
                                    tp.TargetLang, tp.CreatedDateTime, tp.UpdatedDateTime, 
                                    tp.PublicText, u.Nickname AS AuthorNickname, tp.BanText AS BanProject 
                                    FROM Topics tp
                                    JOIN {textTableName} txt ON txt.TopicId = tp.Id
                                    JOIN {tagAssignmentTableName} ta ON ta.TextId = txt.Id
                                    JOIN {tagTableName} tg ON tg.Id = ta.TagId
                                    JOIN AspNetUsers u ON u.Id = tp.UserId
                                    WHERE tp.UserId != '{userId}' AND tp.PublicText = 1 AND tg.Id = {tagId}";

            return await RawSqlQueryHelper.RawSqlGetDinamicDataList<TopicsViewModel>(_db, sqlExpression);
        }

        private async Task LoadRelatedText(ProjectWithText topic)
        {
            if (!string.IsNullOrEmpty(topic.SourceLang))
            {
                SqlExpressions.GetTextTableName(topic.SourceLang, out string sourceTableName);
                if (await RawSqlQueryHelper.SpIfTableExists(_db, sourceTableName))
                {
                    var resultText = await RawSqlQueryHelper.RawSqlGetDinamicDataList<TextModel>(_db, 
                        SqlExpressions.GetTextsByTableNameAndTopicId(sourceTableName, topic.TopicId));

                    if (resultText != null && resultText.Count() > 0)
                    {
                        TextModel sourceText = null;
                        TextModel targetText = null;
                        
                        if (topic.SourceLang == topic.TargetLang)
                        {
                            if (resultText.Count > 1)
                            {
                                sourceText = topic.HowToDisplay == "viceversa" ? resultText.ElementAt(1) : resultText.First();
                                targetText = topic.HowToDisplay == "viceversa" ? resultText.First() : resultText.ElementAt(1);
                            }
                        }
                        else if (!string.IsNullOrEmpty(topic.TargetLang))
                        {
                            SqlExpressions.GetTextTableName(topic.TargetLang, out string targetTableName);
                            if (await RawSqlQueryHelper.SpIfTableExists(_db, targetTableName))
                            {
                                var resultTargetText = await RawSqlQueryHelper.RawSqlGetDinamicDataList<TextModel>(_db,
                                    SqlExpressions.GetTextsByTableNameAndTopicId(targetTableName, topic.TopicId));
                                if(resultTargetText != null && resultTargetText.Count() > 0)
                                {
                                    sourceText = topic.HowToDisplay == "viceversa" ? resultTargetText.ElementAt(0) : resultText.First();
                                    targetText = topic.HowToDisplay == "viceversa" ? resultText.First() : resultTargetText.ElementAt(0);
                                }
                            }
                        }
                        else
                        {
                            sourceText = resultText.First();
                        }

                        if(sourceText != null)
                        {
                            topic.SourceLangId = sourceText.LanguageId;
                            topic.SourceTextId = sourceText.Id;
                            topic.SourceText = TextHelpers.FillSentenceList(sourceText.TextContent);
                        }

                        if(targetText != null)
                        {
                            topic.TargetLangId = targetText.LanguageId;
                            topic.TargetTextId = targetText.Id;
                            topic.TargetText = TextHelpers.FillSentenceList(targetText.TextContent);
                        }
                    }
                }
            }
        }
        public async Task<Topic> CreateTopicAsync(Topic topic)
        {
            await _db.Topics.AddAsync(topic);
            await _db.SaveChangesAsync();
            return topic;
        }
        public async Task<ResponseFromRawQuery> DeleteTopicAsync(HttpContext ctxt, int? id)
        {
            var response = new ResponseFromRawQuery
            {
                RespInt = 0,
                RespStr = "Not found"
            };

            if(id == null)
            {
                return response;
            }

            ClaimsPrincipal user = ctxt.User;
            User currentUser = await _userManager.GetUserAsync(user);

            Topic topic = await IfTheUserHasThisTopic(id);

            if (topic == null && topic.UserId != currentUser.Id)
            {
                return response;
            }

            var copyrightId = topic.CopyrightId;

            if (copyrightId != null && (int)copyrightId > 0)
            {
                if (!await _textCopyrightService.ThisCopyrightNameIsUsedMore((int)copyrightId, topic.Id))
                {
                    await _textCopyrightService.DeleteTextCopyright((int)copyrightId);
                }
            }

            // Get tags from the project texts
            List<TagTopicTextGeneral> tagsSource = await _tagService.GetTagsByTopicIdAndLangCode(topic.Id, topic.SourceLang);
            List<TagTopicTextGeneral> tagsTarget = null;
            if(!string.IsNullOrEmpty(topic.TargetLang))
            {
                tagsTarget = await _tagService.GetTagsByTopicIdAndLangCode(topic.Id, topic.TargetLang);
            }    

            var deletedEntity = _db.Topics.Remove(topic);
            await _db.SaveChangesAsync();
            response.RespInt = deletedEntity.Entity.Id;
            response.RespStr = $"Deletet successfully {_db.ChangeTracker.Entries().Count()} entity(ies).";

            // Check if we need to delete tags from the project texts
            if (tagsSource != null && tagsSource.Count() > 0)
            {
                topic.SourceLang.ToUpperFirstLangTwoChars(out string langSubtag);
                foreach (var ts in tagsSource)
                {
                    if (!await _tagService.TagConnected(langSubtag, ts.Id))
                    {
                        // Delete Tag from the table Tags and may be from the table NormalizedTags
                        string SrceTagDeletingResult = await _tagService.DeleteTag(langSubtag, ts.Id);
                    }
                }
            }

            if (tagsTarget != null && tagsTarget.Count() > 0)
            {
                topic.TargetLang.ToUpperFirstLangTwoChars(out string langSubtag);
                foreach (var tt in tagsTarget)
                {
                    if (!await _tagService.TagConnected(langSubtag, tt.Id))
                    {
                        // Delete Tag from the table Tags and may be from the table NormalizedTags
                        string TrgTagDeletingResult = await _tagService.DeleteTag(langSubtag, tt.Id);
                    }
                }
            }

            return response;
        }

        public async Task<Topic> IfTheUserHasThisTopic(int? id)
        {
            if(id == null)
            {
                return null;
            }

            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;
            User currentUser = await _userManager.GetUserAsync(user);

            if(currentUser == null)
            {
                return null;
            }

            return await _db.Topics.FirstOrDefaultAsync(x => x.Id == (int)id && x.UserId == currentUser.Id);
        }
    }
}
