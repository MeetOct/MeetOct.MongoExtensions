using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetOct.MongoExtensions
{
	internal class MongoManager
    {
		private MongoClient client;
		private object obj = new object();

		internal MongoClient GetClient(string connString)
		{
			if (client == null)
			{
				lock (obj)
				{
					if (client == null)
					{
						client = new MongoClient(connString);
					}
				}
			}
			return client;
		}
	}
}
