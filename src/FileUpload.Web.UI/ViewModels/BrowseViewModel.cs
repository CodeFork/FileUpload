﻿using FileUpload.Models;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class BrowseViewModel
    {
        private readonly string downloadUrl;

        public IReadOnlyList<FileModel> Files { get; }

        public BrowseViewModel(IReadOnlyList<FileModel> files, string downloadUrl)
        {
            Ensure.NotNull(files, "files");
            Ensure.NotNull(downloadUrl, "downloadUrl");
            Files = files;
            this.downloadUrl = downloadUrl;
        }

        public string GetFileUrl(FileModel file)
        {
            if (downloadUrl.EndsWith('/'))
                return downloadUrl + file.Name;
            else
                return downloadUrl + '/' + file.Name;
        }
    }
}
