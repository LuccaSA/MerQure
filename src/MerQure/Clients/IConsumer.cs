using System;

namespace MerQure
{
	/// <summary>
	/// Consumer interface. Used to receive messages from a queue by subscription.
	/// </summary>
	public interface IConsumer
    {
		/// <summary>
		/// name of the queue subscribed by the consumer
		/// </summary>
		string QueueName { get; }

		/// <summary>
		/// Start listening on the queue
		/// </summary>
		/// <param name="OnMessageReceived">Handler called each time a message arrives for this consumer.</param>
		void Consume(EventHandler<IMessagingEvent> OnMessageReceived);

		/// <summary>
		/// Acknowledge a delivered message.
		/// </summary>
		/// <param name="args"></param>
		void AcknowlegdeDeliveredMessage(IMessagingEvent args);

		/// <summary>
		/// Reject a delivered message.
		/// </summary>
		/// <param name="args"></param>
		void RejectDeliveredMessage(IMessagingEvent args);
    }
}
