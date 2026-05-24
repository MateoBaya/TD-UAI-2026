USE [IngDeSoft]
GO

---- ============================================================
---- Tabla: Estado
---- ============================================================
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

--CREATE TABLE [dbo].[Estado] (
--    [Id]          UNIQUEIDENTIFIER NOT NULL,
--    [Nombre]      NVARCHAR(100)    NOT NULL,
--    [Descripcion] NVARCHAR(500)    NOT NULL,
--    CONSTRAINT [PK_Estado] PRIMARY KEY CLUSTERED ([Id] ASC)
--        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
--              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--GO

---- Valores iniciales de Estado
--INSERT INTO [dbo].[Estado] (Id, Nombre, Descripcion) VALUES (NEWID(), 'Pendiente',  'Carrito en curso')
--INSERT INTO [dbo].[Estado] (Id, Nombre, Descripcion) VALUES (NEWID(), 'Aceptado',   'Carrito aprobado')
--INSERT INTO [dbo].[Estado] (Id, Nombre, Descripcion) VALUES (NEWID(), 'Rechazado',  'Carrito rechazado')
--INSERT INTO [dbo].[Estado] (Id, Nombre, Descripcion) VALUES (NEWID(), 'Finalizado', 'Carrito finalizado')
--GO

---- ============================================================
---- Tabla: CarritoTipo
---- ============================================================
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

--CREATE TABLE [dbo].[CarritoTipo] (
--    [Id]          UNIQUEIDENTIFIER NOT NULL,
--    [Nombre]      NVARCHAR(100)    NOT NULL,
--    [Descripcion] NVARCHAR(500)    NOT NULL,
--    CONSTRAINT [PK_CarritoTipo] PRIMARY KEY CLUSTERED ([Id] ASC)
--        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
--              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--GO

-- --Valores iniciales de CarritoTipo
--INSERT INTO [dbo].[CarritoTipo] (Id, Nombre, Descripcion) VALUES (NEWID(), 'Minorista', 'Venta al consumidor final')
--INSERT INTO [dbo].[CarritoTipo] (Id, Nombre, Descripcion) VALUES (NEWID(), 'Mayorista', 'Venta en grandes volumenes')
--GO

---- ============================================================
---- Tabla: Carrito
---- ============================================================
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

--CREATE TABLE [dbo].[Carrito] (
--    [Id]            UNIQUEIDENTIFIER NOT NULL,
--    [NroCarrito]    INT              NOT NULL IDENTITY(1,1),
--    [UsuarioId]     int NULL,
--    [EstadoId]      UNIQUEIDENTIFIER NOT NULL,
--    [CarritoTipoId] UNIQUEIDENTIFIER NOT NULL,
--    [FechaInsert]   DATETIME         NOT NULL,
--    [FechaUpdate]   DATETIME         NOT NULL,
--    CONSTRAINT [PK_Carrito]              PRIMARY KEY CLUSTERED ([Id] ASC)
--        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
--              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
--    CONSTRAINT [FK_Carrito_Estado]       FOREIGN KEY ([EstadoId])      REFERENCES [dbo].[Estado]      ([Id]),
--    CONSTRAINT [FK_Carrito_CarritoTipo]  FOREIGN KEY ([CarritoTipoId]) REFERENCES [dbo].[CarritoTipo] ([Id]),
--    CONSTRAINT [FK_Carrito_Usuario]      FOREIGN KEY ([UsuarioId])     REFERENCES [dbo].[Usuario]    ([Id])
--) ON [PRIMARY]
--GO

---- ============================================================
---- Tabla: CarritoItem
---- ============================================================
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

--CREATE TABLE [dbo].[CarritoItem] (
--    [Id]         UNIQUEIDENTIFIER NOT NULL,
--    [CarritoId]  UNIQUEIDENTIFIER NOT NULL,
--    [ProductoId] UNIQUEIDENTIFIER NOT NULL,
--    [Cantidad]   INT              NOT NULL,
--    [Precio]     FLOAT            NOT NULL,
--    CONSTRAINT [PK_CarritoItem]          PRIMARY KEY CLUSTERED ([Id] ASC)
--        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
--              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
--    CONSTRAINT [FK_CarritoItem_Carrito]  FOREIGN KEY ([CarritoId])  REFERENCES [dbo].[Carrito]  ([Id]),
--    CONSTRAINT [FK_CarritoItem_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id])
--) ON [PRIMARY]
--GO

---- ============================================================
---- Tabla: Venta
---- ============================================================
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

--CREATE TABLE [dbo].[Venta] (
--    [Id]                 UNIQUEIDENTIFIER NOT NULL,
--    [CarritoId]          UNIQUEIDENTIFIER NOT NULL,
--    [UsuarioAprobadorId] int NOT NULL,
--    [EstadoId]           UNIQUEIDENTIFIER NOT NULL,
--    [FechaUpdate]        DATETIME         NOT NULL,
--    [FechaEntrega]       DATETIME         NOT NULL,
--    CONSTRAINT [PK_Venta]                     PRIMARY KEY CLUSTERED ([Id] ASC)
--        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
--              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
--    CONSTRAINT [FK_Venta_Carrito]             FOREIGN KEY ([CarritoId])          REFERENCES [dbo].[Carrito]  ([Id]),
--    CONSTRAINT [FK_Venta_UsuarioAprobador]    FOREIGN KEY ([UsuarioAprobadorId]) REFERENCES [dbo].[Usuario] ([Id]),
--    CONSTRAINT [FK_Venta_Estado]              FOREIGN KEY ([EstadoId])           REFERENCES [dbo].[Estado]   ([Id])
--) ON [PRIMARY]
--GO


-- ============================================================
-- SP: AgregarCantidadesItemCarrito
-- Inserta un item al carrito activo (Pendiente mas reciente).
-- Si ya existe un item con el mismo ProductoId en ese carrito,
-- incrementa la cantidad; de lo contrario inserta uno nuevo.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AgregarCantidadesItemCarrito]
    @Id         UNIQUEIDENTIFIER,
    @ProductoId UNIQUEIDENTIFIER,
    @Cantidad   INT,
    @Precio     FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e ON c.EstadoId = e.Id
    WHERE e.Nombre = 'Pendiente'
    ORDER BY c.FechaInsert DESC;

    IF @CarritoId IS NULL
        RAISERROR('No hay un carrito activo en estado Pendiente.', 16, 1);

    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM dbo.CarritoItem
            WHERE CarritoId = @CarritoId AND ProductoId = @ProductoId
        )
        BEGIN
            UPDATE dbo.CarritoItem
            SET Cantidad = Cantidad + @Cantidad,
                Precio   = @Precio
            WHERE CarritoId = @CarritoId AND ProductoId = @ProductoId;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.CarritoItem (Id, CarritoId, ProductoId, Cantidad, Precio)
            VALUES (@Id, @CarritoId, @ProductoId, @Cantidad, @Precio);
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: BuscarCarrito
-- Busca un carrito por Id o por NroCarrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[BuscarCarrito]
    @Id         UNIQUEIDENTIFIER,
    @NroCarrito INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.Id,
        c.NroCarrito,
        c.UsuarioId,
        c.EstadoId,
        c.CarritoTipoId,
        c.FechaInsert,
        c.FechaUpdate
    FROM dbo.Carrito c
    WHERE
        (@Id != '00000000-0000-0000-0000-000000000000' AND c.Id = @Id)
        OR (@NroCarrito != 0 AND c.NroCarrito = @NroCarrito);
