-- DATABASE CREATION
USE master
IF EXISTS (SELECT *FROM sys.databases WHERE name ='VenueDB10313401')
DROP DATABASE VenueDB10313401
CREATE DATABASE VenueDB10313401

USE VenueDB10313401

--CREATE TABLES
CREATE TABLE Venue (
    VenueID INT IDENTITY(1,1) PRIMARY KEY,
    VenueName NVARCHAR(255) NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    Capacity INT NOT NULL CHECK (Capacity > 0),
    ImageURL NVARCHAR(500) NULL
);

CREATE TABLE EventType(
	EventTypeID INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    EventName NVARCHAR(255) NOT NULL,
    EventDate DATETIME NOT NULL,
    Description TEXT,
    VenueID INT NULL,
	EventTypeID INT NULL,
    FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE SET NULL,
	FOREIGN KEY (EventTypeID) REFERENCES EventType(EventTypeID) ON DELETE SET NULL,
);

CREATE TABLE Booking (
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    VenueID INT NOT NULL,
    BookingDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EventID) REFERENCES Event(EventID) ON DELETE CASCADE,
    FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE CASCADE,
    CONSTRAINT UQ_Venue_Event UNIQUE (VenueID, EventID)
);

CREATE UNIQUE INDEX UQ_Venue_Booking ON Booking (VenueID, BookingDate); --ENSURE NO BOOKINGS OVERLAP

--INSERT VALUES
INSERT INTO Venue (VenueName, Location, Capacity, ImageURL)
VALUES 
('Grand Hall', '123 Main Street, Cityville', 500, 'https://example.com/images/grandhall.jpg'),
('Lakeside Pavilion', '456 Lakeshore Road, Seaside', 200, 'https://example.com/images/lakesidepavilion.jpg'),
('Riverside Conference Center', '789 River Road, Rivertown', 150, 'https://example.com/images/riversideconference.jpg'),
('The Skyline Venue', '101 Skyline Boulevard, Hilltop', 350, 'https://example.com/images/skylinevenue.jpg'),
('The Green Garden', '282 Garden Street, Greenfield', 100, 'https://example.com/images/greengarden.jpg');

INSERT INTO EventType (Name)
VALUES
('Conference'),
('Wedding'),
('Naming'),
('Birthday'),
('Concert');


INSERT INTO Event (EventName, EventDate, Description, VenueID, EventTypeID)
VALUES 
('Tech Conference 2025', '2025-05-15 09:00:00', 'Annual conference on technology and innovations.', 1, 1 ),
('Wedding Reception - Johnson', '2025-06-20 18:00:00', 'Celebration of the marriage between Sarah and John Johnson.', 2, 2),
('Business Seninar', '2025-07-10 14:00:00', 'Seminar on business management and strategy.', 3, 3), 
('Music Concert', '2025-08-25 19:00:00', 'Live music concert featuring popular bands.', 4, 4),
('Garden Party', '2025-09-12 15:00:00', 'Outdoor garden party with refreshments and entertainment.', 5, 5);

INSERT INTO Booking (EventID, VenueID, BookingDate)
VALUES 
(1, 1, '2025-05-15 09:00:00'),
(2, 2, '2025-06-20 18:00:00'),
(3, 3, '2025-07-10 14:00:00'),
(4, 4, '2025-08-25 19:00:00'),
(5, 5, '2025-09-12 15:00:00');

SELECT * FROM Venue;
SELECT * FROM Event;
SELECT * FROM Booking;
SELECT * FROM EventType;