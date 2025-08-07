using RememberText.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTLanguageService
    {
        Task<IEnumerable<Language>> PrimaryLanguagesAsync();
        Task<IEnumerable<Language>> PrimaryLanguageToSelect();
        Task<IEnumerable<Language>> GetAllAvailableLangAsync();
        Task<List<Language>> GetUserPreferredLanguagesAsync(string userLangStr);
        Task<Language>GetPrimaryLangByCommonLang(string commonLang);
        Task<Language> GetLanguageByIdAsync(int id);
        Task<Language> GetLanguageByLangCodeAsync(string langCode);
        int GetLanguageIdByLangCode(string langCode);
        Task<bool> ExistsLangCode(string langCode);
    }
}
