using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CommonLayer.PhotoUpload
{
    public class FileService : IFileService
    {
        private const int MaxPhotoBytes = 2 * 1024 * 1024; // 2 MB
        private readonly string _photosRoot;

        public FileService(IConfiguration config)
        {
            var configured = config["Uploads:PhotosRoot"];
            _photosRoot = string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(AppContext.BaseDirectory, "Photos")
                : configured;
        }

        public async Task<string?> SaveBase64ImageAsync(string base64Image, string folderPath)
        {
            if (string.IsNullOrEmpty(base64Image))
                return null;

            try
            {
                var parts = base64Image.Split(',');
                var cleanBase64 = parts.Length > 1 ? parts[1] : parts[0];

                byte[] imageBytes = Convert.FromBase64String(cleanBase64);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() + ".png";
                await File.WriteAllBytesAsync(Path.Combine(folderPath, fileName), imageBytes);
                return fileName;
            }
            catch
            {
                return null;
            }
        }

        public async Task<(string? RelativePath, string? Error)> SaveBase64PhotoAsync(string base64Image, string subFolder)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
                return (null, "Photo is empty.");

            // Remove a "data:image/...;base64," prefix if present
            var comma = base64Image.IndexOf(',');
            var data = (comma >= 0 ? base64Image[(comma + 1)..] : base64Image).Trim();

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(data);
            }
            catch (FormatException)
            {
                return (null, "Photo is not valid base64.");
            }

            if (bytes.Length > MaxPhotoBytes)
                return (null, "Photo must be 2 MB or smaller.");

            // Decide the extension from the real content, not from what the client claims
            var extension = DetectImageExtension(bytes);
            if (extension == null)
                return (null, "Only JPG or PNG photos are allowed.");

            var relativePath = $"{subFolder}/{Guid.NewGuid():N}{extension}";
            var fullPath = GetFullPath(relativePath)!;

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            await File.WriteAllBytesAsync(fullPath, bytes);

            return (relativePath, null);
        }

        public string? GetFullPath(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            var root = Path.GetFullPath(_photosRoot);
            var full = Path.GetFullPath(Path.Combine(root, relativePath));

            // Never allow a path outside the photos folder
            return full.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? full : null;
        }

        public void DeleteFile(string? relativePath)
        {
            var full = GetFullPath(relativePath);
            if (full != null && File.Exists(full))
                File.Delete(full);
        }

        private static string? DetectImageExtension(byte[] b)
        {
            if (b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF)
                return ".jpg";
            if (b.Length >= 8 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47
                && b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A)
                return ".png";
            return null;
        }
    }
}
