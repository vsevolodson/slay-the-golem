using Game.Core;
using Game.Core.Commands;
using Game.Core.State;
using Game.Unity.Config;
using TMPro;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class CombatScreen : MonoBehaviour
    {
        [SerializeField] private CombatContentAsset _content;
        [SerializeField] private string _enemyId = "rat";
        [SerializeField] private int _playerMaxHealth = 50;
        [SerializeField] private long _seed = 1;

        [SerializeField] private CombatantView _playerView;
        [SerializeField] private CombatantView _enemyView;
        [SerializeField] private IntentView _intentView;
        [SerializeField] private HandView _handView;
        [SerializeField] private TextMeshProUGUI _energy;
        [SerializeField] private TextMeshProUGUI _piles;
        [SerializeField] private TextMeshProUGUI _turn;
        [SerializeField] private TextMeshProUGUI _outcome;

        private Combat _combat;

        private void Start()
        {
            var enemy = _content.FindEnemy(_enemyId);

            var setup = new CombatSetup(
                _playerMaxHealth,
                _playerMaxHealth,
                _content.StartingDeck(),
                enemy.Id,
                enemy.MaxHealth,
                (ulong)_seed);

            _combat = Combat.StartNew(setup, _content.ToCombatConfig());

            Redraw();
        }

        public void OnEndTurnClicked()
        {
            _combat.Execute(new EndTurnCommand());

            Redraw();
        }

        private void Redraw()
        {
            var state = _combat.State;

            _playerView.Show("Player", state.Player.Combatant);
            _enemyView.Show(_content.FindEnemy(state.Enemy.EnemyId).Title, state.Enemy.Combatant);
            _intentView.Show(_combat.EnemyIntent);
            _handView.Show(state.Player.Hand, _content);

            _energy.text = $"Energy {state.Player.Energy}";
            _piles.text = $"Draw {state.Player.DrawPile.Count}   Discard {state.Player.DiscardPile.Count}";
            _turn.text = $"Turn {state.TurnNumber}";
            _outcome.text = OutcomeText(state.Outcome);
        }

        private static string OutcomeText(CombatOutcome outcome)
        {
            switch (outcome)
            {
                case CombatOutcome.PlayerWon: return "Victory";
                case CombatOutcome.PlayerLost: return "Defeat";
                default: return string.Empty;
            }
        }
    }
}