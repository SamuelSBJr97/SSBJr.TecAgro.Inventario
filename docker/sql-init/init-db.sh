#!/bin/bash
# Script de inicializacao do banco de dados SQL Server

# Aguardar SQL Server estar pronto
echo "Aguardando SQL Server iniciar..."
sleep 30s

# Criar banco de dados se nao existir
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "TecAgro@2025!Strong" -C -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InventarioDb')
BEGIN
    CREATE DATABASE InventarioDb;
    PRINT 'Banco de dados InventarioDb criado com sucesso';
END
ELSE
BEGIN
    PRINT 'Banco de dados InventarioDb ja existe';
END
"

echo "Inicializacao do banco de dados concluida!"
