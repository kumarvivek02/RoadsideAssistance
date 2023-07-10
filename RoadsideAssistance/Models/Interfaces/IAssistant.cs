namespace RoadsideAssistance.Models.Interfaces
{
    public interface IAssistant
    {
        string Name { get; set; }
        Geolocation Location { get; set; }
        public bool IsReserved { get; set; }

        DateTime LastAssignmentEndTime { get; set; }
        DateTime LastAssignmentStartTime { get; set; }

        //Might add a Review/ Rating prop
    }
}
