using System;
using System.Collections.Generic;
using Game.Core.State;
using Game.Unity.Config;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class HandView : MonoBehaviour
    {
        [SerializeField] private CardView _cardPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private Transform _dragLayer;

        private readonly List<CardView> _views = new List<CardView>();

        public event Action<CardView> CardPickedUp;
        public event Action<CardView, Vector2> CardDropped;

        public void Show(IReadOnlyList<CardId> hand, CombatContentAsset content)
        {
            while (_views.Count < hand.Count)
                _views.Add(CreateView());

            for (var i = 0; i < _views.Count; i++)
            {
                var visible = i < hand.Count;
                _views[i].gameObject.SetActive(visible);

                if (visible)
                    _views[i].Show(hand[i], content.FindCard(hand[i]));
            }
        }

        private CardView CreateView()
        {
            var view = Instantiate(_cardPrefab, _container);

            view.SetDragLayer(_dragLayer);
            view.DragBegun += OnDragBegun;
            view.DragEnded += OnDragEnded;

            return view;
        }

        private void OnDragBegun(CardView view) => CardPickedUp?.Invoke(view);

        private void OnDragEnded(CardView view, Vector2 screenPosition) => CardDropped?.Invoke(view, screenPosition);
    }
}