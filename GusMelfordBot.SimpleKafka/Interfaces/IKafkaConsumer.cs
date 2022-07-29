﻿using GusMelfordBot.SimpleKafka.Models;

namespace GusMelfordBot.SimpleKafka.Interfaces;

public interface IKafkaConsumer : IDisposable
{
    event EventHandler<ReceivedEventArgs> Received;
    public void Consume(string topic, string groupId, bool enableAutoCommit);
}