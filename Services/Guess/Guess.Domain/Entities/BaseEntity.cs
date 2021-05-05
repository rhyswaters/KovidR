using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Guess.Domain.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = new ObjectId();
        }

        [BsonId]
        public ObjectId Id { get; set; }
    }
}
