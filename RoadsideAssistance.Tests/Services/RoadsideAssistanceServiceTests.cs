using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using RoadsideAssistance.Services;
using RoadsideAssistance.Models;
using RoadsideAssistance.Models.Interfaces;
using RoadsideAssistance.Services.Interfaces;

namespace RoadsideAssistance.Tests.Services
{
    [TestFixture]
    public class RoadsideAssistanceServiceTests
    {
        public RoadsideAssistanceService roadsideAssistanceService;
        private Mock<IAssistant> mockAssistant;
        private Mock<IGeolocation> mockGeolocation;
        private Mock<ICustomer> mockCustomer;
        private List<Assistant> allAssistants;

        [SetUp]
        public void Setup()
        {
            mockAssistant = new Mock<IAssistant>();
            mockAssistant.Setup(a => a.IsReserved).Returns(true);
            mockGeolocation = new Mock<IGeolocation>();
            mockCustomer = new Mock<ICustomer>();
            //allAssistants = new List<Assistant> { new Assistant("AssistantName", new Geolocation(0.0, 0.0)) };

            roadsideAssistanceService = new RoadsideAssistanceService(mockAssistant.Object, mockGeolocation.Object, mockCustomer.Object);
        }

        [Test]
        public void UpdateAssistantLocation_ValidAssistant_UpdatesLocation()
        {
            // Arrange
            Geolocation newLocation = new Geolocation(1.0, 1.0);

            Assistant assistant = new Assistant("AssistantName", new Geolocation(0.0, 0.0));

            // Act
            roadsideAssistanceService.UpdateAssistantLocation(assistant, newLocation);

            // Assert           
            Assert.That(assistant.Location.Latitude, Is.EqualTo(1.0));
            Assert.That(assistant.Location.Longitude, Is.EqualTo(1.0));

        }

        [Test]
        public void FindNearestAssistants_ReturnsSortedSetOfNearestAssistants()
        {
            // Arrange
            var geolocation = new Geolocation(0.0, 0.0);
            var assistant1 = new Assistant("Assistant1", new Geolocation(1.0, 1.0));
            var assistant2 = new Assistant("Assistant2", new Geolocation(2.0, 2.0));
            var assistant3 = new Assistant("Assistant3", new Geolocation(3.0, 3.0));
            var allAssistants = new List<Assistant> { assistant1, assistant2, assistant3 };

            var mockAssistant = new Mock<IAssistant>();
            var mockGeolocation = new Mock<IGeolocation>();
            var mockCustomer = new Mock<ICustomer>();

            var roadsideAssistanceService = new RoadsideAssistanceService(mockAssistant.Object, mockGeolocation.Object, mockCustomer.Object);
            roadsideAssistanceService._allAssistants= allAssistants;

            // Act
            SortedSet<Assistant> nearestAssistants = roadsideAssistanceService.FindNearestAssistants(geolocation, 2);

            // Assert
            Assert.AreEqual(2, nearestAssistants.Count);
            Assert.IsTrue(nearestAssistants.Contains(assistant1));
            Assert.IsTrue(nearestAssistants.Contains(assistant2));
            Assert.IsFalse(nearestAssistants.Contains(assistant3));
        }

        [Test]
        public void ReserveAssistant_ValidInput_AssignsAssistantToCustomer()
        {
            // Arrange
            double customerLatitude = 0.0;
            double customerLongitude = 0.0;
            Geolocation customerLocation = new Geolocation(customerLatitude, customerLongitude);
            Customer customer = new Customer("CustomerName", customerLocation);
            Assistant assistant = new Assistant("Assistant1", customerLocation);
            mockGeolocation.Setup(g => g.Latitude).Returns(customerLatitude);
            mockGeolocation.Setup(g => g.Longitude).Returns(customerLongitude);
            mockAssistant.Setup(a => a.IsReserved).Returns(true);
            mockAssistant.SetupProperty(a => a.Location);
            mockAssistant.Object.Location = customerLocation;
            roadsideAssistanceService._allAssistants = new List<Assistant>()
            {
                new Assistant("Assistant1", new Geolocation(0.0, 0.0)),
                new Assistant("Assistant2", new Geolocation(1.0, 1.0)),
                new Assistant("Assistant3", new Geolocation(2.0, 2.0))
            };

            // Act
            Assistant? reservedAssistant = roadsideAssistanceService.ReserveAssistant(customer, customerLocation);

            // Assert
            Assert.IsNotNull(reservedAssistant);
            Assert.That(reservedAssistant.IsReserved, Is.EqualTo(assistant.IsReserved));
            Assert.That(reservedAssistant.Name, Is.EqualTo(assistant.Name));
            Assert.IsTrue(mockAssistant.Object.IsReserved);
            Assert.That(mockAssistant.Object.Location, Is.EqualTo(customerLocation));
        }

        [Test]
        public void ReserveAssistant_NoAvailableAssistant_ReturnsNull()
        {
            // Arrange
            mockAssistant.Setup(a => a.IsReserved).Returns(true);
            double customerLatitude = 0.0;
            double customerLongitude = 0.0;
            Customer customer = new Customer("CustomerName", new Geolocation(customerLatitude, customerLongitude));

            // Create an Assistant object with IsReserved set to true
            Assistant reservedAssistant = new Assistant("AssistantName", new Geolocation(0.0, 0.0))
            {
                IsReserved = true
            };

            allAssistants = new List<Assistant> { reservedAssistant };

            roadsideAssistanceService = new RoadsideAssistanceService(mockAssistant.Object, mockGeolocation.Object, mockCustomer.Object);
            roadsideAssistanceService._allAssistants = allAssistants;

            // Act
            Assistant? reservedAssistantResult = roadsideAssistanceService.ReserveAssistant(customer, new Geolocation(customerLatitude, customerLongitude));

            // Assert
            Assert.IsNull(reservedAssistantResult);
            mockAssistant.VerifySet(a => a.IsReserved = It.IsAny<bool>(), Times.Never);
        }

        [Test]
        public void ReleaseAssistant_AssistantAssignedToCustomer_ReleasesAssistant()
        {
            // Arrange
            var customer = new Customer("John Doe", new Geolocation(0.0, 0.0));
            var assistant = new Assistant("AssistantName", new Geolocation(0.0, 0.0));

            var tempDictionary = roadsideAssistanceService.GetAssignedAssistants();
            tempDictionary.Add(customer, assistant);
            roadsideAssistanceService.SetAssignedAssistants(tempDictionary);


            // Act
            roadsideAssistanceService.ReleaseAssistant(customer, assistant);

            // Assert
            Assert.IsFalse(assistant.IsReserved);
            Assert.That(assistant.LastAssignmentEndTime.Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.IsFalse(tempDictionary.ContainsKey(customer));
        }

        [Test]
        public void ReleaseAssistant_AssistantNotAssignedToCustomer_ThrowsException()
        {
            // Arrange
            var customer = new Customer("John Doe", new Geolocation(0.0, 0.0));
            var assistant1 = new Assistant("AssistantName1", new Geolocation(0.0, 0.0));
            var assistant2 = new Assistant("AssistantName2", new Geolocation(0.0, 0.0));

            var tempDictionary = roadsideAssistanceService.GetAssignedAssistants();
            tempDictionary.Add(customer, assistant1);
            roadsideAssistanceService.SetAssignedAssistants(tempDictionary);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => roadsideAssistanceService.ReleaseAssistant(customer, assistant2));

        }

    }
}