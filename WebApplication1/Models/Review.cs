using MongoDB.Bson.Serialization.Attributes;

namespace CcStore.Models
{
	public class Review
	{
        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("comment")]
        public string Comment { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [BsonElement("reviewerName")]
        public string ReviewerName { get; set; }

        [BsonElement("reviewerEmail")]
        public string ReviewerEmail { get; set; }

        [BsonElement("images")]
        public string Images { get; set; }
    }
}
