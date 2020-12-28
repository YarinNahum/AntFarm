using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ProducerConsumer
{
    public class ProducerConsumerMessages : IProducerConsumerMessages<string>
    {
        private BlockingCollection<string> messages;
        private Thread consumer;
        private bool working;
        private CancellationTokenSource cts;

        public ProducerConsumerMessages()
        {
            messages = new BlockingCollection<string>();
            consumer = new Thread(Consume);
            cts = new CancellationTokenSource();
            consumer.Start();
            working = true;
        }
        public void ConsumeAll()
        {
            cts.Cancel();
            foreach (string msg in messages)
                Console.WriteLine(msg);
            consumer.Join();
        }

        public void Produce(string message)
        {
            if (working)
                messages.Add(message);
        }

        public void Consume()
        {
            while (working)
            {
                try
                {
                    string msg = messages.Take(cts.Token);
                    Console.WriteLine(msg);
                }catch (OperationCanceledException _)
                {
                    working = false;
                }
            }
        }
    }
}
