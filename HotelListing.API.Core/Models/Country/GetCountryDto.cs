namespace HotelListing.API.Models.Country
{
    public class GetCountryDto : BaseCountryDto
    {
        public int Id { get; set; } // Its read only because of that we can publish it
        //public string Name { get; set; }
        //public string ShortName { get; set; }
    }
}
