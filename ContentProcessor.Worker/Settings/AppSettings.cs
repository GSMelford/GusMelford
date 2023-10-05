namespace ContentProcessor.Worker.Settings;

public class AppSettings
{
    public FeatureSettings FeatureSettings { get; set; } = new ();
    public KafkaSettings KafkaSettings { get; set; } = new ();
}