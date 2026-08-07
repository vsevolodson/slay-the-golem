using System;
using Game.Core.State;
using Game.Unity.Config;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Unity.View
{
    public sealed class RewardCardView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CardFace _face;

        private CardId _card;

        public event Action<CardId> Clicked;

        public void Show(CardId card, CardAsset asset)
        {
            _card = card;

            _face.Show(asset);
        }

        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(_card);
    }
}
