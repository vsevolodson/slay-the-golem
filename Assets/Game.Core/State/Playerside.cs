using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public sealed class PlayerSide
{
    [JsonProperty] private List<CardId> _drawPile;
    [JsonProperty] private List<CardId> _hand;
    [JsonProperty] private List<CardId> _discardPile;
    
    [JsonProperty] public Combatant Combatant { get; private set; }
    [JsonProperty] public int Energy { get; private set; }

    internal PlayerSide(Combatant combatant, IEnumerable<CardId> startingDeck)
    {
        if (startingDeck == null)
            throw new ArgumentNullException(nameof(startingDeck));

        Combatant = combatant ?? throw new ArgumentNullException(nameof(combatant));
        _drawPile = new List<CardId>(startingDeck);
        _hand = new List<CardId>();
        _discardPile = new List<CardId>();
    }

    [JsonConstructor]
    private PlayerSide() {}

    public IReadOnlyList<CardId> DrawPile => _drawPile;
    public IReadOnlyList<CardId> Hand => _hand;
    public IReadOnlyList<CardId> DiscardPile => _discardPile;

    internal void SetEnergy(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "energy must not be negative");

        Energy = amount;
    }

    internal void SpendEnergy(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "energy cost must not be negative");
        if (amount > Energy)
            throw new InvalidOperationException("not enought energy");
        
        Energy -= amount;
    }

    internal CardId RemoveTopOfDrawPile()
    {
        if (_drawPile.Count == 0)
            throw new InvalidOperationException("draw pile is empty");
        
        var topIndex = _drawPile.Count - 1;
        var card = _drawPile[topIndex];
        _drawPile.RemoveAt(topIndex);
        
        return card;
    }

    internal void AddToHand(CardId card) => _hand.Add(card);

    internal void RemoveFromHand(CardId card)
    {
        var result = _hand.Remove(card);
        if (!result)
            throw new InvalidOperationException($"card '{card}' is not in hand");
    }

    internal void AddToDiscard(CardId card) => _discardPile.Add(card);

    internal void ClearDiscardPile() => _discardPile.Clear();

    internal void SetDrawPile(IEnumerable<CardId> cards)
    {
        if (cards == null)
            throw new ArgumentNullException(nameof(cards));
        _drawPile = new List<CardId>(cards);
    }
}