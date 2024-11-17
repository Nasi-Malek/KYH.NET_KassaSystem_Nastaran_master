using System;
using System.IO;
using KYH.NET_KassaSystem_Nastaran.Interface;

namespace KYH.NET_KassaSystem_Nastaran.Services
{
    public class ErrorManager : IErrorManager
    {
        private const string LogFilePath = "ErrorLog.txt"; // Filväg för fel-logg

        
        /// </summary>
        /// <param name="ex">Undantaget som ska loggas.</param>
        public void LogError(Exception ex)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {
                    writer.WriteLine("----- Fel -----");
                    writer.WriteLine($"Datum och tid: {DateTime.Now}");
                    writer.WriteLine($"Typ av fel: {ex.GetType()}");
                    writer.WriteLine($"Felmeddelande: {ex.Message}");
                    writer.WriteLine($"StackTrace: {ex.StackTrace}");
                    writer.WriteLine("----------------\n");
                }
            }
            catch (Exception logEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Kunde inte logga felet: {logEx.Message}");
                Console.ResetColor();
            }
        }

        
        /// <param name="message">Felmeddelandet som ska visas för användaren.</param>
        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ett fel uppstod: {message}");
            Console.ResetColor();
        }
    }
}
