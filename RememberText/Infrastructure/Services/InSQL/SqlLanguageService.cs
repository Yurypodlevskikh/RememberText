using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlLanguageService : IRTLanguageService
    {
        private readonly RememberTextDbContext _db;

        public SqlLanguageService(RememberTextDbContext db) => _db = db;
        public async Task<IEnumerable<Language>> PrimaryLanguagesAsync() => await _db.Languages.Where(x => x.PrimaryLang == true).ToListAsync();
        public async Task<IEnumerable<Language>> PrimaryLanguageToSelect() => 
            await _db.Languages.Where(x => x.PrimaryLang == true && x.LangCode != "mu-Mu").ToListAsync();
        public async Task<IEnumerable<Language>> GetAllAvailableLangAsync() => await _db.Languages.OrderBy(x => x.LangName).ToListAsync();
        public async Task<List<Language>> GetUserPreferredLanguagesAsync(string userLangStr)
        {
            List<Language> selectedLang = new List<Language>();
            string[] selectedLangArr = userLangStr.Split(',');
            foreach (string langId in selectedLangArr)
            {
                if (!string.IsNullOrEmpty(langId))
                {
                    if (Int32.TryParse(langId, out int id))
                    {
                        var lang = await GetLanguageByIdAsync(id);
                        if (lang != null)
                        {
                            selectedLang.Add(lang);
                        }
                    }
                }
            }

            return selectedLang;
        }
        public async Task<Language> GetPrimaryLangByCommonLang(string commonLang) => 
            await _db.Languages.FirstOrDefaultAsync(x => x.PrimaryLang == true && x.LangCode.StartsWith(commonLang));
        public async Task<Language> GetLanguageByIdAsync(int id) => await _db.Languages.FindAsync(id);
        public async Task<Language> GetLanguageByLangCodeAsync(string langCode) => await _db.Languages.FirstOrDefaultAsync(x => x.LangCode == langCode);
        public int GetLanguageIdByLangCode(string langCode) => _db.Languages.FirstOrDefault(x => x.LangCode == langCode).Id;
        public async Task<bool> ExistsLangCode(string langCode) => await _db.Languages.AnyAsync(x => x.LangCode == langCode);
    }
}
