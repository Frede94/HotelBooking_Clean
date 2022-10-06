using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> fakeBookRepo;
        private Mock<IRepository<Room>> fakeRoomRepo;

        public BookingManagerTests(){
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

            //IRepository<Booking> bookingRepository = new FakeBookingRepository(start, end);
            //IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(fakeBookRepo.Object, fakeRoomRepo.Object);
        }

        [Theory]
        [InlineData(9, 9, true)]
        [InlineData(21, 21, true)]
        [InlineData(1, 5, true)]
        [InlineData(2, 7, true)]
        [InlineData(25, 27, true)]
        [InlineData(22, 22, true)]
        [InlineData(65, 75, true)]
        public void CreateBooking_BookingDateAvailable_ReturnsTrue(int start, int end, bool expected)
        {
            var booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(start),
                EndDate = DateTime.Today.AddDays(end)
            };
            bool created = bookingManager.CreateBooking(booking);
            Assert.Equal(expected, created);
        }

        [Theory]
        [InlineData(9, 21, false)]
        [InlineData(9, 10, false)]
        [InlineData(9, 20, false)]
        [InlineData(10, 21, false)]
        [InlineData(20, 21, false)]
        public void CreateBooking_BookingDateNotAvailable_ReturnsFalse(int start, int end, bool expected)
        {
            var booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(start),
                EndDate = DateTime.Today.AddDays(end)
            };
            bool created = bookingManager.CreateBooking(booking);
            Assert.Equal(expected, created);
        }

        [Fact]
        public void CreateBooking_BookingDateUnavailable_ReturnsFalse()
        {
            var booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(10)
            };
            bool created = bookingManager.CreateBooking(booking);
            Assert.False(created);
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date, date);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

    }
}
