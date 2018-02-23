using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents;

namespace TestingApp.Models.Raven
{
    public static class RavenStore
    {
        private static Lazy<IDocumentStore> lazyStore = new Lazy<IDocumentStore>(() =>
        {
            return new DocumentStore()
            {
                Urls = new []{ "http://127.0.0.1:8080" },
                Database = "Testing"
            }.Initialize();
        });

        public static IDocumentStore Store => lazyStore.Value;
    }
}
