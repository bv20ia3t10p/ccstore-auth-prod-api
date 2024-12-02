﻿using MongoDB.Driver;
using CcStore.Models;

public class MongoDbContext
{
	private readonly IMongoDatabase _database;

	public MongoDbContext(string connectionString, string databaseName)
	{
		var client = new MongoClient(connectionString);
		_database = client.GetDatabase(databaseName);
	}

	public IMongoCollection<T> GetCollection<T>(string collectionName)
	{
		return _database.GetCollection<T>(collectionName);
	}

	// Example: Add specific collections for convenience
	public IMongoCollection<User> Users => GetCollection<User>("Users");
	public IMongoCollection<Product> Products => GetCollection<Product>("Products");
}
