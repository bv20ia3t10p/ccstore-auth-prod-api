using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CcStore.Models
{
    public class Meta
    {
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("barcode")]
        public string Barcode { get; set; }

        [BsonElement("qrCode")]
        public string QrCode { get; set; }
    }
}