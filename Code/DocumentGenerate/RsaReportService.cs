using Rsa.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DocumentGenerate
{
    public class RsaReportService
    {
        private static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void GenerateWord(Guid reportHeaderGuid)
        {
            string filename = string.Empty;
            try
            {
                _logger.Info($"{AppSettings.Environment} -- GenerateWord Started for ReportId : {reportHeaderGuid}");
                RsaContext rsaContext = new RsaContext();
                var reportData = GetReportData(reportHeaderGuid);
                var allUsers = rsaContext.Users.AsNoTracking().ToList();
                var userData = allUsers.FirstOrDefault(w => w.Id == reportData.ReportHeader.CreatedBy);
                var approvedByUser = allUsers.FirstOrDefault(w => w.Id == reportData.ReportHeader.ApprovedBy);
                if (userData != null)
                {
                    //var superVisorEmailId = allUsers.FirstOrDefault(w => w.Id == userData.SuperVisorId)?.Email;
                    if (!string.IsNullOrWhiteSpace(userData.Email))
                    {
                        var imageHouses = rsaContext.ImageHouses.AsNoTracking().Where(w => w.ReportGuid == reportHeaderGuid).ToList();
                        List<ImageHouse> imageDataToProcess = new List<ImageHouse>();
                        imageDataToProcess.AddRange(imageHouses);
                        var doc = new WordDocument();
                        filename = doc.Generate(reportData, userData.DisplayName, imageDataToProcess);
                        _logger.Info("GenerateWord Completed");
                        string messageBody = $"Please find the Decanter Report document.\nProject Name:{reportData.SafetyFirstCheck.ProjectName}\nJob No:{reportData.SafetyFirstCheck.JobOrderNumber}";
                        string subject = $"Decanter Report - {reportData.ReportHeader.DocTriggerFrom}";
                        Notification.SendEmail(userData.Email, subject, messageBody, filename);
                        UpdateDocGenerationFlag(reportHeaderGuid);
                        _logger.Info("Sending Email Completed");


                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error in GenerateWord", ex);
                throw ex;
            }
            finally
            {
                if (File.Exists(filename))
                {
                    Thread.Sleep(50000);
                    _logger.Info("Deleting File");
                    Directory.Delete(Path.GetDirectoryName(filename), true);
                }
            }
        }

        private ReportAllDetailsDocVm GetReportData(Guid reportHeaderGuid)
        {
            RsaContext rsaContext = new RsaContext();

            ReportAllDetailsDocVm data = new ReportAllDetailsDocVm();
            data.ReportHeader = rsaContext.ReportHeaders.AsNoTracking()
                .FirstOrDefault(f => f.ReportGuid == reportHeaderGuid);
            data.SafetyFirstCheck = rsaContext.SafetyFirstChecks.AsNoTracking()
                .Include("SafetyFirstCheckDetails")
                .FirstOrDefault(w => w.ReportGuid == reportHeaderGuid);
            data.CustomerEquipmentActivity = rsaContext.CustomerEquipmentActivities.AsNoTracking()
                .FirstOrDefault(w => w.ReportGuid == reportHeaderGuid);
            data.VibrationAnalysisHeader = rsaContext.VibrationAnalysisHeaders.AsNoTracking()
                .FirstOrDefault(f => f.ReportGuid == reportHeaderGuid);
            if(data.VibrationAnalysisHeader!=null)
            {
                data.VibrationAnalysisHeader.VibrationAnalysis = rsaContext.VibrationAnalysis.Where(w => w.VibrationAnalysisHeaderId == data.VibrationAnalysisHeader.Id).ToList();
            }
            data.Observations = rsaContext.Observations.AsNoTracking()
                .Where(w => w.ReportGuid == reportHeaderGuid && w.Status == "A").OrderBy(o=>o.CreatedDateTime).ToList();
            data.Recommendations = rsaContext.Recommendations.AsNoTracking()
                .Where(w => w.ReportGuid == reportHeaderGuid && w.Status == "A").ToList();
            data.SpareParts = rsaContext.SpareParts.AsNoTracking()
               .Where(w => w.ReportGuid == reportHeaderGuid && w.Status == "A").ToList();
            data.Misc = rsaContext.Miscs.AsNoTracking().FirstOrDefault(f => f.ReportGuid == reportHeaderGuid);

            return data;
        }

        private void UpdateDocGenerationFlag(Guid reportHeaderGuid)
        {
            var _dbContex = new RsaContext();
            var report = _dbContex.ReportHeaders.FirstOrDefault(f => f.ReportGuid == reportHeaderGuid);
            report.IsDocTrigger = false;
            _dbContex.SaveChanges();
        }

    }
}
