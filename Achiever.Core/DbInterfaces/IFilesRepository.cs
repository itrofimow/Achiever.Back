using System.Threading.Tasks;

namespace Achiever.Core.DbInterfaces
{
    public interface IFilesRepository
    {
        Task<string> Create();
    }
}