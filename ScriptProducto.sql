USE [IngDeSoft]
GO

--alter table Producto add PrecioActual float

-- ============================================================
-- Tabla: Producto
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--CREATE TABLE [dbo].[Producto] (
--    [Id]              UNIQUEIDENTIFIER  NOT NULL,
--    [Nombre]          NVARCHAR(200)     NOT NULL,
--    [PrecioActual]    FLOAT             NOT NULL,
--    [Marca]           NVARCHAR(100)     NOT NULL,
--    [Modelo]          NVARCHAR(100)     NOT NULL,
--    [AceptaMayorista] BIT               NOT NULL,
--    [AceptaMinorista] BIT               NOT NULL,
--    [Stock]           INT               NOT NULL,
--    CONSTRAINT [PK_Producto] PRIMARY KEY CLUSTERED ([Id] ASC)
--        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,
--              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--GO

-- ============================================================
-- SP: ObtenerProducto
-- Busca un producto por Id o por Nombre (exacto).
-- Usado internamente en BuscarProducto y ModificarProducto.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerProducto]
    @Id     UNIQUEIDENTIFIER,
    @Nombre NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nombre,
        PrecioActual,
        Marca,
        Modelo,
        AceptaMayorista,
        AceptaMinorista,
        Stock
    FROM dbo.Producto
    WHERE
        (@Id != '00000000-0000-0000-0000-000000000000' AND Id = @Id)
        OR (@Nombre != '' AND Nombre = @Nombre);
END
GO

-- ============================================================
-- SP: ObtenerProductos
-- Retorna todos los productos. Usado en BuscarProductos
-- cuando se llama con lista vacía.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerProductos]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nombre,
        PrecioActual,
        Marca,
        Modelo,
        AceptaMayorista,
        AceptaMinorista,
        Stock
    FROM dbo.Producto;
END
GO

-- ============================================================
-- SP: BuscarProductosValidos
-- Filtra productos por los campos provistos; los campos con
-- valor por defecto (Guid.Empty, cadena vacía, 0) se ignoran
-- como criterio de búsqueda. Los booleanos actúan como filtro
-- solo cuando valen 1 (true).
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[BuscarProductosValidos]
    @Id              UNIQUEIDENTIFIER,
    @Nombre          NVARCHAR(200),
    @Marca           NVARCHAR(100),
    @Modelo          NVARCHAR(100),
    @PrecioActual    FLOAT,
    @AceptaMayorista BIT,
    @AceptaMinorista BIT,
    @Stock           INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nombre,
        PrecioActual,
        Marca,
        Modelo,
        AceptaMayorista,
        AceptaMinorista,
        Stock
    FROM dbo.Producto
    WHERE
        (@Id = '00000000-0000-0000-0000-000000000000' OR Id = @Id)
        AND (@Nombre = ''          OR Nombre LIKE '%' + @Nombre + '%')
        AND (@Marca = ''           OR Marca  LIKE '%' + @Marca  + '%')
        AND (@Modelo = ''          OR Modelo LIKE '%' + @Modelo + '%')
        AND (@PrecioActual = 0     OR PrecioActual    = @PrecioActual)
        AND (@AceptaMayorista = 0  OR AceptaMayorista = @AceptaMayorista)
        AND (@AceptaMinorista = 0  OR AceptaMinorista = @AceptaMinorista)
        AND (@Stock = 0            OR Stock            = @Stock);
END
GO

-- ============================================================
-- SP: CrearProducto
-- Inserta un nuevo producto. La transacción es manejada
-- desde el repositorio (IniciarTransaccion / AceptarTransaccion).
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CrearProducto]
    @Id              UNIQUEIDENTIFIER,
    @Nombre          NVARCHAR(200),
    @PrecioActual    FLOAT,
    @Marca           NVARCHAR(100),
    @Modelo          NVARCHAR(100),
    @AceptaMayorista BIT,
    @AceptaMinorista BIT,
    @Stock           INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO dbo.Producto (Id, Nombre, PrecioActual, Marca, Modelo, AceptaMayorista, AceptaMinorista, Stock)
        VALUES (@Id, @Nombre, @PrecioActual, @Marca, @Modelo, @AceptaMayorista, @AceptaMinorista, @Stock);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: EliminarProducto
-- Elimina un producto por su Id (verificación de existencia
-- hecha previamente en el repositorio).
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EliminarProducto]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DELETE FROM dbo.Producto
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- ============================================================
-- SP: ModificarProducto
-- Actualiza todos los campos de un producto. El repositorio
-- ya resolvió los valores finales antes de llamar a este SP.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ModificarProducto]
    @Id              UNIQUEIDENTIFIER,
    @Nombre          NVARCHAR(200),
    @PrecioActual    FLOAT,
    @Marca           NVARCHAR(100),
    @Modelo          NVARCHAR(100),
    @AceptaMayorista BIT,
    @AceptaMinorista BIT,
    @Stock           INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE dbo.Producto
        SET
            Nombre          = @Nombre,
            PrecioActual    = @PrecioActual,
            Marca           = @Marca,
            Modelo          = @Modelo,
            AceptaMayorista = @AceptaMayorista,
            AceptaMinorista = @AceptaMinorista,
            Stock           = @Stock
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
