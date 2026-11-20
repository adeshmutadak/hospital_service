using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.PhotoUpload
{
    public interface IFileService
    {
        Task<string?> SaveBase64ImageAsync(string base64Image, string folderPath);

        // Validates and saves a base64 JPG/PNG under PhotosRoot/<subFolder>.
        // Returns the relative path (e.g. "Patients/abc.jpg") or an error message.
        Task<(string? RelativePath, string? Error)> SaveBase64PhotoAsync(string base64Image, string subFolder);

        string? GetFullPath(string? relativePath);
        void DeleteFile(string? relativePath);
    }
}
