using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Data;

namespace CcStore.Models
{
	public class User
	{

        [BsonElement("_id")]
        [BsonId]
        public ObjectId BsonId { get; set; }

        [BsonElement("id")]
        public int Id { get; set; } = 0;

        [BsonElement("username")]
        public string Username { get; set; } = "defaultUsername";

        [BsonElement("email")]
        public string Email { get; set; } = "default@example.com";

        [BsonElement("firstName")]
        public string FirstName { get; set; } = "First";

        [BsonElement("lastName")]
        public string LastName { get; set; } = "Last";

        [BsonElement("gender")]
        public string Gender { get; set; } = "unspecified";

        [BsonElement("image")]
        public string Image { get; set; } = "https://dummyjson.com/icon/default/128";

        [BsonElement("address")]
        public string Address { get; set; } = "No address provided";

        [BsonElement("role")]
        public Role Role { get; set; } = Role.User;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = "";

        [BsonElement("refreshToken")]
        public string RefreshToken { get; set; } = "";

        // Constructor for flexibility
        public User() { }

		public User(int id, string passwordHash, string username, string email, string firstName, string lastName, string gender, string image, string address, Role role = Role.User)
		{
			Id = id;
			Username = username;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Gender = gender;
			Image = image;
			Address = address;
			Role = role;
			PasswordHash = passwordHash;
		}

		// ToString method for debugging or display
		public override string ToString()
		{
			return $"Id: {Id}, Username: {Username}, Email: {Email}, Name: {FirstName} {LastName}, " +
				   $"Gender: {Gender}, Address: {Address}, Role: {Role}, Image: {Image}";
		}
	}
	public enum Role
	{
		User,
		Admin,
		Moderator
	}
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
		public string FirstName { get; set; }
		public  string LastName { get; set; }
		public string Gender { get; set; } = "not specified";
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }

    public class AuthResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Image { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }


}
