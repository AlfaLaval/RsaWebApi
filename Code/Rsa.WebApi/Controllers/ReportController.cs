using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rsa.Services.Abstractions;
using Rsa.Services.ViewModels;

namespace Rsa.WebApi.Controllers
{
    [Route("api/report")]
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
                return BadRequest(ModelState);

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
        public async Task<ActionResult> SaveReportAllOtherDetails([FromQuery]Guid reportHeaderGuid, [FromBody] ReportAllDetailsVm reportAllDetails)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var res = await _reportActivities.SaveReportOtherDetails(reportHeaderGuid, reportAllDetails);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SaveReportAllOtherDetails)} - Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { data = "Error occurred. Please contact admin" });
            }

        }

        [HttpGet]
        [Consumes("application/json")]
        [Route("getreports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetReports()
        {
            try
            {
                var res = await _reportActivities.GetReports();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetReports)} - Error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { data = "Error occurred. Please contact admin" });
            }

        }

        [HttpGet]
        [Consumes("application/json")]
        [Route("getreportdetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetReportDetails([FromQuery]Guid reportHeaderGuid)
        {
            try
            {
                if (reportHeaderGuid == null)
                    return BadRequest();

                var res = await _reportActivities.GetReportDetails(reportHeaderGuid);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"{nameof(GetReportDetails)} - Error");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { data = "Error occurred. Please contact admin" });
            }

        }

        
        [HttpPost]
        [Route("saveimage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SaveImage(VmImageSaveEntity imageData)
        {
            var result = await _reportActivities.SaveImage(imageData);
            return Ok(result);
        }

        [HttpPost]
        [Route("deleteimagebyid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteImageById([FromBody]int imageHouseId)
        {
            var result = await _reportActivities.DeleteImageById(imageHouseId);
            return Ok(result);
        }

        [HttpGet]
        [Route("getimagesbyentityrefguid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetImagesByEntityRefGuid([FromQuery]Guid reportHeaderGuid, 
            [FromQuery] string entity,[FromQuery] string enityRefGuid)
        {
            if (Guid.TryParse(enityRefGuid, out var guid))
            {
                var result = await _reportActivities.GetImagesByEntityRefGuid(reportHeaderGuid, entity, guid);
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("getimagesbyimagelabels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetImagesByImageLabels([FromQuery] Guid reportHeaderGuid,
            [FromQuery] string entity, [FromQuery] string[] imageLabels)
        {

            var result = await _reportActivities.GetImagesByImageLabels(reportHeaderGuid, entity, imageLabels);
            return Ok(result);
        }

        //[Authorize]
        //[HttpGet]
        //[Route("secret")]
        //public async Task<ActionResult> Secret()
        //{
        //    return Ok("From Secret");
        //}

        //[HttpGet]
        //[Route("authenticate")]
        //public async Task<ActionResult> Authenticate() {
        //    return Ok("Authenticate");
        //}


        [Route("sendtosupervisor")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendToSuperVisor([FromQuery] Guid reportHeaderGuid, [FromQuery] string from)
        {
            if(string.IsNullOrWhiteSpace(from))
                return BadRequest();
            
            var result = await _reportActivities.SendToSuperVisor(reportHeaderGuid, from);
            return Ok(result);
        }


        [Route("getuserlogindata")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetUserLoginData([FromQuery] string username, [FromQuery] string password)
        {
            var result = await _reportActivities.GetUserLoginData(username,password);
            return Ok(result);
        }

    }
}
