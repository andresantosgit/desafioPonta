using desafioPonta.Controllers.v1;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace desafioPonta.Models;

/// <summary>
/// Modelo de Usuario
/// </summary>
public class UsuarioModel
{
    /// <summary>
    /// Construtor padrão
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="entity"></param>
    public UsuarioModel(ControllerBase ctrl, UsuarioEntity entity)
    {
        Id = entity.Id;
        Usuario = entity.Usuario;
        Email = entity.Email;
        Senha = entity.Senha;        

        // Adiciona links HATEOAS ao modelo
        Links["self"] = ctrl.Link<AuthController>(
           nameof(TarefasController.Get), routeValues: new { Id = entity.Id }
        );

        Links["post"] = ctrl.Link<AuthController>(
           nameof(TarefasController.Post), routeValues: new { }
        );
    }

    /// <summary>
    /// ID do usuário.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome de usuário.
    /// </summary>
    public string Usuario { get; set; }

    /// <summary>
    /// Endereço de e-mail.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Senha { get; set; }

    /// <summary>
    /// Links para ações relacionadas.
    /// </summary>
    public Dictionary<string, string> Links { get; set; } = new();
}
