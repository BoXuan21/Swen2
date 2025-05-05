-- Create List table (creating this first since it's referenced by Tours)
CREATE TABLE IF NOT EXISTS list (
    Id VARCHAR(50) PRIMARY KEY,
    -- Add any additional columns for list as needed
    Name VARCHAR(100),
    Description TEXT
);

-- Create Tours table
CREATE TABLE IF NOT EXISTS tours (
    Id VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Fromt VARCHAR(100), -- From location (using "Fromt" as "From" is a reserved word in SQL)
    To VARCHAR(100),
    transportType VARCHAR(50),
    Distance VARCHAR(50),
    EstimatedTime TIME,
    routeInformation TEXT,
    listId VARCHAR(50),
    FOREIGN KEY (listId) REFERENCES list(Id)
);

-- Create Logs table
CREATE TABLE IF NOT EXISTS logs (
    Id VARCHAR(50) PRIMARY KEY,
    dateTime DATETIME NOT NULL,
    Comment TEXT,
    Difficulty INT CHECK (Difficulty BETWEEN 1 AND 10), -- Assuming difficulty is on a scale
    Totaldistance FLOAT,
    totalTime TIME,
    Rating INT CHECK (Rating BETWEEN 1 AND 5), -- Assuming rating is on a scale of 1-5
    TourID VARCHAR(50) NOT NULL,
    FOREIGN KEY (TourID) REFERENCES tours(Id)
);
