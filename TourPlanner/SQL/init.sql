-- Create List table (creating this first since it's referenced by Tours)
CREATE TABLE IF NOT EXISTS lists (
                                    Id VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100),
    Description TEXT
    );

-- Create Tours table
CREATE TABLE IF NOT EXISTS tours (
                                     Id VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    FromLocation VARCHAR(100), -- From location (using "Fromt" as "From" is a reserved word in SQL)
    ToLocation VARCHAR(100),
    transportType VARCHAR(50),
    Distance FLOAT,
    EstimatedTime INT,
    routeInformation TEXT,
    listId VARCHAR(50),
    FOREIGN KEY (listId) REFERENCES lists(Id)
    );

-- Create Logs table
CREATE TABLE IF NOT EXISTS logs (
                                    Id VARCHAR(50) PRIMARY KEY,
    dateTime TIMESTAMP NOT NULL,
    Comment TEXT,
    Difficulty INT CHECK (Difficulty BETWEEN 1 AND 10),
    Totaldistance FLOAT,
    totalTime INTERVAL,
    Rating INT CHECK (Rating BETWEEN 1 AND 10),
    TourID VARCHAR(50) NOT NULL,
    FOREIGN KEY (TourID) REFERENCES tours(Id)
    );