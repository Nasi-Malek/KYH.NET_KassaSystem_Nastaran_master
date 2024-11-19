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

        private static int receiptCounter = LoadReceiptCounter();
        public int ReceiptNumber { get; private set; }
        public List<(Product product, int quantity)> Items { get; private set; } = new List<(Product, int)>();
        public DateTime Date { get; private set; }


        private static string receiptCounterFilePath = "ReceiptCounter.txt";
        private static string receiptFilePath = "../../../Files/Receipts";

        public Receipt(IErrorManager errorManager)
        {
            _errorManager = errorManager;

            Date = DateTime.Now;
            ReceiptNumber = receiptCounter;

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
                    Console.WriteLine($"Campaign: {campaign.Type}, Start: {campaign.StartDate:yyyy-MM-dd}\n\t\t\t      End: {campaign.EndDate:yyyy-MM-dd}  Time: {Date:HH:mm:ss}");

                }

                decimal effectivePrice = product.GetEffectivePrice(DateTime.Now);

                Items.Add((product, quantity));

                Console.WriteLine($"Product : {product.Name}\n\t Quantity: {quantity}\n\t Price: {effectivePrice} kr\n");
            }
            catch (Exception ex)
            {
                _errorManager.LogError(ex);
                _errorManager.DisplayError("Det gick inte att lägga till produkten. Försök igen.");
            }
        }

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

        public void PrintAndSaveReceipt()
        {
            ReceiptNumber = receiptCounter++;

            if (!Directory.Exists(receiptFilePath))
                Directory.CreateDirectory(receiptFilePath);

            string filePath = Path.Combine(receiptFilePath, $"Receipt_{Date:yyyy-MM-dd}_#{ReceiptNumber}.txt");
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                // Kvittohuvud och transaktionsinformation
                writer.WriteLine("*===================================================*");
                writer.WriteLine("    \t\t** FOOD & SUPERMARKET SOLNA **\n");

                writer.WriteLine(" Opening hrs:\t\t\t\t\t\tCentralvägen 16\n Mon-Fri   07:00-22:00\t\t\t\t171 42, SOLNA\n Sat-Sun   08:00-22:00\t\t");
                writer.WriteLine("-----------------------------------------------------");
                writer.WriteLine($"Cashier:1214\t\t\t\t         RECEIPT: #{ReceiptNumber}");
                writer.WriteLine($"Date: {Date:yyyy-MM-dd}\t\t\t\t     Time: {Date:HH:mm:ss}");
                writer.WriteLine("------------------------------------------------------");

                // Totalkostnad
                decimal total = CalculateTotal();
                foreach (var item in Items)
                {
                    decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                    writer.WriteLine($"{item.product.Name} \t\t\t\t\t\t\t{item.quantity} * {item.product.Price} = {itemTotal:0.00} kr ");
                }

                writer.WriteLine("******************************************************");
                writer.WriteLine($"Total: \t\t\t\t\t\t\t\t     {total:0.00} kr");
                writer.WriteLine("******************************************************");
                writer.WriteLine("\n\t\t\t**Thank you, Welcome back!**\n\t\t\t\t   Tel:08-72360000 ");

                // Konsolutdata för kvitto
                Console.WriteLine("*----------------------------------------------*");
                Console.WriteLine("   \t** FOOD & SUPERMARKET SOLNA **\n");
                Console.WriteLine(" Opening hrs:\t\t\tCentralvägen 16\n Mon-Fri   07:00-22:00\t\t171 42, SOLNA\n Sat-Sun   08:00-22:00\t\t");
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine($"Cashier:1214\t\t\tRECEIPT: #{ReceiptNumber}");
                Console.WriteLine($"Date: {Date:yyyy-MM-dd}\t\tTime: {Date:HH:mm:ss}");
                Console.WriteLine("*-----------------------------------------------*");

                foreach (var item in Items)
                {

                    decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                    Console.WriteLine($"{item.product.Name} \t\t\t\t{item.quantity} * {item.product.Price} = {itemTotal:0.00} kr ");

                }
                Console.WriteLine("*************************************************");
                Console.WriteLine($"Total : \t\t\t\t {total:0.00} kr");
                Console.WriteLine("*************************************************");
            }

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

        private static void SaveReceiptCounter()

        {
            File.WriteAllText("ReceiptCounter.txt", receiptCounter.ToString()); // Skriv senaste numret till fil
        }

    }
}
