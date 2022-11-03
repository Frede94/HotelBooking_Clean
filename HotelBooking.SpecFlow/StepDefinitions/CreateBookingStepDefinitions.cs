using HotelBooking.Core;
using Moq;

namespace HotelBooking.SpecFlow.StepDefinitions
{
    [Binding]
    public sealed class CreateBookingStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> fakeBookRepo;
        private Mock<IRepository<Room>> fakeRoomRepo;
        private bool _result = false;
        private Booking booking = new Booking();

        #region Constructor-region
        public CreateBookingStepDefinitions(ScenarioContext scenarioContext)
        {
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);

            var rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" }
            };

            var booking = new List<Booking>
            {
                new Booking { Id=1, StartDate=start, EndDate=end, IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=2, StartDate=start, EndDate=end, IsActive=true, CustomerId=2, RoomId=2 }
            };

            fakeBookRepo = new Mock<IRepository<Booking>>();
            fakeRoomRepo = new Mock<IRepository<Room>>();


            fakeRoomRepo.Setup(r => r.GetAll()).Returns(rooms);
            fakeBookRepo.Setup(r => r.GetAll()).Returns(booking);

            bookingManager = new BookingManager(fakeBookRepo.Object, fakeRoomRepo.Object);
        }
        #endregion

        [Given(@"the first date is (.*)")]
        public void GivenTheFirstDateIs(int daysFromToday)
        {
            booking.StartDate = DateTime.Today.AddDays(daysFromToday);
        }

        [Given(@"the second date is (.*)")]
        public void GivenTheSecondDateIs(int daysFromToday)
        {
            booking.EndDate = DateTime.Today.AddDays(daysFromToday);
        }

        [When(@"a booking is created")]
        public void WhenABookingIsCreated()
        {
            _result = bookingManager.CreateBooking(booking);
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(bool result)
        {
            _result.Should().Be(result);
        }

    }
}