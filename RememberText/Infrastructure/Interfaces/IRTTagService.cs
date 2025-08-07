using RememberText.Models;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTTagService
    {
        Task<int?> CreateTagAsync(int normTagId, string langSubtag, string tagName);
        Task<List<TagTopicTextGeneral>> GetTagsByTopicIdAndLangCode(int topicId, string langCode);
        Task<List<TagViewModel>> GetTagsByLangCodeAndTextId(string langCode, int textId);
        Task<List<TagAndTaggedTexts>> GetTagsByLangCodeAndUserId(string langCode, string userId);
        Task<List<TagAndTaggedTexts>> GetPublishedTagsByLangCode(string langCode);
        Task<List<TagAndTaggedTexts>> GetUsersTagsByUsersLangCode(string langCode, string userId);
        Task<List<TagAndTaggedTexts>> GetPublishedTagsExceptUsersByUsersLangCode(string langCode, string userId);
        Task<List<TagViewModel>> GetOfferTagsExceptTextId(string tagPart, string langCode, int? textId);
        Task<int?> GetTagId(string langSubtag, string tagName);
        Task<bool> TagConnectedToOtherUsersByTagId(string langSubtag, string userId, int tagId);
        Task<bool> TagConnectedToOtherUsersByNormTagId(string langSubtag, string userId, int normTagId);
        Task<bool> TagConnected(string langSubtag, int tagId);
        Task<ResponseFromRawQuery> DisableTagFromProject(int textId, int tagId, string langSubtag);
        Task<string> DisconnectTag(string langCode, int? tagId, int? textId);
        Task<string> DeleteTag(string langSubtag, int tagId);
        Task<ResponseFromRawQuery> DeleteMultipleTags(string langSubtag, string ids);
    }
}
