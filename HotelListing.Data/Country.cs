namespace HotelListing.API.Data
{
    public class Country
    {
        //one to many relative (one country has many hotels)
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; } //country code

        public virtual IList<Hotel> Hotels { get; set; } // you can use ICollection
    }
}