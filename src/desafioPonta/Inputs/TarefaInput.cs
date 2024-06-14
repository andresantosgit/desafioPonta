using desafioPonta.Core.Domain.Tarefa.Events;
using System.ComponentModel.DataAnnotations;

namespace desafioPonta.Inputs;

/// <summary>
/// Modelo de entrada para criar uma tarefa.
/// </summary>
public record CriarTarefaInput(

    /// <summary>
    /// Título da Tarefa
    /// </summary>
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "O título deve ter entre 10 e 100 caracteres.")]
    string titulo,

    /// <summary>
    /// Descrição da Tarefa
    /// </summary>
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 500 caracteres.")]
    string descricao
    
    )
{
    /// <summary>
    /// converte implicitamente um objeto de entrada em um evento de domínio.
    /// </summary>
    /// <param name="instance"></param>
    public static implicit operator CriarTarefaEvent(CriarTarefaInput instance)
        => new(instance.titulo, instance.descricao, string.Empty);
}

public record AtualizarTarefaInput(

    /// <summary>
    /// Título da Tarefa
    /// </summary>
    [Required(ErrorMessage = "O Id da tarefa é obrigatório.")]    
    int id,

    /// <summary>
    /// Título da Tarefa
    /// </summary>
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "O título deve ter entre 10 e 100 caracteres.")]
    string titulo,

    /// <summary>
    /// Descrição da Tarefa
    /// </summary>
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 500 caracteres.")]
    string descricao    
    )
{
    /// <summary>
    /// converte implicitamente um objeto de entrada em um evento de domínio.
    /// </summary>
    /// <param name="instance"></param>
    public static implicit operator AtualizarTarefaEvent(AtualizarTarefaInput instance)
        => new(instance.id, instance.titulo, instance.descricao, string.Empty);
}
