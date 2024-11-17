using System;
using System.Collections.Generic;
using System.Linq;
using KYH.NET_KassaSystem_Nastaran.Services;



namespace KYH.NET_KassaSystem_Nastaran.Models
{
    public class Product
    {
        public int Id { get; set; } // Unikt ID för produkten
        public string Name { get; set; } // Namn på produkten
        public decimal Price { get; set; } // Standardpris för produkten
        public string PriceType { get; set; } // Pristyp: "per unit" eller "per kg"
        public List<Campaign> Campaigns { get; set; } = new List<Campaign>(); // Lista över aktiva kampanjer för produkten

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

       
        /// </summary>
        /// <param name="campaign">Kampanjen som ska läggas till.</param>
        public void AddCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign), "Kampanjen kan inte vara null.");

            Campaigns.Add(campaign);
        }

     
        /// </summary>
        /// <param name="campaign">Kampanjen som ska tas bort.</param>
        public void RemoveCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign), "Kampanjen kan inte vara null.");

            Campaigns.Remove(campaign);
        }

       
        /// </summary>
        /// <param name="date">Datumet för att kontrollera kampanjaktivitet.</param>
        /// <returns>Det justerade priset baserat på kampanj.</returns>
        public decimal GetEffectivePrice(DateTime date)
        {
            var activeCampaign = Campaigns.FirstOrDefault(c => c.IsActive(date));

            return activeCampaign != null ? activeCampaign.ApplyDiscount(Price) : Price;
        }
    }
}
