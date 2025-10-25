#!/bin/bash
# Script para iniciar o ambiente Docker

echo "======================================"
echo "TecAgro Inventario - Iniciar Docker"
echo "======================================"
echo ""

# Verificar se Docker esta instalado
if ! command -v docker &> /dev/null; then
    echo "Erro: Docker nao encontrado. Por favor, instale o Docker primeiro."
    exit 1
fi

# Verificar se Docker Compose esta instalado
if ! command -v docker-compose &> /dev/null; then
  echo "Erro: Docker Compose nao encontrado. Por favor, instale o Docker Compose."
    exit 1
fi

# Perguntar qual ambiente
echo "Selecione o ambiente:"
echo "1) Desenvolvimento (apenas banco de dados)"
echo "2) Producao (banco de dados + API + Nginx)"
read -p "Opcao (1 ou 2): " option

case $option in
    1)
      echo ""
      echo "Iniciando ambiente de DESENVOLVIMENTO..."
        docker-compose -f docker-compose.dev.yml up -d
      echo ""
        echo "? Banco de dados SQL Server iniciado!"
    echo "  - Host: localhost"
        echo "  - Porta: 1433"
  echo "  - Usuario: sa"
        echo "  - Senha: Dev@2025!Pass"
        echo "  - Database: InventarioDb"
        echo ""
  echo "Connection String:"
        echo "Server=localhost,1433;Database=InventarioDb;User Id=sa;Password=Dev@2025!Pass;TrustServerCertificate=True;"
        ;;
    2)
        echo ""
        echo "Iniciando ambiente de PRODUCAO..."
        docker-compose up -d --build
  echo ""
        echo "? Todos os servicos iniciados!"
        echo ""
        echo "Servicos disponiveis:"
        echo "  - API: http://localhost:5000"
        echo "  - Swagger: http://localhost:5000/swagger"
        echo "  - Nginx: http://localhost:80"
      echo "  - SQL Server: localhost:1433"
        echo ""
   echo "Para ver os logs:"
        echo "  docker-compose logs -f"
        ;;
    *)
        echo "Opcao invalida!"
exit 1
        ;;
esac

echo ""
echo "Para parar os containers:"
if [ "$option" = "1" ]; then
    echo "  docker-compose -f docker-compose.dev.yml down"
else
    echo "  docker-compose down"
fi
echo ""
