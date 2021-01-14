
using Rsa.Models.DbEntities;
using Rsa.Services.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rsa.Services.Abstractions
{
    public interface ICommonMasterActivities
    {
        IAsyncEnumerable<CommonMaster> GetAll();

        Task<ResponseData> Save(CommonMaster data);
    }
}
