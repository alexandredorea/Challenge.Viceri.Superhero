# Challenge Viceri

Esta é uma aplicação que implementa um CRUD (Create, Read, Update, Delete) de Super-Heróis com associação de Superpoderes, conforme os requisitos propostos.

## Tecnologias Utilizadas

| Camada | Tecnologia | Versão | Propósito |
| :--- | :--- | :--- | :--- |
| **Backend** | .NET Core | 8.0 | API RESTful para as operações CRUD. |
| | C# | 12.0 | Linguagem de programação. |
| | Entity Framework Core | 8.0.8 | Acesso a dados (utilizando provedor In-Memory para este projeto). |
| | MediatR | 12.2.0 | Implementação do padrão CQS (Command Query Separation). |
| | FluentValidation | 11.9.1 | Validação de dados e regras de negócio. |
| | XUnit | - | Framework para Testes Unitários. |
| **Frontend** | Vue.js | 3.x | Framework JavaScript para a interface do usuário. |
| | Vite | 4.4+ | Build tool e dev server para Vue.js. |
| | Axios | 1.5+ | Cliente HTTP para comunicação com a API. |
| | Tailwind CSS | 3.3+ | Framework CSS para estilização. |

## Arquitetura do Backend

O backend foi estruturado seguindo o princípio de **Responsabilidades Separadas** (Separation of Concerns), com uma arquitetura limpa e focada em CQS (Command Query Separation).

| Projeto | Responsabilidade |
| :--- | :--- |
| `Api` | Camada de Apresentação (Controllers, Configuração da API, Swagger, CORS, Middleware). |
| `Application` | Camada de Aplicação (Commands, Queries, Handlers, DTOs, Validações, Logica, Injeção de Dependência). |
| `Domain` | Camada de Domínio (Entidades, buscando atender DDD minimamente, etc.). |
| `Infrastructure` | Camada de Infraestrutura (Implementação do `DbContext`, EF Core In-Memory, Configurações de Entidades, Injeção de Dependência). |
| `Tests` | Projeto de Testes Unitários (XUnit) para a camada de `Application`. |

### Padrões de Projeto

*   **CQS (Command Query Separation):** Utilizado o **MediatR** para separar as operações de leitura (Queries) das operações de escrita (Commands).
*   **Result Pattern:** Utilizado para encapsular o resultado das operações de negócio, permitindo um tratamento de erros explícito e padronizado (sucesso ou falha com um objeto `Error`).
*   **Validação:** Utilizado o **FluentValidation** para validar os *Commands* de entrada.
*   **Clean Architecture:** Separação clara entre camadas, dependências apontando para o núcleo da aplicação (Domain).
*   **Repository Pattern:** Utilizado o EF Core com abstrações apropriadas para acesso a dados.

## Funcionalidades Implementadas

### Backend (API REST)

#### Heróis

| Método HTTP | Endpoint | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/heroes` | Lista todos os super-heróis cadastrados. |
| `GET` | `/api/heroes/{id}` | Consulta um super-herói por ID. |
| `POST` | `/api/heroes` | Cadastra um novo super-herói. |
| `PUT` | `/api/heroes/{id}` | Atualiza um super-herói existente. |
| `DELETE` | `/api/heroes/{id}` | Exclui um super-herói. |

#### Superpoderes

| Método HTTP | Endpoint | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/superpowers` | Lista todos os superpoderes cadastrados. |
| `GET` | `/api/superpowers/{id}` | Consulta um superpoder por ID. |
| `POST` | `/api/superpowers` | Cadastra um novo superpoder. |
| `PUT` | `/api/superpowers/{id}` | Atualiza um superpoder existente. |
| `DELETE` | `/api/superpowers/{id}` | Exclui um superpoder. |

A documentação completa da API está disponível via **Swagger** na rota `/swagger` quando a aplicação é executada em ambiente de desenvolvimento.

### Frontend (Vue.js)

O frontend provê uma interface interativa para:

#### Gestão de Heróis
1.  Visualizar a lista de heróis com seus superpoderes associados.
2.  Criar um novo herói com seleção de superpoderes.
3.  Editar um herói existente (alterando dados e superpoderes).
4.  Excluir um herói.

#### Gestão de Superpoderes
1.  Visualizar a lista de superpoderes cadastrados.
2.  Criar um novo superpoder.
3.  Editar um superpoder existente.
4.  Excluir um superpoder.

## Como Executar o Projeto

### 1. Backend (.NET 8)

1.  Navegue até a pasta `/backend`.
2.  Execute o comando:
    ```bash
    dotnet run --urls "http://localhost:5110"
    ```
    A API estará disponível em `http://localhost:5110`.
3.  A documentação Swagger estará disponível em `http://localhost:5110/swagger`.

### 2. Frontend (Vue.js)

1.  Navegue até a pasta `/frontend`.
2.  Instale as dependências:
    ```bash
    npm install
    ```
3.  Instale pacotes adicionais:
    ```bash
    npm install axios
    npm install -D tailwindcss postcss autoprefixer
    npx tailwindcss init -p
    ```
4.  Configure o arquivo `.env`:
    ```env
    VITE_API_URL=http://localhost:5110/api
    ```
5.  Inicie o servidor de desenvolvimento:
    ```bash
    npm run dev
    ```
    O frontend estará disponível em `http://localhost:5173`.

### 3. Build para Produção (Frontend)

Para criar uma versão otimizada para produção:
```bash
npm run build
```

Os arquivos otimizados estarão na pasta `dist/`.

### 4. Testes Unitários

Na raiz do projeto backend, execute:
```bash
dotnet test
```

Isso executará os testes do projeto `Challenge.Viceri.Superhero.Tests`.

## Considerações Finais

Busquei seguir a implementação de forma a atender todos os requisitos, incluindo a utilização de .NET 8, Vue.js, Entity Framework Core (In-Memory), Swagger e Testes Unitários (XUnit). O padrão CQS e o Result Pattern foram adotados para garantir um código limpo, manutenível com tratamento de erros, onde tento seguir as melhores práticas da indústria.