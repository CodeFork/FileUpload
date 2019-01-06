﻿using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class UploadViewModel
    {
        public string Url { get; }
        public string DownloadUrl { get; }

        public bool IsDownloadEnabled { get; set; }

        public UploadViewModel(string url, string downloadUrl, bool isDownloadEnabled)
        {
            Ensure.NotNull(url, "url");
            Ensure.NotNull(downloadUrl, "downloadUrl");
            Url = url;
            DownloadUrl = downloadUrl;
            IsDownloadEnabled = isDownloadEnabled;
        }
    }
}
