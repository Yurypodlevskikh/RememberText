using Microsoft.AspNetCore.Http;
using RememberText.Domain.Entities;
using RememberText.Models;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTTopicService
    {
        Task<List<TopicsViewModel>> GetAllPublishedTopics();
        Task<ProjectWithText> GetTopicWithTextById(int? id, string HowToDisplay = "");
        Task<List<TopicsViewModel>> GetAllTopicsByLoggedInUser(string userId);
        Task<List<Topic>> GetOnlyTopicsByUserId(string userId, bool? published = null);
        Task<List<Topic>> GetAllTopicsByCopyrightId(int copyrightId);
        Task<List<TopicsViewModel>> GetUsersTopicsByTagId(int tagId, string langCode, string userId);
        Task<List<TopicsViewModel>> GetPublishedTopicsByTagIdAndLangCode(int tagId, string langCode);
        Task<List<TopicsViewModel>> GetPublishedTopicsByTagIdExceptUsersTopics(int tagId, string langCode, string userId);
        Task<Topic> CreateTopicAsync(Topic topic);
        Task<ResponseFromRawQuery> DeleteTopicAsync(HttpContext ctxt, int? id);
        Task<Topic> IfTheUserHasThisTopic(int? id);
    }
}
