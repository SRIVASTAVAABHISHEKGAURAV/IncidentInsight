using System;
using System.IO;
using System.Threading.Tasks;

namespace IncidentInsight
{
    public static class FileHelper
    {
        private static readonly string DataFolderPath = Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data");

        private static string GetFilePath(int workItemId) =>
            Path.Combine(DataFolderPath, $"{workItemId}.txt");

        public static async Task SaveContentAsync(string content, int workItemId)
        {
            string filePath = GetFilePath(workItemId);

            try
            {
                await File.WriteAllTextAsync(filePath, content);
                Console.WriteLine("Content has been written to the file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving content to the file: {ex.Message}");
            }
        }

        public static bool FileExists(int workItemId) =>
            File.Exists(GetFilePath(workItemId));

        public static string GetFileContent(int workItemId)
        {
            string filePath = GetFilePath(workItemId);

            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("File not found.", filePath);

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                return $"Error occurred while reading the file: {ex.Message}";
            }
        }
    }

}
