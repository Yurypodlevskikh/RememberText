using RememberText.Domain.Entities;
using RememberText.Models;
using RememberText.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTTextService
    {
        Task<List<TextContentWithId>> GetTextContentByLangCodeAndTopicId(string langCode, int topicId);
        Task<string> GetTextContentByTableNameAndTextId(string langSubtag, int textId);
        Task<int> CalculateUserTopicsTextContentAsync(IEnumerable<TopicsViewModel> topics);
        Task<ResponseFromRawQuery> CreateTextAsync(int topicId, string langCode, string topicTitle, int rowsToAdd = 0);
        Task<ResponseFromRawQuery> EditSentenceAsync(EditSentence model, int? RestOfTextVolume = null);
        Task<ResponseFromRawQuery> DeleteSentenceAsync(UpdateSentenceFormText model);
        Task<bool> ExistsText(int textId, string langCode);
    }
}
