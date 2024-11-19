using KYH.NET_KassaSystem_Nastaran.Enum;
using KYH.NET_KassaSystem_Nastaran.Services;
using System;


namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class Campaign
    {
        public CampaignType Type { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Campaign(CampaignType type, decimal discountValue, DateTime startDate, DateTime endDate)
        {
            if (discountValue < 0)
                throw new ArgumentException("The discount value cannot be negative.", nameof(discountValue));

            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.", nameof(endDate));

            Type = type;
            DiscountValue = discountValue;
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool IsActive(DateTime date) => date >= StartDate && date <= EndDate;



        public decimal ApplyDiscount(decimal originalPrice)
        {
            if (originalPrice < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(originalPrice));

            return Type switch
            {
                CampaignType.PercentageDiscount => originalPrice * (1 - DiscountValue / 100),
                CampaignType.FixedDiscount => Math.Max(originalPrice - DiscountValue, 0),
                _ => originalPrice
            };
        }
    }
}
