using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rsa.Models.DbEntities;
using Rsa.Services.Abstractions;
using Rsa.Services.ViewModels;

namespace Rsa.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IReportActivities _reportActivities;
        public ReportController(ILogger<ReportController> logger,IReportActivities reportActivities)
        {
            _logger = logger;
            _reportActivities = reportActivities;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Route("registersafetyfirstcheck")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RegisterSafetyFirstCheck([FromBody] HeaderData headerData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
               var res = await _reportActivities.CreateReport(headerData.ReportHeader, headerData.SafetyFirstCheck);

                return Ok(res);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RegisterSafetyFirstCheck)} - Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { data = "Error occurred, Please contact admin"});
            }

        }

        [HttpPost]
        [Consumes("application/json")]
        [Route("savereportallotherdetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SaveReportAllOtherDetails([FromQuery]int reportHeaderId, [FromBody] ReportAllDetailsVm reportAllDetails)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var res = await _reportActivities.SaveReportOtherDetails(reportHeaderId, reportAllDetails);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SaveReportAllOtherDetails)} - Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { data = "Error occurred. Please contact admin" });
            }

        }
    }
}
