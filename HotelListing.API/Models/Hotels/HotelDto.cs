using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.API.Models.Hotels
{
    public class HotelDto : BaseHotelDto // We will use this for update too
    {
        public int Id { get; set; }
    }
}

