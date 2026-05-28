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

CREATE TABLE EstadosViaje (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);GO

INSERT INTO EstadosViaje (Nombre)
VALUES 
('Programado'),
('En Curso'),
('Completado'),
('Cancelado');
GO

CREATE TABLE Viajes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RutaId INT NOT NULL,
    UnidadId INT NOT NULL,
    ChoferId INT NOT NULL,
    EstadoViajeId INT NOT NULL,
    FechaHoraSalida DATETIME NOT NULL,
    FechaHoraLlegadaEstimada DATETIME NOT NULL,
    MotivoCancelacion NVARCHAR(300) NULL,
    FechaCancelacion DATETIME NULL,

    CONSTRAINT FK_Viajes_Rutas FOREIGN KEY (RutaId) REFERENCES Rutas(Id),
    CONSTRAINT FK_Viajes_Unidades FOREIGN KEY (UnidadId) REFERENCES Unidades(Id),
    CONSTRAINT FK_Viajes_Choferes FOREIGN KEY (ChoferId) REFERENCES Choferes(Id),
    CONSTRAINT FK_Viajes_Estados FOREIGN KEY (EstadoViajeId) REFERENCES EstadosViaje(Id)
);GO


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

-------------DATOS USADOS PARA PRUEBAS DE MODULO 6---------------
INSERT INTO Usuarios 
(NombreUsuario, Clave, Correo, Rol, IntentosFallidos, Bloqueado, FechaBloqueo)
VALUES
('admin', '123', 'admin@transporte.com', 'Administrador', 0, 0, NULL),
('chofer1@transporte.com', '123', 'chofer1@transporte.com', 'Chofer', 0, 0, NULL),
('chofer2@transporte.com', '123', 'chofer2@transporte.com', 'Chofer', 0, 0, NULL),
('pasajero1@transporte.com', '123', 'pasajero1@transporte.com', 'Pasajero', 0, 0, NULL);


/* CHOFERES */
INSERT INTO Choferes
(Identificacion, Nombre, Apellidos, Correo, UsuarioId)
VALUES
('101110111', 'Carlos', 'Ramírez Mora', 'chofer1@transporte.com', 2),
('202220222', 'Andrés', 'Solano Vega', 'chofer2@transporte.com', 3);


/* PASAJEROS */
INSERT INTO Pasajeros
(Identificacion, Nombre, Apellidos, Correo, UsuarioId)
VALUES
('303330333', 'María', 'Gómez Rojas', 'pasajero1@transporte.com', 4);


/* RUTAS */
INSERT INTO Rutas
(Nombre, Origen, Destino, DuracionEstimada, PrecioBase)
VALUES
('San José - Liberia', 'San José', 'Liberia', '04:30:00', 5000),
('Liberia - Nicoya', 'Liberia', 'Nicoya', '02:00:00', 2500),
('San José - Puntarenas', 'San José', 'Puntarenas', '02:15:00', 3200);


/* UNIDADES */
INSERT INTO Unidades
(Placa, Modelo, AnioFabricacion, CapacidadPasajeros)
VALUES
('BUS001', 'Mercedes Benz', 2022, 45),
('BUS002', 'Hyundai County', 2021, 30),
('BUS003', 'Volvo B270F', 2023, 50);

