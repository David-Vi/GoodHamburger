**Guia - Good Hamburger**  
**1. Objetivo do projeto**  
O projeto **Good Hamburger** foi construído para ensinar desenvolvimento moderno de aplicações com:  
- ASP.NET Core 8  
- Blazor WebAssembly  
- Clean Architecture  
- Testes automatizados  
- OpenAPI / Swagger  
- Boas práticas de segurança para APIs  
O domínio do sistema e simples: uma lanchonete que permite consultar o cardápio e gerir pedidos com regras de desconto.  
Este guia foi preparado para os estudantes aprenderem:  
- como o projeto foi pensado  
- como instalar o ambiente  
- como executar cada parte  
- como a arquitetura foi organizada  
- como desenvolver funcionalidades novas  
- como testar manualmente e automaticamente  
**2. O problema de negócio**  
A aplicação resolve um caso simples, mas muito bom para ensino:  
- existe um cardápio fixo  
- o cliente pode montar um pedido  
- cada pedido aceita no máximo um item por categoria  
- o sistema calcula descontos automaticamente  
As categorias atuais são:  
- sanduíche  
- acompanhamento  
- bebida  
As regras de desconto são:  
- sanduíche + batata + refrigerante = 20%  
- sanduíche + refrigerante = 15%  
- sanduíche + batata = 10%  
- outras combinações = 0%  
**3. Tecnologias utilizadas**  
**Backend**  
- .NET 8  
- ASP.NET Core Web API  
- Serilog para logs  
- Swashbuckle para Swagger / OpenAPI  
**Frontend**  
- Blazor WebAssembly  
**Testes**  
- xUnit  
- FluentAssertions  
- Microsoft.AspNetCore.Mvc.Testing  
- coverlet.collector  
**Segurança e qualidade**  
- Problem Details para erros padronizados  
- API Key para endpoints protegidos  
- Rate Limiting  
- Security Headers  
- contrato OpenAPI versionado  
**Ferramentas de apoio**  
- VS Code ou Visual Studio  
- REST Client no VS Code  
- Docker e Docker Compose  
- Git  
**4. Estrutura do projeto**  
GoodHamburger/  
 ├── src/  
 │   ├── GoodHamburger.Api/  
 │   ├── GoodHamburger.Blazor/  
 │   ├── GoodHamburger.Core/  
 │   └── GoodHamburger.Infrastructure/  
 ├── tests/  
 │   └── GoodHamburger.Tests/  
 ├── openapi/  
 │   └── goodhamburger.v1.yaml  
 ├── docs/  
 │   └── GUIA_ESTUDANTES.md  
 ├── api-tests.http  
 ├── docker-compose.yml  
 └── GoodHamburger.sln  
   
