﻿using System;
using System.Collections.Generic;
using System.Linq;
using KYH.NET_KassaSystem_Nastaran.Enum;
using KYH.NET_KassaSystem_Nastaran.Services;


namespace KYH.NET_KassaSystem_Nastaran.Models
{
    public class AdminTool
    {
        public List<Product> Products { get; set; } = new List<Product>(); // Lista med produkter i butiken

        public AdminTool()
        {
            try
            {
                var banana = new Product(300, "Banana", 8m, "per styck");
                var apple = new Product(301, "Apple", 10m, "per styck");
                var coffee = new Product(302, "Coffee", 35m, "per styck");
                var milk = new Product(303, "Milk", 10m, "per styck");

                var appleCampaign = new Campaign(
                    CampaignType.PercentageDiscount,  // Typ av kampanj
                    20m,                              // Rabatt i procent
                    new DateTime(2024, 11, 1),       // Startdatum
                    new DateTime(2024, 12, 25)       // Slutdatum
                );

                apple.AddCampaign(appleCampaign);

                AddProduct(banana);
                AddProduct(apple);
                AddProduct(coffee);
                AddProduct(milk);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid initiering av produkter: {ex.Message}");
            }
        }

        public void AddProduct(Product product)
        {

            if (product == null)
                throw new ArgumentNullException(nameof(product), "Produkt kan inte vara null.");

            Products.Add(product);
        }
    }
}