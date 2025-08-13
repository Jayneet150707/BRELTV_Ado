using System;
using System.Collections.Generic;

namespace BRELTV.Domain.Entities
{
    public class CustomerProfile
    {
        public int Id { get; set; }
        public string ProfileBand { get; set; }
        public string Description { get; set; }
        public int MinScore { get; set; }
        public int? MaxScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        // Navigation properties
        public ICollection<BusinessRule> BusinessRules { get; set; }
    }
}

