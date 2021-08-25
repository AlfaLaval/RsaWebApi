using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsa.Common.Constants;
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
        private readonly string ImageUploadPath = "";
        private IConfiguration _configuration;

        public static IDictionary<int, string> parameterUnits = new Dictionary<int, string>() {
                        { 1, "Sludge Feed~m3/hr" },
                        { 2, "Polymer Feed~lph" },

                        { 3, "Differential Speed~(rpm)" },
                        { 4, "Bowl Speed~(rpm)" },
                        { 5, "Torque~(kNm)" },

                        { 6, "MD side Bearing~Temp: (Deg. C)" },
                        { 7, "BD side Bearing~Temp: (Deg. C)" },

                        { 8,  "MD side Bearing Vibration (mm/s) -~Min" },
                        { 9,  "MD side Bearing Vibration (mm/s) -~Max" },
                        { 10, "BD side Bearing Vibration (mm/s) -~Min" },
                        { 11, "BD side Bearing Vibration (mm/s) -~Max" },

                    };
        public ReportActivities(
            ILogger<ReportActivities> logger,
            IConfiguration configuration,
           RsaContext rsaContext
            )
        {
            _logger = logger;
            _configuration = configuration;
            _rsaContext = rsaContext;
            ImageUploadPath = _configuration.GetValue<string>("ImageSec:ImageUploadPath");
        }

        public async Task<ResponseData> SyncOfflineData(ReportHeader rptHdr, ReportAllDetailsVm reportAllDetails)
        {
            try
            {
                _logger.LogInformation($"{nameof(SyncOfflineData)} - Called");

                var existHeaderData = _rsaContext.ReportHeaders.AsNoTracking().Where(w => w.ReportGuid == rptHdr.ReportGuid).FirstOrDefault();

                rptHdr.CreatedOn = DateTime.UtcNow;
                rptHdr.CreatedBy = rptHdr.CreatedBy;
                rptHdr.IsSafetyFirstComplete = false;
                rptHdr.IsCustomerEquipmentComplete = false;
                rptHdr.IsVibrationAnalysisComplete = false;
                rptHdr.IsObservationComplete = false;
                rptHdr.IsRecommendationComplete = false;
                rptHdr.ApprovedBy = null;
                rptHdr.DocTriggerFrom = "DRAFT";

                if (existHeaderData == null)
                    _rsaContext.Add(rptHdr);
                else
                {
                    rptHdr.Id = existHeaderData.Id;
                    _rsaContext.Update(rptHdr);
                }

                #region SafetyFirstCheck
                if (reportAllDetails.SafetyFirstCheck != null)
                {
                    var eixstsSafety = _rsaContext.SafetyFirstChecks.AsNoTracking().FirstOrDefault(a => a.ReportGuid == rptHdr.ReportGuid);
                    if (eixstsSafety != null)
                    {
                        _rsaContext.SafetyFirstChecks.Remove(eixstsSafety);
                        _rsaContext.SafetyFirstCheckDetails.RemoveRange(_rsaContext.SafetyFirstCheckDetails.AsNoTracking().Where(a => a.ReportGuid == rptHdr.ReportGuid));
                    }

                    reportAllDetails.SafetyFirstCheck.ReportGuid = rptHdr.ReportGuid;
                    reportAllDetails.SafetyFirstCheck.Id = 0;
                    foreach (var item in reportAllDetails.SafetyFirstCheck.SafetyFirstCheckDetails)
                    {
                        item.Id = 0;
                        item.ReportGuid = rptHdr.ReportGuid;
                    }
                    _rsaContext.Add(reportAllDetails.SafetyFirstCheck);
                }
                #endregion

                #region CustomerEquipmentActivity
                if (reportAllDetails.CustomerEquipmentActivity != null)
                {
                    var custEquipAct = _rsaContext.CustomerEquipmentActivities.AsNoTracking().FirstOrDefault(a => a.ReportGuid == rptHdr.ReportGuid);
                    if (custEquipAct != null)
                    {
                        _rsaContext.CustomerEquipmentActivities.Remove(custEquipAct);
                    }

                    reportAllDetails.CustomerEquipmentActivity.ReportGuid = rptHdr.ReportGuid;
                    reportAllDetails.CustomerEquipmentActivity.Id = 0;
                    _rsaContext.Add(reportAllDetails.CustomerEquipmentActivity);

                }
                #endregion


                #region VibrationAnalysisHeader
                if (reportAllDetails.VibrationAnalysisHeader != null)
                {
                    var existVibAna = _rsaContext.VibrationAnalysisHeaders.AsNoTracking().FirstOrDefault(a => a.ReportGuid == rptHdr.ReportGuid);

                    if (existVibAna != null)
                    {
                        _rsaContext.VibrationAnalysisHeaders.Remove(existVibAna);
                        var exisVibAnaDetails = _rsaContext.VibrationAnalysis.AsNoTracking().Where(a => a.ReportGuid == rptHdr.ReportGuid);
                        if (exisVibAnaDetails.Any())
                            _rsaContext.VibrationAnalysis.RemoveRange(exisVibAnaDetails);
                    }
                    reportAllDetails.VibrationAnalysisHeader.ReportGuid = rptHdr.ReportGuid;
                    reportAllDetails.VibrationAnalysisHeader.Id = 0;
                    foreach (var item in reportAllDetails.VibrationAnalysisHeader.VibrationAnalysis)
                    {
                        item.Id = 0;
                        item.ReportGuid = rptHdr.ReportGuid;
                    }
                    _rsaContext.Add(reportAllDetails.VibrationAnalysisHeader);

                }
                #endregion

                #region Observations
                if (reportAllDetails.Observations != null)
                {
                    var existObs = _rsaContext.Observations.AsNoTracking().Where(w => w.ReportGuid == rptHdr.ReportGuid);
                    if(existObs.Any())
                    {
                        _rsaContext.Observations.RemoveRange(existObs);
                    }
                    foreach (var obs in reportAllDetails.Observations)
                    {
                        obs.ReportGuid = rptHdr.ReportGuid;
                        obs.Id = 0;
                        _rsaContext.Add(obs);
                    }
                }
                #endregion

                #region Recommendations
                if (reportAllDetails.Recommendations != null)
                {
                    var existRecomm = _rsaContext.Recommendations.AsNoTracking().Where(w => w.ReportGuid == rptHdr.ReportGuid);
                    if (existRecomm.Any())
                    {
                        _rsaContext.Recommendations.RemoveRange(existRecomm);
                    }
                    foreach (var recomm in reportAllDetails.Recommendations)
                    {
                        recomm.ReportGuid = rptHdr.ReportGuid;
                        recomm.Id = 0;
                        _rsaContext.Add(recomm);
                    }
                }
                #endregion

                #region SpareParts
                if (reportAllDetails.SpareParts != null)
                {
                    var existSparts = _rsaContext.SpareParts.AsNoTracking().Where(w => w.ReportGuid == rptHdr.ReportGuid);
                    if (existSparts.Any())
                    {
                        _rsaContext.SpareParts.RemoveRange(existSparts);
                    }

                    foreach (var sp in reportAllDetails.SpareParts)
                    {
                        sp.ReportGuid = rptHdr.ReportGuid;
                        sp.Id = 0;
                        _rsaContext.Add(sp);
                    }
                }
                #endregion

                if (reportAllDetails.Misc != null)
                {
                    var exisMisc = _rsaContext.Miscs.AsNoTracking().FirstOrDefault(w => w.ReportGuid == rptHdr.ReportGuid);
                    if (exisMisc != null)
                    {
                        _rsaContext.Miscs.Remove(exisMisc);
                    }
                    reportAllDetails.Misc.Id = 0;
                    reportAllDetails.Misc.ReportGuid = rptHdr.ReportGuid;
                    reportAllDetails.Misc.CustomerDate = DateTime.Now;
                    reportAllDetails.Misc.FirmDate = DateTime.Now;
                    _rsaContext.Add(reportAllDetails.Misc);
                }

                if (await _rsaContext.SaveChangesAsync() > 0)
                {
                    return new ResponseData() { status = ResponseStatus.success, data = rptHdr.ReportGuid, message = "Report Synchronized Successfully." };
                }

                return new ResponseData() { status = ResponseStatus.warning, data = rptHdr.ReportGuid, message = "Report Synchronization Failed." };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SyncOfflineData)} - Error", ex);
                return new ResponseData() { status = ResponseStatus.error, data = rptHdr.ReportGuid, message = "Report Synchronization Failed." };
            }
        }

        public async Task<ResponseData> CreateReport(ReportHeader reportHeader, SafetyFirstCheck safetyFirstCheck)
        {
            try
            {
                _logger.LogInformation("Create Report Called");
                var existData = _rsaContext.ReportHeaders.AsNoTracking().Where(w => w.ReportGuid == reportHeader.ReportGuid).FirstOrDefault();

                reportHeader.CreatedOn = DateTime.UtcNow;
                reportHeader.CreatedBy = reportHeader.CreatedBy;
                reportHeader.IsSafetyFirstComplete = false;
                reportHeader.IsCustomerEquipmentComplete = false;
                reportHeader.IsVibrationAnalysisComplete = false;
                reportHeader.IsObservationComplete = false;
                reportHeader.IsRecommendationComplete = false;
                reportHeader.ApprovedBy = null;
                reportHeader.DocTriggerFrom = "DRAFT";

                if (existData == null)
                {
                    _rsaContext.Add(reportHeader);
                }
                else
                {
                    reportHeader.Id = existData.Id;
                    _rsaContext.Update(reportHeader);
                }

                safetyFirstCheck.ReportGuid = reportHeader.ReportGuid;
                foreach (var item in safetyFirstCheck.SafetyFirstCheckDetails)
                {
                    item.Id = 0;
                    item.ReportGuid = reportHeader.ReportGuid;
                }

                _rsaContext.Add(safetyFirstCheck);
                reportHeader.IsSafetyFirstComplete = true;

                if (await _rsaContext.SaveChangesAsync() > 0)
                {
                    _logger.LogInformation("Create Report Completed");
                    return new ResponseData()
                    {
                        status = ResponseStatus.success,
                        data = new { ReportHeader = reportHeader, SafetyFirstCheck = safetyFirstCheck },
                        message = "Report Created Successfully."
                    };
                }

                return new ResponseData()
                {
                    status = ResponseStatus.warning,
                    data = new { ReportHeader = reportHeader, SafetyFirstCheck = safetyFirstCheck },
                    message = "Report Creation Failed."
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"{nameof(CreateReport)} - Error", ex);
                return new ResponseData() { status = ResponseStatus.error, message = "Report Creation Failed.Please contact admin." };
            }
        }

        /// <summary>
        /// It can newly create or modify the existing details
        /// </summary>
        /// <param name="reportHeaderGuid"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<ResponseData> SaveReportOtherDetails(Guid reportHeaderGuid, ReportAllDetailsVm reportAllDetails)
        {
            try
            {
                _logger.LogInformation($"{nameof(SaveReportOtherDetails)} - Called");

                if (reportHeaderGuid != reportAllDetails.ReportGuid || reportAllDetails.SafetyFirstCheck.Id == 0)
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

                if (reportAllDetails.SafetyFirstCheck.SafetyFirstCheckDetails.Where(w => w.Id == 0).Any())
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = "No new entry is allowed in edit mode of Safe First Check"
                    };


                if (reportAllDetails.CustomerEquipmentActivity.Id == 0 &&
                    _rsaContext.CustomerEquipmentActivities.Any(a => a.ReportGuid == reportHeaderGuid))
                {
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = $"Customer Equipment Activity is already exists for Report Header Id {reportHeaderGuid}"
                    };
                }

                if (reportAllDetails.VibrationAnalysisHeader.Id == 0 &&
                    _rsaContext.VibrationAnalysisHeaders.Any(a => a.ReportGuid == reportHeaderGuid))
                {
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = $"Vibration Analysis is already exists for Report Header Id {reportHeaderGuid}"
                    };
                }

                var reportHeader = _rsaContext.ReportHeaders.FirstOrDefault(f => f.ReportGuid == reportHeaderGuid);
                reportHeader.UpdatedOn = DateTime.UtcNow;
                reportHeader.IsCustomerEquipmentComplete = true;
                reportHeader.IsVibrationAnalysisComplete = true;
                reportHeader.IsObservationComplete = true;
                reportHeader.IsRecommendationComplete = true;

                _rsaContext.Update(reportHeader);
                _rsaContext.Update(reportAllDetails.SafetyFirstCheck);

                if (reportAllDetails.CustomerEquipmentActivity.Id == 0)
                {
                    reportAllDetails.CustomerEquipmentActivity.ReportGuid = reportHeaderGuid;
                    _rsaContext.Add(reportAllDetails.CustomerEquipmentActivity);
                }
                else
                {
                    reportAllDetails.CustomerEquipmentActivity.ReportGuid = reportHeaderGuid;
                    _rsaContext.Update(reportAllDetails.CustomerEquipmentActivity);
                
                }

                if (reportAllDetails.VibrationAnalysisHeader != null)
                {
                    foreach (var item in reportAllDetails.VibrationAnalysisHeader.VibrationAnalysis)
                    {
                        item.ReportGuid = reportHeaderGuid;
                    }

                    if (reportAllDetails.VibrationAnalysisHeader.Id == 0)
                    {
                        reportAllDetails.VibrationAnalysisHeader.ReportGuid = reportHeaderGuid;
                        _rsaContext.Add(reportAllDetails.VibrationAnalysisHeader);
                    }
                    else
                    {
                        reportAllDetails.VibrationAnalysisHeader.ReportGuid = reportHeaderGuid;
                        _rsaContext.Update(reportAllDetails.VibrationAnalysisHeader);
                    }
                }

                foreach (var obs in reportAllDetails.Observations)
                {
                    obs.ReportGuid = reportHeaderGuid;
                    if (obs.Id == 0)
                        _rsaContext.Add(obs);
                    else
                        _rsaContext.Update(obs);
                }
                foreach (var recomm in reportAllDetails.Recommendations)
                {
                    recomm.ReportGuid = reportHeaderGuid;
                    if (recomm.Id == 0)
                        _rsaContext.Add(recomm);
                    else
                        _rsaContext.Update(recomm);
                }
                foreach (var sp in reportAllDetails.SpareParts)
                {
                    sp.ReportGuid = reportHeaderGuid;
                    if (sp.Id == 0)
                        _rsaContext.Add(sp);
                    else
                        _rsaContext.Update(sp);
                }


                if (reportAllDetails.Misc.Id == 0)
                {
                    reportAllDetails.Misc.ReportGuid = reportHeaderGuid;
                    //reportAllDetails.Misc.CustomerDate = DateTime.Now;
                    //reportAllDetails.Misc.FirmDate = DateTime.Now;
                    _rsaContext.Add(reportAllDetails.Misc);
                }
                else
                {
                    reportAllDetails.Misc.ReportGuid = reportHeaderGuid;
                    _rsaContext.Update(reportAllDetails.Misc);
                }

                await _rsaContext.SaveChangesAsync();


                return new ResponseData() { status = ResponseStatus.success, data = reportHeaderGuid, message = "Report Saved Successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SaveReportOtherDetails)} - Error", ex);
                return new ResponseData() { status = ResponseStatus.error, data = reportHeaderGuid, message = "Report Save Failed." };
            }

        }

        public async Task<ResponseData> GetReportDetails(Guid reportHeaderGuid)
        {
            ReportAllDetailsVm reportAllDetailsVm = new ReportAllDetailsVm();
            reportAllDetailsVm.ReportGuid = reportHeaderGuid;
            reportAllDetailsVm.SafetyFirstCheck = _rsaContext.SafetyFirstChecks.AsNoTracking()
                .Include(a=>a.SafetyFirstCheckDetails)
                .Where(w => w.ReportGuid == reportHeaderGuid).FirstOrDefault();

            if (reportAllDetailsVm.SafetyFirstCheck == null)
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = "Error while getting data."
                };

            reportAllDetailsVm.CustomerEquipmentActivity = _rsaContext.CustomerEquipmentActivities.AsNoTracking()
                .Where(w => w.ReportGuid == reportHeaderGuid).FirstOrDefault();

            if(reportAllDetailsVm.CustomerEquipmentActivity == null)
            {
                reportAllDetailsVm.CustomerEquipmentActivity = new CustomerEquipmentActivity()
                {
                    ReportGuid = reportHeaderGuid,
                    PreviousServiceDate = DateTime.Now,
                    CurrentServiceDate = DateTime.Now,
                    ReportDate = DateTime.Now
                };
            }
            reportAllDetailsVm.VibrationAnalysisHeader = _rsaContext.VibrationAnalysisHeaders.AsNoTracking()
                .Include(a => a.VibrationAnalysis)
                .Where(w => w.ReportGuid == reportHeaderGuid)
                .FirstOrDefault();

            if (reportAllDetailsVm.VibrationAnalysisHeader == null)
            {
                reportAllDetailsVm.VibrationAnalysisHeader = new VibrationAnalysisHeader()
                {
                    ReportGuid = reportHeaderGuid
                };
            }
            else
            {
                var orderParams = new VibrationAnalysis[parameterUnits.Count];
                int i = 0;
                foreach (var item in parameterUnits.OrderBy(o=>o.Key))
                {
                    orderParams[i] = reportAllDetailsVm.VibrationAnalysisHeader.VibrationAnalysis.First(w => item.Value.Contains($"{w.Parameter}~{w.Units}", StringComparison.OrdinalIgnoreCase));
                    i++;
                }
                reportAllDetailsVm.VibrationAnalysisHeader.VibrationAnalysis = orderParams;
            }
            List<Observation> obs = _rsaContext.Observations.AsNoTracking().Where(w => w.ReportGuid == reportHeaderGuid && w.Status == "A").ToList();
            if (obs == null)
                obs = new List<Observation>();
            reportAllDetailsVm.Observations = obs;

            List<Recommendation> recomms = _rsaContext.Recommendations.AsNoTracking().Where(w => w.ReportGuid == reportHeaderGuid && w.Status == "A").ToList();
            if (recomms == null)
                recomms = new List<Recommendation>();
            reportAllDetailsVm.Recommendations = recomms;

            List<SparePart> spareParts = _rsaContext.SpareParts.AsNoTracking().Where(w => w.ReportGuid == reportHeaderGuid && w.Status == "A").ToList();
            if (spareParts == null)
                spareParts = new List<SparePart>();
            reportAllDetailsVm.SpareParts = spareParts;

            reportAllDetailsVm.Misc = _rsaContext.Miscs.AsNoTracking().FirstOrDefault(f => f.ReportGuid == reportHeaderGuid) ?? new Misc() { FirmDate = DateTime.Now, CustomerDate = DateTime.Now };

            var signFirmImage = _rsaContext.ImageHouses.AsNoTracking().FirstOrDefault(f => f.ReportGuid == reportHeaderGuid && f.ImageLabel == StringConstants.FirmSignature);
            if (signFirmImage != null)
                reportAllDetailsVm.FirmSignatureImageId = signFirmImage.ImageFileGuid;

            var signCustImage = _rsaContext.ImageHouses.AsNoTracking().FirstOrDefault(f => f.ReportGuid == reportHeaderGuid && f.ImageLabel == StringConstants.CustomerSignature);
            if (signCustImage != null)
                reportAllDetailsVm.CustomerSignatureImageId = signCustImage.ImageFileGuid;

            var sfc_signFirmImage = _rsaContext.ImageHouses.AsNoTracking().FirstOrDefault(f => f.ReportGuid == reportHeaderGuid && f.ImageLabel == StringConstants.SfcFirmSignature);
            if (sfc_signFirmImage != null)
                reportAllDetailsVm.SfcFirmSignatureImageId = sfc_signFirmImage.ImageFileGuid;

            var sfc_signCustImage = _rsaContext.ImageHouses.AsNoTracking().FirstOrDefault(f => f.ReportGuid == reportHeaderGuid && f.ImageLabel == StringConstants.SfcCustomerSignature);
            if (sfc_signCustImage != null)
                reportAllDetailsVm.SfcCustomerSignatureImageId = sfc_signCustImage.ImageFileGuid;

            return new ResponseData()
            {
                status = ResponseStatus.success,
                data = reportAllDetailsVm
            };

        }

        public async Task<ResponseData> GetReports(int userId)
        {
            var user = _rsaContext.Users.FirstOrDefault(w => w.Id == userId);

            var reportQuery = _rsaContext.ReportHeaders.AsNoTracking()
                .Join(_rsaContext.SafetyFirstChecks.AsNoTracking(), rh => rh.ReportGuid, sfc => sfc.ReportGuid,
                    (rh, sfc) => new { rh, sfc });

            if (user.IsSuperUser == false)
            {
                reportQuery = reportQuery.Where(w => w.rh.CreatedBy == userId);
            }

            var reports = reportQuery.Select(s => new
            {
                s.rh.ReportGuid,
                s.rh.CreatedBy,
                s.rh.CreatedOn,
                s.sfc.JobOrderNumber,
                s.sfc.ProjectName,
                IsOffLineData = false
            }).OrderByDescending(o => o.CreatedOn);

            var data = await reports.ToListAsync();
            return new ResponseData()
            {
                status = ResponseStatus.success,
                data = data
            };
        }

        public async Task<ResponseData> SaveImage(VmImageSaveEntity imageEntity, bool isFromUpload = false)
        {
            try
            {
                _logger.LogInformation($"{nameof(SaveImage)} - Called");

                ImageHouse imageHouse = new ImageHouse();
                imageHouse.ImageFileGuid = imageEntity.ImageFileGuid;
                imageHouse.Entity = imageEntity.Entity;
                imageHouse.ImageLabel = imageEntity.ImageLabel;
                imageHouse.ReportGuid = imageEntity.ReportGuid;
                imageHouse.EntityRefGuid = imageEntity.EntityRefGuid;
                var oldImage = _rsaContext.ImageHouses.AsNoTracking().Where(w => w.ImageFileGuid == imageHouse.ImageFileGuid).FirstOrDefault();

                if (!isFromUpload)
                {
                    //if (oldImage != null && oldImage.ImageFileGuid != null)
                    //{
                    //    var filesToDelete = Directory.GetFiles($"{ImageUploadPath}", $"{oldImage.ImageFileGuid}.*");
                    //    foreach (var file in filesToDelete)
                    //        File.Delete(file);
                    //}

                    string filePath = $"{ImageUploadPath}{imageEntity.ImageFileGuid}.jpeg";
                    _logger.LogInformation($"Img Saved - Called");
                    if ("signature".Equals(imageEntity.Entity))
                    {
                        filePath = $"{ImageUploadPath}{imageEntity.ImageFileGuid}.png";
                        File.WriteAllBytes(filePath, Convert.FromBase64String(imageEntity.Base64.Split("data:image/png;base64,")[1]));
                    }
                    else
                        File.WriteAllBytes(filePath, Convert.FromBase64String(imageEntity.Base64.Split("data:image/jpeg;base64,")[1]));
                }

                _logger.LogInformation($"Img Saved - Completed");

                if (oldImage == null)
                {
                    imageHouse.Id = 0;
                    _rsaContext.Add(imageHouse);
                }
                else
                {
                    imageHouse.Id = oldImage.Id;
                    _rsaContext.Update(imageHouse);
                }
                if (await _rsaContext.SaveChangesAsync() > 0)
                {
                    //MyImage.ResizeJPEG(filePath);
                    _logger.LogInformation($"{nameof(SaveImage)} - Completed");
                    return new ResponseData()
                    {
                        status = ResponseStatus.success,
                        data = new { Id = imageHouse.Id, ImageFileGuid = imageHouse.ImageFileGuid, Base64 = GetBase64(imageHouse.ImageFileGuid.ToString(), imageHouse.Entity == "signature" ? "png" : "jpeg") },
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
                _logger.LogError(ex, $"{nameof(SaveImage)} - Error");
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = "Error occurred while saving image. Please contact admin."
                };
            }
        }

        public async Task<ResponseData> DeleteImageById(Guid imageHouseGuid)
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteImageById)} - Called");
                var image = _rsaContext.ImageHouses.FirstOrDefault(f=>f.ImageFileGuid == imageHouseGuid);

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

        public async Task<ResponseData> GetImagesByEntityRefGuid(Guid reportHeaderGuid, string entity,Guid entityRefGuid)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetImagesByEntityRefGuid)} - Called");
                List<VmImageSaveEntity> vmImageDataList = new List<VmImageSaveEntity>();
                var images = await _rsaContext.ImageHouses.AsNoTracking()
                    .Where(w => w.Entity == entity && w.ReportGuid == reportHeaderGuid
                    && w.EntityRefGuid == entityRefGuid)
                    .ToListAsync();
                vmImageDataList = GetImages(reportHeaderGuid, images);
                _logger.LogInformation($"{nameof(GetImagesByEntityRefGuid)} - completed");
                return new ResponseData()
                {
                    status = ResponseStatus.success,
                    data = vmImageDataList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetImagesByEntityRefGuid)} - Error.", ex);
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = ex.Message
                };
            }
        }

        public async Task<ResponseData> GetImagesByImageLabels(Guid reportHeaderGuid, string entity, string[] labels)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetImagesByImageLabels)} - Called");
                List<VmImageSaveEntity> vmImageDataList = new List<VmImageSaveEntity>();
                var images = await _rsaContext.ImageHouses.AsNoTracking()
                    .Where(w => w.Entity == entity && w.ReportGuid == reportHeaderGuid && labels.Contains(w.ImageLabel))
                    .ToListAsync();
                vmImageDataList = GetImages(reportHeaderGuid, images);
                _logger.LogInformation($"{nameof(GetImagesByImageLabels)} - completed");
                return new ResponseData()
                {
                    status = ResponseStatus.success,
                    data = vmImageDataList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetImagesByImageLabels)} - Error.", ex);
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = ex.Message
                };
            }
        }
        private List<VmImageSaveEntity> GetImages(Guid reportHeaderGuid, List<ImageHouse> images)
        {
            List<VmImageSaveEntity> vmImageDataList = new List<VmImageSaveEntity>();

            foreach (var img in images)
            {
                vmImageDataList.Add(
                    new VmImageSaveEntity
                    {
                        ImageFileGuid = img.ImageFileGuid,
                        Base64 = GetBase64(img.ImageFileGuid.ToString(), img.Entity == "signature" ? "png":"jpeg"),
                        ImageLabel = img.ImageLabel,
                        Entity = img.Entity,
                        EntityRefGuid = img.EntityRefGuid,
                        ReportGuid = reportHeaderGuid
                    });
            }
            return vmImageDataList;
        }

        private string GetBase64(string guid,string extension)
        {
            try
            {
                string filePath = $"{ImageUploadPath}{guid}.{extension}";
                var fileBytes = File.ReadAllBytes(filePath);

                string encodedFile = Convert.ToBase64String(fileBytes);

                return $"data:image/{extension};base64,{encodedFile}";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

  

        public async Task<ResponseData> SendToSuperVisor(Guid reportHeaderGuid, string from)
        {
            try
            {
                _logger.LogInformation($"{nameof(SendToSuperVisor)} - Called");

                var data = _rsaContext.ReportHeaders.FirstOrDefault(f=>f.ReportGuid == reportHeaderGuid);
                if(data == null)
                    return new ResponseData()
                    {
                        status = ResponseStatus.error,
                        message = $"Data is not available:-{reportHeaderGuid}"
                    };

                if (data.IsDocTrigger)
                {
                    return new ResponseData()
                    {
                        status = ResponseStatus.warning,
                        message = "Document generation is in progress for this Report.Please try after some time."
                    };
                }
                data.IsDocTrigger = true;
                data.DocTriggerFrom = from;
                await _rsaContext.SaveChangesAsync();

                _logger.LogInformation($"{nameof(SendToSuperVisor)} - completed");
                return new ResponseData()
                {
                    status = ResponseStatus.success,
                    data = reportHeaderGuid
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SendToSuperVisor)} - Error.", ex);
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = ex.Message
                };
            }
        }

        public async Task<ResponseData> GetUserLoginData(string username, string password)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetUserLoginData)} - Called");

                var data = await _rsaContext.Users
                    .Where(w => w.Active).FirstOrDefaultAsync(f => f.UserName.ToLower() == username.ToLower() && f.Password == password);
         
                _logger.LogInformation($"{nameof(GetUserLoginData)} - completed");
                return new ResponseData()
                {
                    status = ResponseStatus.success,
                    data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetUserLoginData)} - Error.", ex);
                return new ResponseData()
                {
                    status = ResponseStatus.error,
                    message = ex.Message
                };
            }
        }

    }
}
