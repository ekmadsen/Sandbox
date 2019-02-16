using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace ErikTheCoder.Sandbox.AsyncParallelPerformance
{
    public static class Program
    {
        private const string _originalUrl = "Original URL";
        private const int _bufferSize = 65536;
        private const int _urlPreviewLength = 50;
        private const int _progressBarWidth = 50;
        private const int _progressIntervalMSec = 100;
        private static Dictionary<string, WebPageDownload> _webPageDownloads;
        private static string _directory;
        private static int _downloadedHeaders;
        private static int _downloadedBodies;
        private static long _totalDownloadedBytes;



        // Requires C# 7.1.  Enable via Project Properties > Build > Advanced > Language Version > C# Latest Minor Version.
        // See https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7-1#async-main/
        public static async Task<int> Main([UsedImplicitly] string[] Args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine();
            _directory = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? string.Empty;
            // Build collection of web pages to download.
            List<string> urls =  new List<string>
            {
                "https://www.cs.cornell.edu/courses/cs5430/2013sp/TL04.asymmetric.html",
                "https://en.wikipedia.org/wiki/Transport_Layer_Security",
                "https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.waitall",
                "https://en.wikipedia.org/wiki/Futures_and_promises",
                "https://www.wallpaperup.com/uploads/wallpapers/2014/01/15/228439/9c3928b843a01ec6d3b796583a707704.jpg",
                "https://github.com/paulcbetts/refit",
                "https://www.doc.ic.ac.uk/~susan/475/HowToBeAProgrammer.pdf",
                "http://commonmark.org/help/",
                "https://docs.microsoft.com/en-us/dotnet/core/get-started",
                "https://docs.microsoft.com/en-us/dotnet/core/tutorials/with-visual-studio",
                "https://www.wallpaperup.com/uploads/wallpapers/2013/12/16/197406/54df26048323f74b219da0dca522e1cb.jpg",
                "http://data.consumerfinance.gov/api/views.xml",
                "https://studentaid.ed.gov/sa/sites/default/files/fsawg/datacenter/library/PortfolioSummary.xls",
                "http://www.acq.osd.mil/eie/Downloads/DISDI/installations_ranges.zip",
                "http://www.irs.gov/statistics/soi-tax-stats-individual-income-tax-statistics-zip-code-data-soi"
            };
            _webPageDownloads = new Dictionary<string, WebPageDownload>();
            foreach (string url in urls) _webPageDownloads[url] = new WebPageDownload(url);
            // Create sequential directory or purge files.
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(_directory, "Sequential"));
            Console.Write($"Purging files from {directory} directory...  ");
            if (!directory.Exists) directory.Create();
            foreach (FileInfo file in directory.EnumerateFiles()) file.Delete();
            Console.WriteLine("completed.");
            Console.WriteLine();
            // Download web pages sequentially.
            _totalDownloadedBytes = 0L;
            Console.WriteLine($"Downloading {_webPageDownloads.Count} web pages sequentially.");
            Console.WriteLine();
            Stopwatch stopwatch = Stopwatch.StartNew();
            DownloadSequentially(directory);
            stopwatch.Stop();
            Console.WriteLine($"Downloads completed in {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
            double throughput = 8d * _totalDownloadedBytes / (stopwatch.Elapsed.TotalSeconds * 1024d * 1024d);
            Console.WriteLine($"Throughput = {throughput:0.000} Mbps.");
            Console.WriteLine();
            Console.WriteLine();
            // Create concurrent directory or purge files.
            directory = new DirectoryInfo(Path.Combine(_directory, "Concurrent"));
            Console.Write($"Purging files from {directory} directory...  ");
            if (!directory.Exists) directory.Create();
            foreach (FileInfo file in directory.EnumerateFiles()) file.Delete();
            Console.WriteLine("completed.");
            Console.WriteLine();
            // Download web pages concurrently.
            _totalDownloadedBytes = 0L;
            Console.WriteLine($"Downloading {_webPageDownloads.Count} web pages concurrently.");
            Console.WriteLine();
            stopwatch.Restart();
            await DownloadConcurrentlyAsync(directory);
            stopwatch.Stop();
            Console.WriteLine($"Downloads completed in {stopwatch.Elapsed.TotalSeconds:0.000} seconds.");
            throughput = 8d * _totalDownloadedBytes / (stopwatch.Elapsed.TotalSeconds * 1024d * 1024d);
            Console.WriteLine($"Throughput = {throughput:0.000} Mbps.");
            Console.WriteLine();
            return 0;
        }


        private static void DownloadSequentially(FileSystemInfo Directory)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                foreach (WebPageDownload webPageDownload in _webPageDownloads.Values)
                {
                    string cacheBustingUrl = GetCacheBustingUrl(webPageDownload.Url);
                    Console.Write($"Downloading {webPageDownload.Url} web page...  ");
                    // Get website response headers.
                    using (HttpResponseMessage response = httpClient.GetAsync(cacheBustingUrl, HttpCompletionOption.ResponseHeadersRead).Result)
                    {
                        string filename = Path.Combine(Directory.FullName, SanitizeFilename(webPageDownload.Url));
                        using (Stream responseStream = response.Content.ReadAsStreamAsync().Result)
                        {
                            // Stream response body from web server directly to file.
                            using (Stream fileStream = File.OpenWrite(filename))
                            {
                                responseStream.CopyTo(fileStream);
                                // Increment downloaded bytes.
                                _totalDownloadedBytes += fileStream.Position;
                                Console.WriteLine("completed.");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
        }


        private static async Task DownloadConcurrentlyAsync(FileSystemInfo Directory)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Monitor status.
                Task monitorStatus = MonitorStatusAsync();
                // Download website response headers asynchronously.
                Dictionary<string, Task<HttpResponseMessage>> headerDownloads = new Dictionary<string, Task<HttpResponseMessage>>(_webPageDownloads.Count);
                foreach (WebPageDownload webPageDownload in _webPageDownloads.Values)
                {
                    string cacheBustingUrl = GetCacheBustingUrl(webPageDownload.Url);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, cacheBustingUrl);
                    request.Properties[_originalUrl] = webPageDownload.Url;
                    headerDownloads[webPageDownload.Url] = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                }
                // Monitor header downloads.
                Dictionary<string, Task<HttpResponseMessage>> bodyDownloads = new Dictionary<string, Task<HttpResponseMessage>>(_webPageDownloads.Count);
                while (headerDownloads.Count > 0)
                {
                    // As web servers respond with header data, download the HTTP body asynchronously.
                    Task<HttpResponseMessage> task = await Task.WhenAny(headerDownloads.Values);
                    if (task.IsFaulted)
                    {
                        // Display error message and halt program execution.
                        Console.WriteLine(task.Exception.GetSummary());
                        Environment.Exit(-1);
                    }
                    // Remove task from header downloads collection.
                    string url = task.Result.RequestMessage.Properties[_originalUrl].ToString();
                    headerDownloads.Remove(url);
                    // Update status.
                    WebPageDownload webPageDownload = _webPageDownloads[url];
                    webPageDownload.Bytes = task.Result.Content.Headers.ContentLength;
                    // Update downloaded header count in a thread-safe manner.
                    Interlocked.Increment(ref _downloadedHeaders);
                    // Download the HTTP body asynchronously.
                    bodyDownloads[url] = DownloadWebPageAsync(webPageDownload, Directory, task.Result);
                }
                // Monitor body downloads.
                while (bodyDownloads.Count > 0)
                {
                    // As web servers respond with body data, update status.
                    Task<HttpResponseMessage> task = await Task.WhenAny(bodyDownloads.Values);
                    if (task.IsFaulted)
                    {
                        // Display error message and halt program execution.
                        Console.WriteLine(task.Exception.GetSummary());
                        Environment.Exit(-1);
                    }
                    // Remove task from body downloads collection.
                    string url = task.Result.RequestMessage.Properties[_originalUrl].ToString();
                    bodyDownloads.Remove(url);
                    // Update status.
                    WebPageDownload webPageDownload = _webPageDownloads[url];
                    webPageDownload.BodyDownloaded = true;
                    // Update downloaded body count in a thread-safe manner.
                    Interlocked.Increment(ref _downloadedBodies);
                    if (webPageDownload.Bytes.HasValue)
                    {
                        // Validate correct number of bytes were downloaded.
                        if (webPageDownload.DownloadedBytes != webPageDownload.Bytes)
                        {
                            // Display error message and halt program execution.
                            Console.WriteLine($"Downloaded byte count ({webPageDownload.BodyDownloaded}) does not match Content-Length header ({webPageDownload.Bytes}). ");
                            Environment.Exit(-1);
                        }
                    }
                }
                // Await final status update.
                await monitorStatus;
            }
        }


        private static async Task MonitorStatusAsync()
        {
            int column = Console.CursorLeft;
            int row = Console.CursorTop;
            while (_downloadedBodies < _webPageDownloads.Count)
            {
                UpdateStatus(column, row);
                await Task.Delay(TimeSpan.FromMilliseconds(_progressIntervalMSec));
            }
            UpdateStatus(column, row);
        }


        private static void UpdateStatus(int Column, int Row)
        {
            Console.SetCursorPosition(Column, Row);
            Console.WriteLine($"Downloaded {_downloadedHeaders} of {_webPageDownloads.Count} web page headers.");
            Console.WriteLine();
            int webPageNumber = 1;
            foreach (WebPageDownload webPageDownload in _webPageDownloads.Values)
            {
                string urlPreview = webPageDownload.Url.Length > _urlPreviewLength
                    ? webPageDownload.Url.Substring(0, _urlPreviewLength).PadRight(_urlPreviewLength)
                    : webPageDownload.Url.PadRight(_urlPreviewLength);
                string downloadedBytes = webPageDownload.DownloadedBytes.ToString("#,#").PadLeft(11);
                string bytes = (webPageDownload.Bytes?.ToString("#,#") ?? "?").PadLeft(11);
                double percentComplete = webPageDownload.BodyDownloaded
                    ? 1d
                    : webPageDownload.Bytes.HasValue
                        ? (double)webPageDownload.DownloadedBytes / webPageDownload.Bytes.Value
                        : 0d;
                string percentCompleteFormatted = (100 * percentComplete).ToString("0").PadLeft(3);
                string downloadProgress = new string('\u2588', (int)(percentComplete * _progressBarWidth)).PadRight(_progressBarWidth, '\u2591');
                string complete = webPageDownload.BodyDownloaded ? "  (Completed)" : string.Empty;
                Console.WriteLine($"{webPageNumber:00}  {urlPreview}  {percentCompleteFormatted}%  {downloadProgress}  {downloadedBytes} of {bytes} bytes{complete}");
                webPageNumber++;
            }
            Console.WriteLine();
            Console.WriteLine($"Downloaded {_downloadedBodies} of {_webPageDownloads.Count} web page bodies.");
            Console.WriteLine();
        }


        private static string GetCacheBustingUrl(string Url)
        {
            char separator = Url.IndexOf('?') > -1 ? '&' : '?';
            return $"{Url}{separator}param={Guid.NewGuid()}";
        }


        private static string SanitizeFilename(string Filename)
        {
            string filename = Filename.Replace("https://", null, StringComparison.CurrentCultureIgnoreCase);
            filename = filename.Replace("http://", null, StringComparison.CurrentCultureIgnoreCase);
            filename = filename.Replace("/", "-", StringComparison.CurrentCultureIgnoreCase);
            filename = filename.Replace("?", "-", StringComparison.CurrentCultureIgnoreCase);
            if (!filename.EndsWith(".html", StringComparison.CurrentCultureIgnoreCase) &&
                !filename.EndsWith(".htm", StringComparison.CurrentCultureIgnoreCase) &&
                !filename.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase) &&
                !filename.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) &&
                !filename.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase))
            {
                filename = $"{filename}.html";
            }
            return filename;
        }


        private static async Task<HttpResponseMessage> DownloadWebPageAsync(WebPageDownload WebPageDownload, FileSystemInfo Directory, HttpResponseMessage HeaderResponse)
        {
            string url = HeaderResponse.RequestMessage.Properties[_originalUrl].ToString();
            string filename = Path.Combine(Directory.FullName, SanitizeFilename(url));
            using (Stream responseStream = await HeaderResponse.Content.ReadAsStreamAsync())
            {
                // Stream response body from web server directly to file.
                byte[] buffer = new byte[_bufferSize];
                using (Stream fileStream = File.OpenWrite(filename))
                {
                    int bytesRead;
                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    //while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        //fileStream.Write(buffer, 0, bytesRead);
                        WebPageDownload.DownloadedBytes += bytesRead;
                        // Increment total downloaded bytes in a thread-safe manner.
                        Interlocked.Add(ref _totalDownloadedBytes, bytesRead);
                    }
                }
            }
            return HeaderResponse;
        }
    }
}
