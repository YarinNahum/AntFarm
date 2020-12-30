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
        /// Will try to consume all the messages in the queue. 
        /// blocks the thread when the queue is empty
        /// </summary>
        void Consume();
    }
}
