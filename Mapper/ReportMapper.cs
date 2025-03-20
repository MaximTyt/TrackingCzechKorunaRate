using Domain.DTO;
using Models.Model;

namespace Mapper
{
    public static class ReportMapper
    {
        public static Report ToDomain(this ReportModel report)
        {
            return new Report
            {                
                Currency = report.Currency,
                MinRate = report.MinRate,
                MaxRate = report.MaxRate,
                AvgRate = report.AvgRate
            };
        }

        public static ReportModel ToModel(this Report report)
        {
            return new ReportModel
            {
                Currency = report.Currency,
                MinRate = report.MinRate,
                MaxRate = report.MaxRate,
                AvgRate = report.AvgRate
            };
        }
    }
}
