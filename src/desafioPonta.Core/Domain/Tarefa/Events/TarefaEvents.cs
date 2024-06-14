using desafioPonta.Core.Domain.Tarefa.Entities;

namespace desafioPonta.Core.Domain.Tarefa.Events;

/// <summary>
/// Evento de retorno de tarefa 
/// </summary>
/// <param name="intervaloInicial"></param>
/// <param name="intervaloTamanho"></param>
/// <param name="status"></param>
/// <param name="statusObrigatorio"></param>
public record RetornarTarefaEvent(int intervaloInicial, int intervaloTamanho, string status, bool statusObrigatorio);

/// <summary>
/// Evento de retorno de tarefa 
/// </summary>
/// <param name="usuario"></param>
public record RetornarTarefaUsuarioEvent(string usuario);


/// <summary>
/// Evento de criação da tarefa
/// </summary>
/// <param name="Titulo"></param>
/// <param name="Descricao"></param>
public record CriarTarefaEvent(string Titulo, string Descricao, string Usuario)
{
    public static implicit operator TarefaEntity(CriarTarefaEvent instace)
    {
        return new()
        {
            Titulo = instace.Titulo,
            Descricao = instace.Descricao,
            DataCriacao = DateTimeOffset.Now,
            DataAtualizacao = DateTimeOffset.Now,
            Status = Entities.TarefaStatus.Pendente,
            Usuario = instace.Usuario
        };
    }
}

/// <summary>
/// Evento de atualização da tarefa
/// </summary>
/// <param name="Id"></param>
/// <param name="Titulo"></param>
/// <param name="Descricao"></param>
/// <param name="Usuario"></param>
public record AtualizarTarefaEvent(int Id, string Titulo, string Descricao, string Usuario);

/// <summary>
/// Evento de atualização do Status da tarefa
/// </summary>
/// <param name="Id"></param>
/// <param name="Status"></param>
/// <param name="Usuario"></param>
public record AtualizarStatusTarefaEvent(int Id, string Status, string Usuario);

/// <summary>
/// Evento de andamento da tarefa
/// </summary>
/// <param name="Id"></param>
/// <param name="Usuario"></param>
public record AndamentoTarefaEvent(int Id, string Usuario);

/// <summary>
/// Evento de conclusão da tarefa
/// </summary>
/// <param name="Id"></param>
/// <param name="Usuario"></param>
public record ConclusaoTarefaEvent(int Id, string Usuario);

/// <summary>
/// Evento de exclusão da tarefa
/// </summary>
/// <param name="Id"></param>
/// <param name="Usuario"></param>
public record ExcluirTarefaEvent(int Id, string Usuario);

