namespace RoadsideAssistance.Models.Interfaces
{
    public interface ICustomer
    {
        string Name { get; set; }
        Geolocation Location { get; set; }
    }
}
