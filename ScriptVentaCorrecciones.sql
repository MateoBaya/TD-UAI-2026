USE [IngDeSoft]
GO

-- ============================================================
-- CORRECCIONES A TABLAS
-- ============================================================

-- Agrega columna para registrar quién aprobó/rechazó el carrito.
-- Ejecutar una sola vez.
--ALTER TABLE [dbo].[Carrito] ADD [UsuarioAprobadorId] UNIQUEIDENTIFIER NULL
--GO
--ALTER TABLE [dbo].[Carrito] ADD CONSTRAINT [FK_Carrito_UsuarioAprobador]
--    FOREIGN KEY ([UsuarioAprobadorId]) REFERENCES [dbo].[Usuario] ([Id])
--GO


-- ============================================================
-- CORRECCIONES A SPs EXISTENTES
-- Todos reciben ahora el Id del usuario que realiza la acción.
-- ============================================================

-- ============================================================
-- SP: CrearCarritoMinorista (corregido)
-- Agrega @UsuarioId para registrar al comprador en Carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CrearCarritoMinorista]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME,
    @UsuarioId   UNIQUEIDENTIFIER
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
        INSERT INTO dbo.Carrito (Id, UsuarioId, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @UsuarioId, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: CrearCarritoMinoristaConItem (corregido)
-- Agrega @UsuarioId para registrar al comprador en Carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CrearCarritoMinoristaConItem]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME,
    @UsuarioId   UNIQUEIDENTIFIER,
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
        INSERT INTO dbo.Carrito (Id, UsuarioId, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @UsuarioId, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);

        INSERT INTO dbo.CarritoItem (Id, CarritoId, ProductoId, Cantidad, Precio)
        VALUES (NEWID(), @Id, @ProductoId, @Cantidad, @Precio);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: AceptarCarritoMinorista (corregido)
-- Agrega @UsuarioId para registrar quién aprobó el carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AceptarCarritoMinorista]
    @UsuarioId UNIQUEIDENTIFIER
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
        SET EstadoId          = @EstadoId,
            UsuarioAprobadorId = @UsuarioId,
            FechaUpdate        = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: RechazarCarritoMinorista (corregido)
-- Agrega @UsuarioId para registrar quién rechazó el carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[RechazarCarritoMinorista]
    @UsuarioId UNIQUEIDENTIFIER
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
        SET EstadoId          = @EstadoId,
            UsuarioAprobadorId = @UsuarioId,
            FechaUpdate        = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: CrearCarritoMayorista (corregido)
-- Agrega @UsuarioId para registrar al comprador en Carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CrearCarritoMayorista]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME,
    @UsuarioId   UNIQUEIDENTIFIER
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
        INSERT INTO dbo.Carrito (Id, UsuarioId, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @UsuarioId, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: CrearCarritoMayoristaConItem (corregido)
-- Agrega @UsuarioId para registrar al comprador en Carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CrearCarritoMayoristaConItem]
    @Id          UNIQUEIDENTIFIER,
    @FechaInsert DATETIME,
    @UsuarioId   UNIQUEIDENTIFIER,
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
        INSERT INTO dbo.Carrito (Id, UsuarioId, EstadoId, CarritoTipoId, FechaInsert, FechaUpdate)
        VALUES (@Id, @UsuarioId, @EstadoId, @CarritoTipoId, @FechaInsert, @FechaInsert);

        INSERT INTO dbo.CarritoItem (Id, CarritoId, ProductoId, Cantidad, Precio)
        VALUES (NEWID(), @Id, @ProductoId, @Cantidad, @Precio);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: AceptarCarritoMayorista (corregido)
-- Agrega @UsuarioId para registrar quién aprobó el carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AceptarCarritoMayorista]
    @UsuarioId UNIQUEIDENTIFIER
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
        SET EstadoId          = @EstadoId,
            UsuarioAprobadorId = @UsuarioId,
            FechaUpdate        = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: RechazarCarritoMayorista (corregido)
-- Agrega @UsuarioId para registrar quién rechazó el carrito.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[RechazarCarritoMayorista]
    @UsuarioId UNIQUEIDENTIFIER
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
        SET EstadoId          = @EstadoId,
            UsuarioAprobadorId = @UsuarioId,
            FechaUpdate        = GETDATE()
        WHERE Id = @CarritoId;

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


-- ============================================================
-- NUEVOS SPs PARA LA PANTALLA DE APROBACION DE CARRITOS
-- ============================================================

-- ============================================================
-- SP: ObtenerCarritosPendientesMinorista
-- Retorna todos los carritos minoristas en estado Pendiente,
-- ordenados por fecha de insercion descendente.
-- Usado en la pantalla de aprobacion para listar los carritos
-- que esperan revisión del vendedor.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerCarritosPendientesMinorista]
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
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    WHERE e.Nombre = 'Pendiente' AND ct.Nombre = 'Minorista'
    ORDER BY c.FechaInsert DESC;
END
GO

-- ============================================================
-- SP: MostrarDetalleCarritoPorId
-- Retorna los items de un carrito especifico por su Id.
-- Usado en la pantalla de aprobacion para mostrar el contenido
-- del carrito seleccionado por el vendedor.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MostrarDetalleCarritoPorId]
    @CarritoId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

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
-- SP: AceptarCarritoPorId
-- Cambia el estado de un carrito especifico a 'Aceptado',
-- registra al usuario que realizó la acción e inserta el
-- registro correspondiente en la tabla Venta, todo en una
-- única transacción.
-- @FechaEntrega: fecha de entrega acordada con el comprador,
-- ingresada por el vendedor en la pantalla de aprobacion.
-- Retorna 1 si fue exitoso, 0 si el carrito no existe o ya
-- no está en estado Pendiente.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AceptarCarritoPorId]
    @CarritoId    UNIQUEIDENTIFIER,
    @UsuarioId    UNIQUEIDENTIFIER,
    @FechaEntrega DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoActualNombre NVARCHAR(100);
    DECLARE @EstadoAceptadoId   UNIQUEIDENTIFIER;

    SELECT @EstadoActualNombre = e.Nombre
    FROM dbo.Carrito c
    INNER JOIN dbo.Estado e ON c.EstadoId = e.Id
    WHERE c.Id = @CarritoId;

    IF @EstadoActualNombre IS NULL OR @EstadoActualNombre != 'Pendiente'
    BEGIN
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    SELECT @EstadoAceptadoId = Id FROM dbo.Estado WHERE Nombre = 'Aceptado';

    BEGIN TRY
        UPDATE dbo.Carrito
        SET EstadoId           = @EstadoAceptadoId,
            UsuarioAprobadorId = @UsuarioId,
            FechaUpdate        = GETDATE()
        WHERE Id = @CarritoId;

        INSERT INTO dbo.Venta (Id, CarritoId, UsuarioAprobadorId, EstadoId, FechaUpdate, FechaEntrega)
        VALUES (NEWID(), @CarritoId, @UsuarioId, @EstadoAceptadoId, GETDATE(), @FechaEntrega);

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
