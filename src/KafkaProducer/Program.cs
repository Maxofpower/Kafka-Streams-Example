using Confluent.Kafka;
using Newtonsoft.Json;
using System;

namespace KafkaProducer
{
	class Program
	{
		public static void Main(string[] args)
		{
			var config = new ProducerConfig
			{
				BootstrapServers = "localhost:9092"
			};

			var producer = new ProducerBuilder<Null, string>(config).Build();

			for (int i = 0; i < 10; i++)
			{
				var orderEvent = new MappedEventValue
				{
					FinalKey = $"Order-{i}",
					EventId = Guid.NewGuid().ToString(),
					Value = i
				};

				var orderEventJson = JsonConvert.SerializeObject(orderEvent);

				producer.Produce("order-topic", new Message<Null, string> { Value = orderEventJson });
				Console.WriteLine($"Produced: {orderEventJson}");
			}

			// Wait for any outstanding messages to be delivered
			producer.Flush(TimeSpan.FromSeconds(10));
		}
	}

	public class MappedEventValue
	{
		public string FinalKey { get; set; }
		public string EventId { get; set; }
		public long Value { get; set; }
	}
}
