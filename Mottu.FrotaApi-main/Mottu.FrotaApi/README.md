# Mottu Frota API

API RESTful para gerenciamento de motos, filiais e manutenções, desenvolvida em .NET 9, com documentação Swagger/OpenAPI, HATEOAS e Health Checks implementados.
Versão da API: v1.

---

## Integrantes
- Marcelo Scoleso  
- João Santis  
- João Paulo  

---

## Justificativa da Arquitetura

A API segue o modelo **RESTful**, utilizando **Controllers** para cada entidade (`Motos`, `Filiais`, `Manutencoes`) com endpoints CRUD completos.  

Principais escolhas arquiteturais:

- **.NET Web API**: framework robusto e consolidado para APIs REST.  
- **Entity Framework Core**: persistência de dados e gerenciamento de relacionamentos.  
- **HATEOAS**: links de navegação para facilitar consumo e descoberta de recursos.  
- **Swagger/OpenAPI**: documentação interativa com exemplos de payloads.  
- **JWT**: autenticação e autorização para proteger os endpoints.
- **Health Checks**: monitoramento do status da aplicação.

Essa arquitetura garante **manutenção fácil, escalabilidade e clareza na comunicação** entre o front-end e a API.

---

## Tecnologias Utilizadas
- .NET 9
- ASP.NET Core Web API  
- Entity Framework Core  
- Swagger/OpenAPI  
- Newtonsoft.Json  
- xUnit para testes unitários e de integração
- ML.NET para previsões de disponibilidade

---

## Estrutura do Projeto

Mottu.FrotaApi/
├── Controllers/
│ ├── AuthController.cs    
│ ├── MotosController.cs
│ ├── FiliaisController.cs
│ ├── ManutencoesController.cs 
│ └── MlController.cs
├── Data/
│ └── AppDbContext.cs
├── Models/
│     └── ML/
│         ├── DisponibilidadeInput.cs
│         └── DisponibilidadePrediction.cs
│ ├── Moto.cs
│ ├── Filial.cs
│ └── Manutencao.cs
├── Pages/
│ └── Health.cshtml
├── Services/
  └── DisponibilidadeTrainer.cs
├── docker-compose.yml
├── Program.cs
└── README.md
Mottu.FrotaApi.Tests/
└── ApiIntegrationTests.cs

## Instruções de Execução

1. **Clonar o repositório**:

git clone https://github.com/MarceloScoleso/Mottu.FrotaApi.git
cd Mottu.FrotaApi

2. **Restaurar pacotes e construir o projeto**:

dotnet restore
dotnet build

3. **Executar a API**:

dotnet run

4. **Acessar o Swagger UI**:

https://localhost:5269/swagger

## Autenticação
- Todos os endpoints protegidos requerem **JWT** no header Authorization.
- Exemplo de login:

POST /api/v1/auth/login

Payload:

{
  "username": "admin",
  "password": "123"
}

Resposta:

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6..."
}

Para acessar endpoints protegidos, use o header:

Authorization: Bearer {token}

## Health Checks

GET /health
GET /healthz
- Retorna status da aplicação (Healthy ou Unhealthy)

Exemplo de resposta:

{
  "status": "Healthy"
}

## Endpoints da API (v1)

- Entidade	Método	Rota	Descrição
- Motos	GET	/api/v1/motos	Lista motos (com paginação)
- Motos	GET	/api/v1/motos/{id}	Obter moto por ID
- Motos	POST	/api/v1/motos	Criar nova moto
- Motos	PUT	/api/v1/motos/{id}	Atualizar moto existente
- Motos	DELETE	/api/v1/motos/{id}	Deletar moto
- Filiais	GET	/api/v1/filiais	Lista filiais
- Filiais	GET	/api/v1/filiais/{id}	Obter filial por ID
- Filiais	POST	/api/v1/filiais	Criar nova filial
- Filiais	PUT	/api/v1/filiais/{id}	Atualizar filial existente
- Filiais	DELETE	/api/v1/filiais/{id}	Deletar filial
- Manutencoes	GET	/api/v1/manutencoes	Lista manutenções
- Manutencoes	GET	/api/v1/manutencoes/{id}	Obter manutenção por ID
- Manutencoes	POST	/api/v1/manutencoes	Criar nova manutenção
- Manutencoes	PUT	/api/v1/manutencoes/{id}	Atualizar manutenção existente
- Manutencoes	DELETE	/api/v1/manutencoes/{id}	Deletar manutenção
- ML POST  /api/v1/ml/prever-disponibilidade  Prever disponibilidade de moto
- Health Check GET /health  Retorna status da aplicação

## Exemplos de Uso

Listar motos com paginação e HATEOAS
GET /api/Motos?page=1&pageSize=10


Resposta exemplo:

{
  "totalItems": 25,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3,
  "data": [
    {
      "id": 1,
      "placa": "ABC-1234",
      "modelo": "Honda CG 160",
      "status": "Disponível",
      "filial": {
        "id": 1,
        "nome": "Filial Central"
      },
      "links": [
        { "rel": "self", "href": "/api/Motos/1" },
        { "rel": "update", "href": "/api/Motos/1" },
        { "rel": "delete", "href": "/api/Motos/1" }
      ]
    }
  ],
  "links": [
    { "rel": "self", "href": "/api/Motos?page=1&pageSize=10" },
    { "rel": "next", "href": "/api/Motos?page=2&pageSize=10" },
    { "rel": "prev", "href": null }
  ]
}

Criar uma nova moto
POST /api/Motos


Payload exemplo:

{
  "placa": "XYZ-9876",
  "modelo": "Honda CB 500",
  "status": "Disponível",
  "filialId": 1
}


Resposta: 201 Created com o objeto criado.

## Testes

### Testes Unitários

- Localizados em Mottu.FrotaApi.Tests/ApiIntegrationTests.cs
- Testam lógica principal das entidades (ex.: criação de Moto).

### Testes de Integração

- Utilizam WebApplicationFactory<Program> para simular requests HTTP.
- Testam autenticação, endpoints CRUD e ML.NET.
- Exemplos de testes: 
  - Login retorna token JWT
  - GET /api/v1/filiais retorna 200
  - GET /api/v1/motos retorna 200 (com autenticação)
  - POST /api/v1/ml/prever-disponibilidade retorna resultado de previsão

### Executar Testes

cd Mottu.FrotaApi.Tests

dotnet test

- Todos os testes unitários e de integração devem passar.