namespace UrlShorter.Models
{
    public class UrlManager
    {
        public int Id {  get; set; }
        public string Url { get; set; } = "";
        public string ShortUrl { get; set; } = "";
    }
}
