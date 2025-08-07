using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using RememberText.Infrastructure.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlTagAssignmentService : IRTTagAssignmentService
    {
        private readonly RememberTextDbContext _db;
        public SqlTagAssignmentService(RememberTextDbContext db) => _db = db;

        public async Task<string> CreateTagAssignmentAsync(int textId, int tagId, string langSubtag)
        {
            string exception = string.Empty;
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "dbo.spCreateTagAssignment";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@langSubtag", langSubtag));
                command.Parameters.Add(new SqlParameter("@textId", textId));
                command.Parameters.Add(new SqlParameter("@tagId", tagId));

                await _db.Database.OpenConnectionAsync();
                object scalResult = await command.ExecuteNonQueryAsync();

                if (int.TryParse(scalResult?.ToString(), out int scalInt))
                {
                    if(scalInt >= 0)
                    {
                        exception = @"Unable to assign this tag to the text. Try again, and if 
                            the problem persists see your system administrator.";
                    }
                }

                await _db.Database.CloseConnectionAsync();
            }
            return exception;
        }

        public TagAssignmentGeneral GetTagAssignmentByTextIdAndTagId(string langCode, int textId, int tagId)
        {
            string generalLangCode = langCode.Substring(0, 2);
            switch (generalLangCode)
            {
                case "en":
                    var enTagAssign = _db.EnTagAssignments.Where(x => x.TagId == tagId && x.TextId == textId).FirstOrDefault();
                    return enTagAssign != null ? TagMapping.TagAssignToGeneral(enTagAssign) : null;
                case "sv":
                    var svTagAssign = _db.SvTagAssignments.Where(x => x.TagId == tagId && x.TextId == textId).FirstOrDefault();
                    return svTagAssign != null ? TagMapping.TagAssignToGeneral(svTagAssign) : null;
                case "ru":
                    var ruTagAssign = _db.RuTagAssignments.Where(x => x.TagId == tagId && x.TextId == textId).FirstOrDefault();
                    return ruTagAssign != null ? TagMapping.TagAssignToGeneral(ruTagAssign) : null;
                default:
                    return null;
            }
        }

        public IEnumerable<TagAssignmentGeneral> GetTagAssignmentsByTagId(string langCode, int tagId)
        {
            string generalLangCode = langCode.Substring(0, 2);
            switch (generalLangCode)
            {
                case "en":
                    var enTagAssign = _db.EnTagAssignments.Where(x => x.TagId == tagId);
                    return enTagAssign != null ? TagMapping.TagAssignToGeneral(enTagAssign) : null;
                case "sv":
                    var svTagAssign = _db.SvTagAssignments.Where(x => x.TagId == tagId);
                    return svTagAssign != null ? TagMapping.TagAssignToGeneral(svTagAssign) : null;
                case "ru":
                    var ruTagAssign = _db.RuTagAssignments.Where(x => x.TagId == tagId);
                    return ruTagAssign != null ? TagMapping.TagAssignToGeneral(ruTagAssign) : null;
                default:
                    return null;
            }
        }

        public IEnumerable<TagAssignmentGeneral> GetTagAssignmentsByTextId(string langCode, int textId)
        {
            string generalLangCode = langCode.Substring(0, 2);
            switch (generalLangCode)
            {
                case "en":
                    var enTagAssign = _db.EnTagAssignments.Where(x => x.TextId == textId);
                    return enTagAssign != null ? TagMapping.TagAssignToGeneral(enTagAssign) : null;
                case "sv":
                    var svTagAssign = _db.SvTagAssignments.Where(x => x.TextId == textId);
                    return svTagAssign != null ? TagMapping.TagAssignToGeneral(svTagAssign) : null;
                case "ru":
                    var ruTagAssign = _db.RuTagAssignments.Where(x => x.TextId == textId);
                    return ruTagAssign != null ? TagMapping.TagAssignToGeneral(ruTagAssign) : null;
                default:
                    return null;
            }
        }
    }
}
