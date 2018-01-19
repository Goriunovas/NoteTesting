using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using Noted.Models;
using System;

namespace Noted.Managers
{
    public class DataManager
    {
        public MongoClient _client;
        public MongoServer _server;
        public MongoDatabase _db;

        public DataManager()
        {

            /*string connection = "mongodb+srv://Test:Test@notetesting-msig9.mongodb.net/test";
            _client = new MongoClient(connection);
            _db = _client.GetDatabase("NoteDB");*/

            _client = new MongoClient("mongodb+srv://Test:Test@notetesting-msig9.mongodb.net/test");
            _server = _client.GetServer();
            _db = _server.GetDatabase("NoteDB");
        }
    }
}


