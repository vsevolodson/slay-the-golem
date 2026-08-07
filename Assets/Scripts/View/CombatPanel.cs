using System;
using Game.Core;
using Game.Core.Commands;
using Game.Unity.Config;
using TMPro;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class CombatPanel : MonoBehaviour
    {
        [SerializeField] private CombatantView _playerView;
        [SerializeField] private CombatantView _enemyView;
        [SerializeField] private IntentView _intentView;
        [SerializeField] private HandView _handView;
        [SerializeField] private TextMeshProUGUI _energy;
        [SerializeField] private TextMeshProUGUI _piles;
        [SerializeField] private TextMeshProUGUI _turn;
        [SerializeField] private RectTransform _playZone;
        [SerializeField] private GameObject _playZoneHighlight;

        public event Action<ICommand> CommandIssued;

        private void Awake()
        {
            _handView.CardPickedUp += OnCardPickedUp;
            _handView.CardDropped += OnCardDropped;
        }

        public void Show(Combat combat, CombatContentAsset content)
        {
            var state = combat.State;
            var enemy = content.GetEnemy(state.Enemy.EnemyId);

            _playerView.Show("Gregor", state.Player.Combatant);
            _enemyView.Show(enemy.Title, state.Enemy.Combatant);
            _intentView.Show(combat.EnemyIntent);
            _handView.Show(state.Player.Hand, content);

            _energy.text = $"Energy {state.Player.Energy}";
            _piles.text = $"Draw {state.Player.DrawPile.Count}   Discard {state.Player.DiscardPile.Count}";
            _turn.text = $"Turn {state.TurnNumber}";
        }

        private void OnCardPickedUp(CardView view) => _playZoneHighlight.SetActive(true);

        private void OnCardDropped(CardView view, Vector2 screenPosition)
        {
            _playZoneHighlight.SetActive(false);

            if (RectTransformUtility.RectangleContainsScreenPoint(_playZone, screenPosition, null))
                CommandIssued?.Invoke(new PlayCardCommand(view.Card));
        }
    }
}