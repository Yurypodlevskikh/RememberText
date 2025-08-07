using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RememberText.DAL.Context;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlTextCopyrightService : IRTTextCopyrightService
    {
        private readonly RememberTextDbContext _db;
        private readonly ILogger<SqlTextCopyrightService> _logger;

        public SqlTextCopyrightService(
            RememberTextDbContext db,
            ILogger<SqlTextCopyrightService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<TextCopyrightModel> GetTextCopyrightById(int copyrightId) =>
            await _db.TextCopyrights.FindAsync(copyrightId);

        public async Task<TextCopyrightModel> GetTextCopyrightByCopyrightName(string copyrightName) =>
            await _db.TextCopyrights.FirstOrDefaultAsync(x => x.CopyrightName == copyrightName);

        public async Task<List<TextCopyrightModel>> GetSomeFirstStartWithCopyrights(int? howMuch, string text)
        {
            var copyrights = new List<TextCopyrightModel>();
            string normalizedText = text.ToUpperInvariant();
            if(howMuch != null)
            {
                copyrights = await _db.TextCopyrights.Where(x => x.NormalizedCopyrightName.StartsWith(normalizedText)).Take((int)howMuch).ToListAsync();
            }
            else
            {
                copyrights = await _db.TextCopyrights.Where(x => x.NormalizedCopyrightName.StartsWith(normalizedText)).ToListAsync();
            }
            
            return copyrights;
        }

        public async Task<List<TextCopyrightModel>> GetSomeFirstContainsCopyrights(int? howMuch, string text)
        {
            var copyrights = new List<TextCopyrightModel>();
            string normalizedText = text.ToUpperInvariant();
            if (howMuch != null)
            {
                copyrights = await _db.TextCopyrights.Where(x => x.NormalizedCopyrightName.Contains(normalizedText)).Take((int)howMuch).ToListAsync();
            }
            else
            {
                copyrights = await _db.TextCopyrights.Where(x => x.NormalizedCopyrightName.Contains(normalizedText)).ToListAsync();
            }

            return copyrights;
        }public async Task<List<TextCopyrightModel>> GetSomeContainsExceptFoundCopyrights(int howMuch, string text, HashSet<int> found)
        {
            var copyrights = new List<TextCopyrightModel>();
            string normalizedText = text.ToUpperInvariant();
            if (found != null)
            {
                copyrights = await _db.TextCopyrights.Where(x => x.NormalizedCopyrightName.Contains(normalizedText) && !found.Contains(x.Id)).Take((int)howMuch).ToListAsync();
            }
            else
            {
                copyrights = await _db.TextCopyrights.Where(x => x.NormalizedCopyrightName.Contains(normalizedText)).Take((int)howMuch).ToListAsync();
            }

            return copyrights;
        }

        public async Task<TextCopyrightModel> CreateTextCopyright(string copyrightName)
        {
            try
            {
                if(!string.IsNullOrEmpty(copyrightName))
                {
                    string normalizedCopyright = copyrightName.ToUpperInvariant();
                    var copyright = new TextCopyrightModel
                    {
                        CopyrightName = copyrightName,
                        NormalizedCopyrightName = normalizedCopyright
                    };

                    _db.Add(copyright);
                    await _db.SaveChangesAsync();
                    return copyright;
                }
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred when added copyright name: '{ex.Message}'.");
            }

            return null;
        }

        public async Task DeleteTextCopyright(int copyrightId)
        {
            try
            {
                var textCopyright = await GetTextCopyrightById(copyrightId);
                if(textCopyright != null)
                {
                    _db.TextCopyrights.Remove(textCopyright);
                    await _db.SaveChangesAsync();
                }
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred when deleted Copyright entity: '{ex.Message}'.");
            }
        }

        public async Task EditeTextCopyright(TextCopyrightModel model)
        {
            try
            {
                _db.TextCopyrights.Update(model);
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex, $"An error occurred when edited Copyright: '{ex.Message}'");
            }
        }

        public Task<bool> ThisCopyrightNameIsUsedMore(int topicId, int copyrightId) => 
            _db.Topics.AnyAsync(x => x.Id != topicId && x.CopyrightId == copyrightId);
    }
}
