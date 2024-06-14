using desafioPonta.Controllers.v1;
using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace desafioPonta.Models;

/// <summary>
/// Modelo de tafefas
/// </summary>
public class TarefaModel
{
    /// <summary>
    /// Construtor padrão
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="entity"></param>
    public TarefaModel(ControllerBase ctrl, TarefaEntity entity)
    {
        Id = entity.Id;
        Titulo = entity.Titulo;
        Descricao = entity.Descricao;
        DataCriacao = entity.DataCriacao;
        DataAtualizacao = entity.DataAtualizacao;
        Status = entity.Status;
        Usuario = entity.Usuario;        

        // Adiciona links HATEOAS ao modelo
        Links["self"] = ctrl.Link<TarefasController>(
           nameof(TarefasController.Get), routeValues: new { Id = entity.Id }
        );

        Links["post"] = ctrl.Link<TarefasController>(
           nameof(TarefasController.Post), routeValues: new { }
        );
    }

    /// <summary>
    /// Id da Tarefa    
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Titulo da Tarefa
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da Tarefa
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Data da criação da Tarefa
    /// </summary>
    public DateTimeOffset? DataCriacao { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Data da alteração da Tarefa
    /// </summary>
    public DateTimeOffset? DataAtualizacao { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Status da Tarefa
    /// </summary>
    public TarefaStatus Status { get; set; }

    /// <summary>
    /// Usuário que criou a tarefa
    /// </summary>
    public string Usuario { get; set; }

    /// <summary>
    /// Links para ações relacionadas.
    /// </summary>
    public Dictionary<string, string> Links { get; set; } = new();
}
