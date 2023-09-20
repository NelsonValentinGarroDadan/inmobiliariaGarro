-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 20-09-2023 a las 19:45:06
-- Versión del servidor: 10.4.27-MariaDB
-- Versión de PHP: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliariagarro`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `administradores`
--

CREATE TABLE `administradores` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL,
  `Clave` varchar(250) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `administradores`
--

INSERT INTO `administradores` (`Id`, `UsuarioId`, `Clave`, `Avatar`) VALUES
(36, 36, 'gCMSJ+9GSOk7hRlOgd3dd92s3UVNlZXoEbj5f/VnRg==', '/Uploads\\avatar_36.png');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `Id` int(11) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `InquilinoId` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `Importe` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`Id`, `FechaInicio`, `FechaFin`, `InquilinoId`, `InmuebleId`, `Importe`) VALUES
(27, '2023-09-02', '2023-09-19', 58, 17, '50001'),
(28, '2023-09-23', '2023-09-19', 58, 17, '50000'),
(30, '2023-10-03', '2023-12-09', 58, 17, '98000'),
(31, '2023-09-19', '2024-01-05', 63, 18, '50000'),
(32, '2023-09-19', '2023-09-29', 63, 19, '34000'),
(34, '2023-09-19', '2023-09-30', 67, 17, '50000'),
(35, '2023-09-02', '2023-09-20', 67, 20, '1000'),
(37, '2023-09-02', '2023-09-30', 68, 22, '1000');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `empleados`
--

CREATE TABLE `empleados` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL,
  `Clave` varchar(250) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `empleados`
--

INSERT INTO `empleados` (`Id`, `UsuarioId`, `Clave`, `Avatar`) VALUES
(61, 61, 'cH2faJaCcOHv3PL0SgvKNPU5nakY3ld92Vbaqpzw9Q==', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `CA` int(11) NOT NULL,
  `Direccion` varchar(250) NOT NULL,
  `Precio` decimal(10,0) NOT NULL,
  `PropietarioId` int(11) NOT NULL,
  `TipoUsoId` int(11) NOT NULL,
  `Latitud` varchar(50) NOT NULL,
  `Longitud` varchar(50) NOT NULL,
  `TipoEstadoId` int(11) NOT NULL,
  `TipoInmuebleId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `CA`, `Direccion`, `Precio`, `PropietarioId`, `TipoUsoId`, `Latitud`, `Longitud`, `TipoEstadoId`, `TipoInmuebleId`) VALUES
(17, 3, 'Modulo 13', '70000', 57, 12, '-35.463', '-180.0', 101, 1),
(18, 6, 'carlos gardel 654', '32000000', 64, 12, '-35.463', '-180.0', 101, 1),
(19, 2, 'Pancha Herndez 45', '450000000', 67, 12, '-35.463', '-180.0', 102, 2),
(20, 2, 'MOdulo 11', '70000', 68, 11, '-35.463', '-180.0', 101, 4),
(22, 3, 'MOdulo 23', '70000', 36, 12, '-35.463', '-180.0', 101, 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `UsuarioId`) VALUES
(58, 58),
(62, 62),
(63, 63),
(66, 66),
(67, 67),
(68, 68);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `ContratoId` int(11) NOT NULL,
  `Fecha` date NOT NULL,
  `Importe` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`Id`, `ContratoId`, `Fecha`, `Importe`) VALUES
(5, 27, '0001-01-25', '1000'),
(6, 27, '0001-01-01', '45000'),
(7, 28, '0001-01-01', '45000'),
(8, 28, '0001-01-01', '230000'),
(9, 32, '2023-09-20', '3000');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`Id`, `UsuarioId`) VALUES
(36, 36),
(57, 57),
(64, 64),
(65, 65),
(67, 67),
(68, 68);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tiposestados`
--

CREATE TABLE `tiposestados` (
  `Id` int(11) NOT NULL,
  `Descripcion` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tiposestados`
--

INSERT INTO `tiposestados` (`Id`, `Descripcion`) VALUES
(101, 'Habilitado'),
(102, 'DesHabilitado');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tiposinmuebles`
--

CREATE TABLE `tiposinmuebles` (
  `Id` int(11) NOT NULL,
  `Descripcion` varchar(40) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tiposinmuebles`
--

