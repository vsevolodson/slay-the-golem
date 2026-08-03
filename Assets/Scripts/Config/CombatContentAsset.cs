using System.Collections.Generic;
using Game.Core.Data;
using Game.Core.State;
using UnityEngine;

namespace Game.Unity.Config
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Combat Content", fileName = "CombatContent")]
    public sealed class CombatContentAsset : ScriptableObject
    {
        [SerializeField] private List<CardAsset> _cards = new List<CardAsset>();
        [SerializeField] private List<EnemyAsset> _enemies = new List<EnemyAsset>();
        [SerializeField] private List<CardAsset> _startingDeck = new List<CardAsset>();
        [SerializeField] private int _energyPerTurn = 3;
        [SerializeField] private int _cardsDrawnPerTurn = 5;

        public CombatConfig ToCombatConfig()
        {
            var cards = new List<CardDefinition>();
            foreach (var card in _cards)
                cards.Add(card.ToDefinition());

            var enemies = new List<EnemyDefinition>();
            foreach (var enemy in _enemies)
                enemies.Add(enemy.ToDefinition());

            return new CombatConfig(
                new CardCatalog(cards),
                new EnemyCatalog(enemies),
                new CombatRules(_energyPerTurn, _cardsDrawnPerTurn));
        }

        public List<CardId> StartingDeck()
        {
            var deck = new List<CardId>();
            foreach (var card in _startingDeck)
                deck.Add(card.Id);

            return deck;
        }

        public CardAsset FindCard(CardId id)
        {
            foreach (var card in _cards)
            {
                if (card.Id == id)
                    return card;
            }

            return null;
        }

        public EnemyAsset FindEnemy(string id)
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.Id == id)
                    return enemy;
            }

            return null;
        }
    }
}