END
GO

-- ============================================================
-- SP: BuscarCarritosFiltrados
-- Retorna carritos filtrados por usuario y rango de fechas.
-- Los parametros con valor por defecto se ignoran como filtro.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[BuscarCarritosFiltrados]
    @UsuarioId  UNIQUEIDENTIFIER,
    @FechaDesde DATETIME,
    @FechaHasta DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.Id,
        c.NroCarrito,
        c.UsuarioId,
        c.EstadoId,
        c.CarritoTipoId,
        c.FechaInsert,
        c.FechaUpdate
    FROM dbo.Carrito c
    WHERE
        (@UsuarioId = '00000000-0000-0000-0000-000000000000' OR c.UsuarioId = @UsuarioId)
        AND c.FechaInsert >= @FechaDesde
        AND c.FechaInsert <= @FechaHasta
    ORDER BY c.FechaInsert DESC;
END
GO


-- ============================================================
-- SP: MostrarDetalleCarritoMinorista
-- Retorna los items del carrito minorista activo (Pendiente
-- mas reciente con CarritoTipo = 'Minorista').
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MostrarDetalleCarritoMinorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e      ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Minorista'
    ORDER BY c.FechaInsert DESC;

    SELECT
        ci.Id,
        ci.CarritoId,
        ci.ProductoId,
        ci.Cantidad,
        ci.Precio
    FROM dbo.CarritoItem ci
    WHERE ci.CarritoId = @CarritoId;
END
GO

