using Mirror;
using UnityEngine;

namespace Game.Net
{
    public sealed class NetworkCard : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        [SyncVar(hook = nameof(OnCardIdChanged))]
        private string _cardId;

        [SyncVar]
        private uint _holderNetId;

        public bool IsHeld => _holderNetId != 0;

        public uint HolderNetId => _holderNetId;

        [Server]
        public void SetCard(string cardId) => _cardId = cardId;

        [Server]
        public void SetHolder(uint holderNetId) => _holderNetId = holderNetId;

        public override void OnStartClient() => ApplySprite(_cardId);

        private void OnCardIdChanged(string oldCardId, string newCardId) => ApplySprite(newCardId);

        private void ApplySprite(string cardId)
        {
            if (string.IsNullOrEmpty(cardId) || CardArtProvider.Instance == null)
                return;

            _renderer.sprite = CardArtProvider.Instance.SpriteFor(cardId);
        }
    }
}