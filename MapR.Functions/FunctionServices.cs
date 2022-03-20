using AutoMapper;
using Azure.Storage.Blobs;
using MapR.DataStores.Configuration;
using MapR.DataStores.Models;
using MapR.DataStores.Stores;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace MapR.Functions
{
    public static class FunctionServices
    {

        static string MapConnectionString => Environment.GetEnvironmentVariable("MapR:TableStorageConnectionString");
        static string BlobConnectionSting => Environment.GetEnvironmentVariable("MapR:BlobStorageConnectionString");
    }
}