-- ============================================================
-- SP: CrearCarritoMinorista
-- Crea un carrito minorista vacio en estado Pendiente.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CrearCarritoMinorista]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoId      UNIQUEIDENTIFIER;
    DECLARE @CarritoTipoId UNIQUEIDENTIFIER;

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Pendiente';
    IF @EstadoId IS NULL
        RAISERROR('Estado Pendiente no encontrado en la tabla Estado.', 16, 1);

    SELECT @CarritoTipoId = Id FROM dbo.CarritoTipo WHERE Nombre = 'Minorista';
    IF @CarritoTipoId IS NULL
        RAISERROR('Tipo Minorista no encontrado en la tabla CarritoTipo.', 16, 1);

    BEGIN TRY
        INSERT INTO dbo.Carrito (Id, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: CrearCarritoMinoristaConItem
-- Crea un carrito minorista en estado Pendiente e inserta
-- el primer item en una sola operacion.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CrearCarritoMinoristaConItem]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME,
    @ProductoId  UNIQUEIDENTIFIER,
    @Cantidad    INT,
    @Precio      FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoId      UNIQUEIDENTIFIER;
    DECLARE @CarritoTipoId UNIQUEIDENTIFIER;

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Pendiente';
    IF @EstadoId IS NULL
        RAISERROR('Estado Pendiente no encontrado en la tabla Estado.', 16, 1);

    SELECT @CarritoTipoId = Id FROM dbo.CarritoTipo WHERE Nombre = 'Minorista';
    IF @CarritoTipoId IS NULL
        RAISERROR('Tipo Minorista no encontrado en la tabla CarritoTipo.', 16, 1);

    BEGIN TRY
        INSERT INTO dbo.Carrito (Id, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);

        INSERT INTO dbo.CarritoItem (Id, CarritoId, ProductoId, Cantidad, Precio)
        VALUES (NEWID(), @Id, @ProductoId, @Cantidad, @Precio);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: ValidarItemMinorista
-- Verifica que el producto acepte venta minorista y que el
-- stock disponible sea suficiente para la cantidad solicitada.
-- Retorna 1 si valido, 0 si no.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ValidarItemMinorista]
    @ProductoId UNIQUEIDENTIFIER,
    @Cantidad   INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM dbo.Producto
        WHERE Id = @ProductoId
          AND AceptaMinorista = 1
          AND Stock >= @Cantidad
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END
GO

-- ============================================================
-- SP: AceptarCarritoMinorista
-- Cambia el estado del carrito minorista activo a 'Aceptado'.
-- Retorna 1 si la operacion fue exitosa, 0 si no habia carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AceptarCarritoMinorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;
    DECLARE @EstadoId  UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Minorista'
    ORDER BY c.FechaInsert DESC;

    IF @CarritoId IS NULL
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Aceptado';

    BEGIN TRY
        UPDATE dbo.Carrito
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: RechazarCarritoMinorista
-- Cambia el estado del carrito minorista activo a 'Rechazado'.
-- Retorna 1 si la operacion fue exitosa, 0 si no habia carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RechazarCarritoMinorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;
    DECLARE @EstadoId  UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Minorista'
    ORDER BY c.FechaInsert DESC;

    IF @CarritoId IS NULL
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Rechazado';

    BEGIN TRY
        UPDATE dbo.Carrito
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: FinalizarCarritoMinorista
-- Cambia el estado del carrito minorista activo a 'Finalizado'.
-- Retorna 1 si la operacion fue exitosa, 0 si no habia carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[FinalizarCarritoMinorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;
    DECLARE @EstadoId  UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Minorista'
    ORDER BY c.FechaInsert DESC;

    IF @CarritoId IS NULL
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Finalizado';

    BEGIN TRY
        UPDATE dbo.Carrito
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


-- ============================================================
-- SP: MostrarDetalleCarritoMayorista
-- Retorna los items del carrito mayorista activo (Pendiente
-- mas reciente con CarritoTipo = 'Mayorista').
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MostrarDetalleCarritoMayorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Mayorista'
    ORDER BY c.FechaInsert DESC;

    SELECT
        ci.Id,
        ci.CarritoId,
        ci.ProductoId,
        ci.Cantidad,
        ci.Precio
    FROM dbo.CarritoItem ci
    WHERE ci.CarritoId = @CarritoId;
END
GO

-- ============================================================
-- SP: CrearCarritoMayorista
-- Crea un carrito mayorista vacio en estado Pendiente.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CrearCarritoMayorista]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoId      UNIQUEIDENTIFIER;
    DECLARE @CarritoTipoId UNIQUEIDENTIFIER;

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Pendiente';
    IF @EstadoId IS NULL
        RAISERROR('Estado Pendiente no encontrado en la tabla Estado.', 16, 1);

    SELECT @CarritoTipoId = Id FROM dbo.CarritoTipo WHERE Nombre = 'Mayorista';
    IF @CarritoTipoId IS NULL
        RAISERROR('Tipo Mayorista no encontrado en la tabla CarritoTipo.', 16, 1);

    BEGIN TRY
        INSERT INTO dbo.Carrito (Id, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: CrearCarritoMayoristaConItem
-- Crea un carrito mayorista en estado Pendiente e inserta
-- el primer item en una sola operacion.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CrearCarritoMayoristaConItem]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME,
    @ProductoId  UNIQUEIDENTIFIER,
    @Cantidad    INT,
    @Precio      FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoId      UNIQUEIDENTIFIER;
    DECLARE @CarritoTipoId UNIQUEIDENTIFIER;

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Pendiente';
    IF @EstadoId IS NULL
        RAISERROR('Estado Pendiente no encontrado en la tabla Estado.', 16, 1);

    SELECT @CarritoTipoId = Id FROM dbo.CarritoTipo WHERE Nombre = 'Mayorista';
    IF @CarritoTipoId IS NULL
        RAISERROR('Tipo Mayorista no encontrado en la tabla CarritoTipo.', 16, 1);

    BEGIN TRY
        INSERT INTO dbo.Carrito (Id, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);

        INSERT INTO dbo.CarritoItem (Id, CarritoId, ProductoId, Cantidad, Precio)
        VALUES (NEWID(), @Id, @ProductoId, @Cantidad, @Precio);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: ValidarItemMayorista
