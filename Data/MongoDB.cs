using System;
using Api.Data.Collections;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Api_mongo_dio.Data
{
    public class MongoDB
    {
        public IMongoDatabase DB { get; }

        public MongoDB(IConfiguration configuration)
        {
            try
            {
                // Conexão com o banco
                var settings = MongoClientSettings.FromUrl(new MongoUrl(configuration["ConnectionString"]));
                
                // Criando um client
                var client = new MongoClient(settings);
                
                // Startando o banco
                DB = client.GetDatabase(configuration["NomeBanco"]);
                MapClasses();
            }
            catch (Exception ex)
            {
                throw new MongoException("It was not possible to connect to MongoDB", ex);
            }
        }

        private void MapClasses() // Criando os mappings para o Mongo, com base na nosso classe
        // Obs.: Pode ser usado com dataAnnotations
        {
            // Configurando para criar os objetos com camelCase
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            // Se não tiver nenhuma classe mapeada para nossa class infectado, então o mapei
            if (!BsonClassMap.IsClassMapRegistered(typeof(Infectado))) 
            {
                BsonClassMap.RegisterClassMap<Infectado>(i =>
                {
                    i.AutoMap(); // Todas as propriedades da nossa classe, podem ter o mesmo nome e tipo no banco
                    i.SetIgnoreExtraElements(true); // Ignore elementos extras, para não para a aplicação
                });
            }
        }
    }
}