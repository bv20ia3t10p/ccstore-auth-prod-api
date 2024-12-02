using MongoDB.Bson.Serialization.Attributes;

namespace CcStore.Models
{
    public class Dimensions
    {
        [BsonElement("width")]
        public double Width { get; set; }

        [BsonElement("height")]
        public double Height { get; set; }

        [BsonElement("depth")]
        public double Depth { get; set; }
    }
}
