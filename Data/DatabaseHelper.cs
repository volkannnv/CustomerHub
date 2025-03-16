using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CustomerHub.Models;
using System.Collections.Generic;

namespace CustomerHub.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CustomerHubConnection");
        }

        
        // User

        public bool ValidateUser(string username, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT COUNT(1) FROM Users WHERE Username = @u AND [Password] = @p";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);
            int count = (int)cmd.ExecuteScalar();
            return (count == 1);
        }

        public void CreateUser(string email, string username, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                INSERT INTO Users (Email, Username, [Password])
                VALUES (@Email, @Username, @Password);
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.ExecuteNonQuery();
        }

        public int GetUserIdByUsername(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT UserId FROM Users WHERE Username = @u";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@u", username);
            object result = cmd.ExecuteScalar();
            return (result == null) ? 0 : (int)result;
        }

        
        // Customer

        public List<Customer> GetCustomersForUser(int userId)
        {
            var result = new List<Customer>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                SELECT c.CustomerId, c.[Name]
                FROM Customers c
                INNER JOIN UserCustomers uc ON c.CustomerId = uc.CustomerId
                WHERE uc.UserId = @u
                ORDER BY c.[Name] ASC;
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@u", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var cust = new Customer
                {
                    CustomerId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
                result.Add(cust);
            }
            return result;
        }

        
        public int CreateCustomerForUser(int userId, string name)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sqlCust = @"
                INSERT INTO Customers ([Name])
                VALUES (@Name);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            using var cmdCust = new SqlCommand(sqlCust, connection);
            cmdCust.Parameters.AddWithValue("@Name", name);
            int newCustomerId = (int)cmdCust.ExecuteScalar();
            string sqlLink = @"
                INSERT INTO UserCustomers (UserId, CustomerId)
                VALUES (@UserId, @CustomerId);
            ";
            using var cmdLink = new SqlCommand(sqlLink, connection);
            cmdLink.Parameters.AddWithValue("@UserId", userId);
            cmdLink.Parameters.AddWithValue("@CustomerId", newCustomerId);
            cmdLink.ExecuteNonQuery();
            return newCustomerId;
        }

        
        public void UpdateCustomerName(int customerId, string newName)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "UPDATE Customers SET [Name] = @Name WHERE CustomerId = @Id";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Name", newName);
            cmd.Parameters.AddWithValue("@Id", customerId);
            cmd.ExecuteNonQuery();
        }

        
        public void DeleteCustomer(int customerId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            
            string sqlAddresses = "DELETE FROM CustomerAddresses WHERE CustomerId = @Id";
            using (var cmdAddr = new SqlCommand(sqlAddresses, connection))
            {
                cmdAddr.Parameters.AddWithValue("@Id", customerId);
                cmdAddr.ExecuteNonQuery();
            }
            
            string sqlPhones = "DELETE FROM CustomerPhones WHERE CustomerId = @Id";
            using (var cmdPhones = new SqlCommand(sqlPhones, connection))
            {
                cmdPhones.Parameters.AddWithValue("@Id", customerId);
                cmdPhones.ExecuteNonQuery();
            }
            
            string sqlUserCustomers = "DELETE FROM UserCustomers WHERE CustomerId = @Id";
            using (var cmdUserCust = new SqlCommand(sqlUserCustomers, connection))
            {
                cmdUserCust.Parameters.AddWithValue("@Id", customerId);
                cmdUserCust.ExecuteNonQuery();
            }
            
            string sqlCustomer = "DELETE FROM Customers WHERE CustomerId = @Id";
            using (var cmdCust = new SqlCommand(sqlCustomer, connection))
            {
                cmdCust.Parameters.AddWithValue("@Id", customerId);
                cmdCust.ExecuteNonQuery();
            }
        }

        
        // Adres

        public void CreateAddress(int customerId, string addressLine)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                INSERT INTO CustomerAddresses (CustomerId, AddressLine)
                VALUES (@CustomerId, @AddressLine);
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@AddressLine", addressLine);
            cmd.ExecuteNonQuery();
        }

        public List<CustomerAddress> ListAddresses(int customerId)
        {
            var addresses = new List<CustomerAddress>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                SELECT AddressId, AddressLine
                FROM CustomerAddresses
                WHERE CustomerId = @custId
                ORDER BY AddressId DESC;
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@custId", customerId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                addresses.Add(new CustomerAddress
                {
                    AddressId = reader.GetInt32(0),
                    AddressLine = reader.GetString(1)
                });
            }
            return addresses;
        }

        public void UpdateAddress(int addressId, string newAddressLine)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                UPDATE CustomerAddresses
                SET AddressLine = @line
                WHERE AddressId = @addrId;
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@line", newAddressLine);
            cmd.Parameters.AddWithValue("@addrId", addressId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAddress(int addressId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                DELETE FROM CustomerAddresses
                WHERE AddressId = @addrId;
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@addrId", addressId);
            cmd.ExecuteNonQuery();
        }

        
        // Telefon
        
        public void CreatePhone(int customerId, string phoneNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                INSERT INTO CustomerPhones (CustomerId, PhoneNumber)
                VALUES (@CustomerId, @PhoneNumber);
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.ExecuteNonQuery();
        }

        public List<CustomerPhone> ListPhones(int customerId)
        {
            var phones = new List<CustomerPhone>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                SELECT PhoneId, PhoneNumber
                FROM CustomerPhones
                WHERE CustomerId = @custId
                ORDER BY PhoneId DESC;
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@custId", customerId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                phones.Add(new CustomerPhone
                {
                    PhoneId = reader.GetInt32(0),
                    PhoneNumber = reader.GetString(1)
                });
            }
            return phones;
        }

        public void UpdatePhone(int phoneId, string newPhoneNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                UPDATE CustomerPhones
                SET PhoneNumber = @num
                WHERE PhoneId = @pId;
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@num", newPhoneNumber);
            cmd.Parameters.AddWithValue("@pId", phoneId);
            cmd.ExecuteNonQuery();
        }

        public void DeletePhone(int phoneId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
                DELETE FROM CustomerPhones
                WHERE PhoneId = @pId;
            ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@pId", phoneId);
            cmd.ExecuteNonQuery();
        }

        
        // Kullanıcı ayarları
        
        public User GetUserById(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = "SELECT UserId, Username, [Password], Email FROM Users WHERE UserId = @id";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", userId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Email = reader.GetString(3)
                };
            }
            return null;
        }

        public void UpdateUser(int userId, string newUsername, string newPassword)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string sql = @"
             UPDATE Users
             SET Username = @uname,
                 [Password] = @pwd
             WHERE UserId = @id;
             ";
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@uname", newUsername);
            cmd.Parameters.AddWithValue("@pwd", newPassword);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }
    }
}
