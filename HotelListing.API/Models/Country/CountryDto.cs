using HotelListing.API.Models.Hotels;

namespace HotelListing.API.Models.Country
{
    public class CountryDto
    {
        public int Id { get; set; } // Its read only because of that we can publish it
        public string Name { get; set; }
        public string ShortName { get; set; }

        public List<HotelDto> Hotels { get; set; }
    }
}
