using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Services;
using System;
using System.Collections.Generic;
using System.Linq;





namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class Admin
    {

        private readonly AdminTool _adminTool;
        private List<Product> _products;
        public Admin(List<Product> products)
        {
            _products = products ?? throw new ArgumentNullException(nameof(products));
        }

        public Admin(AdminTool adminTool)
        {
            _adminTool = adminTool;
            _products = adminTool.Products ?? new List<Product>();
        }
        public List<Product> Products => _products;


       
        public void DisplayProductListFromFile()
        {
            string _filePath = "../../../Files/Products.txt";
            if (File.Exists(_filePath))
            {
                try
                {
                    var productData = File.ReadAllLines(_filePath);
                    Console.WriteLine("\n--- Product List ---");

                    if (productData.Length == 0)
                    {
                        Console.WriteLine("The file is empty. No products to display.");
                    }
                    else
                    {
                        Console.Clear();
                        foreach (var product in productData)

                        {
                            Console.WriteLine(product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading product file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No product file found.");
            }
        }

    }
}