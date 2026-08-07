using System;
using System.Collections.Generic;
using Game.Core.State;
using Game.Unity.Config;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class RewardPanel : MonoBehaviour
    {
        [SerializeField] private RewardCardView[] _cards;

        public event Action<CardId> CardChosen;

        private void Awake()
        {
            foreach (var card in _cards)
                card.Clicked += OnCardClicked;
        }

        public void Show(IReadOnlyList<CardId> offer, GameContentAsset content)
        {
            for (var i = 0; i < _cards.Length; i++)
            {
                var visible = i < offer.Count;
                _cards[i].gameObject.SetActive(visible);

                if (visible)
                    _cards[i].Show(offer[i], content.GetCard(offer[i]));
            }
        }

        private void OnCardClicked(CardId card) => CardChosen?.Invoke(card);
    }
}