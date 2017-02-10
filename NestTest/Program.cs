using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NestTest
{
    class Program
    {
        static ElasticClient _client;
        const string Index = "importrecords";
        static void Main(string[] args)
        {
            var node = new Uri("http://192.168.210.36:9200/");
            var settings = new ConnectionSettings(
                node
            );

            settings.Proxy(new Uri("http://127.0.0.1:8888"), "", "");
            _client = new ElasticClient(settings);
            //CreateIndex();
            //Insert();


            var result = _client.Search<Media.Net.Entity.EPaperInEsEntity>(s => s
                .Index(Index)
                .Type("data")
                .Take(10000)
                //.Query(q => q.QueryString(qs => qs.Query("文舟琬胤海修").Fields("Author").DefaultOperator(Operator.And)))
                .Query(q => q.MatchPhrase(mq => mq.Field("Title").Query("大 众  全国").Slop(1)))

            );
            Debug.Flush();
            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject("用时:" + result.Took + "ms"));
            foreach (var entity in result.Documents)
            {
                Debug.WriteLine( entity.Title);
                //Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(entity));
            }
        }

        private static void CreateIndex()
        {

            var exists = _client.IndexExists(Index);

            //基本配置  
            IIndexState indexState = new IndexState()
            {
                Settings = new IndexSettings()
                {
                    NumberOfReplicas = 1,//副本数  
                    NumberOfShards = 5//分片数  
                }
            };
            if (!exists.Exists)
                _client.CreateIndex(Index, p => p
        .InitializeUsing(indexState)
        .Mappings(ms =>
            ms.Map<Person>(m =>
                m.AutoMap()
                ))
                )

                ;
        }

        private static void Insert()
        {
            IEnumerable<Person> persons = new List<Person>
{
    new Person()
    {
        Id = 1,
        Firstname = "Boterhuis-040",
        Lastname = "Gusto-040",
        Chains = new[]{ "a","b","c" },
    },
    new Person()
    {
        Id = 2,
        Firstname = "sales@historichousehotels.com",
        Lastname = "t Boterhuis 1",
        Chains = new[]{ "a","c" },
    },
    new Person()
    {
        Id = 3,
        Firstname = "Aberdeen #110",
        Lastname = "sales@historichousehotels.com",
        Chains = new[]{ "b","c" },
    },
    new Person()
    {
        Id = 4,
        Firstname = "Aberdeen #110",
        Lastname = "t Boterhuis 2",
        Chains = new[]{ "a","b" },
    },
};

            foreach (var person in persons)
            {
                var request = new IndexRequest<Person>(person, Index);
                _client.Index(request);
            }
        }

    }

    [ElasticsearchType(IdProperty = "Id", Name = "nesttest")]
    public class Person
    {
        [Number()]
        public int Id { get; set; }
        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Firstname { get; set; }
        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string Lastname { get; set; }

        [String(Index = FieldIndexOption.Analyzed, Store = false)]
        public string[] Chains { get; set; }
    }
}
