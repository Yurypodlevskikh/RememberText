using RememberText.Domain.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTTextCopyrightService
    {
        Task<TextCopyrightModel> GetTextCopyrightById(int copyrightId);
        Task<TextCopyrightModel> GetTextCopyrightByCopyrightName(string copiryghtName);
        Task<List<TextCopyrightModel>> GetSomeFirstStartWithCopyrights(int? howMuch, string text);
        Task<List<TextCopyrightModel>> GetSomeFirstContainsCopyrights(int? howMuch, string text);
        Task<List<TextCopyrightModel>> GetSomeContainsExceptFoundCopyrights(int howMuch, string text, HashSet<int> found);
        Task<TextCopyrightModel> CreateTextCopyright(string text);
        Task EditeTextCopyright(TextCopyrightModel model);
        Task<bool> ThisCopyrightNameIsUsedMore(int topicId, int copyrightId);
        Task DeleteTextCopyright(int copyrightId);
    }
}
