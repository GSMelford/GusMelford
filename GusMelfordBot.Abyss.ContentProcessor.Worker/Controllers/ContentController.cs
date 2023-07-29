using System.Threading.Tasks;
using GusMelfordBot.Events;
using Kyoto.Kafka.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContentProcessor.Worker.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController : Controller
{
    private readonly IKafkaProducer<string> _kafkaProducer;

    public ContentController(IKafkaProducer<string> kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    [HttpPost]
    public async Task Post(ContentEvent contentEvent)
    {
        await _kafkaProducer.ProduceAsync(contentEvent, string.Empty);
    }
}