using Game.Unity.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Unity.View
{
    public sealed class CardFace : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private Image _art;

        public void Show(CardAsset asset)
        {
            _title.text = asset.Title;
            _cost.text = asset.Cost.ToString();

            _art.enabled = asset.Icon != null;
            if (_art.enabled)
                _art.sprite = asset.Icon;
        }
    }
}
