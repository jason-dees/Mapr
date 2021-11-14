using System;
using System.Linq;
using Microsoft.Azure.Cosmos.Table;

namespace MapR.DataStores.Extensions {
    public static class TableEntityExtensions {
        public static void GenerateRandomId(this ITableEntity entity) {
            entity.RowKey = RandomString(6);
        }

        private static readonly Random random = new Random();
        private static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
