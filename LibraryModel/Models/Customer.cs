using Lab2MPA.Models;

namespace LibraryModel.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public int? CityID { get; set; }
        public City? City { get; set; }
        public DateTime? BirthDate { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
