using KYH.NET_KassaSystem_Nastaran.Interface;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace KYH.NET_KassaSystem_Nastaran.Models
{
    public class Receipt
    {
        private readonly IErrorManager _errorManager;

        private static int receiptCounter = LoadReceiptCounter(); // Laddar senaste kvittonummer från fil
        public int ReceiptNumber { get; private set; }
        public List<(Product product, int quantity)> Items { get; private set; } = new List<(Product, int)>();
        public DateTime Date { get; private set; }


        private static string receiptCounterFilePath = "ReceiptCounter.txt"; // Sökväg för filen som lagrar kvittonumret
        private static string receiptFilePath = "../../../Files/Receipts"; // Sökväg för filen som lagrar kvittoutskrifter


        public Receipt(IErrorManager errorManager)
        {
            _errorManager = errorManager;

            Date = DateTime.Now;
            ReceiptNumber = receiptCounter; // Öka kvittonumret för varje nytt kvitto
        }

        // Lägg till produkt och kvantitet till kvittot
        public void AddItem(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (quantity <= 0)
                throw new ArgumentException("Antal måste vara positivt.", nameof(quantity));

            try
            {
                foreach (var campaign in product.Campaigns)
                {
                    Console.WriteLine($"Campaign type: {campaign.Type}, Start: {campaign.StartDate}, End: {campaign.EndDate}, Active?(true/False): {campaign.IsActive(DateTime.Now)}");
                }
                // Hämta det effektiva priset baserat på dagens datum
                decimal effectivePrice = product.GetEffectivePrice(DateTime.Now);


                // Lägg till produkten och mängden i listan
                Items.Add((product, quantity));

                // Eventuellt logga till konsolen (för debugging)
                Console.WriteLine($"Product added: {product.Name}, Quantity: {quantity}, Price: {effectivePrice} kr");
            }
            catch (Exception ex)
            {
                _errorManager.LogError(ex);
                _errorManager.DisplayError("Det gick inte att lägga till produkten. Försök igen.");
            }
        }

        // Beräkna totalkostnaden för alla produkter i kvittot
        public decimal CalculateTotal()
        {
            try
            {
                return Items.Sum(item => item.product.GetEffectivePrice(Date) * item.quantity);
            }
            catch (Exception ex)
            {
                _errorManager.LogError(ex);
                _errorManager.DisplayError("Det gick inte att beräkna totalen. Försök igen.");
                return 0;
            }
        }

        // Skriv ut och spara kvittot i en textfil
        public void PrintAndSaveReceipt()
        {
            ReceiptNumber = receiptCounter++; // Öka och tilldela kvittonummer vid sparandet

            if (!Directory.Exists(receiptFilePath))
                Directory.CreateDirectory(receiptFilePath);

            string filePath = Path.Combine(receiptFilePath, $"Receipt_{Date:yyyy-MM-dd}_#{ReceiptNumber}.txt");
            using (StreamWriter writer = new StreamWriter(filePath, true)) // Öppna filen i tilläggsläge
            {
                // Kvittohuvud och transaktionsinformation
                writer.WriteLine("*==============================*");
                writer.WriteLine("   **IKEA FOOD & SUPERMARKET**   \n");
                writer.WriteLine($"RECEIPT: #{ReceiptNumber}\n");
                writer.WriteLine($"Cashier:\t\tDate:{Date:yyyy-MM-dd}\n\t\t\t    Time: {Date:HH:mm:ss}");
                Console.WriteLine("\n   Thank you, Welcome back!");

                

                // Totalkostnad
                decimal total = CalculateTotal();
                writer.WriteLine("*************************************");
                foreach (var item in Items)
                {

                    decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                    writer.WriteLine($"{item.product.Name} \t\t{item.quantity} * {item.product.Price} = {itemTotal:0.00} kr ");


                }
                writer.WriteLine("*************************************");
                writer.WriteLine($"Total: \t\t\t\t {total:0.00} kr");
                writer.WriteLine(new string('-', 35));
                writer.WriteLine("\n**Thank you, Welcome back!** ");

                // Konsolutdata för kvitto

                Console.WriteLine("*===================================*");
                Console.WriteLine("    **IKEA FOOD & SUPERMARKET**\n");
                Console.WriteLine($"RECEIPT:  #{ReceiptNumber}\n");
                Console.WriteLine($"Cashier:\tDate: {Date:yyyy-MM-dd}\n\t\tTime: {Date:HH:mm:ss}");
                Console.WriteLine("*===================================*");
                foreach (var item in Items)
                {

                    decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                    Console.WriteLine($"{item.product.Name} \t\t{item.quantity} * {item.product.Price} = {itemTotal:0.00} kr ");


                }
                Console.WriteLine("*************************************");
                Console.WriteLine($"Total : \t\t{total} kr");
                Console.WriteLine("*************************************");
            }

            // Uppdatera kvittonummer i fil
            SaveReceiptCounter();
        }

        // Ladda senaste kvittonummer från fil
        private static int LoadReceiptCounter()
        {
            string path = "ReceiptCounter.txt";
            if (File.Exists(path))
                return int.Parse(File.ReadAllText(path));
            return 0; // Om filen inte finns, börja på 0
        }
        // Spara uppdaterat kvittonummer till fil
        private static void SaveReceiptCounter()
        {
            File.WriteAllText("ReceiptCounter.txt", receiptCounter.ToString()); // Skriv senaste numret till fil
        }

    }
}
