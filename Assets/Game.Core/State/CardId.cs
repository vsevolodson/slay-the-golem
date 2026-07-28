using System;
using Newtonsoft.Json;

[JsonConverter (typeof(CardIdJsonConverter))]
public readonly struct CardId : IEquatable<CardId>
{
    private readonly string _value;

    public CardId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("card id must not be empty");

        _value = value;
    }

    public string Value => _value ?? string.Empty;
    public bool Equals(CardId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
    public override bool Equals(object obj) => obj is CardId other && Equals(other);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
    public static bool operator ==(CardId left, CardId right) => left.Equals(right);
    public static bool operator !=(CardId left, CardId right) => !left.Equals(right);
}