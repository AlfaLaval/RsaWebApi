﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Rsa.Models.DbEntities;
using Rsa.Services.Abstractions;
using Rsa.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rsa.Services.Implementations
{
    class ReportActivities : IReportActivities
    {
        private readonly ILogger _logger;
        private readonly RsaContext _rsaContext;
        private readonly string ImageUploadPath = @"E:\Github\Alfa.Laval.Rsa\ImagesUpload\";
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
                reportHeader.CreatedBy = 1;
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

                return new ResponseData()
                {
                    status = ResponseStatus.success,
                    data = new { reportHeader, safetyFirstCheck },
                    message = "Report Created Successfully."
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"{nameof(CreateReport)} - Error", ex);
                return new ResponseData() { status = ResponseStatus.error, message = "Report Creation Failed." };
            }
        }

        public async Task<ResponseData> GetReportDetails(int reportHeaderId)
        {
            ReportAllDetailsVm reportAllDetailsVm = new ReportAllDetailsVm();
            reportAllDetailsVm.ReportHeaderId = reportHeaderId;
            reportAllDetailsVm.SafetyFirstCheck = _rsaContext.SafetyFirstChecks.AsNoTracking()
                .Include(a=>a.SafetyFirstCheckDetails)
                .Where(w => w.ReportHeaderId == reportHeaderId).FirstOrDefault();

            if (reportAllDetailsVm.SafetyFirstCheck == null)
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = "Error while getting data."
                };

            reportAllDetailsVm.CustomerEquipmentActivity = _rsaContext.CustomerEquipmentActivities.AsNoTracking()
                .Where(w => w.ReportHeaderId == reportHeaderId).FirstOrDefault();

            if(reportAllDetailsVm.CustomerEquipmentActivity == null)
            {
                reportAllDetailsVm.CustomerEquipmentActivity = new CustomerEquipmentActivity()
                {
                    ReportHeaderId = reportHeaderId
                };
            }
            reportAllDetailsVm.VibrationAnalysisHeader = _rsaContext.VibrationAnalysisHeaders.AsNoTracking()
                .Include(a => a.VibrationAnalysis)
                .Where(w => w.ReportHeaderId == reportHeaderId)
                .FirstOrDefault();

            if (reportAllDetailsVm.VibrationAnalysisHeader == null)
            {
                reportAllDetailsVm.VibrationAnalysisHeader = new VibrationAnalysisHeader()
                {
                    ReportHeaderId = reportHeaderId
                };
            }
            List<Observation> obs = _rsaContext.Observations.AsNoTracking().Where(w => w.ReportHeaderId == reportHeaderId).ToList();
            if (obs == null)
                obs = new List<Observation>();
            reportAllDetailsVm.Observations = obs;

            List<Recommendation> recomms = _rsaContext.Recommendations.AsNoTracking().Where(w => w.ReportHeaderId == reportHeaderId).ToList();
            if (recomms == null)
                recomms = new List<Recommendation>();
            reportAllDetailsVm.Recommendations = recomms;

            return new ResponseData()
            {
                status = ResponseStatus.success,
                data = reportAllDetailsVm
            };

        }

        public async Task<ResponseData> GetReports()
        {
            var reports = _rsaContext.ReportHeaders.AsNoTracking()
                .Join(_rsaContext.SafetyFirstChecks.AsNoTracking(), rh => rh.Id, sfc => sfc.ReportHeaderId,
                    (rh, sfc) => new { rh, sfc })
                .Select(s => new
                {
                    s.rh.Id,
                    s.rh.CreatedBy,
                    s.rh.CreatedOn,
                    s.sfc.JobOrderNumber,
                    s.sfc.ProjectName
                }).OrderByDescending(o=>o.CreatedOn);

            var data = await reports.ToListAsync();
            return new ResponseData()
            {
                status = ResponseStatus.success,
                data = data
            };
        }

        public async Task<ResponseData> SaveImage(VmImageSaveEntity imageEntity)
        {
            try
            {
                _logger.LogInformation($"{nameof(SaveImage)} - Called");

                var newGuid = Guid.NewGuid().ToString();
                ImageHouse imageHouse = new ImageHouse();
                imageHouse.ImageFileGuid = newGuid;
                imageHouse.Entity = imageEntity.Entity;
                imageHouse.ImageLabel = imageEntity.ImageLabel;
                imageHouse.ReportHeaderId = imageEntity.ReportHeaderId;
                imageHouse.EntityRefGuid = imageEntity.EntityRefGuid;
                string filePath = $"{ImageUploadPath}{newGuid}.jpeg";
                File.WriteAllBytes(filePath, Convert.FromBase64String(imageEntity.Base64.Split("data:image/jpeg;base64,")[1]));
                _rsaContext.Add(imageHouse);
                if (await _rsaContext.SaveChangesAsync() > 0)
                {
                    //MyImage.ResizeJPEG(filePath);
                    _logger.LogInformation($"{nameof(SaveImage)} - Completed");
                    return new ResponseData()
                    {
                        status = ResponseStatus.success,
                        data = imageHouse.Id,
                        message = "Image Saved Successfully."
                    };
                }
                
                return new ResponseData()
                {
                    status = ResponseStatus.warning,
                    data = 1,
                    message = "Try again after some time."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SaveImage)} - Error",ex);
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = "Error occurred while saving image. Please contact admin."
                };
            }
        }

        public async Task<ResponseData> DeleteImageById(int imageHouseId)
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteImageById)} - Called");
                if (imageHouseId <= 0)
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = "Image is not valid"
                    };

                var image = _rsaContext.ImageHouses.Find(imageHouseId);

                if (image == null)
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = "Image is not valid"
                    };

                _rsaContext.Remove(image);

                if (await _rsaContext.SaveChangesAsync() > 0)
                {

                    File.Delete($"{ImageUploadPath}{image.ImageFileGuid}.jpeg");
                    _logger.LogInformation($"{nameof(DeleteImageById)} - Completed");
                    return new ResponseData()
                    {
                        status = ResponseStatus.success,
                        message = "Image deleted successfully."
                    };
                }

                return new ResponseData()
                {
                    status = ResponseStatus.warning,
                    message = "Image is not valid"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(DeleteImageById)}");
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = "Error occurred. Please contact admin"
                };
            }
        }

        public async Task<ResponseData> GetImages(int reportHeaderId, string entity,Guid EntityRefGuid)
        {
            try
            {
                List<VmImageSaveEntity> vmImageDataList = new List<VmImageSaveEntity>();
                var images = await _rsaContext.ImageHouses.AsNoTracking()
                    .Where(w => w.Entity == entity && w.ReportHeaderId == reportHeaderId).ToListAsync();
                foreach (var img in images)
                {
                    vmImageDataList.Add(
                        new VmImageSaveEntity
                        {
                            Base64 = GetBase64(img.ImageFileGuid),
                            ImageLabel = img.ImageLabel,
                            Entity = img.Entity,
                            EntityRefGuid = img.EntityRefGuid,
                            ReportHeaderId = reportHeaderId
                        });
                }
                return new ResponseData()
                {
                    status = ResponseStatus.success,
                    data = vmImageDataList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetImages)} - Error.", ex);
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = ex.Message
                };
            }
        }

        private string GetBase64(string guid)
        {
            try
            {
                string filePath = $"{ImageUploadPath}{guid}.jpeg";
                var fileBytes = File.ReadAllBytes(filePath);

                string encodedFile = Convert.ToBase64String(fileBytes);

                return "data:image/jpeg;base64," + encodedFile;
            }
            catch (Exception ex)
            {
                throw ex;
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

                foreach(var obs in reportAllDetails.Observations)
                {
                    obs.ReportHeaderId = reportHeaderId;
                    if (obs.Id == 0)
                        _rsaContext.Add(obs);
                    else
                        _rsaContext.Update(obs);
                }
                foreach (var recomm in reportAllDetails.Recommendations)
                {
                    recomm.ReportHeaderId = reportHeaderId;
                    if (recomm.Id == 0)
                        _rsaContext.Add(recomm);
                    else
                        _rsaContext.Update(recomm);
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
