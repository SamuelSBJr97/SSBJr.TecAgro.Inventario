#!/bin/bash

# Script de Deploy - SSBJr.TecAgro.Inventario
# Este script facilita o deploy da aplica��o usando Docker

set -e

echo "=========================================="
echo "SSBJr TecAgro Invent�rio - Deploy Script"
echo "=========================================="

# Verificar se Docker est� instalado
if ! command -v docker &> /dev/null; then
    echo "? Docker n�o est� instalado. Instale o Docker primeiro."
    exit 1
fi

if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
    echo "? Docker Compose n�o est� instalado. Instale o Docker Compose primeiro."
    exit 1
fi

# Fun��o para parar e remover containers
cleanup() {
    echo "?? Limpando containers antigos..."
    docker-compose down -v
}

# Fun��o para build
build() {
    echo "?? Construindo imagens Docker..."
    docker-compose build --no-cache
}

# Fun��o para iniciar
start() {
    echo "?? Iniciando containers..."
    docker-compose up -d
    
    echo ""
    echo "? Aguardando servi�os ficarem prontos..."
    sleep 10
    
    echo ""
    echo "? Deploy conclu�do com sucesso!"
    echo ""
    echo "?? Status dos containers:"
    docker-compose ps
    echo ""
    echo "?? Acesse a API em: http://localhost:5000"
    echo "?? Swagger UI: http://localhost:5000/swagger"
    echo ""
    echo "?? Usu�rio padr�o:"
    echo "   Login: admin"
    echo "   Senha: admin123"
    echo ""
    echo "?? Para ver logs: docker-compose logs -f"
    echo "?? Para parar: docker-compose down"
}

# Fun��o para ver logs
logs() {
    echo "?? Exibindo logs..."
    docker-compose logs -f
}

# Fun��o para reiniciar
restart() {
    echo "?? Reiniciando aplica��o..."
    docker-compose restart
}

# Fun��o para parar
stop() {
    echo "?? Parando aplica��o..."
    docker-compose down
}

# Menu principal
case "${1}" in
    cleanup)
        cleanup
        ;;
    build)
        build
        ;;
    start)
        start
    ;;
    deploy)
   cleanup
        build
        start
        ;;
    logs)
        logs
        ;;
    restart)
        restart
        ;;
    stop)
     stop
   ;;
    *)
        echo "Uso: $0 {deploy|build|start|stop|restart|logs|cleanup}"
        echo ""
        echo "Comandos:"
    echo "  deploy   - Limpa, reconstr�i e inicia tudo (recomendado)"
        echo "  build    - Apenas reconstr�i as imagens"
  echo "  start- Apenas inicia os containers"
        echo "  stop     - Para os containers"
     echo "  restart  - Reinicia os containers"
        echo "  logs     - Exibe logs em tempo real"
        echo "  cleanup  - Remove containers e volumes"
  exit 1
    ;;
esac
