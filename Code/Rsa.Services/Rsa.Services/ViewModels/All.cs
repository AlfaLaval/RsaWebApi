using Rsa.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rsa.Services.ViewModels
{
    public class HeaderData
    {
        public ReportHeader ReportHeader { get; set; }
        public SafetyFirstCheck SafetyFirstCheck { get; set; }
    }

    public class ReportAllDetailsVm {
        public Guid ReportGuid { get; set; }
        public SafetyFirstCheck SafetyFirstCheck { get; set; }
        public CustomerEquipmentActivity CustomerEquipmentActivity { get; set; }
        public VibrationAnalysisHeader VibrationAnalysisHeader { get; set; }
        public List<Observation> Observations { get; set; }
        public List<Recommendation> Recommendations { get; set; }
        public List<SparePart> SpareParts { get; set; }
        public Misc Misc { get; set; }
        public Guid FirmSignatureImageId { get; set; }
        public Guid CustomerSignatureImageId { get; set; }
    }

    public class OfflineWrapper
    {
        public ReportHeader rptHdr { get; set; }
        public ReportAllDetailsVm reportAllDetail { get; set; }
    }

    public class ResponseData
    {
        public ResponseStatus status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public enum ResponseStatus
    {
        error = -1,
        warning = 0,
        success = 1
    }

    public class VmImageSaveEntity
    {
        public Guid ImageFileGuid { get; set; }
        public string Base64 { get; set; }
        public string Entity { get; set; }
        public Guid EntityRefGuid { get; set; }
        public Guid ReportGuid { get; set; }
        public string ImageLabel { get; set; }

    }
}
