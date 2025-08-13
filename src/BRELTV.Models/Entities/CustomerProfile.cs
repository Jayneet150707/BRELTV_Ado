using System;

namespace BRELTV.Models.Entities
{
    public class CustomerProfile
    {
        public int Id { get; set; }
        public string ProfileBand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}

