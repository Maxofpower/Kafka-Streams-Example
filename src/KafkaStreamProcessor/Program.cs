using System;
using System.Threading;
using System.Threading.Tasks;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;


using Streamiz.Kafka.Net.Processors.Public;
using Streamiz.Kafka.Net.State;

namespace OrderTransformation
{
	public static class Program
	{
		static readonly string INPUT_TOPIC = "order-topic";
		static readonly string OUTPUT_TOPIC = "processed-order-topic";
		public class MappedEventValue
		{
			public string FinalKey { get; set; }
			public string EventId { get; set; }
			public long Value { get; set; }
		}

		static string GetEnvironmentVariable(string var, string @default)
		{
			return Environment.GetEnvironmentVariable(var) ?? @default;
		}

		static async Task Main(string[] args)
		{
			CancellationTokenSource source = new CancellationTokenSource();
			string bootstrapServer = GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVER", "localhost:9092");

			var config = new StreamConfig<StringSerDes, StringSerDes>
			{
				ApplicationId = "order-processor-app",
				ClientId = "order-processor-client",
				BootstrapServers = bootstrapServer,
				AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest,
				CommitIntervalMs = 10 * 1000
			};

			Topology topology = GetTopology();
			KafkaStream stream = new(topology, config);

			Console.CancelKeyPress += (o, e) =>
			{
				source.Cancel();
			};

			await stream.StartAsync(source.Token);
		}

		static Topology GetTopology()
		{
			var builder = new StreamBuilder();

			string storeName = "order-store";
			TimeSpan windowSize = TimeSpan.FromMinutes(5); // You can adjust this based on your needs

			// Create a state store for the order processing
			IStoreBuilder dedupStoreBuilder = Stores.WindowStoreBuilder(
				Stores.InMemoryWindowStore(
					storeName,
					windowSize,
					windowSize
				),
				new StringSerDes(),
				new JsonSerDes<MappedEventValue>()
			);

			// Define the stream processing topology
			builder.Stream(INPUT_TOPIC, new StringSerDes(), new JsonSerDes<MappedEventValue>())
			   .SelectKey((k, v) => v.FinalKey)
			   .Repartition()  // Repartition if necessary
				.Transform(new TransformerBuilder<string, MappedEventValue, string, MappedEventValue>()
					//.StateStore(dedupStoreBuilder)  // Attach the state store
					.Transformer<OrderProcessorTransformer>()  // Attach the custom transformer for processing orders
					.Build())
				.To(OUTPUT_TOPIC, new StringSerDes(), new JsonSerDes<MappedEventValue>());  // Publish to the output topic

			// Return the constructed topology
			return builder.Build();
		}
	}
}
