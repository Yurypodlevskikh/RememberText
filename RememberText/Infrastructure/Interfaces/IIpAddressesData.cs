using RememberText.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IIpAddressesData
    {
        IEnumerable<IpAddress> GetIpAddresses();
        IpAddress GetIpAddressById(int id);
        IQueryable<IpAddress> GetIpAddressByIpAddress(string ip);
        Task<List<IpAddress>> GetIpAddressByIpAddressAsync(string ip);
        Task<List<IpAddress>> GetIpAddressesByUserId(string userId);
        Task<IpAddress> GetIpAddressByIpAndUserIdAsync(string ip, string UserId);
        IpAddress CreateIpAddress(IpAddress IpAddress);
        Task<IpAddress> CreateIpAddressAsync(IpAddress IpAddress);
        Task<bool> IpAddressExistsAsync(string ip);
        int CreateIpAddressSimple(IpAddress ipAddress);
        Task Edit(IpAddress ipAddress);
        Task DeleteIpAdressesByUserId(string userId);
    }
}
