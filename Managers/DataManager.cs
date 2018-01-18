using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using Noted.Models;

namespace Noted.Managers
{
    public class DataManager
    {
        public MongoClient _client;
        public MongoServer _server;
        public MongoDatabase _db;

        public DataManager()
        {
            _client = new MongoClient("mongodb://Garetas:<9SF3dHKDcAVCfHq2>@notetesting-shard-00-00-msig9.mongodb.net:27017,notetesting-shard-00-01-msig9.mongodb.net:27017,notetesting-shard-00-02-msig9.mongodb.net:27017/admin?replicaSet=NoteTesting-shard-0&ssl=true");
            _server = _client.GetServer();
            _db = _server.GetDatabase("NotedDB");
        }
    }
}