using System.Threading.Tasks;
using Achiever.Core.Models.Test;

namespace Achiever.Core.DbInterfaces
{
    public interface ITestRepository
    {
        Task InsertB(ModelB model);

        Task InsertA(ModelA model);

        Task<ModelA> Get(string id);
    }
}