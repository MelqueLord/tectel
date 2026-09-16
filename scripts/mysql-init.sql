-- MySQL initialization script for Tectel
-- This script runs when the MySQL container starts for the first time

-- Ensure proper character set
ALTER DATABASE IF EXISTS tectel CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Create user with proper permissions
-- (User is already created by MYSQL_USER env var, but let's ensure permissions)
GRANT ALL PRIVILEGES ON tectel.* TO 'tectel'@'%';
FLUSH PRIVILEGES;

-- Set MySQL optimizations for production
SET GLOBAL innodb_buffer_pool_size = 268435456; -- 256MB
SET GLOBAL max_connections = 100;
SET GLOBAL innodb_log_file_size = 268435456; -- 256MB
SET GLOBAL innodb_flush_log_at_trx_commit = 2;
SET GLOBAL innodb_flush_method = 'O_DIRECT';
