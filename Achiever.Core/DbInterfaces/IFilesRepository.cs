using System.Collections.Generic;
using System.Threading.Tasks;

namespace Achiever.Core.DbInterfaces
{
    public interface IFilesRepository
    {
        Task<string> Create();

        Task<string> StartImageCompression();

        Task FinishImageCompression(string id, string compressedImagePath);
    }
}