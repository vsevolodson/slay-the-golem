using UnityEngine;
using UnityEngine.UI;

namespace Game.Unity.View
{
    public sealed class RandomSpriteView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _variants;
        [SerializeField] private string _key = "general_panel_variant";

        private void Awake()
        {
            var index = PlayerPrefs.GetInt(_key, 0);

            _image.sprite = _variants[index % _variants.Length];

            PlayerPrefs.SetInt(_key, index + 1);
        }
    }
}