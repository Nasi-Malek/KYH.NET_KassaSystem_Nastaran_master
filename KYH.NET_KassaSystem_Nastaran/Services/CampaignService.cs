using KYH.NET_KassaSystem_Nastaran.Enum;
using KYH.NET_KassaSystem_Nastaran.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class CampaignService
    {
        private const string FilePath = "Campaign.txt";

        public List<Campaign> LoadCampaigns()
        {
            if (!File.Exists(FilePath)) return new List<Campaign>();

            return File.ReadAllLines(FilePath)
                       .Select(Campaign.FromString)
                       .ToList();
        }

        public void SaveCampaigns(List<Campaign> campaigns)
        {
            var lines = campaigns.Select(c => c.ToString()).ToArray();
            File.WriteAllLines(FilePath, lines);
        }


        // Spara kampanjer till fil
        public static void SaveCampaignsToFile(List<Product> products, string filePath)
        {
            try
            {
                var lines = new List<string>();

                foreach (var product in products)
                {
                    foreach (var campaign in product.Campaigns)
                    {
                        lines.Add($"{product.Id};{campaign.ToFileString()}");
                    }
                }

                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sparande av kampanjer: {ex.Message}");
            }
        }

        // Ladda kampanjer från fil
        public static List<Campaign> LoadCampaignsFromFile(int productId, string filePath)
        {
            var campaigns = new List<Campaign>();

            if (!File.Exists(filePath))
                return campaigns;

            try
            {
                var lines = File.ReadAllLines(filePath)
                                .Where(line => line.StartsWith($"{productId};"));

                foreach (var line in lines)
                {
                    var data = line.Split(';', 2);
                    if (data.Length < 2)
                        continue;

                    try
                    {
                        var campaign = FromString(data[1]);
                        campaigns.Add(campaign);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fel vid inläsning av kampanj: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid inläsning från fil: {ex.Message}");
            }

            return campaigns;
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
