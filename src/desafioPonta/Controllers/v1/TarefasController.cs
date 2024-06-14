using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Tarefa.Events;
using desafioPonta.Core.Domain.Tarefa.Interfaces;
using desafioPonta.Extensions;
using desafioPonta.Inputs;
using desafioPonta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace desafioPonta.Controllers.v1;

/// <summary>
/// Representa uma tarefa
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/tarefas")]
public class TarefasController : ControllerBase
{

    private readonly ILogger<TarefasController> _logger;

    /// <summary>
    /// Construtor Padrão
    /// </summary>
    /// <param name="logger"></param>
    public TarefasController(ILogger<TarefasController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Obtem todas as tarefas, podendo expota-las para CSV conforme parametro.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="exportar"></param>    
    /// <param name="retornarTarefaEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Get(
         [FromQuery] string? status,
         [FromQuery] bool? exportar,
         [FromServices] IRequestHandler<DomainEvent<RetornarTarefaEvent>, List<TarefaEntity>> retornarTarefaEventHandler,
         CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Obtem todas as tarefas, podendo expota-las para CSV conforme parametro.");

        _logger.LogInformation("Buscando tarefas do repositório.");

        // Define os intervalos de chave
        int intervaloInicial = 0;
        int intervaloTamanho = 1000;

        var evento = this.CriarEventoDominio(new RetornarTarefaEvent(intervaloInicial, intervaloTamanho, status, false));
        var tarefas = await retornarTarefaEventHandler.Handle(evento, cancellationToken);

        if (!tarefas.Any())
        {            
            return NoContent();
        }

        if (exportar.HasValue && exportar.Value)
        {
            _logger.LogInformation("Exportando tarefas para CSV.");
            var csv = new StringBuilder();
            csv.AppendLine($"{nameof(TarefaModel.Id)};{nameof(TarefaModel.Titulo)};{nameof(TarefaModel.Descricao)};{nameof(TarefaModel.Status)};{nameof(TarefaModel.DataCriacao)};{nameof(TarefaModel.DataAtualizacao)};{nameof(TarefaModel.Usuario)}");

            foreach (var tarefa in tarefas)
            {
                csv.AppendLine($"{tarefa.Id};{tarefa.Titulo};{tarefa.Descricao};{tarefa.Status};{tarefa.DataCriacao};{tarefa.DataAtualizacao};{tarefa.Usuario}");
            }

            var fileContentResult = new FileContentResult(Encoding.UTF8.GetBytes(csv.ToString()), "application/octet-stream")
            {
                FileDownloadName = $"RelatorioTarefas.csv"
            };

            return fileContentResult;
        }

        _logger.LogInformation("Retornando tarefas encontradas.");
        return Ok(tarefas.Select(x => CriarModelo(x)).ToList());
    }

    /// <summary>
    /// Obtem todas as tarefas por status.
    /// </summary>
    /// <param name="status"></param>    
    /// <param name="retornarTarefaEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> GetByStatus(
         [FromQuery] string status,
         [FromServices] IRequestHandler<DomainEvent<RetornarTarefaEvent>, List<TarefaEntity>> retornarTarefaEventHandler,
         CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Obtem todas as tarefas por status.");

        _logger.LogInformation("Buscando tarefas com status: {Status}", status);

        // Define os intervalos de chave
        int intervaloInicial = 0;
        int intervaloTamanho = 1000;

        var evento = this.CriarEventoDominio(new RetornarTarefaEvent(intervaloInicial, intervaloTamanho, status, true));
        var tarefas = await retornarTarefaEventHandler.Handle(evento, cancellationToken);

        if (!tarefas.Any())
        {            
            return NoContent();
        }

        // Processamento paralelo para converter as entidades para o modelo
        var tarefasModel = tarefas
            .AsParallel()
            .WithCancellation(cancellationToken)
            .Select(CriarModelo)
            .ToList();

