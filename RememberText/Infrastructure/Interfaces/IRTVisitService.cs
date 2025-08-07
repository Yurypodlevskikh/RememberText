using System.Threading.Tasks;

namespace RememberText.Infrastructure.Interfaces
{
    public interface IRTVisitService
    {
        Task SaveVisiting(int ipId);
    }
}
