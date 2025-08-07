using RememberText.DAL.Context;
using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlVisitService : IRTVisitService
    {
        private readonly RememberTextDbContext _db;
        public SqlVisitService(RememberTextDbContext db)
        {
            _db = db;
        }

        public async Task SaveVisiting(int ipId)
        {
            Visit visit = new Visit
            {
                Visited = DateTime.Now,
                IpId = ipId
            };

            await _db.Visits.AddAsync(visit);
            await _db.SaveChangesAsync();
        }
    }
}
