using System.Threading.Tasks;
using System.Text;
using NATS.Client;
using Lib;

namespace Lib
{
    public class NatsPublisher : IPublisher
    {
        public void Publish(string subjectName, string data)
        {
            Task.Factory.StartNew(() => Produce(subjectName, Encoding.UTF8.GetBytes(data)));
        }

        private void Produce(string subjectName, byte[] data)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection connection = cf.CreateConnection())
            {
                connection.Publish(subjectName, data);
                connection.Drain();
            }
        }
    }
}