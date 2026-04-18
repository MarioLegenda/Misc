psql -v ON_ERROR_STOP=1 --username postgres <<-EOSQL
    CREATE DATABASE employees;
    \c employees
    CREATE SCHEMA employees;
EOSQL

pg_restore -U postgres -d employees -Fc employees.sql.gz -c -v --no-owner --no-privileges