INSERT INTO `tiposinmuebles` (`Id`, `Descripcion`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(3, 'Galpon'),
(4, 'Estacionamiento');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tiposusos`
--

CREATE TABLE `tiposusos` (
  `Id` int(11) NOT NULL,
  `Descripcion` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tiposusos`
--

INSERT INTO `tiposusos` (`Id`, `Descripcion`) VALUES
(11, 'Comercial'),
(12, 'Residencial');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `DNI` varchar(30) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Telefono` bigint(32) NOT NULL,
  `Mail` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `DNI`, `Nombre`, `Apellido`, `Telefono`, `Mail`) VALUES
(36, '44643100', 'Nelson', 'Garro', 2664910392, 'nelsonvgarro@sanluis.edu.ar'),
(57, '12345678', 'Amargado', 'Quiroga', 12345678, 'example@example.com'),
(58, '12345678', 'Burlao', 'Segundo', 123456789, 'example@example.com'),
(59, '24983199', 'Quiricocho', 'Dadan', 2664729009, 'nicovaleauri@gmail.com'),
(60, '24983199', 'Mariela', 'Dadan', 2664729009, 'nicovaleauri@gmail.com'),
(61, '12345679', 'Aurora', 'Garro', 123456789, 'example@gmail.com'),
(62, '42914999', 'Nicolas', 'Dadan', 234567890, 'example@gmail.com'),
(63, '21876398', 'Leandro', 'Zuares', 98765432, 'example@gmail.com'),
(64, '32654786', 'Juan', 'Juan', 234523456, 'example@example.com'),
(65, '65234789', 'Gustavo', 'Sosa', 453298765, 'example@example.com'),
(66, '23456876', 'Ana', 'Videla', 243658798, 'example@gmail.com'),
(67, '23456789', 'Gustavo', 'Quiroga', 2435676767, 'example@example.com'),
(68, '12345678', 'Fernando', 'Erwin', 123456789, 'example@example.com');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `administradores`
--
ALTER TABLE `administradores`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`);

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InmuebleId` (`InmuebleId`),
  ADD KEY `contratos_ibfk_2` (`InquilinoId`);

--
-- Indices de la tabla `empleados`
--
ALTER TABLE `empleados`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdTipoUso` (`TipoUsoId`),
  ADD KEY `inmuebles_ibfk_3` (`TipoEstadoId`),
  ADD KEY `TipoInmuebleId` (`TipoInmuebleId`),
  ADD KEY `inmuebles_ibfk_1` (`PropietarioId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdContrato` (`ContratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `UsuarioId` (`UsuarioId`);

--
-- Indices de la tabla `tiposestados`
--
ALTER TABLE `tiposestados`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `tiposinmuebles`
--
ALTER TABLE `tiposinmuebles`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `tiposusos`
--
ALTER TABLE `tiposusos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `administradores`
--
ALTER TABLE `administradores`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=61;

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

--
-- AUTO_INCREMENT de la tabla `empleados`
--
ALTER TABLE `empleados`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=62;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=69;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=69;

--
-- AUTO_INCREMENT de la tabla `tiposestados`
--
ALTER TABLE `tiposestados`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=107;

--
-- AUTO_INCREMENT de la tabla `tiposinmuebles`
--
ALTER TABLE `tiposinmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `tiposusos`
--
ALTER TABLE `tiposusos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=69;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `administradores`
--
ALTER TABLE `administradores`
  ADD CONSTRAINT `administradores_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`);

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`),
  ADD CONSTRAINT `contratos_ibfk_4` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`);

--
-- Filtros para la tabla `empleados`
--
ALTER TABLE `empleados`
  ADD CONSTRAINT `empleados_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`);

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`),
  ADD CONSTRAINT `inmuebles_ibfk_2` FOREIGN KEY (`TipoUsoId`) REFERENCES `tiposusos` (`Id`),
  ADD CONSTRAINT `inmuebles_ibfk_3` FOREIGN KEY (`TipoEstadoId`) REFERENCES `tiposestados` (`Id`),
  ADD CONSTRAINT `inmuebles_ibfk_4` FOREIGN KEY (`TipoInmuebleId`) REFERENCES `tiposinmuebles` (`Id`);

--
-- Filtros para la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD CONSTRAINT `inquilinos_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`);

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`ContratoId`) REFERENCES `contratos` (`Id`);

--
-- Filtros para la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD CONSTRAINT `propietarios_ibfk_1` FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
