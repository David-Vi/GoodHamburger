**🍔 Good Hamburger — Sistema de Pedidos**  
Sistema completo para gerenciamento de pedidos da lanchonete **Good Hamburger**, construído com  **Clean Architecture**,  **ASP.NET Core 8**,  **Blazor WebAssembly** e conformidade com o  **OWASP Top 10**.  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OYQ1AABSAwc8mi5wvkwZyCKCAACr4Z7a7BLfMzFYdAQDwF+da3dX+9QQAgNeuB6feBdUJcyS2AAAAAElFTkSuQmCC)  
**📋 Índice**  
- [Funcionalidades](#anchor-1 "#anchor-1")  
- [Arquitetura](#anchor-2 "#anchor-2")  
- [Pré-requisitos](#anchor-3 "#anchor-3")  
- [Como Executar](#anchor-4 "#anchor-4")  
- [Endpoints da API](#anchor-5 "#anchor-5")  
- [Regras de Desconto](#anchor-6 "#anchor-6")  
- [Testes](#anchor-7 "#anchor-7")  
- [Segurança OWASP](#anchor-8 "#anchor-8")  
- [Decisões de Arquitetura](#anchor-9 "#anchor-9")  
- [O que ficou fora](#anchor-10 "#anchor-10")  
- [Material para Aula](#anchor-11 "#anchor-11")  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANElEQVR4nO3OQQmAABRAsSdYxKa/i8WMIR7ECt5E2BJsmZmt2gMA4C+Otbqr8+sJAACvXQ85PAYartXEogAAAABJRU5ErkJggg==)  
**✅ Funcionalidades**  
| | |  
|-|-|  
| **Requisito** | **Status** |   
| API REST CRUD de pedidos | ✅ |   
| Cálculo de descontos (10%, 15%, 20%) | ✅ |   
| Validação de itens duplicados | ✅ |   
| Endpoint de cardápio | ✅ |   
| Frontend Blazor WebAssembly | ✅ |   
| Testes unitários das regras de negócio | ✅ |   
| Testes de integração dos endpoints | ✅ |   
| Conformidade OWASP Top 10 | ✅ |   
| Docker + Docker Compose | ✅ |   
| Swagger UI | ✅ |   
   
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OMQ2AABAAsSNhwgJOUPcjIpnRgQU2QtIq6DIze3UGAMBf3Gu1VcfXEwAAXrseaJEEL8XMiYMAAAAASUVORK5CYII=)  
**🎓 Material para Aula**  
Para ensino passo a passo dos estudantes, usa o guia dedicado:  
- [docs/GUIA_ESTUDANTES.md](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/docs/GUIA_ESTUDANTES.md:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/docs/GUIA_ESTUDANTES.md:1")  
Esse material inclui:  
- visão do projeto  
- instalação do ambiente  
- sequência de desenvolvimento  
- arquitetura explicada  
- ferramentas de teste  
- roteiro de aulas  
- exercícios práticos  
Para documentação técnica detalhada de implementação, lógica e decisões de engenharia:  
- [docs/MANUAL_TECNICO_DESENVOLVIMENTO.md](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/docs/MANUAL_TECNICO_DESENVOLVIMENTO.md:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/docs/MANUAL_TECNICO_DESENVOLVIMENTO.md:1")  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OMQ2AABAAsSNBCkLfFDZwwIgHRiywEZJWQZeZ2ao9AAD+4lyruzq+ngAA8Nr1AOH0BedHjjlfAAAAAElFTkSuQmCC)  
**🏗️ Arquitetura**  
O projeto segue **Clean Architecture** com separação clara de responsabilidades:  
GoodHamburger/  
 ├── src/  
 │   ├── GoodHamburger.Core/          # Domínio puro — entidades, exceções, interfaces  
 │   │   ├── Entities/                # Order (agregado raiz), MenuItem  
 │   │   ├── Enums/                   # MenuItemCategory, MenuItemId, OrderStatus  
 │   │   ├── Exceptions/              # DomainException, NotFoundException  
 │   │   └── Interfaces/              # IOrderRepository, IMenuRepository  
 │   │  
 │   ├── GoodHamburger.Infrastructure/ # Implementações concretas (repositórios)  
 │   │   └── Repositories/            # InMemoryOrderRepository, InMemoryMenuRepository  
 │   │  
 │   ├── GoodHamburger.Api/           # ASP.NET Core — controllers, serviços, middleware  
 │   │   ├── Controllers/             # OrdersController, MenuController  
 │   │   ├── DTOs/                    # Request/Response objects  
 │   │   ├── Services/                # OrderService, MenuService  
 │   │   ├── Middleware/              # ExceptionHandlerMiddleware, SecurityHeadersMiddleware  
 │   │   └── Program.cs               # Composição de dependências + pipeline  
 │   │  
 │   └── GoodHamburger.Blazor/        # Frontend WebAssembly  
 │       ├── Pages/                   # Index (cardápio), Pedidos (lista), NovoPedido (form)  
 │       ├── Services/                # ApiService (HTTP client)  
 │       └── wwwroot/                 # CSS, index.html  
 │  
 └── tests/  
     └── GoodHamburger.Tests/  
         ├── Unit/                    # OrderDiscountTests — regras de negócio  
         └── Integration/             # OrdersApiTests — endpoints HTTP  
   
**Fluxo de uma requisição**  
HTTP Request  
     → SecurityHeadersMiddleware      (adiciona headers OWASP)  
     → ExceptionHandlerMiddleware     (captura exceções → Problem Details RFC 7807)  
     → RateLimiter                    (100 req/min — OWASP A04)  
     → Controller                     (valida ModelState)  
     → Service                        (orquestra casos de uso)  
     → Domain Entity                  (aplica regras de negócio — DomainException)  
     → Repository                     (persiste)  
     → Response DTO  
   
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OMQ2AABAAsSPBCj5fFgpQwYwEZiywEZJWQZeZ2ao9AAD+4lyruzq+ngAA8Nr1AMTRBeEgNK9YAAAAAElFTkSuQmCC)  
**🔧 Pré-requisitos**  
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0 "https://dotnet.microsoft.com/download/dotnet/8.0")  
- [VS Code com extensões recomendadas (.vscode/extensions.json)](https://code.visualstudio.com/ "https://code.visualstudio.com/")  
- Docker + Docker Compose *(opcional — para rodar via container)*  
**Instalar o .NET 8 no Ubuntu**  
# Adicionar repositório Microsoft  
 wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb  
 sudo dpkg -i packages-microsoft-prod.deb  
 rm packages-microsoft-prod.deb  
   
 # Instalar SDK  
 sudo apt-get update  
 sudo apt-get install -y dotnet-sdk-8.0  
   
 # Verificar instalação  
 dotnet --version   # deve exibir 8.0.x  
   
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OMQ2AABAAsSNhYMEBIpD4ArCJDyywEZJWQZeZOaorAAD+4l6rrTq/ngAA8Nr+AEqmA1hl45m5AAAAAElFTkSuQmCC)  
**🚀 Como Executar**  
**Opção 1 — Linha de comando (recomendado para desenvolvimento)**  
# Clone o repositório  
 git clone <URL_DO_SEU_REPO>  
 cd GoodHamburger  
   
 # Restaurar dependências  
 dotnet restore  
   
 # Rodar apenas a API  
 dotnet run --project src/GoodHamburger.Api --urls "http://localhost:5000"  
   
 # Em outro terminal — rodar o Blazor  
 dotnet run --project src/GoodHamburger.Blazor --urls "http://localhost:5001"  
   
**Acesse:**  
- 🔗 API + Swagger UI: http://localhost:5000  
- 🔗 Frontend Blazor: http://localhost:5001  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OMQ2AABAAsSNBCkJfFEIwwIgHRiywEZJWQZeZ2ao9AAD+4lyruzq+ngAA8Nr1AOHsBegrsOrIAAAAAElFTkSuQmCC)  
**Opção 2 — VS Code (F5)**  
1. Abra a pasta GoodHamburger no VS Code  
2. Instale as extensões sugeridas (Ctrl+Shift+P → "Show Recommended Extensions")  
3. Pressione F5 para iniciar a API com debugger  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANElEQVR4nO3OQQmAABRAsSdYxKY/jMFMIZ7ECt5E2BJsmZmt2gMA4C+Otbqr8+sJAACvXQ85QgYXd/O+eQAAAABJRU5ErkJggg==)  
**Opção 3 — Docker Compose**  
# Construir e iniciar tudo  
 docker compose up --build  
   
 # Em background  
 docker compose up -d --build  
   
 # Parar  
 docker compose down  
   
**Acesse:**  
- 🔗 API: http://localhost:5000  
- 🔗 Frontend: http://localhost:5001  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OQQmAABRAsSd4NIGBzPXBmAawhhW8ibAl2DIze3UGAMBf3Gu1VcfXEwAAXrsehaQEN+8fLHEAAAAASUVORK5CYII=)  
**📡 Endpoints da API**  
A documentação interativa completa está disponível em **http://localhost:5000** (Swagger UI).  
**Cardápio**  
| | | |  
|-|-|-|  
| **Método** | **Rota** | **Descrição** |   
| GET | /api/v1/menu | Lista todos os itens do cardápio |   
   
**Pedidos**  
| | | |  
|-|-|-|  
| **Método** | **Rota** | **Descrição** |   
| GET | /api/v1/orders | Lista todos os pedidos |   
| GET | /api/v1/orders/{id} | Busca pedido por ID |   
| POST | /api/v1/orders | Cria novo pedido |   
| PUT | /api/v1/orders/{id} | Atualiza pedido existente |   
| DELETE | /api/v1/orders/{id} | Remove pedido |   
   
**Exemplo — Criar pedido**  
POST /api/v1/orders  
 Content-Type: application/json  
   
 {  
   "menuItemIds": [1, 4, 5]  
 }  
   
**Resposta 201 Created:**  
{  
   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",  
   "status": "Open",  
   "items": [  
     { "menuItemId": 1, "name": "X Burger",     "unitPrice": 5.00, "category": "Sandwich" },  
     { "menuItemId": 4, "name": "Batata Frita", "unitPrice": 2.00, "category": "SideDish" },  
     { "menuItemId": 5, "name": "Refrigerante", "unitPrice": 2.50, "category": "Drink"    }  
   ],  
   "subtotal": 9.50,  
   "discountRate": 0.20,  
   "discountAmount": 1.90,  
   "total": 7.60,  
   "createdAt": "2024-01-15T14:30:00Z",  
   "updatedAt": "2024-01-15T14:30:00Z"  
 }  
   
**IDs do Cardápio**  
| | | | |  
|-|-|-|-|  
| **ID** | **Nome** | **Preço** | **Categoria** |   
| 1 | X Burger | R$ 5,00 | Sandwich |   
| 2 | X Egg | R$ 4,50 | Sandwich |   
| 3 | X Bacon | R$ 7,00 | Sandwich |   
| 4 | Batata Frita | R$ 2,00 | SideDish |   
| 5 | Refrigerante | R$ 2,50 | Drink |   
   
**Testes manuais com REST Client**  
Abra o arquivo api-tests.http no VS Code com a extensão [REST Client e clique em "Send Request" em cada bloco.](https://marketplace.visualstudio.com/items?itemName=humao.rest-client "https://marketplace.visualstudio.com/items?itemName=humao.rest-client")  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OQQmAABRAsSdYxZ4/mJjEsxE8W8GbCFuCLTOzVXsAAPzFuVZ3dXw9AQDgtesBxPEF3bv7x0IAAAAASUVORK5CYII=)  
**🏷️ Regras de Desconto**  
| | |  
|-|-|  
| **Combinação** | **Desconto** |   
| Sanduíche + Batata Frita + Refrigerante | **20%** |   
| Sanduíche + Refrigerante | **15%** |   
| Sanduíche + Batata Frita | **10%** |   
| Qualquer outra combinação | **0%** |   
   
**Restrições:**  
- Cada pedido aceita **no máximo 1 item por categoria** (sanduíche, acompanhamento, bebida)  
- Itens duplicados retornam **422 Unprocessable Entity** com mensagem clara  
- Pedidos e itens inexistentes retornam **404 Not Found**  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAM0lEQVR4nO3KsQ0AIRAEsUW6Qij1KvnevhMSYmKQ7GiCGd09k3wBAOAVf+2o4wYAwE1qAdYuAy151mgcAAAAAElFTkSuQmCC)  
**🧪 Testes**  
# Rodar todos os testes  
 dotnet test  
   
 # Rodar com saída detalhada  
 dotnet test --verbosity normal  
   
 # Rodar apenas testes unitários  
 dotnet test --filter "FullyQualifiedName~Unit"  
   
 # Rodar apenas testes de integração  
 dotnet test --filter "FullyQualifiedName~Integration"  
   
 # Gerar relatório de cobertura  
 dotnet test --collect:"XPlat Code Coverage"  
   
**Cobertura dos testes**  
**Unitários (** **OrderDiscountTests** **):**  
- ✅ Desconto 20% — todas as combinações de sanduíche  
- ✅ Desconto 15% — sanduíche + refrigerante  
- ✅ Desconto 10% — sanduíche + batata  
- ✅ Sem desconto — sanduíche isolado, batata+bebida, bebida isolada  
- ✅ Exceção por sanduíche duplicado  
- ✅ Exceção por acompanhamento duplicado  
- ✅ Exceção por bebida duplicada  
- ✅ ReplaceItems recalcula desconto corretamente  
- ✅ Total nunca negativo  
- ✅ Total + Desconto = Subtotal (sanity check)  
**Integração (** **OrdersApiTests** **):**  
- ✅ GET /menu → 5 itens  
- ✅ POST combo completo → 201 + 20% desconto  
- ✅ POST sanduíche+bebida → 15% desconto  
- ✅ POST sanduíche+batata → 10% desconto  
- ✅ POST duplicata → 422  
- ✅ POST item inexistente → 404  
- ✅ POST lista vazia → 422  
- ✅ GET all → lista não vazia  
- ✅ GET por ID válido → 200  
- ✅ GET por ID inválido → 404  
- ✅ PUT → recalcula desconto  
- ✅ PUT ID inválido → 404  
- ✅ DELETE válido → 204  
- ✅ DELETE + GET → 404  
- ✅ DELETE inválido → 404  
- ✅ Headers de segurança presentes  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANElEQVR4nO3OQQmAUBBAwSf8GGLWDWFDY3ixgjcRZhLMNjNHdQYAwF9cq1rV/vUEAIDX7gcRXAQ2s/16gwAAAABJRU5ErkJggg==)  
**🔒 Segurança**  
O projeto mitiga ativamente as vulnerabilidades :  
| | | |  
|-|-|-|  
| **OWASP** | **Vulnerabilidade** | **Mitigação implementada** |   
| A01 | Broken Access Control | Endpoints sem dados sensíveis; sem autenticação exposta |   
| A02 | Cryptographic Failures | Nenhum dado sensível armazenado; HTTPS em produção |   
| A03 | Injection | Sem SQL/ORM direto; IDs tipados (GUID, enum) — sem string concatenation |   
| A04 | Insecure Design | Rate limiter: 100 req/min por IP — AddRateLimiter |   
| A05 | Security Misconfiguration | SecurityHeadersMiddleware: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy, Permissions-Policy; server_tokens off no nginx; usuário não-root no Docker |   
| A06 | Vulnerable Components | .NET 8 LTS com dependências mínimas e conhecidas |   
| A07 | Auth Failures | N/A (sem auth neste escopo) |   
| A08 | Software Integrity | Dockerfile multi-stage; imagens oficiais Microsoft |   
| A09 | Logging Failures | Serilog estruturado; stack traces **nunca** expostos em produção; sem dados sensíveis nos logs |   
| A10 | SSRF | Sem chamadas HTTP server-side a URLs externas |   
   
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OQQmAABRAsSeYxZw/lieLGMACBrCCNxG2BFtmZquOAAD4i3Ot7mr/egIAwGvXA6fGBdgoVMwYAAAAAElFTkSuQmCC)  
**🧠 Decisões de Arquitetura**  
**Por que Clean Architecture?**  
Separa **regras de negócio** (Core) de  **infraestrutura** (repositórios, HTTP) e  **entrega** (controllers). O domínio não depende de nenhum framework — testável de forma unitária pura, sem mocks de banco ou HTTP.  
**Por que as regras de desconto ficam no agregado **Order **?**  
Lógica de negócio pertence ao domínio, não ao serviço. O agregado Order protege seus invariantes: valida duplicatas no AddItem, calcula descontos em propriedades computed. Isso garante que **nenhum consumidor possa criar um ** **Order** ** inválido**.  
**Por que **record ** para **OrderItem **?**  
OrderItem é um **snapshot imutável** do momento da compra. Usar record comunica essa intenção e previne mutação acidental.  
**Por que repositório In-Memory com **ConcurrentDictionary **?**  
A interface IOrderRepository isola completamente a persistência. Trocar para **EF Core + PostgreSQL** exige apenas uma nova implementação da interface — zero mudanças em controllers ou serviços. O ConcurrentDictionary garante thread-safety sem locks manuais.  
**Por que Problem Details (RFC 7807)?**  
Padrão HTTP para respostas de erro. Fornece type, title, status, detail e traceId de forma consistente. O traceId permite correlacionar erros nos logs sem expor stack traces ao cliente.  
**Por que Serilog?**  
Logging estruturado (JSON em produção, legível no console em dev). Permite correlação por TraceId, integração fácil com Elasticsearch/Seq/CloudWatch.  
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OMQ2AABAAsSNhZscYahheJwqQgQU2QtIq6DIze3UGAMBf3Gu1VcfXEwAAXrseoqcEQXyAWBgAAAAASUVORK5CYII=)  
**🚧 O que ficou fora**  
Por ser um desafio técnico com escopo definido, as seguintes funcionalidades foram conscientemente omitidas:  
| | |  
|-|-|  
| **Feature** | **Motivo da omissão** |   
| **Banco de dados persistente** (EF Core + PostgreSQL) | A interface IOrderRepository está pronta para isso; optei por In-Memory para eliminar dependências de infra no setup |   
| **Autenticação/Autorização** (JWT) | Não estava no escopo; a estrutura de middleware suporta adicionar |   
| **Paginação** no GET /orders | Dataset pequeno no escopo do desafio |   
| **Migrations** | Sem banco de dados real neste escopo |   
| **Health Checks** (/health) | Útil em produção; omitido para manter foco |   
| **CI/CD** (GitHub Actions) | Fora do prazo; .github/workflows/ seria o próximo passo |   
| **Internacionalização** | Sistema focado em PT-BR |   
   
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OMQ2AABAAsSNBCkLfE07YGfHAiAU2QtIq6DIzW7UHAMBfnGt1V8fXEwAAXrse4eQF6VhvmPsAAAAASUVORK5CYII=)  
**📦 Tecnologias**  
| | | |  
|-|-|-|  
| **Tecnologia** | **Versão** | **Uso** |   
| .NET / ASP.NET Core | 8.0 LTS | API REST |   
| Blazor WebAssembly | 8.0 | Frontend SPA |   
| Serilog | 8.x | Logging estruturado |   
| Swashbuckle | 6.x | Swagger UI |   
| xUnit | 2.6 | Framework de testes |   
| FluentAssertions | 6.x | Assertions expressivas |   
| Docker + nginx | latest/alpine | Containerização |   
   
![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAM0lEQVR4nO3OMQ0AIAwAwZKQ6kBqjSAOJywYYCIkd9OP36pqRMQMAAB+sfqJfLoBAMCN3NYoAzBA+QG0AAAAAElFTkSuQmCC)  
**👤 Autor** **: David Viegas Mateus**  
Desenvolvido como desafio técnico C#.  
# GoodHamburger
