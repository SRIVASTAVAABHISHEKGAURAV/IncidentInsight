using System;
using System.IO;
using System.Threading.Tasks;

namespace IncidentInsight
{
    public static class FileHelper
    {
        public static async Task SaveContentAsync(string content, int workItemId, string folderName)
        {
            await SaveAsync(content, workItemId, GetFolderPath(folderName));
        }

        private static async Task SaveAsync(string content, int workItemId, string folderPath)
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

        public static bool FileExists(int workItemId, string folderName)
        {
            string filePath = GetFilePath(GetFolderPath(folderName), workItemId);
            return File.Exists(filePath);
        }

        public static string GetFileContent(int workItemId, string folderName)
        {
            string filePath = GetFilePath(GetFolderPath(folderName), workItemId);

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

        private static string GetFolderPath(string folderName)
        {
            string folderPath = folderName.ToLower() switch
            {
                "data" => Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Data"),
                "result" => Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "Result"),
                _ => throw new ArgumentException($"Invalid folder name: {folderName}")
            };

            return folderPath;
        }

        private static string GetFilePath(string folderPath, int workItemId)
        {
            return Path.Combine(folderPath, $"{workItemId}.txt");
        }
    }
}
