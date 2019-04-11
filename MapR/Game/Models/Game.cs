using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Game.Models {
    public class Game : TableEntity{

        public Game() { }

        public Game(string owner) {
            Owner = owner;
            PartitionKey = owner;
            RowKey = RandomString(6);
        }
        [IgnoreProperty]
        public string Id => RowKey;

        public string Owner { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }

        private static readonly Random random = new Random();
        private static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void RegenerateId() {
            RowKey = RandomString(6);
        }
    }
}
