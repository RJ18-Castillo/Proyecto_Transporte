Scripts utilizados para crear la base de datos:

USE GestorDeTransporte;
GO

--------------------------------------------------
-------------------CREAR TABLAS-------------------
--------------------------------------------------

CREATE DATABASE GestorDeTransporte;
GO

USE GestorDeTransporte;
GO

CREATE TABLE Usuarios (
    Cedula VARCHAR(9) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Clave VARCHAR(255) NOT NULL,
    TipoUsuario VARCHAR(20) NOT NULL,
    IntentosFallidos INT NOT NULL DEFAULT 0,
    Bloqueado BIT NOT NULL DEFAULT 0,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_Usuarios_TipoUsuario
        CHECK (TipoUsuario IN ('Administrador', 'Chofer', 'Pasajero'))
);
GO

CREATE TABLE Pasajeros (
    Cedula VARCHAR(9) PRIMARY KEY,
    Nombre1 VARCHAR(50) NOT NULL,
    Nombre2 VARCHAR(50) NULL,
    Apellido1 VARCHAR(50) NOT NULL,
    Apellido2 VARCHAR(50) NULL,

    CONSTRAINT FK_Pasajeros_Usuarios
        FOREIGN KEY (Cedula) REFERENCES Usuarios(Cedula)
);
GO

CREATE TABLE Choferes (
    Cedula VARCHAR(9) PRIMARY KEY,
    Nombre1 VARCHAR(50) NOT NULL,
    Nombre2 VARCHAR(50) NULL,
    Apellido1 VARCHAR(50) NOT NULL,
    Apellido2 VARCHAR(50) NULL,

    CONSTRAINT FK_Choferes_Usuarios
        FOREIGN KEY (Cedula) REFERENCES Usuarios(Cedula)
);
GO

CREATE TABLE Rutas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Origen VARCHAR(100) NOT NULL,
    Destino VARCHAR(100) NOT NULL,
    DuracionEstimada TIME NULL,
    PrecioBase DECIMAL(10,2) NOT NULL
);
GO

CREATE TABLE Unidades (
    Placa VARCHAR(20) PRIMARY KEY,
    Modelo VARCHAR(100) NOT NULL,
    AnoFabricacion SMALLINT NOT NULL,
    Capacidad SMALLINT NOT NULL,

    CONSTRAINT CK_Unidades_AnoFabricacion
        CHECK (AnoFabricacion BETWEEN 1900 AND YEAR(GETDATE())),

    CONSTRAINT CK_Unidades_Capacidad
        CHECK (Capacidad BETWEEN 1 AND 10000)
);
GO

CREATE TABLE Viajes (
    NumeroViaje INT IDENTITY(1,1) PRIMARY KEY,
    IdRuta INT NOT NULL,
    PlacaUnidad VARCHAR(20) NOT NULL,
    CedulaChofer VARCHAR(9) NOT NULL,
    EstadoViaje VARCHAR(20) NOT NULL,
    FechaHoraSalida DATETIME NOT NULL,
    MotivoCancelacion VARCHAR(255) NULL,
    FechaHoraCancelacion DATETIME NULL,

    CONSTRAINT FK_Viajes_Rutas
        FOREIGN KEY (IdRuta) REFERENCES Rutas(Id),

    CONSTRAINT FK_Viajes_Unidades
        FOREIGN KEY (PlacaUnidad) REFERENCES Unidades(Placa),

    CONSTRAINT FK_Viajes_Choferes
        FOREIGN KEY (CedulaChofer) REFERENCES Choferes(Cedula),

    CONSTRAINT CK_Viajes_EstadoViaje
        CHECK (EstadoViaje IN ('Programado', 'EnCurso', 'Finalizado', 'Cancelado'))
);
GO

CREATE TABLE Reservas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumeroViaje INT NOT NULL,
    CedulaPasajero VARCHAR(9) NOT NULL,
    NumeroAsiento INT NOT NULL,
    MontoPagado DECIMAL(10,2) NOT NULL,
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Reservas_Viajes
        FOREIGN KEY (NumeroViaje) REFERENCES Viajes(NumeroViaje),

    CONSTRAINT FK_Reservas_Pasajeros
        FOREIGN KEY (CedulaPasajero) REFERENCES Pasajeros(Cedula),

    CONSTRAINT UQ_Reservas_Viaje_Asiento
        UNIQUE (NumeroViaje, NumeroAsiento)
);
GO

--------------------------------------------------
------------------INSERTAR DATOS------------------
--------------------------------------------------

INSERT INTO Usuarios
(Cedula, NombreUsuario, Correo, Clave, TipoUsuario)
VALUES
('100000001', 'Administrador', 'admin@transporte.com', 'TicoBus2025*', 'Administrador');

