using Microsoft.AspNetCore.Mvc.ViewEngines;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CcStore.Models
{
	public class Product
	{
		[BsonId]
		public ObjectId _Id { get; set; }
        [BsonElement("id")]
		public int Id { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("discountPercentage")]
        public double DiscountPercentage { get; set; }

        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; }

        [BsonElement("brand")]
        public string Brand { get; set; }

        [BsonElement("sku")]
        public string Sku { get; set; }

        [BsonElement("weight")]
        public double Weight { get; set; }

        [BsonElement("dimensions")]
        public Dimensions Dimensions { get; set; }

        [BsonElement("warrantyInformation")]
        public string WarrantyInformation { get; set; }

        [BsonElement("shippingInformation")]
        public string ShippingInformation { get; set; }

        [BsonElement("availabilityStatus")]
        public string AvailabilityStatus { get; set; }

        [BsonElement("reviews")]
        public List<Review> Reviews { get; set; }

        [BsonElement("returnPolicy")]
        public string ReturnPolicy { get; set; }

        [BsonElement("minimumOrderQuantity")]
        public int MinimumOrderQuantity { get; set; }

        [BsonElement("meta")]
        public Meta Meta { get; set; }

        [BsonElement("images")]
        public List<string> Images { get; set; }

        [BsonElement("thumbnail")]
        public string Thumbnail { get; set; }
    }
}
