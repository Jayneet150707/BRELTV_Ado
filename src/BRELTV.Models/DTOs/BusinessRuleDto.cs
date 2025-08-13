using System;

namespace BRELTV.Models.DTOs
{
    public class BusinessRuleDto
    {
        /// <summary>
        /// Business rule ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Customer profile band (e.g., "0", "-1", "650-700", "700-750", "750+")
        /// </summary>
        public string CustomerProfileBand { get; set; } = string.Empty;

        /// <summary>
        /// LTV percentage when no income proof is available
        /// </summary>
        public decimal NoIncomeProofLTV { get; set; }

        /// <summary>
        /// Maximum LTV percentage when income proof meets conditions
        /// </summary>
        public decimal MaxLTVWithProof { get; set; }

        /// <summary>
        /// Field Investigation requirement ("FI Mandatory" or "FI Waiver")
        /// </summary>
        public string FIRequirement { get; set; } = string.Empty;

        /// <summary>
        /// Minimum income proof amount required to qualify for max LTV
        /// </summary>
        public decimal MinIncomeProofAmount { get; set; }

        /// <summary>
        /// Minimum floating money percentage required to qualify for max LTV
        /// </summary>
        public decimal MinFloatingMoneyPercentage { get; set; }

        /// <summary>
        /// Whether the rule is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Whether the rule has been approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// When the rule was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}

