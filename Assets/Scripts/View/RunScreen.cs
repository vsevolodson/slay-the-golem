using System;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.State;
using Game.Unity.Config;
using TMPro;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class RunScreen : MonoBehaviour
    {
        [SerializeField] private GameContentAsset _content;
        [SerializeField] private long _seed = 1;

        [SerializeField] private CombatPanel _combatPanel;
        [SerializeField] private RewardPanel _rewardPanel;
        [SerializeField] private TextMeshProUGUI _outcome;
        [SerializeField] private GameObject _actionButton;
        [SerializeField] private TextMeshProUGUI _actionButtonLabel;

        private RunConfig _config;
        private Run _run;

        private void Start()
        {
            if (_content == null)
                throw new InvalidOperationException($"{name}: content asset is not assigned");

            _config = _content.ToRunConfig();

            _combatPanel.CommandIssued += OnCommandIssued;
            _rewardPanel.CardChosen += OnCardChosen;

            StartRun((ulong)_seed);
        }

        public void OnActionButtonClicked()
        {
            if (_run.State.Phase == RunPhase.InCombat)
            {
                OnCommandIssued(new EndTurnCommand());
                return;
            }

            StartRun((ulong)DateTime.Now.Ticks);
        }

        private void StartRun(ulong seed)
        {
            _run = Run.StartNew(_config, seed);

            Redraw();
        }

        private void OnCommandIssued(ICommand command)
        {
            _run.Execute(command);

            Redraw();
        }

        private void OnCardChosen(CardId card) => OnCommandIssued(new ChooseRewardCommand(card));

        private void Redraw()
        {
            var phase = _run.State.Phase;
            var runIsOver = phase == RunPhase.Won || phase == RunPhase.Lost;
            var showCombat = phase != RunPhase.ChoosingReward;

            _combatPanel.gameObject.SetActive(phase != RunPhase.ChoosingReward);
            _rewardPanel.gameObject.SetActive(phase == RunPhase.ChoosingReward);

            if (showCombat && _run.CurrentCombat != null)
                _combatPanel.Show(_run.CurrentCombat, _content);

            if (phase == RunPhase.ChoosingReward)
                _rewardPanel.Show(_run.State.RewardOffer, _content);

            _outcome.text = OutcomeText(phase);

            _actionButton.SetActive(phase != RunPhase.ChoosingReward);
            _actionButtonLabel.text = runIsOver ? "New game" : "End turn";
        }

        private static string OutcomeText(RunPhase phase)
        {
            switch (phase)
            {
                case RunPhase.Won: return "Victory";
                case RunPhase.Lost: return "Dedeat";
                default: return string.Empty;
            }
        }
    }
}