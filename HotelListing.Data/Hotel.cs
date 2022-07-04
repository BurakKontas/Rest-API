using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.API.Data
{
    public class Hotel
    {
        //one to one relative (one hotel has one country)
        public int Id { get; set; } //primary key
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }

        [ForeignKey(nameof(CountryId))] // If return type is string you can type "countryId" but if it changes to for example int you will be get error nameof() better
        public int CountryId { get; set; } //foreign key
        public Country Country { get; set; }

    }
}
