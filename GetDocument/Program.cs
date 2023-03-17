using Application.Interfaces;
using Application.Services;
using Azure.Storage.Blobs;
using Confluent.Kafka;
using Domain.Models;
using GetDocument;
using Kafka.Consumer;
using Kafka.Interfaces;
using Kafka.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

Host.CreateDefaultBuilder(args)
   .ConfigureServices((context, services) =>
   {
       // Register Azure Blob Container Client
       services.AddSingleton(x => new BlobContainerClient(config["BlobConnectionString"], config["BlobContainerName"]));
       services.AddTransient(x => new BlobServiceClient(config["BlobConnectionString"]));

       var clientConfig = new ClientConfig()
       {
           BootstrapServers = config["KafkaConsumerConfig:bootstrapservers"],
           SaslUsername = config["KafkaConsumerConfig:SaslUsername"],
           SaslPassword = config["KafkaConsumerConfig:SaslPassword"],
           SecurityProtocol = SecurityProtocol.SaslSsl,
           SaslMechanism = SaslMechanism.Plain,
           //Acks = Acks.All
       };

       var consumerConfig = new ConsumerConfig(clientConfig)
       {
           GroupId = config["KafkaConsumerConfig:GroupId"],
           //EnableAutoOffsetStore = false,
           EnableAutoCommit = true,
           AutoOffsetReset = AutoOffsetReset.Earliest,
           StatisticsIntervalMs = 5000,
           SessionTimeoutMs = 6000,
           //EnablePartitionEof = true,
           //PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
       };

       var producerConfig = new ProducerConfig(clientConfig)
       {
           Acks = Acks.All  // Best practice for Kafka producer to prevent data loss
       };


       services.AddSingleton(producerConfig);
       services.AddSingleton(typeof(IKafkaProducer<,>), typeof(KafkaProducer<,>));

       services.AddSingleton(consumerConfig);
       services.AddSingleton(typeof(IKafkaConsumer<,>), typeof(KafkaConsumer<,>));

       services.AddScoped<IKafkaHandler<string, GetDocumentInbound>, MessageHandler>();
       services.AddHostedService<MessageConsumer>();

       // Add services to the container.
       services.AddTransient<IAzureStorage, AzureStorage>();
     
   })
    .Build()
    .Run();
