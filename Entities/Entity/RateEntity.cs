using Entities.Entity.Abstract;

namespace Entities.Entity
{
    public class RateEntity : BaseEntity
    {        
        public required DateOnly Date { get; set; }
        public required string Currency { get; set; }
        public required decimal RateValue { get; set; }
    }
}
