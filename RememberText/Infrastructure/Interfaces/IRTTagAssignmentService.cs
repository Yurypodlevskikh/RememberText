using RememberText.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTTagAssignmentService
    {
        Task<string> CreateTagAssignmentAsync(int textId, int tagId, string langSubtag);
        IEnumerable<TagAssignmentGeneral> GetTagAssignmentsByTagId(string langCode, int tagId);
        IEnumerable<TagAssignmentGeneral> GetTagAssignmentsByTextId(string langCode, int textId);
        TagAssignmentGeneral GetTagAssignmentByTextIdAndTagId(string langCode, int textId, int tagId);
    }
}
