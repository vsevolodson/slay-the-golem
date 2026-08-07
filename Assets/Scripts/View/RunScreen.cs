using System;
using Game.Core;
using Game.Core.Commands;
using Game.Core.Data;
using Game.Core.State;
using Game.Core.Serialization;
using Game.Unity.Config;
using Game.Unity.Save;
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
        [SerializeField] private string _saveFileName = "run.json";

        private RunConfig _config;
        private Run _run;
        private RunSaveStorage _storage;
        private RunPhase _savedPhase;

        private void Start()
        {
            if (_content == null)
                throw new InvalidOperationException($"{name}: content asset is not assigned");

            _config = _content.ToRunConfig();
            _storage = new RunSaveStorage(_saveFileName);

            _combatPanel.CommandIssued += OnCommandIssued;
            _rewardPanel.CardChosen += OnCardChosen;

            if (TryLoadRun())
                Redraw();
            else
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

        private bool TryLoadRun()
        {
            if (!_storage.TryLoad(out var json))
                return false;

            if (!RunSaveSerializer.TryLoad(json, out var state))
                return false;

            if (!Run.TryResume(state, _config, out _run))
                return false;

            _savedPhase = _run.State.Phase;

            return true;
        }

        private void StartRun(ulong seed)
        {
            _run = Run.StartNew(_config, seed);
            _savedPhase = _run.State.Phase;

            SaveRun();
            Redraw();
        }

        private void OnCommandIssued(ICommand command)
        {
            _run.Execute(command);

            SaveIfPhaseChanged();
            Redraw();
        }

        private void SaveIfPhaseChanged()
        {
            if (_run.State.Phase == _savedPhase)
                return;

            _savedPhase = _run.State.Phase;

            SaveRun();
        }

        private void SaveRun() => _storage.Save(RunSaveSerializer.Save(_run.State));

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
                case RunPhase.Lost: return "Defeat";
                default: return string.Empty;
            }
        }
    }
}