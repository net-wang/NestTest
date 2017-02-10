using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestTest
{
    class Program
    {
        static ElasticClient client;
        const string Index = "nesttest";
        static void Main(string[] args)
        {
            var node = new Uri("http://192.168.210.36:9200/");
            var settings = new ConnectionSettings(
                node
            );
            client = new ElasticClient(settings);
            //CreateIndex();
            // Insert();


            client.Search<Person>(s => s
    .Index(Index)
    .Query(q => q.QueryString(qs => qs.Query("bo").DefaultOperator(Operator.And))));

        }

        static void CreateIndex()
        {

            var exists = client.IndexExists(Index);

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
                client.CreateIndex(Index, p => p
        .InitializeUsing(indexState)
        .Mappings(ms =>
            ms.Map<Person>(m =>
                m.AutoMap()
                ))
                )

                ;
        }

        static void Insert()
        {
            IEnumerable<Person> persons = new List<Person>
{
    new Person()
    {
        Id = 1,
        Firstname = "Boterhuis-040",
        Lastname = "Gusto-040",
        Chains = new string[]{ "a","b","c" },
    },
    new Person()
    {
        Id = 2,
        Firstname = "sales@historichousehotels.com",
        Lastname = "t Boterhuis 1",
        Chains = new string[]{ "a","c" },
    },
    new Person()
    {
        Id = 3,
        Firstname = "Aberdeen #110",
        Lastname = "sales@historichousehotels.com",
        Chains = new string[]{ "b","c" },
    },
    new Person()
    {
        Id = 4,
        Firstname = "Aberdeen #110",
        Lastname = "t Boterhuis 2",
        Chains = new string[]{ "a","b" },
    },
};

            foreach (var person in persons)
            {
                var request = new IndexRequest<Person>(person, Index);
                client.Index(request);
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
