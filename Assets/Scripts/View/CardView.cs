using Game.Core.State;
using Game.Unity.Config;
using TMPro;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class CardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _cost;

        public CardId Card { get; private set; }

        public void Show(CardId card, CardAsset asset)
        {
            Card = card;

            _title.text = asset != null ? asset.Title : card.Value;
            _cost.text = asset != null ? asset.Cost.ToString() : "?";
        }
    }
}