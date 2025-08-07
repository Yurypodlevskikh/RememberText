using Microsoft.EntityFrameworkCore;
using RememberText.DAL.Context;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlGuestBookService : IRTGuestBookService
    {
        private readonly RememberTextDbContext _db;
        public SqlGuestBookService(RememberTextDbContext db) => _db = db;

        public IQueryable<GuestBookEntry> GetAllMessanges() =>
            _db.GuestBookEntries.Include(g => g.User);

        public async Task AddMessage(GuestBookEntry entry)
        {
            _db.Add(entry);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveMessage(int id)
        {
            var guestBookEntry = await _db.GuestBookEntries.FindAsync(id);
            if(guestBookEntry != null)
            {
                _db.GuestBookEntries.Remove(guestBookEntry);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<GuestBookEntry> GetMessageByIdAsync(int id)
        {
            return await _db.GuestBookEntries
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
