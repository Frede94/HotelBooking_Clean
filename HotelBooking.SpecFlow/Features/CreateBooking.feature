Feature: CreateBooking

A short summary of the feature

@mytag
Scenario: Try Create Booking(true)
	Given the first date is 9
	And the second date is 9
	When a booking is created
	Then the result should be true

Scenario: Try Create Booking(false)
	Given the first date is 9
	And the second date is 21
	When a booking is created
	Then the result should be false

Scenario Outline: Try Creating Multiple Bookings
	Given the first date is <firstDate>
	And the second date is <secondDate>
	When a booking is created
	Then the result should be <bool>

	Examples:
	| firstDate | secondDate  | bool  |
	| 21        | 21          | true  |
	| 111       | 112         | true  |
	| 113       | 114         | true  |
	| 200       | 210         | true  |
	| 9         | 21          | false |
	| 9         | 10          | false |
	| 10        | 21          | false |
	| 20        | 21          | false |