using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rsa.Models.DbEntities;
using Rsa.Services.Abstractions;
using Rsa.Services.ViewModels;


namespace Rsa.WebApi.Controllers
{

    [Route("api/commonmaster")]
    [ApiController]
    public class CommonMasterController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICommonMasterActivities _commonMasterActivities;
        public CommonMasterController(ILogger<CommonMasterController> logger, ICommonMasterActivities commonMasterActivities)
        {
            _logger = logger;
            _commonMasterActivities = commonMasterActivities;
        }

        [HttpGet]
        [Consumes("application/json")]
        [Route("getall")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetAll()
        {
            try
            {
                var res = _commonMasterActivities.GetAll();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetAll)} - Error");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { data = "Error occurred. Please contact admin" });
            }

        }

        [HttpPost]
        [Consumes("application/json")]
        [Route("save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Save(CommonMaster data)
        {
            try
            {
                var res = await _commonMasterActivities.Save(data);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Save)} - Error");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { data = "Error occurred. Please contact admin" });
            }

        }
    }
}
