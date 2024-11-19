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
        private Receipt currentReceipt;
        private IErrorManager errorManager = new ErrorManager(); 

        public CashRegister()
        {

            var adminTool = new AdminTool(); 
            _admin = new Admin(adminTool);   

        }

        // Starta huvudloopen för kassan
        public void Start()
        {
            while (true)
            {

                Console.WriteLine();
                Console.WriteLine("1. Ny Kund\n2. Admin\n0. Avsluta"); 
                var input = Console.ReadLine();
                if (input == "1")
                {
                    StartNewTransactionTest(); 
                }
                else if (input == "2")
                {
                    _admin.ShowAdminMenu();
                }
                else if (input == "0")
                {
                    break; 
                }
            }

        }

        // Starta en ny transaktion för en kund
        public void StartNewTransactionTest()
        {

            currentReceipt = new Receipt(errorManager); 
            Console.WriteLine("KASSA");
            Console.WriteLine($"KVITTO:\t\t{DateTime.Now:yyyy-MM-dd\t HH:mm:ss}"); 

            while (true)
            {

                Console.WriteLine("Kommandon: <productid> <antal> eller PAY"); 
                var command = Console.ReadLine()?.Split(' ');

                if (command[0] == "PAY") 
                {
                    currentReceipt.PrintAndSaveReceipt(); 
                    break; 
                }
                else if (int.TryParse(command[0], out int productId) && int.TryParse(command[1], out int quantity))
                {
                    try
                    {
                        // Hämta produkten via produkt-ID och kvantitet
                        var product = _admin.Products.FirstOrDefault(p => p.Id == productId);
                        if (product != null)
                        {
                             decimal effectivePrice = product.GetEffectivePrice(DateTime.Now);

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
                        errorManager.DisplayError("Ett fel inträffade vid tillägg av produkt.");
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
