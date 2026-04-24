**Manual Técnico de Desenvolvimento**  
**1. Finalidade deste manual**  
Este documento descreve tecnicamente como o projeto **Good Hamburger** foi construído, quais lógicas foram implementadas, onde cada responsabilidade está localizada e como evoluir a solução sem quebrar a arquitetura.  
Este não é um guia introdutório. O objetivo aqui é explicar:  
- decisões de engenharia  
- responsabilidades por camada  
- fluxo interno da aplicação  
- lógicas de negócio implementadas  
- lógicas de segurança aplicadas  
- estratégia de testes  
- pontos de extensão e manutenção  
**2. Visão geral da solução**  
O projeto é uma solução .NET 8 baseada em Clean Architecture, separada em quatro projetos principais:  
- GoodHamburger.Core  
- GoodHamburger.Infrastructure  
- GoodHamburger.Api  
- GoodHamburger.Blazor  
Além disso, existe um projeto de testes:  
- GoodHamburger.Tests  
E um contrato OpenAPI versionado:  
- [openapi/goodhamburger.v1.yaml](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/openapi/goodhamburger.v1.yaml:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/openapi/goodhamburger.v1.yaml:1")  
**3. Objetivo arquitetural**  
O objetivo da arquitetura foi separar claramente:  
- regra de negócio  
- aplicação  
- infraestrutura  
- entrega HTTP/UI  
Isso foi feito para garantir:  
- baixo acoplamento  
- alta testabilidade  
- substituição de infraestrutura com impacto mínimo  
- manutenção simples  
- possibilidade de crescimento incremental  
**4. Estrutura técnica por projeto**  
**4.1 **GoodHamburger.Core  
É a camada central de domínio.  
Responsabilidades:  
- modelar entidades  
- proteger invariantes de negócio  
- declarar exceções de domínio  
- definir contratos de persistência  
Não deve conhecer:  
- ASP.NET Core  
- banco de dados  
- JSON  
- controllers  
- Swagger  
Arquivos centrais:  
- [Order.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/Order.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/Order.cs:1")  
- [MenuItem.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/MenuItem.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/MenuItem.cs:1")  
- [IRepositories.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Interfaces/IRepositories.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Interfaces/IRepositories.cs:1")  
- [DomainExceptions.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Exceptions/DomainExceptions.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Exceptions/DomainExceptions.cs:1")  
**4.2 **GoodHamburger.Infrastructure  
É a implementação concreta da infraestrutura.  
Responsabilidades:  
- implementar repositórios definidos no Core  
- persistir os dados segundo a tecnologia escolhida  
No estado atual:  
- usa armazenamento em memória  
Arquivo central:  
- [InMemoryRepositories.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Infrastructure/Repositories/InMemoryRepositories.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Infrastructure/Repositories/InMemoryRepositories.cs:1")  
**4.3 **GoodHamburger.Api  
É a camada de entrada HTTP.  
Responsabilidades:  
- expor endpoints REST  
- validar requests  
- aplicar autenticação e autorização  
- configurar pipeline HTTP  
- mapear exceções para respostas HTTP  
- integrar documentação OpenAPI  
Arquivos centrais:  
- [Program.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Program.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Program.cs:1")  
- [OrdersController.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Controllers/OrdersController.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Controllers/OrdersController.cs:1")  
- [OrderService.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Services/OrderService.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Services/OrderService.cs:1")  
- [ExceptionHandlerMiddleware.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/ExceptionHandlerMiddleware.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/ExceptionHandlerMiddleware.cs:1")  
- [SecurityHeadersMiddleware.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/SecurityHeadersMiddleware.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/SecurityHeadersMiddleware.cs:1")  
- [ApiKeyAuthenticationHandler.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationHandler.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationHandler.cs:1")  
**4.4 **GoodHamburger.Blazor  
É a interface cliente da aplicação.  
Responsabilidades:  
- consumir a API  
- representar o fluxo do utilizador  
- encapsular chamadas HTTP no serviço de frontend  
Arquivo central:  
- [ApiService.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Blazor/Services/ApiService.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Blazor/Services/ApiService.cs:1")  
**4.5 **GoodHamburger.Tests  
É a camada de verificação automatizada.  
Responsabilidades:  
- validar regras de negócio puras  
- validar pipeline HTTP real  
- proteger regressões  
Arquivos centrais:  
- [OrderDiscountTests.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Unit/OrderDiscountTests.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Unit/OrderDiscountTests.cs:1")  
- [OrdersApiTests.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Integration/OrdersApiTests.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Integration/OrdersApiTests.cs:1")  
**5. Fluxo técnico de uma requisição**  
Fluxo padrão para um POST /api/v1/orders:  
1. A requisição entra pelo pipeline ASP.NET Core.  
2. O middleware de Security Headers agenda os cabeçalhos de resposta.  
3. O middleware de exceções envolve toda a execução.  
4. O middleware de autenticação valida a API key.  
5. O middleware de autorização permite ou bloqueia o acesso.  
6. O Rate Limiter avalia o cliente e limita abuso.  
7. O controller recebe e valida o DTO.  
8. O OrderService orquestra o caso de uso.  
9. A entidade Order aplica as regras de domínio.  
10. O repositório persiste.  
11. O resultado é mapeado para DTO de resposta.  
12. A resposta HTTP é devolvida ao cliente.  
**6. Lógica de domínio implementada**  
**6.1 Agregado **Order  
O agregado principal do domínio é [Order.cs.](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/Order.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Core/Entities/Order.cs:1")  
Ele foi modelado como agregado raiz porque:  
- concentra o estado do pedido  
- controla os itens internos  
- protege as regras de desconto  
- impede combinações inválidas  
**6.2 Estado interno do pedido**  
O pedido mantém:  
- Id  
- CreatedAt  
- UpdatedAt  
- Status  
- coleção privada _items  
A coleção é privada para impedir mutação externa direta.  
Essa decisão foi aplicada para garantir que:  
- toda mudança passe pelas regras do domínio  
- nenhum consumidor altere o pedido sem validação  
**6.3 Snapshot de item**  
Ao adicionar um item, a entidade não guarda referência viva ao MenuItem.  
Ela cria um OrderItem com:  
- MenuItemId  
- Name  
- UnitPrice  
- Category  
Isto é importante porque preserva o preço histórico do pedido mesmo que o catálogo mude futuramente.  
**6.4 Regra de item duplicado**  
Método envolvido:  
- ValidateDuplicate  
Lógica implementada:  
- só é permitido um item por categoria  
- se já existir um sanduíche, não entra outro sanduíche  
- o mesmo vale para acompanhamento e bebida  
Quando a regra é violada:  
- o domínio lança DomainException  
Motivo técnico:  
- a regra pertence ao negócio  
- não deve depender do controller nem do banco  
- precisa ser garantida independentemente do ponto de entrada  
**6.5 Regra de desconto**  
Método envolvido:  
- CalculateDiscountRate  
Lógica implementada:  
- sanduíche + acompanhamento + bebida = 20%  
- sanduíche + bebida = 15%  
- sanduíche + acompanhamento = 10%  
- restante = 0%  
A implementação usa avaliação booleana por categoria:  
- hasSandwich  
- hasSide  
- hasDrink  
E em seguida aplica um switch por tupla.  
Motivo técnico:  
- deixa a regra explícita  
- melhora legibilidade  
- facilita evolução  
- reduz if/else aninhado  
**6.6 Cálculo financeiro**  
Propriedades computadas:  
- Subtotal  
- DiscountRate  
- DiscountAmount  
- Total  
O valor do desconto usa:  
- Math.Round(..., 2, MidpointRounding.AwayFromZero)  
Isso foi aplicado para evitar ambiguidades de arredondamento financeiro.  
Motivo técnico:  
- 1.125 deve arredondar para 1.13  
- isso é mais previsível para cenários financeiros  
**6.7 Controle de atualização**  
Sempre que o pedido muda:  
- Touch() atualiza UpdatedAt  
Isso foi aplicado em:  
- AddItem  
- ReplaceItems  
- Close  
Motivo técnico:  
- manter rastreabilidade da última alteração  
**7. Lógica de aplicação implementada**  
**7.1 **OrderService  
O serviço [OrderService.cs implementa a orquestração dos casos de uso.](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Services/OrderService.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Services/OrderService.cs:1")  
Ele não deve substituir o domínio.  
A responsabilidade dele é:  
- obter dados  
- resolver dependências  
- coordenar repositórios  
- mapear entidade para DTO  
**7.2 **CreateAsync  
Fluxo:  
1. valida se o request está vazio  
2. cria uma nova instância de Order  
3. resolve IDs para itens reais do menu  
4. adiciona os itens ao pedido  
5. persiste o pedido  
6. devolve OrderResponse  
Importante:  
- a duplicidade é validada pela entidade, não pelo service  
**7.3 **UpdateAsync  
Fluxo:  
1. localiza o pedido  
2. se não existir, lança NotFoundException  
3. valida request  
4. resolve itens  
5. chama ReplaceItems  
6. persiste atualização  
**7.4 **DeleteAsync  
Fluxo:  
1. verifica se o pedido existe  
2. se não existir, lança NotFoundException  
3. remove do repositório  
**7.5 Resolução de itens do menu**  
Método:  
- ResolveMenuItemsAsync  
Responsabilidade:  
- transformar uma lista de IDs em entidades válidas de catálogo  
Se algum ID for inválido:  
- lança NotFoundException  
Motivo técnico:  
- manter o domínio trabalhando com objetos válidos  
- evitar propagação de IDs inválidos para a lógica central  
**8. Lógica de transporte HTTP**  
**8.1 Controllers**  
Os controllers foram mantidos finos.  
Responsabilidade:  
- receber a request  
- delegar ao service  
- devolver a resposta HTTP apropriada  
Decisão aplicada:  
- evitar regra de negócio em controller  
**8.2 DTOs**  
Os DTOs estão em [OrderDTOs.cs.](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/DTOs/OrderDTOs.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/DTOs/OrderDTOs.cs:1")  
Funções:  
- modelar entrada  
- modelar saída  
- modelar erro  
O CreateOrderRequest usa:  
- [Required]  
- [MinLength(1)]  
- [MaxLength(3)]  
Motivo técnico:  
- deixar parte da validação já no boundary HTTP  
- bloquear requests inválidos cedo  
**8.3 Problem Details**  
O projeto padroniza erros com application/problem+json.  
Isso foi aplicado para:  
- consistência de respostas  
- clareza para consumidores  
- alinhamento com RFC 7807  
**9. Pipeline HTTP e inicialização**  
Toda a composição principal está em [Program.cs.](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Program.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Program.cs:1")  
**9.1 Bootstrap de logging**  
O projeto usa Serilog.  
Objetivos:  
- logs estruturados  
- bootstrap seguro  
- visibilidade de erros no startup  
**9.2 Registro de serviços**  
Principais serviços registrados:  
- controllers  
- problem details  
- autenticação  
- autorização  
- repositórios  
- serviços de aplicação  
- CORS  
- Swagger/OpenAPI  
- rate limiter  
**9.3 Desabilitar **Server Header  
Configuração:  
- builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);  
Motivo técnico:  
- reduzir exposição de detalhes do servidor  
**9.4 CORS**  
Foi criada uma política com origens lidas da configuração:  
- Cors:AllowedOrigins  
Motivo técnico:  
- limitar consumo cross-origin a clientes conhecidos  
- não abrir a API com AllowAnyOrigin  
**9.5 Swagger/OpenAPI**  
O Swagger foi configurado com:  
- metadados da API  
- esquema de segurança por API key  
- operation filter para marcar endpoints protegidos  
- comentários XML  
Motivo técnico:  
- tornar o contrato explícito  
- melhorar DX  
- aproximar a solução de uma abordagem API-first  
**9.6 Rate limiting**  
Foi escolhido um GlobalLimiter particionado.  
A partição é feita por:  
- nome do cliente autenticado, quando autenticado  
- IP, quando anónimo  
Configuração atual:  
- 100 requisições por minuto  
- fila de 10  
Motivo técnico:  
- mitigar abuso  
- proteger fluxo de negócio  
- reduzir consumo descontrolado  
**9.7 Ordem do pipeline**  
A ordem aplicada foi:  
1. SecurityHeadersMiddleware  
2. ExceptionHandlerMiddleware  
3. UseHsts e UseHttpsRedirection fora de desenvolvimento  
4. UseCors  
5. UseAuthentication  
6. UseAuthorization  
7. UseRateLimiter  
8. UseSwagger no ambiente de desenvolvimento  
9. MapControllers  
Essa ordem foi escolhida para garantir:  
- captura ampla de exceções  
- autenticação antes do acesso  
- limitação antes da execução final do endpoint  
**10. Lógica de segurança implementada**  
**10.1 Autenticação por API key**  
Arquivos:  
- [ApiKeyAuthenticationDefaults.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationDefaults.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationDefaults.cs:1")  
- [ApiKeyAuthenticationOptions.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationOptions.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationOptions.cs:1")  
- [ApiKeyAuthenticationHandler.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationHandler.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Auth/ApiKeyAuthenticationHandler.cs:1")  
Lógica:  
- a chave é lida do header X-API-Key  
- a chave esperada vem da configuração  
- se o header não existir, o handler devolve NoResult  
- se a chave for inválida, devolve Fail  
- se estiver correta, cria um ClaimsPrincipal  
Motivo técnico:  
- proteger endpoints sensíveis do CRUD  
- introduzir autenticação simples para um projeto didático  
**10.2 Challenge e forbidden customizados**  
O handler também customiza:  
- 401 Unauthorized  
- 403 Forbidden  
Resposta:  
- application/problem+json  
Motivo técnico:  
- manter padrão de erro uniforme em toda a API  
**10.3 Headers de segurança**  
Arquivo:  
- [SecurityHeadersMiddleware.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/SecurityHeadersMiddleware.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/SecurityHeadersMiddleware.cs:1")  
Headers aplicados:  
- Content-Security-Policy  
- X-Content-Type-Options  
- X-Frame-Options  
- Referrer-Policy  
- Permissions-Policy  
- X-Permitted-Cross-Domain-Policies  
- Cache-Control  
Motivo técnico:  
- reduzir superfície de ataque em clientes HTTP/browser  
- evitar sniffing  
- evitar framing  
- reduzir vazamento de contexto  
- evitar cache indevido de respostas  
**10.4 HSTS e HTTPS**  
Fora de desenvolvimento:  
- UseHsts()  
- UseHttpsRedirection()  
Motivo técnico:  
- forçar navegação segura em produção  
**10.5 Tratamento global de exceções**  
Arquivo:  
- [ExceptionHandlerMiddleware.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/ExceptionHandlerMiddleware.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/src/GoodHamburger.Api/Middleware/ExceptionHandlerMiddleware.cs:1")  
Mapeamentos:  
- NotFoundException -> 404  
- DomainException -> 422  
- erro inesperado -> 500  
Detalhe importante:  
- em produção, o detalhe do erro interno é ocultado  
- em desenvolvimento, a mensagem real pode ser usada para diagnóstico  
Motivo técnico:  
- não vazar stack trace  
- manter observabilidade  
- garantir respostas previsíveis  
**11. Estratégia de persistência**  
**11.1 Repositórios em memória**  
No estado atual, a persistência foi implementada em memória para simplificar o projeto.  
Vantagens:  
- setup zero  
- execução rápida  
- facilidade para testes  
- foco no ensino de arquitetura  
Trade-off:  
- os dados não sobrevivem ao reinício  
- não há concorrência transacional real  
**11.2 Thread safety**  
O repositório de pedidos usa ConcurrentDictionary.  
Motivo técnico:  
- permitir acesso concorrente básico com segurança  
**12. Estratégia de documentação da API**  
**12.1 Swagger gerado em runtime**  
O projeto gera documentação interativa via Swashbuckle.  
Objetivo:  
- inspeção rápida  
- testes manuais  
- onboarding técnico  
**12.2 Contrato versionado**  
Arquivo:  
- [goodhamburger.v1.yaml](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/openapi/goodhamburger.v1.yaml:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/openapi/goodhamburger.v1.yaml:1")  
Objetivo:  
- manter uma referência explícita do contrato  
- apoiar integração  
- permitir evolução controlada  
Limitação atual:  
- ainda não existe geração automática bidirecional entre implementação e contrato  
Ou seja:  
- o projeto está mais próximo de contract-aware do que design-first puro  
**13. Estratégia de testes**  
**13.1 Testes unitários**  
Arquivo:  
- [OrderDiscountTests.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Unit/OrderDiscountTests.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Unit/OrderDiscountTests.cs:1")  
Objetivo:  
- validar regras do agregado Order  
Cobertura típica:  
- descontos  
- duplicidade por categoria  
- consistência de subtotal e total  
- comportamento de substituição de itens  
**13.2 Testes de integração**  
Arquivo:  
- [OrdersApiTests.cs](/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Integration/OrdersApiTests.cs:1 "/home/halow-tecnolog/Documentos/GoodHamburger/GoodHamburger/tests/GoodHamburger.Tests/Integration/OrdersApiTests.cs:1")  
Objetivo:  
- validar o pipeline HTTP real  
Cobertura típica:  
- status codes  
- CRUD completo  
- headers de segurança  
- autenticação obrigatória  
- integração controller/service/repository  
**13.3 Por que duas camadas de teste**  
Essa separação foi aplicada porque:  
- testes unitários são rápidos e isolam regra de negócio  
- testes de integração validam o sistema de ponta a ponta  
**14. Decisões técnicas importantes**  
**14.1 Regra no domínio, não no controller**  
Decisão:  
- validação de duplicidade e desconto ficam na entidade  
Ganho:  
- a regra fica protegida independentemente da interface  
**14.2 Repositório atrás de interface**  
Decisão:  
- acesso a dados abstraído no Core  
Ganho:  
- infraestrutura trocável  
**14.3 DTO separado da entidade**  
Decisão:  
- não expor entidade diretamente para API  
Ganho:  
- boundary claro  
- menor acoplamento  
**14.4 Erro padronizado**  
Decisão:  
- usar ProblemResponse  
Ganho:  
- previsibilidade para cliente  
**14.5 Segurança como parte do pipeline**  
Decisão:  
- autenticação, rate limiting, headers e exception handling vivem no pipeline HTTP  
Ganho:  
- defesa consistente e transversal  
**15. Como evoluir o projeto**  
**15.1 Trocar persistência por banco real**  
Passos técnicos esperados:  
1. criar implementação com EF Core  
2. manter interfaces do Core  
3. trocar registro em Program.cs  
**15.2 Migrar API key para JWT**  
Passos técnicos esperados:  
1. trocar handler custom por JwtBearer  
2. incluir claims reais  
3. implementar autorização por papel ou dono  
**15.3 Implementar BOLA real**  
Hoje não há conceito de utilizador dono de pedido.  
Para implementar isso:  
1. adicionar identidade do criador do pedido  
2. armazenar owner no domínio  
3. validar acesso por owner ou role  
**15.4 Evoluir para API-first real**  
Passos:  
1. definir contrato primeiro  
2. validar implementação contra contrato  
3. automatizar checks de drift  
**16. Riscos e limitações atuais**  
- persistência em memória  
- autenticação simples por API key  
- ausência de identidade de utilizador por pedido  
- ausência de autorização por objeto  
- sem banco real  
- sem versionamento múltiplo da API em runtime  
Esses pontos não invalidam a solução para ensino, mas precisam ser conhecidos para evolução profissional.  
**17. Resumo técnico final**  
O projeto foi construído com os seguintes princípios:  
- domínio forte  
- controllers finos  
- serviços de aplicação enxutos  
- infraestrutura substituível  
- contrato HTTP explícito  
- testes automatizados  
- segurança embutida no pipeline  
