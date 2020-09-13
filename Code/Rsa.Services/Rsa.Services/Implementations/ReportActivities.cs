using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Rsa.Models.DbEntities;
using Rsa.Services.Abstractions;
using Rsa.Services.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rsa.Services.Implementations
{
    class ReportActivities : IReportActivities
    {
        private readonly ILogger _logger;
        private readonly RsaContext _rsaContext;
        public ReportActivities(
            ILogger<ReportActivities> logger,
           RsaContext rsaContext
            )
        {
            _logger = logger;
            _rsaContext = rsaContext;
        }
        public async Task<ResponseData> CreateReport(ReportHeader reportHeader, SafetyFirstCheck safetyFirstCheck)
        {
            try
            {
                _logger.LogInformation("Create Report Called");

                reportHeader.CreatedOn = DateTime.UtcNow;
                reportHeader.IsSafetyFirstComplete = false;
                reportHeader.IsCustomerEquipmentComplete = false;
                reportHeader.IsVibrationAnalysisComplete = false;
                reportHeader.IsObservationComplete = false;
                reportHeader.IsRecommendationComplete = false;

                _rsaContext.Add(reportHeader);
                if (_rsaContext.SaveChanges() > 0)
                {
                    safetyFirstCheck.ReportHeaderId = reportHeader.Id;
                    _rsaContext.Add(safetyFirstCheck);
                    reportHeader.IsSafetyFirstComplete = true;
                    _rsaContext.Update(reportHeader);
                    await _rsaContext.SaveChangesAsync();
                }

                _logger.LogInformation("Create Report Completed");

                return new ResponseData() { status = ResponseStatus.success, message = "Report Created Successfully." };
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"{nameof(CreateReport)} - Error", ex);
                return new ResponseData() { status = ResponseStatus.error, message = "Report Creation Failed." };
            }
        }

        /// <summary>
        /// It can newly create or modify the existing details
        /// </summary>
        /// <param name="reportHeaderId"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<ResponseData> SaveReportOtherDetails(int reportHeaderId, ReportAllDetailsVm reportAllDetails)
        {
            try
            {
                _logger.LogInformation($"{nameof(SaveReportOtherDetails)} - Called");

                if (reportHeaderId != reportAllDetails.ReportHeaderId || reportAllDetails.SafetyFirstCheck.Id == 0)
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = "Call is not genuine"
                    };

                if (reportAllDetails.SafetyFirstCheck.SafetyFirstCheckDetails == null ||
                    reportAllDetails.SafetyFirstCheck.SafetyFirstCheckDetails.Count != 10)
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = "Data rows are not in expected range in Safety First Check"
                    };

                if(reportAllDetails.SafetyFirstCheck.SafetyFirstCheckDetails.Where(w=>w.Id == 0).Any())
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = "No new entry is allowed in edit mode of Safe First Check"
                    };


                if (reportAllDetails.CustomerEquipmentActivity.Id == 0 && 
                    _rsaContext.CustomerEquipmentActivities.Any(a=>a.ReportHeaderId == reportHeaderId)) {
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = $"Customer Equipment Activity is already exists for Report Header Id {reportHeaderId}"
                    };
                }

                if (reportAllDetails.VibrationAnalysisHeader.Id == 0 &&
                    _rsaContext.VibrationAnalysisHeaders.Any(a => a.ReportHeaderId == reportHeaderId))
                {
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = $"Vibration Analysis is already exists for Report Header Id {reportHeaderId}"
                    };
                }

                var reportHeader = _rsaContext.ReportHeaders.Find(reportHeaderId);
                reportHeader.UpdatedOn = DateTime.UtcNow;
                reportHeader.IsCustomerEquipmentComplete = true;
                reportHeader.IsVibrationAnalysisComplete = true;
                reportHeader.IsObservationComplete = true;
                reportHeader.IsRecommendationComplete = true;

                _rsaContext.Update(reportHeader);
                _rsaContext.Update(reportAllDetails.SafetyFirstCheck);

                if (reportAllDetails.CustomerEquipmentActivity.Id == 0)
                {
                    reportAllDetails.CustomerEquipmentActivity.ReportHeaderId = reportHeaderId;
                    _rsaContext.Add(reportAllDetails.CustomerEquipmentActivity);
                }
                else {
                    reportAllDetails.CustomerEquipmentActivity.ReportHeaderId = reportHeaderId;
                    _rsaContext.Update(reportAllDetails.CustomerEquipmentActivity);
                }

                if (reportAllDetails.VibrationAnalysisHeader.Id == 0)
                {
                    reportAllDetails.VibrationAnalysisHeader.ReportHeaderId = reportHeaderId;
                    _rsaContext.Add(reportAllDetails.VibrationAnalysisHeader);
                }
                else {
                    reportAllDetails.VibrationAnalysisHeader.ReportHeaderId = reportHeaderId;
                    _rsaContext.Update(reportAllDetails.VibrationAnalysisHeader);
                }

                await _rsaContext.SaveChangesAsync();


                return new ResponseData() { status = ResponseStatus.success,data= reportHeaderId, message = "Report Saved Successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SaveReportOtherDetails)} - Error",ex);
                return new ResponseData() { status = ResponseStatus.error, data = reportHeaderId, message = "Report Save Failed." };
            }

        }

    }
}