        _logger.LogInformation("Retornando tarefas encontradas.");
        return Ok(tarefasModel);
    }

    /// <summary>
    /// Obtem todas as tarefas por Usuário.
    /// </summary>        
    /// <param name="retornarTarefaUsuarioEventHandler"></param>
    /// <param name="cancellationToken"></param>    
    /// <returns></returns>
    [HttpGet("usuario")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> GetByUsuario(
         [FromServices] IRequestHandler<DomainEvent<RetornarTarefaUsuarioEvent>, List<TarefaEntity>> retornarTarefaUsuarioEventHandler,
         CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Obtem todas as tarefas por Usuário.");
        try
        {
            if (User.Identity is ClaimsIdentity identity)
            {
                var userClaims = identity.Claims;
                var usuario = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(usuario))
                {
                    _logger.LogWarning("Usuário não identificado.");
                    return BadRequest(new ErrorModel { Title = "Usuário não identificado." });
                }

                _logger.LogInformation("Buscando tarefas para o usuário: {Usuario}", usuario);

                var evento = this.CriarEventoDominio(new RetornarTarefaUsuarioEvent(usuario));
                var tarefas = await retornarTarefaUsuarioEventHandler.Handle(evento, cancellationToken);

                if (!tarefas.Any())
                {                    
                    return NoContent();
                }

                // Processamento paralelo para converter as entidades para o modelo
                var tarefasModel = tarefas
                    .AsParallel()
                    .WithCancellation(cancellationToken)
                    .Select(CriarModelo)
                    .ToList();

                _logger.LogInformation("Retornando tarefas encontradas.");
                return Ok(tarefasModel);
            }
            else
            {
                return Unauthorized();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tarefas por usuário.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Title = "Erro interno", Stack = ex.Message });
        }
    }


    /// <summary>
    /// Obtem todas as tarefas.
    /// </summary>    
    /// <param name="tarefaRepository"></param>
    /// <param name="status"></param>
    /// <param name="numeroPaginas"></param>
    /// <param name="quantidadePagina"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> GetAll(
         [FromQuery] string? status,
         [FromServices] ITarefaRepository tarefaRepository,
         CancellationToken cancellationToken,
         [FromQuery] int numeroPaginas = 1,
         [FromQuery] int quantidadePagina = 10)
    {
        _logger.LogInformation("Iniciando método: Obtem todas as tarefas.");
        try
        {
            _logger.LogInformation("Buscando todas as tarefas do repositório.");
            var tarefas = await tarefaRepository.FindAllAsync(cancellationToken);
            if (!tarefas.Any())
            {
                _logger.LogWarning("Nenhuma tarefa encontrada.");
                return NoContent();
            }

            // Aplica os filtros de status, se fornecidos
            if (!string.IsNullOrWhiteSpace(status))
            {
                _logger.LogInformation("Aplicando filtro de status: {Status}", status);
                tarefas = tarefas.Where(x => x.Status.Equals(status)).ToList();
            }

            // Paginação
            _logger.LogInformation("Aplicando paginação: Página {NumeroPagina}, Tamanho {QuantidadePagina}", numeroPaginas, quantidadePagina);
            var tarefasPaginadas = tarefas.Skip((numeroPaginas - 1) * quantidadePagina).Take(quantidadePagina).ToList();

            _logger.LogInformation("Retornando tarefas paginadas.");
            return Ok(tarefasPaginadas.Select(x => CriarModelo(x)).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as tarefas.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel { Title = "Erro interno", Stack = ex.Message });
        }
    }


    /// <summary>
    /// Cria uma tarefa.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="criarTarefaEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult<TarefaModel>> Post(
        [FromBody] CriarTarefaInput input,
        [FromServices] IRequestHandler<DomainEvent<CriarTarefaEvent>, TarefaEntity> criarTarefaEventHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Cria uma tarefa.");


        if (User.Identity is ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;
            var usuario = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(usuario))
            {
                _logger.LogWarning("Usuário não identificado.");
                return BadRequest(new ErrorModel { Title = "Usuário não identificado." });
            }

            _logger.LogInformation("Usuário identificado: {Usuario}", usuario);

            var evento = this.CriarEventoDominio(new CriarTarefaEvent(input.titulo, input.descricao, usuario));
            var novaTarefa = await criarTarefaEventHandler.Handle(evento, cancellationToken);

            _logger.LogInformation("Tarefa criada com sucesso: {IdTarefa}", novaTarefa.Id);
            return Ok(novaTarefa);
        }
        else
        {
            return Unauthorized();
        }
    }


    /// <summary>
    /// Atualiza uma Tarefa.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="atualizarTarefaEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("atualizar")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Post(
        [FromBody] AtualizarTarefaInput input,
        [FromServices] IRequestHandler<DomainEvent<AtualizarTarefaEvent>, TarefaEntity> atualizarTarefaEventHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Atualiza uma Tarefa.");


        if (User.Identity is ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;
            var usuario = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usuario))
            {
                _logger.LogWarning("Usuário não identificado.");
                return BadRequest(new ErrorModel { Title = "Usuário não identificado." });
            }

            _logger.LogInformation("Usuário identificado: {Usuario}", usuario);

            var evento = this.CriarEventoDominio(new AtualizarTarefaEvent(input.id, input.titulo, input.descricao, usuario));
            var tarefa = await atualizarTarefaEventHandler.Handle(evento, cancellationToken);

            _logger.LogInformation("Tarefa atualizada com sucesso: {IdTarefa}", tarefa.Id);
            return Ok(CriarModelo(tarefa));
        }
        else
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Andamento em uma Tarefa.
    /// </summary>
    /// <param name="idTarefa"></param>
    /// <param name="andamentoTarefaEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("andamento/{idTarefa}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Post(
        [FromRoute] int idTarefa,
        [FromServices] IRequestHandler<DomainEvent<AndamentoTarefaEvent>, TarefaEntity> andamentoTarefaEventHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Andamento em uma Tarefa.");


        if (User.Identity is ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;
            var usuario = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usuario))
            {
                _logger.LogWarning("Usuário não identificado.");
                return BadRequest(new ErrorModel { Title = "Usuário não identificado." });
            }

            _logger.LogInformation("Usuário identificado: {Usuario}", usuario);

            var evento = this.CriarEventoDominio(new AndamentoTarefaEvent(idTarefa, usuario));
            var tarefa = await andamentoTarefaEventHandler.Handle(evento, cancellationToken);

            _logger.LogInformation("Tarefa atualizada para andamento com sucesso: {IdTarefa}", tarefa.Id);
            return Ok(CriarModelo(tarefa));
        }
        else
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Conclusão de uma Tarefa.
    /// </summary>
    /// <param name="idTarefa"></param>
    /// <param name="conclusaoTarefaEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("conclusao/{idTarefa}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Post(
        [FromRoute] int idTarefa,
        [FromServices] IRequestHandler<DomainEvent<ConclusaoTarefaEvent>, TarefaEntity> conclusaoTarefaEventHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Conclusão de uma Tarefa.");


        if (User.Identity is ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;
            var usuario = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usuario))
            {
                _logger.LogWarning("Usuário não identificado.");
                return BadRequest(new ErrorModel { Title = "Usuário não identificado." });
            }

            _logger.LogInformation("Usuário identificado: {Usuario}", usuario);

            var evento = this.CriarEventoDominio(new ConclusaoTarefaEvent(idTarefa, usuario));
            var tarefa = await conclusaoTarefaEventHandler.Handle(evento, cancellationToken);

            _logger.LogInformation("Tarefa concluída com sucesso: {IdTarefa}", tarefa.Id);
            return Ok(CriarModelo(tarefa));
        }
        else
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Exclui uma tarefa.
    /// </summary>
    /// <param name="id">O ID da tarefa a ser excluída.</param>
    /// <param name="excluirTarefaEventHandler">O manipulador do evento de exclusão de tarefa.</param>
    /// <param name="cancellationToken">O token de cancelamento da operação.</param>
    /// <returns>Um ActionResult representando o resultado da operação.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Delete(
        int id,
        [FromServices] IRequestHandler<DomainEvent<ExcluirTarefaEvent>> excluirTarefaEventHandler,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Exclui uma Tarefa.");


        if (User.Identity is ClaimsIdentity identity)
        {
            var userClaims = identity.Claims;
            var usuario = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usuario))
            {
                _logger.LogWarning("Usuário não identificado ao excluir tarefa.");
                return BadRequest(new ErrorModel { Title = "Usuário não identificado ao excluir tarefa." });
            }

            _logger.LogInformation("Usuário identificado: {Usuario}", usuario);

            var evento = this.CriarEventoDominio(new ExcluirTarefaEvent(id, usuario));
            await excluirTarefaEventHandler.Handle(evento, cancellationToken);

            _logger.LogInformation("Tarefa excluída com sucesso: {IdTarefa}", id);
            return Ok();
        }
        else
        {
            return Unauthorized();
        }

    }

    /// <summary>
    /// Obtem as informações da tarefa.
    /// </summary>
    /// <param name="idTarefa">O ID da tarefa a ser obtida.</param>
    /// <param name="tarefaRepository">O repositório de tarefas.</param>
    /// <param name="configuration">A configuração do sistema.</param>
    /// <param name="cancellationToken">O token de cancelamento da operação.</param>
    /// <returns>Um ActionResult representando o resultado da operação.</returns>
    [HttpGet("{idTarefa}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult<TarefaModel>> Get(
        [FromRoute] int idTarefa,
        [FromServices] ITarefaRepository tarefaRepository,
        [FromServices] IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando método: Obtem as informações da tarefa.");

        var tarefa = await tarefaRepository.FindByIdAsync(idTarefa, cancellationToken);

        if (tarefa is null)
        {
            _logger.LogWarning("Tarefa não encontrada para o ID: {IdTarefa}", idTarefa);
            return NoContent();
        }

        _logger.LogInformation("Tarefa encontrada: {IdTarefa} - {Titulo}", tarefa.Id, tarefa.Titulo);
        return Ok(CriarModelo(tarefa));
    }
    private TarefaModel CriarModelo(TarefaEntity entidade) => new(this, entidade);        
}
