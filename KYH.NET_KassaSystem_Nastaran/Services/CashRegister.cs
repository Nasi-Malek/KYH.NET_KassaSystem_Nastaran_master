using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KYH.NET_KassaSystem_Nastaran.Interface;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Services;



namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class CashRegister
    {
        private Admin _admin;
        //private Admin _admin = new Admin(new List<Product>()); // Skapa instans av Admin för att hantera produkter
        private Receipt currentReceipt; // Kvitto för aktuell transaktion
        private IErrorManager errorManager = new ErrorManager(); // Instans av felhanterare

        public CashRegister()
        {
            var adminTool = new AdminTool(); // Skapa en instans av AdminTool
            _admin = new Admin(adminTool);   // Skicka AdminTool-instansen till Admin
        }



        // Starta huvudloopen för kassan
        public void Start()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1. Ny Kund\n2. Admin\n0. Avsluta"); // Visa alternativ
                var input = Console.ReadLine();
                if (input == "1")
                {
                    StartNewTransactionTest(); // Starta ny kundtransaktion
                }
                else if (input == "2")
                {
                    _admin.ShowAdminMenu();
                }
                else if (input == "0")
                {
                    break; // Avsluta programmet
                }
            }
        }

        // Starta en ny transaktion för en kund
        public void StartNewTransactionTest()
        {
            currentReceipt = new Receipt(errorManager); // Skapa ett nytt kvitto
            Console.WriteLine("KASSA");
            Console.WriteLine($"KVITTO:\t\t{DateTime.Now:yyyy-MM-dd\t HH:mm:ss}"); // Visa kassaskärm med datum

            while (true)
            {
                Console.WriteLine("Kommandon: <productid> <antal> eller PAY"); // Inmatningskommando för produkter eller betalning
                var command = Console.ReadLine()?.Split(' ');

                if (command[0] == "PAY") // Om kunden vill betala
                {
                    currentReceipt.PrintAndSaveReceipt(); // Skriv ut och spara kvittot
                    break; // Avsluta transaktionen
                }
                else if (int.TryParse(command[0], out int productId) && int.TryParse(command[1], out int quantity))
                {
                    try
                    {
                        // Hämta produkten via produkt-ID och kvantitet
                        var product = _admin.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                            // Beräkna det effektiva priset baserat på dagens datum
                            decimal effectivePrice = product.GetEffectivePrice(DateTime.Now);

                            // Lägg till produkten i kvittot med rätt pris
                            currentReceipt.AddItem(product, quantity);
                        }
                        else
                        {
                            Console.WriteLine("Produkten med angivet ID hittades inte.");
                        }
                    }
                    catch (Exception ex)
                    {
                        errorManager.LogError(ex); // Logga fel
                        errorManager.DisplayError("Ett fel inträffade vid tillägg av produkt."); // Visa felmeddelande
                    }
                }
                else
                {
                    Console.WriteLine("Ogiltigt kommando. Ange produkt-ID och antal, eller PAY för att avsluta transaktionen.");
                }
            }
        }
    }
}
