﻿using Confluent.Kafka;

namespace Nike.EventBus.Kafka
{
    public class KafkaConsumerConnection : IKafkaConsumerConnection
    {
        public KafkaConsumerConnection(string brokers, string groupId)
        {
            MillisecondsTimeout = MillisecondsTimeout == 0 ? 1000 : MillisecondsTimeout;

            Config = new ConsumerConfig
            {
                BootstrapServers = brokers,
                GroupId = groupId,
                StatisticsIntervalMs = StatisticsIntervalMs == 0 ? 1000 : StatisticsIntervalMs,
                SessionTimeoutMs = SessionTimeoutMs == 0 ? 6000 : SessionTimeoutMs,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false,
                AllowAutoCreateTopics = false,
                EnablePartitionEof = true,
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin
            };

            IsConnected = true;
        }

        public bool IsConnected { get; }
        public int MillisecondsTimeout { get; set; }
        public ConsumerConfig Config { get; }
        public int StatisticsIntervalMs { get; set; }
        public int SessionTimeoutMs { get; set; }
    }
}