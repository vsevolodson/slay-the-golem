using Game.Core.State;
using Game.Unity.Config;
using UnityEngine;

namespace Game.Net
{
    public sealed class CardArtProvider : MonoBehaviour
    {
        [SerializeField] private GameContentAsset _content;

        public static CardArtProvider Instance { get; private set; }

        private void Awake() => Instance = this;

        public Sprite SpriteFor(string cardId) => _content.GetCard(new CardId(cardId)).Icon;
    }
}