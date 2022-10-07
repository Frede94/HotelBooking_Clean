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

        #region Startup-region
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
        #endregion

        #region CreateBooking-Tests-Region
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
        #endregion

        #region FindAvailableRoom-Tests-Region
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
        [Fact]
        public void FindAvailableRoom_RoomNotAvailable_RoomIdIsMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(5);
            DateTime endDate = DateTime.Today.AddDays(13);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, endDate);
            // Assert
            Assert.Equal(-1, roomId);
        }
        [Theory]
        [InlineData(9, 21, -1)]
        [InlineData(9, 10, -1)]
        [InlineData(9, 20, -1)]
        [InlineData(10, 21, -1)]
        [InlineData(20, 21, -1)]
        public void IsAvailableRoom_RoomNotAvailable_RoomIdIsMinusOne(int start, int end, int expected)
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(start);
            DateTime endDate = DateTime.Today.AddDays(end);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, endDate);
            // Assert
            Assert.Equal(expected, roomId);
        }
        #endregion

        #region GetFullyOccupiedDates-Tests-Region
        [Fact]
        public void GetFullyOccupiedDates_StartDateNotLaterThanEndDate_ThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today;
            // Act
            Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory]
        [InlineData(1, 9, 0)]
        [InlineData(18, 300, 3)]
        [InlineData(19, 300, 2)]
        [InlineData(20, 300, 1)]
        [InlineData(17, 300, 4)]
        [InlineData(16, 300, 5)]
        [InlineData(15, 300, 6)]
        [InlineData(14, 300, 7)]
        [InlineData(13, 300, 8)]
        [InlineData(12, 300, 9)]
        [InlineData(2, 300, 11)]
        [InlineData(3, 300, 11)]
        [InlineData(77, 300, 0)]
        [InlineData(7, 300, 11)]
        [InlineData(64, 300, 0)]
        [InlineData(90, 300, 0)]
        [InlineData(88, 300, 0)]
        [InlineData(78, 300, 0)]
        [InlineData(65, 300, 0)]
        [InlineData(33, 300, 0)]
        [InlineData(32, 300, 0)]
        [InlineData(44, 300, 0)]
        public void GetFullyOccupiedDates_DatesRangingBetweenFullyAndNonFullyOccupiedDates_ReturnsTheRightAmountOfFullyOccupiedDates(int x, int y, int expected)
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(x);
            DateTime endDate = DateTime.Today.AddDays(y);
            // Act
            var noOfOccupiedDates = bookingManager.GetFullyOccupiedDates(date, endDate).Count;
            // Assert
            Assert.Equal(expected, noOfOccupiedDates);
        }
        #endregion

    }
}
