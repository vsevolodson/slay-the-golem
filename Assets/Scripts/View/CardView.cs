using System;
using Game.Core.State;
using Game.Unity.Config;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Unity.View
{
    public sealed class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Transform _dragLayer;
        private Transform _home;
        private int _homeIndex;

        public CardId Card { get; private set; }

        public event Action<CardView> DragBegun;
        public event Action<CardView, Vector2> DragEnded;

        public void SetDragLayer(Transform dragLayer) => _dragLayer = dragLayer;

        public void Show(CardId card, CardAsset asset)
        {
            Card = card;

            _title.text = asset != null ? asset.Title : card.Value;
            _cost.text = asset != null ? asset.Cost.ToString() : "?";
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _home = transform.parent;
            _homeIndex = transform.GetSiblingIndex();

            transform.SetParent(_dragLayer, true);
            _canvasGroup.blocksRaycasts = false;

            DragBegun?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position += (Vector3)eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(_home, false);
            transform.SetSiblingIndex(_homeIndex);
            _canvasGroup.blocksRaycasts = true;

            DragEnded?.Invoke(this, eventData.position);
        }
    }
}