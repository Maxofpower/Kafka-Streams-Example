using System;
using Newtonsoft.Json.Linq;
using Streamiz.Kafka.Net.Processors.Public;
using Streamiz.Kafka.Net.State;
using Streamiz.Kafka.Net.Stream;
using static OrderTransformation.Program;

namespace OrderTransformation
{
	public class OrderProcessorTransformer : ITransformer<string, MappedEventValue, string, MappedEventValue>
	{
		private ProcessorContext<string, MappedEventValue> _context;
		private IWindowStore<string, MappedEventValue> _orderStore;
		private readonly string _storeName;
		private readonly double _leftDurationMs;
		private readonly double _rightDurationMs;

		public OrderProcessorTransformer() { }

		public OrderProcessorTransformer(string storeName, double maintainDurationPerEventInMs)
		{
			if (maintainDurationPerEventInMs < 1)
			{
				throw new ArgumentException("Maintain duration per event must be >= 1");
			}
			_leftDurationMs = maintainDurationPerEventInMs / 2;
			_rightDurationMs = maintainDurationPerEventInMs - _leftDurationMs;
			this._storeName = storeName;
		}

		public void Init(ProcessorContext<string, MappedEventValue> context)
		{
			_context = context;
		//	_orderStore = (IWindowStore<string, MappedEventValue>)context.GetStateStore(_storeName);
		}

		public Record<string, MappedEventValue> Process(Record<string, MappedEventValue> record)
		{
			if (record.Key == null || record.Value == null)
			{
				return record;
			}
			Console.WriteLine($"Processing order: {record.Value.Value}");
			// Change status of order to "processed"
			var order = record.Value;
			order.EventId = "processed_" + order.EventId; // Modify as needed

			// Transform the value (example transformation)
			var transformedValue = $"Processed-{record.Value.Value}";

			// Log the transformed value
			Console.WriteLine($"Transformed order: {transformedValue}");
			// Store the processed order in the state store

			//PutOrderInStore(order.FinalKey, order, _context.Timestamp); // it has to be configured before using

			return record; // Return the modified order
		}

		private void PutOrderInStore(string eventKey, MappedEventValue value, long timestamp)
		{
			_orderStore.Put(eventKey, value, timestamp);
		}

		public void Close()
		{
			// Any cleanup if necessary
		}
	}
}
