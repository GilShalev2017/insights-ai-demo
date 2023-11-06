namespace FRServer.Models
{
    public class VideoDetails
    {
        public TimeSpan Duration { get; set; }
        public string Resolution { get;  }
        public double Framerate { get; }
        public long Size { get; }
        public string Codec { get; }
        public VideoDetails(TimeSpan duration, string resolution, double framerate, long size, string codec)
        {
            Duration = duration;
            Resolution = resolution;
            Framerate = framerate;
            Size = size;
            Codec = codec;
        }
    }
}
