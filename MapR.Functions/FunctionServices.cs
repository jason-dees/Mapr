using AutoMapper;
using MapR.DataStores.Configuration;
using MapR.DataStores.Stores;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapR.Functions
{
    public static class FunctionServices
    {

        static string MapConnectionString => Environment.GetEnvironmentVariable("MapR:CosmosConnectionString");

        private static CloudStorageAccount _account;
        static CloudStorageAccount Account {
            get {
                if (_account == null) _account = CloudStorageAccount.Parse(MapConnectionString);

                return _account;
            }
        }

        private static CloudTable _mapTable;
        static CloudTable MapTable {
            get {
                if(_mapTable == null) _mapTable = Account.CreateCloudTableClient().GetTableReference("gamemaps");
                return _mapTable;
            }
        }

        private static CloudTable _gameTable;
        static CloudTable GameTable {
            get {
                if (_gameTable == null) _gameTable = Account.CreateCloudTableClient().GetTableReference("games");
                return _gameTable;
            }
        }

        private static CloudTable _markerTable;
        static CloudTable MarkerTable {
            get {
                if (_markerTable == null) _markerTable = Account.CreateCloudTableClient().GetTableReference("mapmarkers");
                return _markerTable;
            }
        }

        private static CloudBlobContainer _mapContainer;
        static CloudBlobContainer MapContainer {
            get {
                if(_mapContainer == null) _mapContainer = Account.CreateCloudBlobClient().GetContainerReference("mapimagestorage");

                return _mapContainer;
            }
        }

        private static CloudBlobContainer _markerContainer;
        static CloudBlobContainer MarkerContainer {
            get {
                if (_markerContainer == null) _markerContainer = Account.CreateCloudBlobClient().GetContainerReference("markericonstorage");

                return _markerContainer;
            }
        }

        private static readonly IMapper mapper = AutoMapperConfiguration.MapperConfiguration();

        private static MapStore _mapStore;
        public static  MapStore MapStore {
            get {
                if(_mapStore == null) _mapStore = new MapStore(MapTable, MapContainer, mapper);

                return _mapStore;
            }
        }

        private static GameStore _gameStore;
        public static GameStore GameStore {
            get {
                if (_gameStore == null) _gameStore = new GameStore(GameTable, mapper);
                return _gameStore;
            }
        }

        private static MarkerStore _markerStore;
        public static MarkerStore MarkerStore {
            get {
                if (_markerStore == null) _markerStore = new MarkerStore(MarkerTable, MarkerContainer, mapper);
                return _markerStore;
            }
        }
    }
}
