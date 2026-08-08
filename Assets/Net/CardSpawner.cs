using Game.Unity.Config;
using Mirror;
using UnityEngine;

namespace Game.Net
{
    public sealed class CardSpawner : NetworkBehaviour
    {
        [SerializeField] private GameContentAsset _content;
        [SerializeField] private NetworkCard _cardPrefab;
        [SerializeField] private float _spacing = 1.8f;

        public override void OnStartServer()
        {
            if (_content == null || _cardPrefab == null)
            {
                Debug.LogError($"{name}: content or card prefab is not assigned", this);
                return;
            }

            var cards = _content.Cards;
            var x = -(cards.Count - 1) * _spacing / 2f;

            foreach (var card in cards)
            {
                var instance = Instantiate(_cardPrefab, new Vector3(x, 0f, 0f), Quaternion.identity);

                instance.SetCard(card.Id.Value);
                NetworkServer.Spawn(instance.gameObject);

                x += _spacing;
            }
        }
    }
}
