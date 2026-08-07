using System;
using System.Collections.Generic;
using Game.Core.Data;
using Game.Core.State;
using UnityEngine;

namespace Game.Unity.Config
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Game Content", fileName = "GameContent")]
    public sealed class GameContentAsset : ScriptableObject
    {
        [SerializeField] private List<CardAsset> _cards = new List<CardAsset>();
        [SerializeField] private List<EnemyAsset> _enemies = new List<EnemyAsset>();
        [SerializeField] private List<CardAsset> _startingDeck = new List<CardAsset>();
        [SerializeField] private int _energyPerTurn = 3;
        [SerializeField] private int _cardsDrawnPerTurn = 5;
        [SerializeField] private List<EnemyAsset> _fights = new List<EnemyAsset>();
        [SerializeField] private List<CardAsset> _rewardPool = new List<CardAsset>();
        [SerializeField] private int _rewardChoices = 3;
        [SerializeField] private int _playerMaxHealth = 50;

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

        public CardAsset GetCard(CardId id)
        {
            foreach (var card in _cards)
            {
                if (card.Id == id)
                    return card;
            }

            throw new InvalidOperationException($"card '{id}' is not in {name}");
        }

        public EnemyAsset GetEnemy(string id)
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.Id == id)
                    return enemy;
            }

            throw new InvalidOperationException($"enemy '{id}' is not in {name}");
        }

        public RunConfig ToRunConfig()
        {
            var fights = new List<string>();
            foreach (var enemy in _fights) fights.Add(enemy.Id);

            var rewards = new List<CardId>();
            foreach (var card in _rewardPool) rewards.Add(card.Id);

            return new RunConfig(ToCombatConfig(), fights, rewards, StartingDeck(), _playerMaxHealth, _rewardChoices);
        }
    }
}