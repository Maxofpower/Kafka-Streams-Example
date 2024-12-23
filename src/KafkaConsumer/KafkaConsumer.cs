using Confluent.Kafka;
using System;
using System.Threading;

namespace KafkaConsumer
{
	class Program
	{
		public static void Main(string[] args)
		{
			var config = new ConsumerConfig
			{
				BootstrapServers = "localhost:9092",  // Kafka server address
				GroupId = "order-group-consumer",     // Consumer group ID
				AutoOffsetReset = AutoOffsetReset.Earliest  // Start consuming from the earliest message
			};

			using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
			{
				consumer.Subscribe("processed-order-topic");  // Subscribe to the topic

				CancellationTokenSource cts = new CancellationTokenSource();
				Console.CancelKeyPress += (_, e) =>
				{
					e.Cancel = true;
					cts.Cancel();  // Handle cancellation on Ctrl+C
				};

				try
				{
					while (!cts.Token.IsCancellationRequested)
					{
						try
						{
							// Consume the next message
							var consumeResult = consumer.Consume(cts.Token);
							Console.WriteLine($"Consumed message: {consumeResult.Message.Value}");

							// Simulate further processing here (e.g., logging, transforming data)
							var finalProcessedMessage = $"Final Processing: {consumeResult.Message.Value}";
							Console.WriteLine(finalProcessedMessage);  // Final output of processed message
						}
						catch (ConsumeException e)
						{
							Console.WriteLine($"Error consuming message: {e.Error.Reason}");
						}
					}
				}
				catch (OperationCanceledException)
				{
					Console.WriteLine("Consuming was canceled.");
				}
				finally
				{
					consumer.Close();  // Gracefully close the consumer
				}
			}
		}
	}
}
