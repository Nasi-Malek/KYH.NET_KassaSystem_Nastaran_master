using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Interface;

namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class Receipt
    {
        private readonly IErrorManager _errorManager;
        private const decimal VatRate = 0.25m; // Momssats (25%)

        private static int receiptCounter = LoadReceiptCounter();
        public int ReceiptNumber { get; private set; }
        public List<(Product product, int quantity)> Items { get; private set; } = new List<(Product, int)>();
        public DateTime Date { get; private set; } = DateTime.Now;

        private static string receiptCounterFilePath = "ReceiptCounter.txt";
        private static string receiptFilePath = "../../../Files/Receipts";

        public Receipt(IErrorManager errorManager)
        {
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            ReceiptNumber = receiptCounter;
        }

        public void AddItem(Product product, int quantity)
        {
            if (product == null)
            {
                _errorManager.DisplayError("Produkten är ogiltig.");
                return;
            }
            if (quantity <= 0)
            {
                _errorManager.DisplayError("Antalet måste vara positivt.");
                return;
            }

            foreach (var campaign in product.Campaigns)
            {
                Console.WriteLine($"Kampanj: {campaign.Type}, Start: {campaign.StartDate:yyyy-MM-dd}, Slut: {campaign.EndDate:yyyy-MM-dd}");
            }

            decimal effectivePrice = product.GetEffectivePrice(DateTime.Now);
            Items.Add((product, quantity));
            Console.WriteLine($"Produkt: {product.Name}\nAntal: {quantity}\nPris: {effectivePrice:C}");
        }

        private decimal CalculateTotalExcludingVat()
        {
            return Items.Sum(item => item.product.GetEffectivePrice(Date) * item.quantity);
        }

        private decimal CalculateVat()
        {
            return CalculateTotalExcludingVat() * VatRate;
        }

        public void PrintAndSaveReceipt()
        {
            ReceiptNumber = receiptCounter++;
            decimal totalExclVat = CalculateTotalExcludingVat();
            decimal vat = CalculateVat();
            decimal totalInclVat = totalExclVat + vat;

            if (!Directory.Exists(receiptFilePath))
                Directory.CreateDirectory(receiptFilePath);

            string filePath = Path.Combine(receiptFilePath, $"Receipt_{Date:yyyy-MM-dd}_#{ReceiptNumber}.txt");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("*===================================================*");
                writer.WriteLine("\t\t** FOOD & SUPERMARKET SOLNA **\n");
                writer.WriteLine(" Opening hrs:\t\t\t\tCentralvägen 16\n Mon-Fri   07:00-22:00\t\t171 42, SOLNA\n Sat-Sun   08:00-22:00");
                writer.WriteLine("-----------------------------------------------------");
                writer.WriteLine($"Cashier: 1214\t\tRECEIPT: #{ReceiptNumber}");
                writer.WriteLine($"Date: {Date:yyyy-MM-dd}\tTime: {Date:HH:mm:ss}");
                writer.WriteLine("-----------------------------------------------------");
                foreach (var item in Items)
                {
                    decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                    writer.WriteLine($"{item.product.Name}\t{item.quantity} x {item.product.Price:C} = {itemTotal:C}");
                }
                writer.WriteLine("-----------------------------------------------------");
                writer.WriteLine($"Total (exkl. moms): {totalExclVat:C}");
                writer.WriteLine($"Moms (25%):         {vat:C}");
                writer.WriteLine($"Total (inkl. moms): {totalInclVat:C}");
                writer.WriteLine("-----------------------------------------------------");
                writer.WriteLine("\t\t** Tack, Välkommen åter! **");
            }

            Console.WriteLine("\n--- Kvitto ---");
            foreach (var item in Items)
            {
                decimal itemTotal = item.product.GetEffectivePrice(Date) * item.quantity;
                Console.WriteLine($"{item.product.Name}\t{item.quantity} x {item.product.Price:C} = {itemTotal:C}");
            }
            Console.WriteLine("------------------------");
            Console.WriteLine($"Total (exkl. moms): {totalExclVat:C}");
            Console.WriteLine($"Moms (25%): {vat:C}");
            Console.WriteLine($"Total (inkl. moms): {totalInclVat:C}");
            Console.WriteLine("------------------------");

            SaveReceiptCounter();
        }

        private static int LoadReceiptCounter()
        {
            string path = "ReceiptCounter.txt";
            if (File.Exists(path))
                return int.Parse(File.ReadAllText(path));
            return 0;
        }

        private static void SaveReceiptCounter()
        {
            File.WriteAllText("ReceiptCounter.txt", receiptCounter.ToString());
        }

    }
}
