using System;
using System.Collections.Generic;
using System.Linq;
using KYH.NET_KassaSystem_Nastaran.Services;


namespace KYH.NET_KassaSystem_Nastaran.Models
{
    public class Product
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public decimal Price { get; set; } 
        public string PriceType { get; set; } 
        public List<Campaign> Campaigns { get; set; } = new List<Campaign>(); 

        public Product(int id, string name, decimal price, string priceType)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Produktnamn kan inte vara tomt.", nameof(name));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Priset kan inte vara negativt.");

            Id = id;
            Name = name;
            Price = price;
            PriceType = priceType;
        }

        public void AddCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign), "Kampanjen kan inte vara null.");

            Campaigns.Add(campaign);
        }

        public void RemoveCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign), "Kampanjen kan inte vara null.");

            Campaigns.Remove(campaign);
        }

        public decimal GetEffectivePrice(DateTime date)
        {
            var activeCampaign = Campaigns.FirstOrDefault(c => c.IsActive(date));

            return activeCampaign != null ? activeCampaign.ApplyDiscount(Price) : Price;
        }
    }
}
