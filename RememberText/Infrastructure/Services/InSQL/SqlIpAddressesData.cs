using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using RememberText.DAL.Context;
using System.Threading.Tasks;
using RememberText.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlIpAddressesData : IIpAddressesData
    {
        private readonly RememberTextDbContext _db;
        private readonly ILogger<SqlIpAddressesData> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlIpAddressesData(RememberTextDbContext db,
            ILogger<SqlIpAddressesData> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<IpAddress> GetIpAddresses() => _db.IpAddresses;

        public IpAddress GetIpAddressById(int id) => _db.IpAddresses.Find(id);
        public IQueryable<IpAddress> GetIpAddressByIpAddress(string ip) => 
            _db.IpAddresses.Where(x => x.Ip == ip).Include(x => x.Visits).AsNoTracking();
        public async Task<List<IpAddress>> GetIpAddressByIpAddressAsync(string ip) => 
            await _db.IpAddresses.Where(x => x.Ip == ip).Include(x => x.Visits).AsNoTracking().ToListAsync();
        public async Task<IpAddress> GetIpAddressByIpAndUserIdAsync(string ip, string UserId) => 
            await _db.IpAddresses.FirstOrDefaultAsync(x => x.Ip == ip && x.UserId == UserId);
        public async Task<List<IpAddress>> GetIpAddressesByUserId(string userId) =>
            await _db.IpAddresses.Where(x => x.UserId == userId).ToListAsync();
        public IpAddress CreateIpAddress(IpAddress IpAddress)
        {
            if (IpAddress is null)
                throw new ArgumentNullException(nameof(IpAddress));
            
            var ipaddress = _db.IpAddresses.Add(IpAddress);

            _db.SaveChanges();

            return ipaddress.Entity;
        }
        public async Task<IpAddress> CreateIpAddressAsync(IpAddress ipAddress)
        {
            if (ipAddress is null)
                throw new ArgumentNullException(nameof(ipAddress));

            try
            {
                var ipaddress = await _db.IpAddresses.AddAsync(ipAddress);
                await _db.SaveChangesAsync();
                return ipaddress.Entity;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Unable to save ip address in the database: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> IpAddressExistsAsync(string ip)
        {
            return await _db.IpAddresses.AnyAsync(x => x.Ip == ip);
        }

        public int CreateIpAddressSimple(IpAddress ipAddress)
        {
            int n = 0;
            if(ipAddress != null)
            {
                try
                {
                    _db.IpAddresses.Add(ipAddress);
                    n = _db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, $"Unable to save ip address in the database: {ex.Message}");
                }
            }
            return n;
        }

        public async Task Edit(IpAddress ipAddress)
        {
            try
            {
                _db.IpAddresses.Update(ipAddress);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"An error uccurred when editing ip address data: {ex.Message}");
            }
        }

        public async Task DeleteIpAdressesByUserId(string userId)
        {
            List<IpAddress> ipAddresses = await GetIpAddressesByUserId(userId);
            if(ipAddresses != null && ipAddresses.Count > 0)
            {
                try
                {
                    _db.IpAddresses.RemoveRange(ipAddresses);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unexpected error occurred deleting Ip Addresses with User Id '{userId}'.");
                }
            }
        }
    }
}
