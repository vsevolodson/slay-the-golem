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

        private readonly List<CardView> _views = new List<CardView>();

        public void Show(IReadOnlyList<CardId> hand, CombatContentAsset content)
        {
            while (_views.Count < hand.Count)
                _views.Add(Instantiate(_cardPrefab, _container));

            for (var i = 0; i < _views.Count; i++)
            {
                var visible = i < hand.Count;
                _views[i].gameObject.SetActive(visible);

                if (visible)
                    _views[i].Show(hand[i], content.FindCard(hand[i]));
            }
        }
    }
}