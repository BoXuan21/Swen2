-- Drop tables in reverse order of dependency
DROP TABLE IF EXISTS "TourLogs" CASCADE;
DROP TABLE IF EXISTS "Tours" CASCADE;
DROP TABLE IF EXISTS "Lists" CASCADE;
