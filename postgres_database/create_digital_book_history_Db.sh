#!/bin/bash
BASEDIR=$(dirname $0)
psql -U postgres -f "$BASEDIR/dropdb.sql" &&
createdb -U postgres digital_book_history &&
psql -U postgres -d digital_book_history -f "$BASEDIR/schema.sql" &&
psql -U postgres -d digital_book_history -f "$BASEDIR/user.sql"
psql -U postgres -d digital_book_history -f "$BASEDIR/data.sql"