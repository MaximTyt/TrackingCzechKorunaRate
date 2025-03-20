using Domain.DTO;
using Entities.Entity;
using Models.Model;

namespace Mapper
{
    public static class RateMapper
    {
        public static RateEntity ToEntity(this Rate rate)
        {
            return new RateEntity
            {
                Date = rate.Date,
                Currency = rate.Currency,
                RateValue = rate.RateValue
            };
        }

        public static Rate ToDomain(this RateEntity rate)
        {
            return new Rate
            {
                Date = rate.Date,
                Currency = rate.Currency,
                RateValue = rate.RateValue
            };
        }

        public static Rate ToDomain(this RateModel rate)
        {
            return new Rate
            {
                Date = rate.Date,
                Currency = rate.Currency,
                RateValue = rate.RateValue
            };
        }

        public static RateModel ToModel(this Rate rate)
        {
            return new RateModel
            {
                Date = rate.Date,
                Currency = rate.Currency,
                RateValue = rate.RateValue
            };
        }
    }
}
