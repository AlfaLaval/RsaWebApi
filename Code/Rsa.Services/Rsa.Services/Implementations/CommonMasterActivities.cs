using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsa.Models.DbEntities;
using Rsa.Services.Abstractions;
using Rsa.Services.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsa.Services.Implementations
{
    public class CommonMasterActivities : ICommonMasterActivities
    {
        private readonly ILogger _logger;
        private readonly RsaContext _rsaContext;
        private IConfiguration _configuration;
        public CommonMasterActivities(
                  ILogger<CommonMasterActivities> logger,
                  IConfiguration configuration,
                  RsaContext rsaContext
            )
        {
            _logger = logger;
            _configuration = configuration;
            _rsaContext = rsaContext;
        }
        public IAsyncEnumerable<CommonMaster> GetAll()
        {
           return _rsaContext.CommonMasters.OrderBy(w=>w.Type).AsAsyncEnumerable();
        }

        public async Task<ResponseData> Save(CommonMaster data)
        {
            try
            {
                _logger.LogInformation($"{nameof(Save)} - Called");

                var allMasterData = _rsaContext.CommonMasters.AsNoTracking().ToList();

                if (data.Id > 0)
                {
                    if(allMasterData.Any(a=>a.DisplayText == data.DisplayText && data.Type == a.Type && a.Id!=data.Id))
                    {
                        return new ResponseData()
                        {
                            status = ResponseStatus.warning,
                            message = $"The Display Text {data.DisplayText} in master {data.Type} is already exists."
                        };
                    }
                    _rsaContext.CommonMasters.Update(data);
                }
                else
                {
                    if (allMasterData.Any(a => a.DisplayText == data.DisplayText && data.Type == a.Type))
                    {
                        return new ResponseData()
                        {
                            status = ResponseStatus.warning,
                            message = $"The Display Text {data.DisplayText} in master {data.Type} is already exists."
                        };
                    }

                    _rsaContext.CommonMasters.Add(data);
                }
                await _rsaContext.SaveChangesAsync();

                _logger.LogInformation($"{nameof(Save)} - completed");
                return new ResponseData()
                {
                    status = ResponseStatus.success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Save)} - Error.", ex);
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = ex.Message
                };
            }
        }
    }
}
