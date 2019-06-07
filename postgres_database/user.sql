-- ********************************************************************************
-- This script creates the database users and grants them the necessary permissions
-- ********************************************************************************

CREATE USER digital_book_history_owner WITH PASSWORD 'Noo8nsVSXwzG7WgeArye';

GRANT ALL
ON ALL TABLES IN SCHEMA public
TO digital_book_history_owner;

GRANT ALL
ON ALL SEQUENCES IN SCHEMA public
TO digital_book_history_owner;

CREATE USER digital_book_history_appuser WITH PASSWORD 'AAtjSUbLTRJVbu95LH3T';

GRANT SELECT, INSERT, UPDATE, DELETE
ON ALL TABLES IN SCHEMA public
TO digital_book_history_appuser;

GRANT USAGE, SELECT
ON ALL SEQUENCES IN SCHEMA public
TO digital_book_history_appuser;
