using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Data;
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
        public async Task<IEnumerable<Language>> GetAllAvailableLangAsync() => await _db.Languages.OrderBy(x => x.LangName).ToListAsync();
        public async Task<Language> GetLanguageByIdAsync(int id) => await _db.Languages.FindAsync(id);

        public async Task<Language> GetLanguageByLangCodeAsync(string langCode) => await _db.Languages.FirstOrDefaultAsync(x => x.LangCode == langCode);
    }
}
