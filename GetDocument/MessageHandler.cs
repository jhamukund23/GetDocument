using Kafka.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Application.Interfaces;
using Application.Constants;
using Application.Services;
using Confluent.Kafka;
using Kafka.Producer;

namespace GetDocument
{
    public class MessageHandler : IKafkaHandler<string, GetDocumentInbound>
    {
        private readonly IKafkaProducer<string, GetDocumentOutbound> _outboundProducer;
        private readonly IKafkaProducer<string, GetDocumentError> _errorOutboundProducer;
        private readonly IAzureStorage _storage;
        public MessageHandler(
             IKafkaProducer<string, GetDocumentOutbound> outboundProducer,
             IKafkaProducer<string, GetDocumentError> errorOutboundProducer,
             IAzureStorage storage
            )
        {
            _storage = storage;
            _outboundProducer = outboundProducer;
            _errorOutboundProducer = errorOutboundProducer;
        }
        public Task HandleAsync(string key, GetDocumentInbound value)
        {
            try
            {
                // Here we can actually write the code to call microservices
                Console.WriteLine($"Consuming topic message with the below data\n CorrelationId: {value.CorrelationId}\n FileName: {value.FileName}\n FileSize: {value.FileSize}");

                var blob = _storage.GetBlobAsync(value.FileName);
                if (blob != null)
                { }

                // Get the SAS URI.
                Uri? sasUrl = _storage.GetServiceSasUriForContainer();

                // Send correlation id and sasUrl to Kafka response topic.
                ProduceAddDocumentOutbound(value.CorrelationId, sasUrl);


            }
            catch (Exception ex)
            {
                // Send correlation id and error message to Kafka error response topic.
                ProduceAddDocumentError(value.CorrelationId, ex.Message);
            }
            return Task.CompletedTask;
        }

        #region Private Method      
        private void ProduceAddDocumentOutbound(Guid correlationId, Uri? sasUrl)
        {
            GetDocumentOutbound getDocumentOutbound = new()
            {
                SasUrl = sasUrl,
                CorrelationId = correlationId
            };
            var topicPart = new TopicPartition(KafkaTopics.GetDocumentOutbound, new Partition(1));
            _outboundProducer.ProduceAsync(topicPart, Convert.ToString(correlationId), getDocumentOutbound);
        }
        private void ProduceAddDocumentError(Guid correlationId, string ex)
        {
            GetDocumentError getDocumentError = new()
            {
                CorrelationId = correlationId,
                Error = ex
            };
            var topicPart = new TopicPartition(KafkaTopics.GetDocumentError, new Partition(1));
            _errorOutboundProducer.ProduceAsync(topicPart, Convert.ToString(correlationId), getDocumentError);
        }
        #endregion
    }


}
