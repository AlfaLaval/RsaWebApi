using Rsa.Models.DbEntities;
using Rsa.Services.ViewModels;
using System;
using System.Threading.Tasks;

namespace Rsa.Services.Abstractions
{
    public interface IReportActivities
    {
        Task<ResponseData> CreateReport(ReportHeader reportHeader, SafetyFirstCheck safetyFirstCheck);
        Task<ResponseData> SaveReportOtherDetails(int reportHeaderId, ReportAllDetailsVm reportAllDetails);
        Task<ResponseData> GetReports();
        Task<ResponseData> GetReportDetails(int reportHeaderId);
        Task<ResponseData> SaveImage(VmImageSaveEntity imageEntity);
        Task<ResponseData> DeleteImageById(int imageHouseId);
        Task<ResponseData> GetImagesByEntityRefGuid(int reportHeaderId, string entity,Guid enityRefGuid);
        Task<ResponseData> GetImagesByImageLabels(int reportHeaderId, string entity, string[] labels);
        Task<ResponseData> SendToSuperVisor(int reportHeaderId,string from);
        Task<ResponseData> GetUserLoginData(string username, string password);

    }
}
