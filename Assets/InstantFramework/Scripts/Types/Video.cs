namespace TurboLabz.InstantFramework
{
    public class Video
    {
        public string videoId;
        public float? progress;

        public Video(string videoId, float? progress)
        {
            this.videoId = videoId;
            this.progress = progress;
        }
    }
}

