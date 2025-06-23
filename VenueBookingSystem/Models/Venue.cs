using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //import to add
using Microsoft.AspNetCore.Http;// important to add

namespace VenueBookingSystem.Models
{
    public class Venue
    {
        public int VenueID { get; set; }

        public string? VenueName { get; set; }

        public int Capacity { get; set; }

        public string? Location { get; set; }

        public string? ImageURL { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public List<Booking> Booking { get; set; } = new();
    }

}

