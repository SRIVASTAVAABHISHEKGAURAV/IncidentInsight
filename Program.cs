namespace TicketManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter the Incident ID:");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int workItemId))
                {
                    IncidentManager.ProcessWorkItem(workItemId).Wait();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer Incident ID.");
                }

                Console.WriteLine("\n Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}