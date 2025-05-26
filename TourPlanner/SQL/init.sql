-- Create tables in order to respect foreign keys
CREATE TABLE IF NOT EXISTS "Lists" (
    "Id" VARCHAR(255) PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT
);

CREATE TABLE IF NOT EXISTS "Tours" (
    "Id" VARCHAR(255) PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "FromLocation" VARCHAR(255) NOT NULL,
    "ToLocation" VARCHAR(255) NOT NULL,
    "TransportType" VARCHAR(255) NOT NULL,
    "Distance" REAL NOT NULL,
    "EstimatedTime" INTEGER NOT NULL,
    "RouteInformation" TEXT,
    "ListId" VARCHAR(255) NULL,
    CONSTRAINT "FK_Tours_Lists_ListId" FOREIGN KEY ("ListId") REFERENCES "Lists" ("Id")
);

CREATE TABLE IF NOT EXISTS "TourLogs" (
    "Id" VARCHAR(255) PRIMARY KEY,
    "Date" TIMESTAMP NOT NULL,
    "Comment" TEXT NOT NULL,
    "Difficulty" INTEGER NOT NULL,
    "Rating" INTEGER NOT NULL,
    "Duration" INTERVAL NOT NULL,
    "TourId" VARCHAR(255) NOT NULL,
    CONSTRAINT "FK_TourLogs_Tours_TourId" FOREIGN KEY ("TourId") REFERENCES "Tours" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_TourLogs_TourId" ON "TourLogs" ("TourId");
CREATE INDEX IF NOT EXISTS "IX_Tours_ListId" ON "Tours" ("ListId");