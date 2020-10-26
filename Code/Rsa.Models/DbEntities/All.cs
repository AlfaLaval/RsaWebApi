using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rsa.Models.DbEntities
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(100)]
		public string DisplayName { get; set; }
		[Required]
		[MaxLength(100)]
		public string UserName { get; set; }
		[Required]
		[MaxLength(100)]
		public string Email { get; set; }
		public bool Active { get; set; }
		public bool IsSuperVisor { get; set; }

		public int? SuperVisorId { get; set; }

		public string OTP { get; set; }

		public DateTime? OTPGeneratedOn { get; set; }

		public string Region { get; set; }

		public bool IsSuperUser { get; set; }
        public string Password { get; set; }

    }
	public class ReportHeader
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public int? UpdatedBy { get; set; }
		public DateTime? UpdatedOn { get; set; }
		public bool IsSafetyFirstComplete { get; set; }
		public bool IsCustomerEquipmentComplete { get; set; }
		public bool IsVibrationAnalysisComplete { get; set; }
		public bool IsObservationComplete { get; set; }
        public bool IsRecommendationComplete { get; set; }
        public bool IsDocTrigger { get; set; }
		public int? ApprovedBy { get; set; }
	}

	public class SafetyFirstCheck
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int ReportHeaderId { get; set; }
		
        [Required]
		[MaxLength(200)]
		public string EngineerName { get; set; }
		[Required]
		[MaxLength(200)]
		public string ProjectName { get; set; }
		[Required]
		[MaxLength(200)]
		public string SiteSafetyContact { get; set; }
		public DateTime StartDate { get; set; }
		[Required]
		[MaxLength(50)]
		public string JobOrderNumber { get; set; }
		[Required]
		[MaxLength(50)]
		public string ContactNumber { get; set; }
        public IList<SafetyFirstCheckDetail> SafetyFirstCheckDetails { get; set; }
    }

	public class SafetyFirstCheckDetail
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(50)]
		public string CheckPointName { get; set; }
		public bool IsSelected { get; set; }
		public string Remarks { get; set; }
		public int SafetyFirstCheckId { get; set; }

		[ForeignKey("SafetyFirstCheckId")]
		public SafetyFirstCheck SafetyFirstCheck { get; set; }

    }

	public class CustomerEquipmentActivity
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(100)]
		public string Country { get; set; }
		[Required]
		[MaxLength(200)]
		public string Customer { get; set; }
		[Required]
		[MaxLength(200)]
		public string ServiceEngineer { get; set; }
		[Required]
		[MaxLength(50)]
		public string ReportNumber { get; set; }
		public DateTime PreviousServiceDate { get; set; }
		public DateTime CurrentServiceDate { get; set; }
		public DateTime ReportDate { get; set; }
		[Required]
		[MaxLength(50)]
		public string Equipment { get; set; }
		[Required]
		[MaxLength(100)]
		public string SiteLocation { get; set; }
		[Required]
		[MaxLength(100)]
		public string DecanterModel { get; set; }
		[Required]
		[MaxLength(100)]
		public string DecanterSerialNumber { get; set; }
		
		[MaxLength(100)]
		public string BowlSerialNumber { get; set; }
		
		[MaxLength(100)]
		public string CustomerReference { get; set; }
		public int RunningHours { get; set; }
		[Required]
		[MaxLength(50)]
		public string Controller { get; set; }
		[Required]
		[MaxLength(50)]
		public string HmiModel { get; set; }
		[Required]
		[MaxLength(50)]
		public string HmiSwVersion { get; set; }
		[Required]
		[MaxLength(50)]
		public string CpuModel { get; set; }
		[Required]
		[MaxLength(50)]
		public string CpuSwVersion { get; set; }
		[Required]
		[MaxLength(50)]
		public string ScopeOfWrok { get; set; }
		
		[MaxLength(50)]
		public string ScoperOfWorkOthers { get; set; }
		[Required]
		[MaxLength(50)]
		public string WorkStatus { get; set; }
		[Required]
		[MaxLength(50)]
		public string DecanterStatus { get; set; }
		public int ReportHeaderId { get; set; }
	}

	public class VibrationAnalysisHeader
	{
		[Key]
		public int Id { get; set; }
		public bool BsDryRunActive { get; set; }
		public bool BsProduction { get; set; }
		public bool AsDryRun { get; set; }
		public bool AsWaterTest { get; set; }
		public bool AsProduction { get; set; }
		public int ReportHeaderId { get; set; }
		public string Remarks { get; set; }

        public bool MdMotor { get; set; }
		public bool BdMotor { get; set; }
        public string MdDriveEndMain { get; set; }
		public string MdDriveEndBack { get; set; }
		public string MdNonDriveEndMain { get; set; }
		public string MdNonDriveEndBack { get; set; }

		public string BdDriveEndMain { get; set; }
		public string BdDriveEndBack { get; set; }
		public string BdNonDriveEndMain { get; set; }
		public string BdNonDriveEndBack { get; set; }
		public IList<VibrationAnalysis> VibrationAnalysis { get; set; }
    }

	public class VibrationAnalysis
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(200)]
		public string Parameter { get; set; }
		[Required]
		[MaxLength(20)]
		public string Units { get; set; }
		
		[MaxLength(50)]
		public string BsDryRun { get; set; }

		[MaxLength(50)]
		public string BsProduction { get; set; }

		[MaxLength(50)]
		public string AsDryRun { get; set; }

		[MaxLength(50)]
		public string AsWaterTest { get; set; }

		[MaxLength(50)]
		public string AsProduction { get; set; }
		public int VibrationAnalysisHeaderId { get; set; }

	}

	public class Recommendation
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string Remarks { get; set; }
        public bool ImmediateAction { get; set; }
		public bool MidTermAction { get; set; }
		public bool Observation { get; set; }
		public int ReportHeaderId { get; set; }
        public Guid EntityRefGuid { get; set; }
		public char Status { get; set; }
	}

	public class Observation
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Title { get; set; }
		
		[MaxLength(100)]
		public string Remarks { get; set; }
		
		[MaxLength(100)]
		public string ActionTaken { get; set; }
		public int ReportHeaderId { get; set; }
		public Guid EntityRefGuid { get; set; }
        public char Status { get; set; }
    }

	public class ImageHouse
	{
		[Key]
		public int Id { get; set; }
        public int ReportHeaderId { get; set; }
        public string Entity { get; set; }
		//public int EntityRefId { get; set; }
		public Guid ImageFileGuid { get; set; }
		public string ImageLabel { get; set; }
		public Guid EntityRefGuid { get; set; }

	}

	public class Misc
    {
		[Key]
        public int Id { get; set; }
        public int ReportHeaderId { get; set; }
        public string FirmComments { get; set; }
		public string CustomerComments { get; set; }
	}
}
