using Rsa.Models.DbEntities;
using Rsa.Services.ViewModels;
using System;
using System.Threading.Tasks;

namespace Rsa.Services.Abstractions
{
    public interface IReportActivities
    {
        Task<ResponseData> CreateReport(ReportHeader reportHeader, SafetyFirstCheck safetyFirstCheck);
        Task<ResponseData> SaveReportOtherDetails(Guid reportHeaderGuid, ReportAllDetailsVm reportAllDetails);
        Task<ResponseData> GetReports();
        Task<ResponseData> GetReportDetails(Guid reportHeaderGuid);
        Task<ResponseData> SaveImage(VmImageSaveEntity imageEntity);
        Task<ResponseData> DeleteImageById(Guid imageHouseGuid);
        Task<ResponseData> GetImagesByEntityRefGuid(Guid reportHeaderGuid, string entity,Guid enityRefGuid);
        Task<ResponseData> GetImagesByImageLabels(Guid reportHeaderGuid, string entity, string[] labels);
        Task<ResponseData> SendToSuperVisor(Guid reportHeaderGuid, string from);
        Task<ResponseData> GetUserLoginData(string username, string password);

    }
}
