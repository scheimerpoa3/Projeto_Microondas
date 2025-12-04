READ ME – MICRO-ONDAS DIGITAL

Sistema de simulação de um micro-ondas digital com Web API, interface Web e testes de negócio.
Desenvolvido em .NET Framework 4.7.2.

ARQUITETURA DO SISTEMA

O sistema utiliza camadas separadas:
Microondas.Domain → entidades, DTOs, exceções
Microondas.Business → regras de negócio (serviços)
Microondas.Infrastructure → repositório JSON, logger e segurança
Microondas.WebApi → API para acionamento do micro-ondas e CRUD de programas
Microondas.Tests → testes unitários (NUnit)

REQUISITOS DE EXECUÇÃO

Windows
Visual Studio 2019 ou superior
.NET Framework 4.7.2
Opcional: Postman ou Insomnia para testar a API

CONFIGURAÇÃO

3.1) Autenticação
A API usa Bearer Token. As credenciais estão no Web.config:
Usuario: admin
Senha: senha123

Caso altere a senha, será necessário atualizar o hash no Web.config usando SHA256.
3.2) Persistência
Programas customizados são armazenados em arquivo JSON, automaticamente criado na primeira execução:

programas_custom.json
COMO EXECUTAR O SISTEMA

4.1) Abrir a solução no Visual Studio
Definir o projeto Microondas.WebApi como Startup Project.
Executar com F5.

4.2) Acessar a interface Web
Abrir o navegador com o endereço exibido pelo IIS Express.
Exemplo:
http://localhost:xxxxx/Content/index.html

USANDO O SISTEMA NA INTERFACE WEB

Passos principais:
Fazer Login
Informar usuário: admin
Informar senha: senha123

Clicar em Login
A interface exibirá que está autenticado e liberará os botões.
Selecionar um programa pré-definido
ou utilizar tempo, potência e caractere manualmente.

Clicar em INICIAR
Se um programa estiver selecionado, será utilizado.
Caso contrário, usa as opções manuais preenchidas.
Se nada for informado, usa padrão: 30 segundos, potência 10, caractere “.” Durante o aquecimento

A cada segundo, novos caracteres são exibidos.Ao final, aparece “Aquecimento concluído”

Botão INÍCIO RÁPIDO

Se não houver aquecimento em andamento → inicia com 30s e potência 10
Se já estiver aquecendo → não altera sessão

Pausar e Cancelar
1º clique pausa
2º clique cancela e limpa tudo

Cadastro de programas customizados
Informar: Nome, Alimento, Tempo, Potência e Caractere
Caractere não pode ser ponto (.) nem repetir
Será exibido na lista em itálico como “(custom)”

USANDO A API PELO POSTMAN

6.1) Login
POST /api/auth/login
Body JSON:
{
"Usuario": "admin",
"Senha": "senha123"
}

Resposta contém:
token → obrigatório para chamadas seguintes

Enviar nos headers:
Authorization: Bearer SEU_TOKEN_AQUI

6.2) Endpoints Principais

Iniciar aquecimento
POST /api/microondas/iniciar
Body exemplo:
{
"TempoSegundos": 45,
"Potencia": 8,
"CaractereAquecimento": "*"
}

Início rápido
POST /api/microondas/iniciorapido

Pausar/Cancelar
POST /api/microondas/pausarcancelar

Tick (processar 1s)
POST /api/microondas/tick

Status do aquecimento
GET /api/microondas/status

6.3) Programas de Aquecimento

Listar programas
GET /api/programas

Cadastrar customizado
POST /api/programas/custom

Atualizar customizado
PUT /api/programas/{id}

Remover customizado
DELETE /api/programas/{id}

TRATAMENTO DE EXCEÇÕES E LOGS

Regras de negócio → resposta HTTP 400

Erros inesperados → HTTP 500

Logs são gravados no arquivo:
logs.txt
Incluindo: mensagem, inner exception, stack trace e data/hora

TESTES UNITÁRIOS

Para executar os testes:

Abrir Test Explorer no Visual Studio

Rodar todos os testes do projeto Microondas.Tests

São cobertas regras de negócio do micro-ondas e do cadastro de programas.

CONSIDERAÇÕES FINAIS

O objetivo do projeto é aplicar:
Orientação a Objetos
Boas práticas de arquitetura
Separação de camadas
Web API com autenticação
Persistência de dados
Testes automatizados

O design da interface foi mantido simples conforme requisitos.
