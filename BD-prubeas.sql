DROP DATABASE IF EXISTS desafioFuture;
-- Crear base de datos
CREATE DATABASE IF NOT EXISTS desafioFuture;
USE desafioFuture;

-- Tabla usuarios
CREATE TABLE usuarios (
    id INT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL UNIQUE,
    telefono VARCHAR(15) UNIQUE
);

-- Tabla codigos_verificacion
CREATE TABLE codigos_verificacion (
    user_id INT,
    codigo VARCHAR(20) NOT NULL,
    telefono VARCHAR(15),
    expiracion DATETIME,
    usado BOOLEAN DEFAULT FALSE,
    PRIMARY KEY (user_id, codigo),
    FOREIGN KEY (user_id) REFERENCES usuarios(id)
);

-- Inserción de datos en usuarios
INSERT INTO usuarios (id, nombre, telefono) VALUES
(1, 'Paola', '3124567890');

-- Inserción de datos en codigos_verificacion
INSERT INTO codigos_verificacion (user_id, codigo, telefono, expiracion, usado) VALUES
(1, 'AB123', '3124567890', '2025-06-14 18:00:00', FALSE);
