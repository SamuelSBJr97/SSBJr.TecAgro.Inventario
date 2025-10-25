#!/bin/bash
# Script para build do App MAUI usando Docker

echo "======================================"
echo "TecAgro Inventario - Build App MAUI"
echo "======================================"
echo ""

# Verificar se Docker esta instalado
if ! command -v docker &> /dev/null; then
    echo "Erro: Docker nao encontrado. Por favor, instale o Docker primeiro."
  exit 1
fi

echo "Construindo APK do aplicativo Android..."
echo ""

# Build da imagem Docker
docker build -f Dockerfile.maui -t tecagro-maui-builder .

# Criar diretorio de output se nao existir
mkdir -p ./output/android

# Extrair o APK do container
echo "Extraindo APK..."
docker create --name temp-maui-container tecagro-maui-builder
docker cp temp-maui-container:/app/publish/. ./output/android/
docker rm temp-maui-container

echo ""
echo "======================================"
echo "Build concluido!"
echo "APK disponivel em: ./output/android/"
echo "======================================"
