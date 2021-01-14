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
        public void GenerateWord(int headerId)
        {
            string filename = string.Empty;
            try
            {
                _logger.Info($"{AppSettings.Environment} -- GenerateWord Started for ReportId : {headerId}");
                RsaContext rsaContext = new RsaContext();
                var reportData = GetReportData(headerId);
                var allUsers = rsaContext.Users.AsNoTracking().ToList();
                var userData = allUsers.FirstOrDefault(w => w.Id == reportData.ReportHeader.CreatedBy);
                var approvedByUser = allUsers.FirstOrDefault(w => w.Id == reportData.ReportHeader.ApprovedBy);
                if (userData != null)
                {
                    //var superVisorEmailId = allUsers.FirstOrDefault(w => w.Id == userData.SuperVisorId)?.Email;
                    if (!string.IsNullOrWhiteSpace(userData.Email))
                    {
                        var imageHouses = rsaContext.ImageHouses.AsNoTracking().Where(w => w.ReportHeaderId == headerId).ToList();
                        List<ImageHouse> imageDataToProcess = new List<ImageHouse>();
                        imageDataToProcess.AddRange(imageHouses);
                        var doc = new WordDocument();
                        filename = doc.Generate(reportData, userData.DisplayName, imageDataToProcess);
                        _logger.Info("GenerateWord Completed");
                        string messageBody = $"Please find the Decanter Report document.\nProject Name:{reportData.SafetyFirstCheck.ProjectName}\nJob No:{reportData.SafetyFirstCheck.JobOrderNumber}";
                        string subject = $"Decanter Report - {reportData.ReportHeader.DocTriggerFrom}";
                        Notification.SendEmail(userData.Email, subject , messageBody, filename);
                        UpdateDocGenerationFlag(headerId);
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

        private ReportAllDetailsDocVm GetReportData(int reportHeaderId)
        {
            RsaContext rsaContext = new RsaContext();

            ReportAllDetailsDocVm data = new ReportAllDetailsDocVm();
            data.ReportHeader = rsaContext.ReportHeaders.AsNoTracking()
                .FirstOrDefault(f => f.Id == reportHeaderId);
            data.SafetyFirstCheck = rsaContext.SafetyFirstChecks.AsNoTracking()
                .Include("SafetyFirstCheckDetails")
                .FirstOrDefault(w => w.ReportHeaderId == reportHeaderId);
            data.CustomerEquipmentActivity = rsaContext.CustomerEquipmentActivities.AsNoTracking()
                .FirstOrDefault(w => w.ReportHeaderId == reportHeaderId);
            data.VibrationAnalysisHeader = rsaContext.VibrationAnalysisHeaders.AsNoTracking()
                .FirstOrDefault(f => f.ReportHeaderId == reportHeaderId);
            if(data.VibrationAnalysisHeader!=null)
            {
                data.VibrationAnalysisHeader.VibrationAnalysis = rsaContext.VibrationAnalysis.Where(w => w.VibrationAnalysisHeaderId == data.VibrationAnalysisHeader.Id).ToList();
            }
            data.Observations = rsaContext.Observations.AsNoTracking()
                .Where(w => w.ReportHeaderId == reportHeaderId ).ToList();
            data.Recommendations = rsaContext.Recommendations.AsNoTracking()
                .Where(w => w.ReportHeaderId == reportHeaderId ).ToList();
            data.SpareParts = rsaContext.SpareParts.AsNoTracking()
               .Where(w => w.ReportHeaderId == reportHeaderId ).ToList();
            data.Misc = rsaContext.Miscs.AsNoTracking().FirstOrDefault(f => f.ReportHeaderId == reportHeaderId);

            return data;
        }

        private void UpdateDocGenerationFlag(int reportHeaderId)
        {
            var _dbContex = new RsaContext();
            var report = _dbContex.ReportHeaders.FirstOrDefault(f => f.Id == reportHeaderId);
            report.IsDocTrigger = false;
            _dbContex.SaveChanges();
        }

    }
}
