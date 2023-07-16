using RabbitMQ.Client;

namespace UdemyRabbitMQWeb.Watermark.Services
{
    public class RabbitMQClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";
        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ILogger<RabbitMQClientService> logger, ConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            Connect();
        }
        
        public IModel Connect()
        {
            _connection= _connectionFactory.CreateConnection();

            if(_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true, false);

            _channel.QueueDeclare(QueueName, true, false, false, null);

            _channel.QueueBind(QueueName, ExchangeName, RoutingWatermark);

            _logger.LogInformation("RabbitMQ is connected!");

            return _channel;


        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();            
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ is DISCONNECTED!");
        }
    }
}
