using System.ComponentModel.DataAnnotations;

namespace VenueBookingSystem.Models;

public class Event
{
    public int EventID { get; set; }
    public string EventName { get; set; }
    public DateTime EventDate { get; set; }
    public string Description { get; set; }
    public int VenueID { get; set; }
    public Venue? Venue { get; set; }
    public int? EventTypeID { get; set; } //Step 4 
    public EventType? EventType { get; set; }
    public List<Booking> Booking { get; set; } = new();

}
    