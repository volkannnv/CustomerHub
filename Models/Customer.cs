namespace CustomerHub.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }

    public class CustomerAddress
    {
        public int AddressId { get; set; }
        public string AddressLine { get; set; }
    }

    public class CustomerPhone
    {
        public int PhoneId { get; set; }
        public string PhoneNumber { get; set; }
    }
}
