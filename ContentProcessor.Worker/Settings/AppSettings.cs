namespace ContentProcessor.Worker.Settings;

public class AppSettings
{
    public KafkaSettings KafkaSettings { get; set; } = new ();
}