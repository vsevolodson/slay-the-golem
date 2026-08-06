using System;
using Game.Core.State;
using Game.Unity.Config;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Unity.View
{
    public sealed class RewardCardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private Image _art;

        private CardId _card;

        public event Action<CardId> Clicked;

        public void Show(CardId card, CardAsset asset)
        {
            _card = card;

            _title.text = asset != null ? asset.Title : card.Value;
            _cost.text = asset != null ? asset.Cost.ToString() : "?";

            _art.enabled = asset != null && asset.Icon != null;
            if (_art.enabled)
                _art.sprite = asset.Icon;
        }

        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(_card);
    }
}