-- Verifica que el producto acepte venta mayorista y que el
-- stock disponible sea suficiente para la cantidad solicitada.
-- Retorna 1 si valido, 0 si no.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ValidarItemMayorista]
    @ProductoId UNIQUEIDENTIFIER,
    @Cantidad   INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM dbo.Producto
        WHERE Id = @ProductoId
          AND AceptaMayorista = 1
          AND Stock >= @Cantidad
    )
        SELECT CAST(1 AS BIT);
    ELSE
        SELECT CAST(0 AS BIT);
END
GO

-- ============================================================
-- SP: AceptarCarritoMayorista
-- Cambia el estado del carrito mayorista activo a 'Aceptado'.
-- Retorna 1 si la operacion fue exitosa, 0 si no habia carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AceptarCarritoMayorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;
    DECLARE @EstadoId  UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Mayorista'
    ORDER BY c.FechaInsert DESC;

    IF @CarritoId IS NULL
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Aceptado';

    BEGIN TRY
        UPDATE dbo.Carrito
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: RechazarCarritoMayorista
-- Cambia el estado del carrito mayorista activo a 'Rechazado'.
-- Retorna 1 si la operacion fue exitosa, 0 si no habia carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RechazarCarritoMayorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;
    DECLARE @EstadoId  UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Mayorista'
    ORDER BY c.FechaInsert DESC;

    IF @CarritoId IS NULL
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Rechazado';

    BEGIN TRY
        UPDATE dbo.Carrito
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: FinalizarCarritoMayorista
-- Cambia el estado del carrito mayorista activo a 'Finalizado'.
-- Retorna 1 si la operacion fue exitosa, 0 si no habia carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[FinalizarCarritoMayorista]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CarritoId UNIQUEIDENTIFIER;
    DECLARE @EstadoId  UNIQUEIDENTIFIER;

    SELECT TOP 1 @CarritoId = c.Id
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Mayorista'
    ORDER BY c.FechaInsert DESC;

    IF @CarritoId IS NULL
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    SELECT @EstadoId = Id FROM dbo.Estado WHERE Nombre = 'Finalizado';

    BEGIN TRY
        UPDATE dbo.Carrito
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


-- ============================================================
-- SP: GenerarVenta
-- Registra una venta asociada a un carrito finalizado.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GenerarVenta]
    @Id                 UNIQUEIDENTIFIER,
    @CarritoId          UNIQUEIDENTIFIER,
    @UsuarioAprobadorId UNIQUEIDENTIFIER,
    @EstadoId           UNIQUEIDENTIFIER,
    @FechaUpdate        DATETIME,
    @FechaEntrega       DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO dbo.Venta (Id, CarritoId, UsuarioAprobadorId, EstadoId, FechaUpdate, FechaEntrega)
        VALUES (@Id, @CarritoId, @UsuarioAprobadorId, @EstadoId, @FechaUpdate, @FechaEntrega);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
