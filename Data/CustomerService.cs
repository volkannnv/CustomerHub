using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CustomerHub.Models;

namespace CustomerHub.Services
{
    public class CustomerService
    {
        private readonly string _connectionString;

        public CustomerService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CustomerHubConnection");
        }

        // Gets a list of admin customers for a specific user
        public List<AdminCustomer> GetAdminCustomers(int userId)
        {
            // Create our result list
            var customers = new List<AdminCustomer>();

            // SQL query to get customer data
            string sql = @"
                SELECT 
                    c.CustomerId,
                    c.Name AS CustomerName,
                    (
                        SELECT TOP 1 ca.AddressLine 
                        FROM CustomerAddresses ca 
                        WHERE ca.CustomerId = c.CustomerId 
                        ORDER BY ca.AddressId 
                    ) AS FirstAddress,
                    (
                        SELECT TOP 1 cp.PhoneNumber 
                        FROM CustomerPhones cp 
                        WHERE cp.CustomerId = c.CustomerId 
                        ORDER BY cp.PhoneId
                    ) AS FirstPhone
                FROM Customers c
                JOIN UserCustomers uc ON c.CustomerId = uc.CustomerId
                WHERE uc.UserId = @UserId
                ORDER BY c.Name;
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create new customer from each row
                                var customer = new AdminCustomer
                                {
                                    CustomerId = reader.GetInt32(0),
                                    CustomerName = reader.GetString(1),
                                    FirstAddress = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    FirstPhone = reader.IsDBNull(3) ? "" : reader.GetString(3)
                                };

                                customers.Add(customer);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Console.WriteLine($"Database error: {ex.Message}");
                return new List<AdminCustomer>();
            }

            return customers;
        }
    }
}