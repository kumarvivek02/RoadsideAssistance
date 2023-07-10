using Microsoft.CodeAnalysis;
using RoadsideAssistance.Models;
using RoadsideAssistance.Models.Interfaces;
using RoadsideAssistance.Services.Interfaces;

namespace RoadsideAssistance.Services
{
    public class RoadsideAssistanceService : IRoadsideAssistanceService
    {
        private readonly IAssistant _assistant;
        private readonly IGeolocation _geolocation;
        private readonly ICustomer _customer;
        internal IDictionary<Customer, Assistant> _assignedAssistants;
        public List<Assistant> _allAssistants = new List<Assistant>();// This should be pulled in from a relational table or flat file. Keeping it simple here.


        public RoadsideAssistanceService(IAssistant assistant, IGeolocation geolocation, ICustomer customer)
        {
            _assistant = assistant;
            _geolocation = geolocation;
            _customer = customer;
            _assignedAssistants = new Dictionary<Customer, Assistant>();            
        }       

        public void ReleaseAssistant(Customer customer, Assistant assistant)
        {
            if (_assignedAssistants.TryGetValue(customer, out Assistant assignedAssistant) && assignedAssistant == assistant)
            {
                _assignedAssistants.Remove(customer);
                               
                assistant.IsReserved = false; // Keeping it simple. Can use a enum of Statuses if more than T/F is needed.
                assistant.LastAssignmentEndTime = DateTime.UtcNow;
                // Additional actions can be performed.
                return; 
            }

            // Handle scenario where the provided assistant is not associated with the customer.
            throw new InvalidOperationException("The provided assistant is not associated with the customer.");
        }

        public Assistant? ReserveAssistant(Customer customer, Geolocation customerLocation)
        {
            SortedSet<Assistant> nearestAssistants = FindNearestAssistants(customerLocation);
            if (nearestAssistants.Count > 0)
            {
                Assistant reservedAssistant = nearestAssistants.Min;

                // Reserve the assistant for the customer
                if (IsAssistantAvailable(reservedAssistant))
                {
                    // Associate the reserved assistant with the customer
                    _assignedAssistants[customer] = reservedAssistant;
                    reservedAssistant.LastAssignmentStartTime = DateTime.UtcNow;
                    // Can perform other necessary updates to add onto the logic implemented here.

                    return reservedAssistant;
                }
            }

            return null;
        }

        private bool IsAssistantAvailable(Assistant assistant)
        {
            // Keeping it simple, using a flag to ascertain availability
            return !assistant.IsReserved;
        }

        private double CalculateDistance(Geolocation location1, Geolocation location2)
        {
            double latitudeDifference = location2.Latitude - location1.Latitude;
            double longitudeDifference = location2.Longitude - location1.Longitude;
            double distance = Math.Sqrt(latitudeDifference * latitudeDifference + longitudeDifference * longitudeDifference);
            return distance;
        }

        public SortedSet<Assistant> FindNearestAssistants(Geolocation geolocation, int limit = 5)
        {
            SortedSet<Assistant> nearestAssistants = new SortedSet<Assistant>(Comparer<Assistant>.Create((a1, a2) =>
            {
                double distance1 = CalculateDistance(a1.Location, geolocation);
                double distance2 = CalculateDistance(a2.Location, geolocation);
                return distance1.CompareTo(distance2);
            }));

            // Sample implementation to fetch nearest assistants            
            // Here, we are assuming a list of assistants called "_allAssistants" exists
            foreach (Assistant assistant in _allAssistants)
            {
                nearestAssistants.Add(assistant);
                if (nearestAssistants.Count > limit)
                {
                    nearestAssistants.Remove(nearestAssistants.Max);
                }
            }
            return nearestAssistants;
        }

        public void UpdateAssistantLocation(Assistant assistant, Geolocation assistantLocation)
        {
            assistant.Location = assistantLocation;
        }

        //Helper methods to allow access to _assignedAssistants private member. Multiple ways to expose private variable for testing.
        public IDictionary<Customer, Assistant> GetAssignedAssistants()
        {
            return _assignedAssistants;
        }

        public void SetAssignedAssistants(IDictionary<Customer, Assistant> assignedAssistants)
        {
            _assignedAssistants = assignedAssistants;
        }
    }
}
