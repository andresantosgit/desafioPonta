using desafioPonta.Core.Common.Helper;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace desafioPonta.Core.Domain.Tarefa.Entities;

public class TarefaEntity : Entity
{
    /// <summary>
    /// Id da Tarefa
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Titulo da tarefa
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da Tarefa
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da Tarefa
    /// </summary>
    public TarefaStatus Status { get; set; } 

    /// <summary>
    /// Data de criação da tarefa
    /// </summary>
    public DateTimeOffset? DataCriacao { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Data de atualização da tarefa
    /// </summary>
    public DateTimeOffset? DataAtualizacao { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Usuario que criou a tarefa
    /// </summary>
    public string Usuario { get; set; } = string.Empty;
}
public enum TarefaStatus
{
    [Display(Name = "Pendente", Description = "Tarefa está pendente")]
    Pendente,

    [Display(Name = "Em Andamento", Description = "Tarefa está em andamento")]
    EmAndamento,

    [Display(Name = "Concluída", Description = "Tarefa está concluída")]
    Concluida
}

public static class EnumHelper
{
    public static T ConvertStringToEnum<T>(string value) where T : struct
    {
        if (!Enum.TryParse(value, true, out T result))
        {
            throw new ArgumentException($"'{value}' não é um valor válido para o enum '{typeof(T).Name}'");
        }
        return result;
    }

    public static bool ValidateStringToEnum<T>(string value) where T : struct
    {
        return Enum.GetNames(typeof(TarefaStatus)).Any(x => x.Equals(value, StringComparison.OrdinalIgnoreCase));        
    }
}