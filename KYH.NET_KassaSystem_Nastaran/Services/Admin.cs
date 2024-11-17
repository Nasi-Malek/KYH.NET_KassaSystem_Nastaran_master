using KYH.NET_KassaSystem_Nastaran.Enum;
using KYH.NET_KassaSystem_Nastaran.Models;
using KYH.NET_KassaSystem_Nastaran.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class Admin
    {
        private List<Product> _products;

        public Admin(AdminTool adminTool)
        {
            _products = adminTool.Products ?? new List<Product>();
        }

        public List<Product> Products => _products;

        // Visa huvudmenyn för Admin
        public void ShowAdminMenu()
        {
            int choice;
            do
            {
                Console.WriteLine("\n--- Admin Menu ---");
                Console.WriteLine("1. Change Product Name or Price");
                Console.WriteLine("2. Add New Product");
                Console.WriteLine("3. Manage Campaign Prices");
                Console.WriteLine("4. Add/Remove Campaigns");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Choose an option: ");

                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    try
                    {
                        switch (choice)
                        {
                            case 1:
                                UpdateProductDetails();
                                break;
                            case 2:
                                AddNewProduct();
                                break;
                            case 3:
                                ManageCampaignPrices();
                                break;
                            case 4:
                                AddOrRemoveCampaign();
                                break;
                            case 5:
                                Console.WriteLine("Exiting Admin Menu.");
                                break;
                            default:
                                Console.WriteLine("Invalid option, please try again.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            } while (choice != 5);
        }

        // 1. Uppdatera produktnamn eller pris
        private void UpdateProductDetails()
        {
            try
            {
                Console.Write("Enter Product ID to update: ");
                if (int.TryParse(Console.ReadLine(), out int productId))
                {
                    var product = _products.FirstOrDefault(p => p.Id == productId);
                    if (product == null)
                    {
                        Console.WriteLine("Product not found.");
                        return;
                    }

                    Console.Write($"Enter new name for {product.Name} (or press Enter to keep current name): ");
                    string newName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        product.Name = newName;
                    }

                    Console.Write($"Enter new price for {product.Name} (or press Enter to keep current price): ");
                    string newPriceInput = Console.ReadLine();
                    if (decimal.TryParse(newPriceInput, out decimal newPrice) && newPrice >= 0)
                    {
                        product.Price = newPrice;
                    }

                    Console.WriteLine("Product details updated successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid Product ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
            }
        }

        // 2. Lägg till en ny produkt
        private void AddNewProduct()
        {
            try
            {
                Console.Write("Enter new product ID: ");
                if (int.TryParse(Console.ReadLine(), out int id) && !_products.Exists(p => p.Id == id))
                {
                    Console.Write("Enter product name: ");
                    string name = Console.ReadLine();

                    Console.Write("Enter product price: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal price) && price >= 0)
                    {
                        Console.Write("Enter price type (per unit/per kg): ");
                        string priceType = Console.ReadLine();

                        var newProduct = new Product(id, name, price, priceType);
                        _products.Add(newProduct);
                        Console.WriteLine("New product added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid price.");
                    }
                }
                else
                {
                    Console.WriteLine("Product ID already exists or invalid.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
            }
        }

        // 3. Hantera kampanjpriser för produkter
        private void ManageCampaignPrices()
        {
            try
            {
                Console.Write("Enter Product ID to set a campaign price: ");
                if (int.TryParse(Console.ReadLine(), out int productId))
                {
                    var product = _products.FirstOrDefault(p => p.Id == productId);
                    if (product == null)
                    {
                        Console.WriteLine("Product not found.");
                        return;
                    }

                    Console.WriteLine("Choose campaign type: 1. Percentage Discount 2. Fixed Discount");
                    if (int.TryParse(Console.ReadLine(), out int campaignTypeChoice))
                    {
                        CampaignType campaignType = campaignTypeChoice switch
                        {
                            1 => CampaignType.PercentageDiscount,
                            2 => CampaignType.FixedDiscount,
                            _ => throw new InvalidOperationException("Invalid campaign type.")
                        };

                        Console.Write("Enter discount value: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal discountValue) && discountValue > 0)
                        {
                            Console.Write("Enter campaign start date (yyyy-MM-dd): ");
                            DateTime startDate = DateTime.Parse(Console.ReadLine());

                            Console.Write("Enter campaign end date (yyyy-MM-dd): ");
                            DateTime endDate = DateTime.Parse(Console.ReadLine());

                            var newCampaign = new Campaign(campaignType, discountValue, startDate, endDate);
                            product.AddCampaign(newCampaign);
                            Console.WriteLine("Campaign added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid discount value.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid campaign type.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Product ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error managing campaign prices: {ex.Message}");
            }
        }

        // 4. Lägg till eller ta bort kampanjer för produkter
        private void AddOrRemoveCampaign()
        {
            try
            {
                Console.Write("Enter Product ID to manage campaigns: ");
                if (int.TryParse(Console.ReadLine(), out int productId))
                {
                    var product = _products.FirstOrDefault(p => p.Id == productId);
                    if (product == null)
                    {
                        Console.WriteLine("Product not found.");
                        return;
                    }

                    Console.WriteLine("1. Add new campaign");
                    Console.WriteLine("2. Remove existing campaign");
                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                ManageCampaignPrices();
                                break;

                            case 2:
                                if (product.Campaigns.Any())
                                {
                                    Console.WriteLine("Available Campaigns:");
                                    for (int i = 0; i < product.Campaigns.Count; i++)
                                    {
                                        Console.WriteLine($"{i + 1}. {product.Campaigns[i].Type} - {product.Campaigns[i].DiscountValue}");
                                    }

                                    Console.Write("Select campaign number to remove: ");
                                    if (int.TryParse(Console.ReadLine(), out int campaignIndex) &&
                                        campaignIndex > 0 &&
                                        campaignIndex <= product.Campaigns.Count)
                                    {
                                        product.Campaigns.RemoveAt(campaignIndex - 1);
                                        Console.WriteLine("Campaign removed successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid campaign number.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No campaigns available to remove.");
                                }
                                break;

                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Product ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error managing campaigns: {ex.Message}");
            }
        }
    }
}
