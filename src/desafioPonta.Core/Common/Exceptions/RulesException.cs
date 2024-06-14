using System.Diagnostics.CodeAnalysis;

namespace desafioPonta.Core.Common.Exceptions;

public class RulesException : Exception
{
    public RulesException(List<(string Member, string Message)> messages, string? message = null)
        : base(message)
    {
        Messages = messages.Where(m => !string.IsNullOrWhiteSpace(m.Member)).ToList();
    }

    public RulesException(string member, string message)
        : base(null)
    {
        Messages = new()
        {
            new (member, message)
        };
    }

    public static void ThrowIfNull([NotNull] object? argument, string member, string message)
    {
        if (argument is null)
        {
            Throw(member, message);
        }
    }

    [DoesNotReturn]
    private static void Throw(string member, string message) =>
        throw new RulesException(member, message);

    public List<(string Member, string Message)> Messages { get; }
}
