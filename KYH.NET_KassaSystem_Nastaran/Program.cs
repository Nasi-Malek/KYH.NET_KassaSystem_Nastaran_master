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

            Admin admin = new Admin(new AdminTool()); 
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Ny Kund");
                Console.WriteLine("2. Admin");
                Console.WriteLine("0. Avsluta");


                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Kundmeny: Hantera kundfunktioner här.");

                            break;
                        case 2:
                            admin.ShowAdminMenu(); 
                            break;
                        case 0:
                            Console.WriteLine("Programmet avslutas...");
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Ogiltigt val. Försök igen.");
                            break;
                    }
                }

                // Skapa en instans av ErrorManager för felhantering
                var errorManager = new ErrorManager();

                // Skapa en instans av Receipt och passera in felhanteraren
                var receipt = new Receipt(errorManager);

                // Skapa ett nytt AdminTool för att hantera produkter och kampanjer
                var adminTool = new AdminTool();

                try
                {

                    var cashRegister = new CashRegister();
                    cashRegister.StartNewTransactionTest();
                    cashRegister.Start();
                    Console.Clear();
                    receipt.PrintAndSaveReceipt();

                }
                catch (Exception ex)
                {
                    errorManager.LogError(ex);
                    errorManager.DisplayError("Ett oväntat fel inträffade.");
                }
            }
        }
    }
}
