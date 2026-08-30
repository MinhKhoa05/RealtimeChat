-- schema.sql
-- Create database if not exists
-- USE realtime_chat_db;

SET FOREIGN_KEY_CHECKS = 0;

CREATE TABLE IF NOT EXISTS users (
    id BIGINT SIGNED AUTO_INCREMENT PRIMARY KEY,
) ENGINE=InnoDB;

SET FOREIGN_KEY_CHECKS = 1;
