using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer
{
    public interface IProducerConsumerMessages <T>
    {
        /// <summary>
        /// Produce a message in the queue
        /// </summary>
        /// <param name="message"></param>
        void Produce(T message);

        /// <summary>
        /// Consume all the messages in the queue and stop the producer-consumer
        /// </summary>
        void ConsumeAll();

        /// <summary>
        /// Consume a single message
        /// </summary>
        void Consume();
    }
}
