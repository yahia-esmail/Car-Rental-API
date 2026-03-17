using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer
{
    public static class Helper
    {
        public static string ComputeHash(string input)
        {
            //SHA is Hash Algorithm.
            // Create an instance of the SHA-256 algorithm
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash value from the UTF-8 encoded input string
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));


                // Convert the byte array to a lowercase hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static class ImageUploaderHelper
        {
            private static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            public static bool IsValidImage(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    return false;

                var extension = Path.GetExtension(file.FileName).ToLower();
                return AllowedExtensions.Contains(extension);
            }

            public static async Task<(bool success, string? fileName, string? url, string? error)> UploadImageAsync(
                IFormFile file,
                string uploadFolderRelativePath,
                string host,
                string scheme,
                string? customFileNamePrefix = null)
            {
                try
                {
                    if (!IsValidImage(file))
                        return (false, null, null, "Invalid file type or empty file.");

                    var extension = Path.GetExtension(file.FileName).ToLower();

                    var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolderRelativePath);

                    if (!Directory.Exists(uploadsRootFolder))
                        Directory.CreateDirectory(uploadsRootFolder);

                    var fileName = $"{customFileNamePrefix}_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsRootFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var url = $"{scheme}://{host}/{uploadFolderRelativePath.Replace("\\", "/")}/{fileName}";
                    return (true, fileName, url, null);
                }
                catch (Exception ex)
                {
                    return (false, null, null, $"Upload failed: {ex.Message}");
                }
            }
        }
    }
}
