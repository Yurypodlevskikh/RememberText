using RememberText.Domain.Entities;
using RememberText.Models;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTNormalizedTagService
    {
        Task<int?> CreateNormTagAsync(string tagName);
        Task<NormalizedTag> GetNormTagByNormTagName(string normTagName);
        Task<string> DeleteNormTag(int normTagId);
        Task<ResponseFromRawQuery> DeleteMultipleNormTags(string ids);
    }
}
