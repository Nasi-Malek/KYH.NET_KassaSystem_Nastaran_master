using KYH.NET_KassaSystem_Nastaran.Services;
using System;
using System.Globalization;





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
                throw new ArgumentException("Rabattvärdet kan inte vara negativt.", nameof(discountValue));

            if (endDate < startDate)
                throw new ArgumentException("Slutdatum kan inte vara tidigare än startdatum.", nameof(endDate));

            Type = type;
            DiscountValue = discountValue;
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool IsActive(DateTime date) => date >= StartDate && date <= EndDate;


        public decimal ApplyDiscount(decimal originalPrice)
        {
            if (originalPrice < 0)
                throw new ArgumentException("Pris kan inte vara negativt.", nameof(originalPrice));

            return Type switch
            {
                CampaignType.PercentageDiscount => originalPrice * (1 - DiscountValue / 100),
                CampaignType.FixedDiscount => Math.Max(originalPrice - DiscountValue, 0),
                _ => originalPrice
            };
        }

        public string ToFileString()
        {
            return $"{Type}:{DiscountValue}:{StartDate:yyyy-MM-dd}:{EndDate:yyyy-MM-dd}";
        }

      
        public static Campaign FromString(string data)
        {
            var parts = data.Split(':');
            return new Campaign(
                CampaignType.Parse<CampaignType>(parts[0]),
                decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture)
            );
        }
    }
}
