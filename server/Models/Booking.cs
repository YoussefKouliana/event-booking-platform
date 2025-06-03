namespace server.Models
{
   public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; }

    public DateTime CreatedAt { get; set; }
}

}
