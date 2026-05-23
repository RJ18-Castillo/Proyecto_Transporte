Scripts utilizados para crear la base de datos:

USE GestorDeTransporte;
GO

--------------------------------------------------
-------------------CREAR TABLAS-------------------
--------------------------------------------------

CREATE TABLE Usuarios
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario NVARCHAR(100) NOT NULL,
    Clave NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) NOT NULL,
    Rol NVARCHAR(50) NOT NULL,
    IntentosFallidos INT NOT NULL DEFAULT 0,
    Bloqueado BIT NOT NULL DEFAULT 0,
    FechaBloqueo DATETIME NULL
);
GO

CREATE TABLE Choferes
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Identificacion NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) NOT NULL,
    UsuarioId INT NOT NULL,
    CONSTRAINT FK_Chofer_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(Id)
);
GO

CREATE TABLE Pasajeros
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Identificacion NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) NOT NULL,
    UsuarioId INT NOT NULL,
    CONSTRAINT FK_Pasajero_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(Id)
);
GO

CREATE TABLE Rutas
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Origen NVARCHAR(100) NOT NULL,
    Destino NVARCHAR(100) NOT NULL,
    DuracionEstimada TIME NOT NULL,
    PrecioBase DECIMAL(18,2) NOT NULL
);
GO

CREATE TABLE Unidades
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Placa NVARCHAR(20) NOT NULL,
    Modelo NVARCHAR(100) NOT NULL,
    AnioFabricacion INT NOT NULL,
    CapacidadPasajeros INT NOT NULL,
    CONSTRAINT UQ_Unidades_Placa UNIQUE (Placa)
);
GO

--------------------------------------------------
------------------INSERTAR DATOS------------------
--------------------------------------------------

INSERT INTO Usuarios
(
    NombreUsuario,
    Clave,
    Correo,
    Rol,
    IntentosFallidos,
    Bloqueado,
    FechaBloqueo
)
VALUES
(
    'Administrador',
    'TicoBus2025*',
    'proyectolenguajes2026@gmail.com',
    'Administrador',
    0,
    0,
    NULL
),
(
    'chavezangulo9@gmail.com',
    '33f99b9f',
    'chavezangulo9@gmail.com',
    'Chofer',
    0,
    0,
    NULL
),
(
    'joseelchido85@gmail.com',
    'd0018664',
    'joseelchido85@gmail.com',
    'Pasajero',
    0,
    0,
    NULL
);
GO

INSERT INTO Choferes
(
    Identificacion,
    Nombre,
    Apellidos,
    Correo,
    UsuarioId
)
VALUES
(
    '901180430',
    'Martin',
    'Chavez Angulo',
    'chavezangulo9@gmail.com',
    2
);
GO
