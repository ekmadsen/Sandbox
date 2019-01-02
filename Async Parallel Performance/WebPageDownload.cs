namespace ErikTheCoder.Sandbox.AsyncParallelPerformance
{
    public class WebPageDownload
    {
        public string Url { get; }
        public long? Bytes { get; set; }
        public long DownloadedBytes { get; set; }
        public bool BodyDownloaded { get; set; }


        public WebPageDownload(string Url)
        {
            this.Url = Url;
        }
    }
}
