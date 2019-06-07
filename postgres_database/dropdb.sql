-- **************************************************************
-- This script destroys the database and associated users
-- **************************************************************

-- The following line terminates any active connections to the database so that it can be destroyed
SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'digital_book_history';

DROP DATABASE digital_book_history;

DROP USER digital_book_history_owner;
DROP USER digital_book_history_appuser;