**O papel de cada camada**  
*GoodHamburger.Core*  
Contém as regras de negócio puras:  
- entidades  
- enums  
- exceções de domínio  
- interfaces  
Esta camada não deve depender de banco de dados, web, UI ou framework externo.  
*GoodHamburger.Infrastructure*  
Contém implementações concretas, como repositórios.  
Hoje o projeto usa repositórios in-memory, o que facilita o ensino e os testes. Mais tarde os estudantes podem trocar por:  
- Entity Framework Core  
- SQL Server  
- PostgreSQL  
*GoodHamburger.Api*  
É a porta de entrada HTTP do sistema.  
Aqui ficam:  
- controllers  
- DTOs  
- services de aplicação  
- middlewares  
- autenticação  
- configuração do pipeline  
*GoodHamburger.Blazor*  
É o frontend que consome a API.  
Serve para mostrar aos estudantes:  
- comunicação cliente-servidor  
- consumo de endpoints  
- fluxo completo do utilizador  
*GoodHamburger.Tests*  
Contém os testes automatizados.  
Foi separado em:  
- Unit para regras de negócio  
- Integration para endpoints HTTP  
**5. Como o projeto foi desenvolvido**  
Uma boa forma de explicar aos estudantes é mostrar a ordem de construção:  
**Passo 1. Entender o domínio**  
Antes de escrever código, definimos:  
- quais entidades existem  
- quais regras precisam ser protegidas  
- quais ações o utilizador pode fazer  
Neste projeto, isso gerou a entidade Order como agregado principal.  
**Passo 2. Modelar o domínio**  
A lógica mais importante foi colocada no Core:  
- impedir itens duplicados por categoria  
- calcular desconto  
- manter consistência do pedido  
Isto ensina um ponto importante:  
- a regra de negócio deve viver no domínio, e não espalhada no controller  
**Passo 3. Criar interfaces**  
Foram definidas interfaces como:  
- IOrderRepository  
- IMenuRepository  
Isto permite trocar a infraestrutura sem reescrever a lógica principal.  
**Passo 4. Implementar infraestrutura simples**  
Foi escolhida uma implementação em memória para:  
- facilitar o arranque  
- evitar configuração de base de dados na fase inicial  
- concentrar o ensino em arquitetura e regras  
**Passo 5. Expor a API**  
Depois do domínio pronto, a API foi construída com:  
- rotas REST  
- DTOs  
- validação  
- responses HTTP adequadas  
**Passo 6. Criar testes**  
Primeiro faz sentido testar a lógica de negócio.  
Depois testamos o pipeline HTTP completo.  
**Passo 7. Adicionar documentação e segurança**  
Por fim, foram adicionados:  
- Swagger  
- contrato OpenAPI  
- autenticação  
- rate limiting  
- headers de segurança  
- tratamento global de exceções  
**6. Requisitos para os estudantes**  
Cada estudante deve instalar:  
- .NET 8 SDK  
- VS Code  
- extensão C#  
- extensão REST Client  
- Git  
Opcional:  
- Docker Desktop ou Docker Engine  
**7. Instalação do ambiente**  
**Ubuntu**  
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb  
 sudo dpkg -i packages-microsoft-prod.deb  
 rm packages-microsoft-prod.deb  
 sudo apt-get update  
 sudo apt-get install -y dotnet-sdk-8.0  
 dotnet --version  
   
**Windows**  
Passos sugeridos:  
1. Baixar o .NET 8 SDK no site oficial da Microsoft.  
2. Instalar normalmente.  
3. Instalar VS Code.  
4. Instalar Git.  
5. Confirmar no terminal:  
dotnet --version  
 git --version  
   
**Verificações iniciais**  
Depois da instalação, cada estudante deve conseguir executar:  
dotnet --version  
 dotnet --info  
 git --version  
 docker --version  
   
**8. Como abrir o projeto**  
**Clonar o repositório**  
git clone <URL_DO_REPOSITORIO>  
 cd GoodHamburger  
   
**Restaurar dependências**  
dotnet restore  
   
**Abrir no VS Code**  
code .  
   
**9. Como executar o projeto**  
**Opção A. Rodar a API**  
dotnet run --project src/GoodHamburger.Api  
   
Por padrão, a API fica disponível em:  
- http://localhost:5000  
**Opção B. Rodar o frontend**  
Em outro terminal:  
dotnet run --project src/GoodHamburger.Blazor --urls "http://localhost:5001"  
   
Frontend:  
- http://localhost:5001  
**Opção C. Rodar com Docker Compose**  
docker compose up --build  
   
Parar:  
docker compose down  
   
