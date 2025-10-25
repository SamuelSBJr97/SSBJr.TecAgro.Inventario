#!/bin/bash

echo "========================================"
echo "Executando Testes - TecAgro Inventario"
echo "========================================"
echo ""

echo "[1/4] Executando testes do Domain..."
dotnet test tests/SSBJr.TecAgro.Inventario.Domain.Tests --verbosity minimal
if [ $? -ne 0 ]; then
    echo "ERRO: Testes do Domain falharam!"
 exit 1
fi
echo ""

echo "[2/4] Executando testes da Infrastructure..."
dotnet test tests/SSBJr.TecAgro.Inventario.Infrastructure.Tests --verbosity minimal
if [ $? -ne 0 ]; then
    echo "ERRO: Testes da Infrastructure falharam!"
    exit 1
fi
echo ""

echo "[3/4] Executando testes do Server..."
dotnet test tests/SSBJr.TecAgro.Inventario.Server.Tests --verbosity minimal
if [ $? -ne 0 ]; then
    echo "ERRO: Testes do Server falharam!"
    exit 1
fi
echo ""

echo "[4/4] Executando todos os testes..."
dotnet test --verbosity minimal
if [ $? -ne 0 ]; then
    echo "ERRO: Alguns testes falharam!"
    exit 1
fi
echo ""

echo "========================================"
echo "SUCESSO: Todos os testes passaram!"
echo "========================================"
