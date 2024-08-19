using RememberText.Domain.Entities;
using RememberText.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using RememberText.DAL.Context;
using System.Threading.Tasks;
using RememberText.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RememberText.Infrastructure.Services.InSQL
{
    public class SqlIpAddressesData : IIpAddressesData
    {
        private readonly RememberTextDbContext _db;
        
        public SqlIpAddressesData(RememberTextDbContext db) => _db = db;

        public IEnumerable<IpAddress> GetIpAddresses() => _db.IpAddresses;

        public IpAddress GetIpAddressById(int id) => _db.IpAddresses.Find(id);
        public IpAddress GetIpAddressByIpAddress(string ip) => _db.IpAddresses.FirstOrDefault(x => x.Ip == ip);
        public async Task<IpAddress> GetIpAddressByIpAddressAsync(string ip) => await _db.IpAddresses.FirstOrDefaultAsync(x => x.Ip == ip);
        public IpAddress CreateIpAddress(IpAddress IpAddress)
        {
            if (IpAddress is null)
                throw new ArgumentNullException(nameof(IpAddress));
            
            var ipaddress = _db.IpAddresses.Add(IpAddress);

            _db.SaveChanges();

            return ipaddress.Entity;
        }
        public async Task<IpAddress> CreateIpAddressAsync(IpAddress IpAddress)
        {
            if (IpAddress is null)
                throw new ArgumentNullException(nameof(IpAddress));
            
            var ipaddress = await _db.IpAddresses.AddAsync(IpAddress);

            await _db.SaveChangesAsync();

            return ipaddress.Entity;
        }

        public int CreateIpAddressSimple(IpAddress ipAddress)
        {
            int n = 0;
            if(ipAddress != null)
            {
                _db.IpAddresses.Add(ipAddress);
                n = _db.SaveChanges();
            }
            return n;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Edit(int id, IpAddress IpAddress)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
