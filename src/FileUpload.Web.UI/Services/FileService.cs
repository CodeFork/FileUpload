﻿using FileUpload.Models;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Services
{
    public class FileService
    {
        public IReadOnlyList<FileModel> FindList(UploadSettings configuration)
        {
            Ensure.NotNull(configuration, "configuration");

            if (!configuration.IsBrowserEnabled)
                return null;

            List<FileModel> files = Directory
                .EnumerateFiles(configuration.StoragePath)
                .Where(f => configuration.SupportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(f => new FileModel(Path.GetFileName(f), new FileInfo(f).Length))
                .ToList();

            return files;
        }

        public (FileStream Content, string ContentType)? FindContent(UploadSettings configuration, string fileName, string extension)
        {
            Ensure.NotNull(configuration, "configuration");

            if (!configuration.IsDownloadEnabled)
                return null;

            if (extension == null)
                return null;

            extension = "." + extension;
            fileName = fileName + extension;
            if (fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar) || fileName.Contains("..") || Path.IsPathRooted(fileName))
                return null;

            extension = extension.ToLowerInvariant();
            if (!configuration.SupportedExtensions.Contains(extension))
                return null;

            string filePath = Path.Combine(configuration.StoragePath, fileName);
            if (File.Exists(filePath))
            {
                string contentType = "application/octet-stream";
                if (extension == ".gif")
                    contentType = "image/gif";
                else if (extension == ".png")
                    contentType = "image/png";
                else if (extension == ".jpg" || extension == ".jpeg")
                    contentType = "image/jpg";

                return (new FileStream(filePath, FileMode.Open), contentType);
            }

            return null;
        }

        public async Task<bool> SaveAsync(UploadSettings configuration, string name, long length, Stream content)
        {
            Ensure.NotNull(configuration, "configuration");

            if (length > configuration.MaxLength)
                return false;

            string extension = Path.GetExtension(name);
            if (extension == null)
                return false;

            extension = extension.ToLowerInvariant();
            if (!configuration.SupportedExtensions.Contains(extension))
                return false;

            DirectoryInfo directory = new DirectoryInfo(configuration.StoragePath);
            if (configuration.MaxStorageLength != null && directory.GetLength() + length > configuration.MaxStorageLength.Value)
                return false;

            if (!directory.Exists)
                directory.Create();

            string filePath = Path.Combine(configuration.StoragePath, name);
            if (File.Exists(filePath))
            {
                if (configuration.IsOverrideEnabled)
                    File.Delete(filePath);
                else
                    return false;
            }

            using (Stream fileContent = new FileStream(filePath, FileMode.OpenOrCreate))
                await content.CopyToAsync(fileContent);

            return true;
        }
    }
}
