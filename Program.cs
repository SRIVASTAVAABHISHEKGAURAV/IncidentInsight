using Microsoft.Extensions.Configuration;

namespace TicketManager
{
    public class Program
    {
        private static readonly string environment = Utility.GetEnvironment();

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter the Incident ID:");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input cannot be empty. Please enter a valid Incident ID.");
                }
                else
                {
                    if (environment == "ADO")
                    {
                        if (int.TryParse(input, out int workItemId))
                        {
                            IncidentManager.ProcessWorkItem(workItemId).Wait();
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a valid integer Incident ID.");
                        }
                    }
                    else if (environment == "JIRA")
                    {
                        IncidentManager.ProcessWorkItem(input).Wait();
                    }
                    else
                    {
                        Console.WriteLine("Invalid Environment.");
                    }
                }

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}