using RememberText.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTLanguageService
    {
        Task<IEnumerable<Language>> PrimaryLanguagesAsync();
        Task<IEnumerable<Language>> GetAllAvailableLangAsync();
        Task<Language> GetLanguageByIdAsync(int id);
        Task<Language> GetLanguageByLangCodeAsync(string langCode);
    }
}
