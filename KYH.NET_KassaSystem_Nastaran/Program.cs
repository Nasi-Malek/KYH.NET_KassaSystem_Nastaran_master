using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using KYH.NET_KassaSystem_Nastaran.Enum;
using KYH.NET_KassaSystem_Nastaran.Interface;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Services;


namespace KYH.NET_KassaSystem_Nastaran
{
    class Program
    {
        static void Main(string[] args)
        {
            // Skapa en instans av ErrorManager för felhantering
            var errorManager = new ErrorManager();

            // Skapa en instans av Receipt och passera in felhanteraren
            var receipt = new Receipt(errorManager);

            // Skapa ett nytt AdminTool för att hantera produkter och kampanjer
            var adminTool = new AdminTool();

            try
            {
                // Konsolutdata för att visa kvittot i terminalen
                Console.WriteLine(" (301 => 20% rabatt -> 10 kr till 8.00 kr per styck)");
                Console.WriteLine(" -----------------------------------------------------");

                // Starta en ny kassatransaktion på kampanjdatumet
                var cashRegister = new CashRegister();
                cashRegister.StartNewTransactionTest();

                // Starta huvudloopen för kassaflödet
                cashRegister.Start();
                Console.Clear();
                receipt.PrintAndSaveReceipt();

            }
            catch (Exception ex)
            {
                // Logga oväntade fel och visa ett generellt felmeddelande
                errorManager.LogError(ex);
                errorManager.DisplayError("Ett oväntat fel inträffade.");
            }
        }
    }
}
