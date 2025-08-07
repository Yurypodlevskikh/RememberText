using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using RememberText.Models;
using RememberText.RTTools;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlNormalizedTagService : IRTNormalizedTagService
    {
        private readonly RememberTextDbContext _db;

        public SqlNormalizedTagService(RememberTextDbContext db)
        {
            _db = db;
        }
        public async Task<int?> CreateNormTagAsync(string tagName)
        {
            string normTagName = tagName.ToUpperInvariant();

            NormalizedTag normalizedTag = await GetNormTagByNormTagName(normTagName);

            if (normalizedTag == null)
            {
                normalizedTag = new NormalizedTag 
                {
                    NormalizedTagName = normTagName
                };

                try
                {
                    await _db.NormalizedTags.AddAsync(normalizedTag);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateException /*ex*/)
                {
                    return null;
                }
            }

            return normalizedTag.Id;
        }

        public Task<NormalizedTag> GetNormTagByNormTagName(string normTagName)
            => _db.NormalizedTags.FirstOrDefaultAsync(x => x.NormalizedTagName == normTagName);

        public async Task<string> DeleteNormTag(int normTagId)
        {
            var normTag = await _db.NormalizedTags.FindAsync(normTagId);

            if(normTag == null)
            {
                return "The Normalized Tag is Not Found";
            }

            string response = "";

            try
            {
                _db.NormalizedTags.Remove(normTag);
                await _db.SaveChangesAsync();
                response = "The Normalized Tag has been removed successful.";
            }
            catch(DbUpdateConcurrencyException /*ex*/)
            {
                response = "Concurrenct Exception, Normalized Tag";
            }

            return response;
        }

        public async Task<ResponseFromRawQuery> DeleteMultipleNormTags(string ids)
        {
            string sqlExpression = $@"DELETE FROM NormalizedTags WHERE Id IN ({ids})";
            return await RawSqlQueryHelper.RawNonQuery(_db, sqlExpression);
        }
    }
}
