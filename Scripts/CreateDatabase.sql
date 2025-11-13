-- =============================================
-- Script de Criação do Banco de Dados
-- Sistema: Troca de Baterias
-- =============================================

-- Criar banco de dados (se não existir)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TrocaBateriaDB')
BEGIN
    CREATE DATABASE TrocaBateriaDB;
    PRINT 'Banco de dados TrocaBateriaDB criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Banco de dados TrocaBateriaDB já existe.';
END
GO

USE TrocaBateriaDB;
GO


PRINT 'Use o comando: dotnet ef database update';
PRINT 'Para criar o banco de dados automaticamente.';
GO
