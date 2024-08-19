using RememberText.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IIpAddressesData
    {
        IEnumerable<IpAddress> GetIpAddresses();
        IpAddress GetIpAddressById(int id);
        IpAddress GetIpAddressByIpAddress(string ip);
        Task<IpAddress> GetIpAddressByIpAddressAsync(string ip);
        IpAddress CreateIpAddress(IpAddress IpAddress);
        Task<IpAddress> CreateIpAddressAsync(IpAddress IpAddress);
        int CreateIpAddressSimple(IpAddress ipAddress);
        void Edit(int id, IpAddress IpAddress);
        bool Delete(int id);
        void SaveChanges();
    }
}
