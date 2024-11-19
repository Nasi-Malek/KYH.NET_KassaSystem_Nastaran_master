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
            var errorManager = new ErrorManager();
            var receipt = new Receipt(errorManager);
            var adminTool = new AdminTool();

            while (true)
            {
                try
                {

                    Console.Clear();
                    Console.WriteLine("\n---** Huvud Meny **---\n");
                    Console.WriteLine("1. Ny Kund");
                    Console.WriteLine("2. Admin");
                    Console.WriteLine("0. Avsluta");
                    Console.Write("\nVälj ett alternativ: ");

                    // Läsa in användarens val
                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                HandleCustomer(receipt);
                                break;
                            case 2:
                                admin.ShowAdminMenu();
                                break;
                            case 0:
                                Console.WriteLine("\nProgrammet avslutas. Tack för att du använde vår applikation!");
                                Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("\nOgiltigt val. Försök igen.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nFelaktig inmatning. Ange ett giltigt nummer.");
                    }
                }
                catch (Exception ex)
                {
                    // Hantera oväntade fel
                    errorManager.LogError(ex);
                    errorManager.DisplayError("Ett oväntat fel inträffade. Försök igen.");
                }

                Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
        }

        static void HandleCustomer(Receipt receipt)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("--- Kund Meny ---\n");
                Console.WriteLine("Här kan du lägga till produkter, hantera kvitton m.m.");


                var cashRegister = new CashRegister();
                cashRegister.StartNewTransactionTest();
                cashRegister.Start();


                receipt.PrintAndSaveReceipt();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Ett fel inträffade när kundfunktionen hanterades: " + ex.Message);
            }
        }
    }
}