**10. Fluxo que o estudante deve entender**  
Quando alguém faz uma requisição para criar um pedido:  
1. a chamada chega ao controller  
2. o DTO recebe os dados  
3. o service orquestra o caso de uso  
4. a entidade Order aplica as regras  
5. o repositório guarda o pedido  
6. a API devolve a resposta  
Na prática, o estudante deve aprender a seguir o fluxo entre camadas.  
**11. Conceitos que devem ser explicados em aula**  
**API REST**  
Explicar:  
- GET para consultar  
- POST para criar  
- PUT para atualizar  
- DELETE para remover  
**DTO**  
DTO significa Data Transfer Object.  
Serve para:  
- receber dados da request  
- devolver dados da response  
- evitar expor diretamente a entidade de domínio  
**Entidade de domínio**  
A entidade é onde vivem as regras importantes do negócio.  
Exemplo no projeto:  
- a entidade Order não deixa criar um pedido inválido  
**Service de aplicação**  
O service não deve conter a regra mais importante do domínio.  
Ele serve para:  
- orquestrar operações  
- conversar com repositórios  
- transformar resultado em DTO  
**Repositório**  
Abstrai a forma de persistência.  
Permite trocar a infraestrutura sem mudar o resto do sistema.  
**12. Segurança implementada**  
O projeto agora inclui pontos importantes para ensinar segurança de API:  
- autenticação com X-API-Key  
- rate limiting  
- Problem Details  
- Content-Security-Policy  
- X-Content-Type-Options  
- X-Frame-Options  
- HSTS em produção  
- ocultação do header do servidor  
**O que os estudantes devem aprender aqui**  
- segurança não é um detalhe no fim  
- autenticação protege operações sensíveis  
- códigos HTTP importam  
- respostas de erro devem ser padronizadas  
- APIs devem ser documentadas e limitadas  
**13. Como testar manualmente**  
**Swagger**  
Abrir:  
- http://localhost:5000  
Ali o estudante pode:  
- ver os endpoints  
- testar chamadas  
- informar a API key  
**REST Client**  
O arquivo [api-tests.http permite testar tudo pelo VS Code.](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/api-tests.http:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/api-tests.http:1")  
Passos:  
1. instalar a extensão REST Client  
2. abrir o ficheiro api-tests.http  
3. clicar em Send Request  
**curl**  
Exemplo para listar menu:  
curl http://localhost:5000/api/v1/menu  
   
Exemplo para criar pedido:  
curl -X POST http://localhost:5000/api/v1/orders \  
   -H "Content-Type: application/json" \  
   -H "X-API-Key: goodhamburger-local-dev-key" \  
   -d '{"menuItemIds":[1,4,5]}'  
   
**14. Como testar automaticamente**  
**Rodar todos os testes**  
dotnet test  
   
**Rodar só testes unitários**  
dotnet test --filter "FullyQualifiedName~Unit"  
   
**Rodar só testes de integração**  
dotnet test --filter "FullyQualifiedName~Integration"  
   
**Gerar cobertura de testes**  
dotnet test --collect:"XPlat Code Coverage"  
   
**15. Ferramentas de teste que os estudantes vão utilizar**  
xUnit  
Usado para escrever testes.  
Ensina:  
- estrutura de testes  
- cenários de sucesso  
- cenários de erro  
FluentAssertions  
Usado para deixar os asserts mais legíveis.  
Exemplo mental:  
- em vez de comparar de forma crua, usamos uma linguagem mais próxima do domínio  
Microsoft.AspNetCore.Mvc.Testing  
Permite subir a API em memória para testes de integração.  
Isso é ótimo para ensinar:  
- pipeline real  
- rotas reais  
- status codes reais  
REST Client  
Ferramenta excelente para aulas práticas.  
Permite que o estudante teste endpoints sem precisar abrir Postman.  
Swagger  
Serve para:  
- explorar a API  
- testar endpoints  
- entender o contrato  
**16. Contrato da API**  
O projeto possui um contrato OpenAPI versionado em:  
- [goodhamburger.v1.yaml](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/openapi/goodhamburger.v1.yaml:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/openapi/goodhamburger.v1.yaml:1")  
Isto é importante para ensinar:  
- documentação formal da API  
- versionamento de contrato  
- integração entre equipas  
- conceito de API-first  
**17. Sugestão de roteiro de aula**  
**Aula 1. Introdução ao projeto**  
Objetivos:  
- apresentar o problema de negócio  
- mostrar a arquitetura  
- explicar a estrutura das pastas  
Atividade:  
- abrir a solution  
- navegar pelos projetos  
**Aula 2. Domínio e regras de negócio**  
Objetivos:  
- estudar a entidade Order  
- explicar invariantes  
- mostrar cálculo de descontos  
Atividade:  
- criar novos cenários de desconto  
- alterar regra e ver impacto nos testes  
**Aula 3. API e controllers**  
Objetivos:  
- explicar rotas REST  
- explicar DTOs  
- explicar services  
Atividade:  
- criar um novo endpoint  
**Aula 4. Testes automatizados**  
Objetivos:  
- diferença entre unitário e integração  
- leitura dos testes existentes  
Atividade:  
- pedir ao estudante para escrever um teste novo  
**Aula 5. Segurança de API**  
Objetivos:  
- autenticação por API key  
- headers de segurança  
- rate limiting  
- erros padronizados  
Atividade:  
- testar endpoint sem chave  
- observar 401  
- testar respostas de erro  
**Aula 6. Frontend com Blazor**  
Objetivos:  
- mostrar consumo da API  
- explicar fluxo do utilizador  
Atividade:  
- alterar o frontend para mostrar nova informação  
**18. Exercícios para os estudantes**  
**Exercício 1**  
Adicionar um novo item ao cardápio.  
**Exercício 2**  
Criar um endpoint para fechar um pedido.  
**Exercício 3**  
Escrever testes unitários para o fechamento do pedido.  
**Exercício 4**  
Trocar o repositório in-memory por uma base de dados real.  
**Exercício 5**  
Adicionar autenticação mais avançada com JWT.  
**Exercício 6**  
Criar uma nova versão da API em v2.  
**19. Erros comuns que os estudantes podem encontrar**  
**A API não arranca**  
Verificar:  
- se o .NET 8 está instalado  
- se a porta 5000 já está em uso  
**O frontend não comunica com a API**  
Verificar:  
- se a API está a correr  
- se o endereço base está correto  
- se a política de CORS permite a origem  
**Os testes falham**  
Verificar:  
- se o projeto foi restaurado  
- se houve alteração nas regras de negócio  
- se a API key usada nos testes continua correta  
**20. Boas práticas para ensinar com este projeto**  
- começar pelo problema de negócio antes do código  
- explicar o motivo de cada camada  
- mostrar fluxo real de uma request  
- pedir sempre teste antes e depois de mudança  
- ensinar HTTP, arquitetura e segurança juntos  
- reforçar que código limpo sem testes não basta  
**21. Resultado esperado de aprendizagem**  
No final, o estudante deve conseguir:  
- entender a estrutura de uma solution .NET  
- criar endpoints REST  
- modelar regras no domínio  
- escrever testes unitários e de integração  
- documentar APIs  
- aplicar práticas básicas de segurança  
- evoluir o projeto sem quebrar a arquitetura  
**22. Comandos essenciais para os estudantes**  
dotnet restore  
 dotnet build  
 dotnet run --project src/GoodHamburger.Api  
 dotnet run --project src/GoodHamburger.Blazor --urls "http://localhost:5001"  
 dotnet test  
 dotnet test --filter "FullyQualifiedName~Unit"  
 dotnet test --filter "FullyQualifiedName~Integration"  
 dotnet test --collect:"XPlat Code Coverage"  
 docker compose up --build  
 docker compose down  
   
**23. Leitura recomendada dentro do projeto**  
Para estudar o backend:  
- [Program.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Program.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Program.cs:1")  
- [OrdersController.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Controllers/OrdersController.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Controllers/OrdersController.cs:1")  
- [OrderService.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Services/OrderService.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Services/OrderService.cs:1")  
- [Order.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/Order.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/Order.cs:1")  
Para estudar segurança:  
- [ExceptionHandlerMiddleware.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/ExceptionHandlerMiddleware.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/ExceptionHandlerMiddleware.cs:1")  
- [SecurityHeadersMiddleware.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/SecurityHeadersMiddleware.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/SecurityHeadersMiddleware.cs:1")  
- [ApiKeyAuthenticationHandler.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationHandler.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationHandler.cs:1")  
Para estudar testes:  
- [OrderDiscountTests.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Unit/OrderDiscountTests.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Unit/OrderDiscountTests.cs:1")  
- [OrdersApiTests.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Integration/OrdersApiTests.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Integration/OrdersApiTests.cs:1")  
**24. Conclusão**  
Este projeto foi organizado para ser pequeno o suficiente para ensinar, mas completo o suficiente para mostrar um fluxo profissional de desenvolvimento.  
Ele permite ensinar ao mesmo tempo:  
- arquitetura  
- backend  
- frontend  
- testes  
- documentação  
- segurança  
