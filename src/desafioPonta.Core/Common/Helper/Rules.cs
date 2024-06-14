using desafioPonta.Core.Common.Exceptions;
using System.Text.RegularExpressions;

namespace desafioPonta.Core.Common.Helper;

public class Rules
{
    private readonly List<(string Member, string Message)> _messages = new();

    internal static Rules Create()
        => new();

    public Rules NotNullEqualFalse(object @null, string name, Task<bool> value, string errorMessage = null)
    {
        if (@null is null || @null is string @string && string.IsNullOrWhiteSpace(@string))
            return this;

        return IsFalse(name, value.Result, errorMessage);
    }

    public Rules NotNullEqualTrue(object @null, string name, Task<bool> value, string errorMessage = null)
    {
        if (@null is null || @null is string @string && string.IsNullOrWhiteSpace(@string))
            return this;

        return IsTrue(name, value.Result, errorMessage);
    }

    public Rules NotEmpty(string name, string value, string? errorMessage = null)
    {
        return IsTrue(name, !string.IsNullOrWhiteSpace(value), errorMessage);
    }

    public Rules AreNotEquals(string name, object value1, object value2, string? errorMessage = null)
    {
        return IsTrue(name, value1 != value2, errorMessage);
    }

    public Rules AreEquals(string name, object value1, object value2, string? errorMessage = null)
    {
        return IsTrue(name, value1 == value2, errorMessage);
    }

    public Rules RegularExpression(string name, string pattern, string value, string? errorMessage = null)
    {
        return IsTrue(name, value != default && new Regex(pattern).IsMatch(value), errorMessage);
    }

    public Rules Length(string name, int length, string value, string? errorMessage = null)
    {
        return IsTrue(name, value?.Length == length, errorMessage);
    }

    public Rules MinLength(string name, int length, string value, string? errorMessage = null)
    {
        return IsTrue(name, value?.Length >= length, errorMessage);
    }

    public Rules MinLength<T>(string name, int length, IEnumerable<T> value, string? errorMessage = null)
    {
        return IsTrue(name, value.Count() >= length, errorMessage);
    }

    public Rules MaxLength(string name, int length, string value, string? errorMessage = null)
    {
        return IsTrue(name, value == default || value.Length <= length, errorMessage);
    }

    public Rules NotZero(string name, int value, string? errorMessage = null)
    {
        return IsTrue(name, value != 0, errorMessage);
    }

    public Rules NotZero(string name, decimal value, string? errorMessage = null)
    {
        return IsTrue(name, value != 0.0m, errorMessage);
    }

    public Rules Must<T>(string name, Func<T, bool> specification, T value, string? errorMessage = null)
    {
        return IsTrue(name, specification(value), errorMessage);
    }

    public Rules NotNull<T>(string name, T value, string? errorMessage = null)
    {
        bool validValue = false;
        if (value is not string || !string.IsNullOrWhiteSpace(value as string))
            validValue = value != null;

        return IsTrue(name, validValue, errorMessage);
    }

    public Rules Null(string name, object value, string? errorMessage = null)
    {
        return IsTrue(name, value == null, errorMessage);
    }

    public Rules IsFalse(string name, bool value, string? errorMessage)
    {
        return IsTrue(name, !value, errorMessage);
    }

    public Rules IsTrue(string name, bool value, string? errorMessage)
    {
        if (!value)
            _messages.Add((name, string.Format(errorMessage ?? string.Empty, value)));

        return this;
    }

    public bool HasErrors()
    {
        return _messages.Count != 0;
    }

    public Rules Validate(string? message = null)
    {
        if (HasErrors())
            throw new RulesException(_messages.Distinct().ToList(), message);

        return this;
    }

    public Rules Combine(params Rules[] rules)
    {
        if (rules == null)
            return this;
        foreach (var rule in rules)
            if (rule != default) _messages.AddRange(rule._messages);

        return this;
    }

    public Rules Combine(IEnumerable<Rules> rules)
    {
        if (rules == null)
            return this;
        foreach (var rule in rules)
            if (rule != default) _messages.AddRange(rule._messages);

        return this;
    }

    public List<(string Member, string Message)> Messages { get => _messages; }
}
