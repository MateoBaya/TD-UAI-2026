USE [IngDeSoft]
GO

-- ============================================================
-- CORRECCIONES A TABLAS
-- FechaEntrega es nullable porque al rechazar un carrito no
-- corresponde una fecha de entrega.
-- ============================================================

--ALTER TABLE [dbo].[Venta] ALTER COLUMN [FechaEntrega] DATETIME NULL
--GO


-- ============================================================
-- CORRECCIONES A SPs EXISTENTES
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
-- Cambia el estado del carrito minorista activo a 'Aceptado'
-- e inserta el registro en Venta con el usuario aprobador.
-- @FechaEntrega: fecha acordada con el comprador.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AceptarCarritoMinorista]
    @UsuarioId    UNIQUEIDENTIFIER,
    @FechaEntrega DATETIME
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

        INSERT INTO dbo.Venta (Id, CarritoId, UsuarioAprobadorId, EstadoId, FechaUpdate, FechaEntrega)
        VALUES (NEWID(), @CarritoId, @UsuarioId, @EstadoId, GETDATE(), @FechaEntrega);

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: RechazarCarritoMinorista (corregido)
-- Cambia el estado del carrito minorista activo a 'Rechazado'
-- e inserta el registro en Venta. FechaEntrega es NULL porque
-- no hay entrega en un rechazo.
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
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        INSERT INTO dbo.Venta (Id, CarritoId, UsuarioAprobadorId, EstadoId, FechaUpdate, FechaEntrega)
        VALUES (NEWID(), @CarritoId, @UsuarioId, @EstadoId, GETDATE(), NULL);

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
-- Cambia el estado del carrito mayorista activo a 'Aceptado'
-- e inserta el registro en Venta con el usuario aprobador.
-- @FechaEntrega: fecha acordada con el comprador.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AceptarCarritoMayorista]
    @UsuarioId    UNIQUEIDENTIFIER,
    @FechaEntrega DATETIME
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

        INSERT INTO dbo.Venta (Id, CarritoId, UsuarioAprobadorId, EstadoId, FechaUpdate, FechaEntrega)
        VALUES (NEWID(), @CarritoId, @UsuarioId, @EstadoId, GETDATE(), @FechaEntrega);

        SELECT CAST(1 AS BIT);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: RechazarCarritoMayorista (corregido)
-- Cambia el estado del carrito mayorista activo a 'Rechazado'
-- e inserta el registro en Venta. FechaEntrega es NULL porque
-- no hay entrega en un rechazo.
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
        SET EstadoId    = @EstadoId,
            FechaUpdate = GETDATE()
        WHERE Id = @CarritoId;

        INSERT INTO dbo.Venta (Id, CarritoId, UsuarioAprobadorId, EstadoId, FechaUpdate, FechaEntrega)
        VALUES (NEWID(), @CarritoId, @UsuarioId, @EstadoId, GETDATE(), NULL);

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
-- Cambia el estado de un carrito especifico a 'Aceptado' e
-- inserta el registro en Venta con el usuario aprobador y la
-- fecha de entrega, todo en una unica transaccion.
-- Retorna 1 si exitoso, 0 si el carrito no existe o no esta
-- en estado Pendiente.
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
        SET EstadoId    = @EstadoAceptadoId,
            FechaUpdate = GETDATE()
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


-- ============================================================
-- NUEVOS SPs PARA EL MODULO MIS PEDIDOS
-- ============================================================

-- ============================================================
-- SP: ObtenerPedidosPorUsuario
-- Retorna todos los carritos (minoristas y mayoristas) del
-- usuario indicado, con tipo, estado y fecha de entrega
-- (via LEFT JOIN con Venta, ya que solo existe cuando el
-- carrito fue aceptado o rechazado).
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerPedidosPorUsuario]
    @UsuarioId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.Id,
        c.NroCarrito,
        ct.Nombre                                                AS Tipo,
        e.Nombre                                                 AS Estado,
        c.FechaInsert,
        ISNULL(CONVERT(VARCHAR(10), v.FechaEntrega, 103), '-')  AS FechaEntregaTexto
    FROM dbo.Carrito c
    INNER JOIN dbo.CarritoTipo ct ON c.CarritoTipoId = ct.Id
    INNER JOIN dbo.Estado e       ON c.EstadoId      = e.Id
    LEFT  JOIN dbo.Venta v        ON v.CarritoId     = c.Id
    WHERE c.UsuarioId = @UsuarioId
    ORDER BY c.FechaInsert DESC;
END
GO

-- ============================================================
-- SP: ObtenerDetallePedidoPorId
-- Retorna los items de un carrito con el nombre del producto,
-- cantidad, precio unitario y subtotal calculado.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerDetallePedidoPorId]
    @CarritoId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ci.Id,
        p.Nombre                               AS NombreProducto,
        ci.Cantidad,
        ci.Precio,
        CAST(ci.Cantidad * ci.Precio AS FLOAT) AS Subtotal
    FROM dbo.CarritoItem ci
    INNER JOIN dbo.Producto p ON ci.ProductoId = p.Id
    WHERE ci.CarritoId = @CarritoId;
END
GO
