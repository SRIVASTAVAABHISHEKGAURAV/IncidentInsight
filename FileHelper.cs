using System;
using System.IO;
using System.Threading.Tasks;
using TicketManager;

namespace IncidentInsight
{
    public static class FileHelper
    {
        public static async Task SaveContentAsync(string content, string workItemId, string folderName)
        {
            await SaveAsync(content, workItemId, GetFolderPath(Utility.GetEnvironment(), folderName));
        }

        private static async Task SaveAsync(string content, string workItemId, string folderPath)
        {
            string filePath = GetFilePath(folderPath, workItemId);

            if (!File.Exists(filePath))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            try
            {
                await File.WriteAllTextAsync(filePath, content);
                Console.WriteLine($"Content has been written to {filePath} successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving content to {filePath}: {ex.Message}");
            }
        }

        public static bool FileExists(string workItemId, string folderName)
        {
            string filePath = GetFilePath(GetFolderPath(Utility.GetEnvironment(), folderName), workItemId);
            return File.Exists(filePath);
        }

        public static string GetFileContent(string workItemId, string folderName)
        {
            string filePath = GetFilePath(GetFolderPath(Utility.GetEnvironment(), folderName), workItemId);

            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("File not found.", filePath);

                return File.ReadAllText(filePath);
            }
            catch (FileNotFoundException ex)
            {
                return $"File not found: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error occurred while reading the file: {ex.Message}";
            }
        }

        private static string GetFolderPath(string environment, string folderName)
        {
            string rootFolderPath = Path.Combine(
                Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, environment);

            string folderPath = Path.Combine(rootFolderPath, folderName);


            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"'{folderName}' folder not found for '{environment}' environment. Creating directory...");
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        private static string GetFilePath(string folderPath, string workItemId)
        {
            return Path.Combine(folderPath, $"{workItemId}.txt");
        }
    }
}
