-- Создаём по схеме на каждый микросервис.
-- Postgres-контейнер выполняет файлы из /docker-entrypoint-initdb.d при первом запуске.
CREATE SCHEMA IF NOT EXISTS users;
CREATE SCHEMA IF NOT EXISTS categories;
CREATE SCHEMA IF NOT EXISTS transactions;
CREATE SCHEMA IF NOT EXISTS reports;
