using System.Threading.Tasks;

namespace PlayStats.Services
{
    public class ApplicationUpdateService : IApplicationUpdateService
    {
        public Task<bool> CheckForUpdates()
        {
            return Task.FromResult(false);
        }

        public Task Update()
        {
            return Task.CompletedTask;
        }
    }

    public interface IApplicationUpdateService
    {
        Task<bool> CheckForUpdates();
        Task Update();
    }
}
