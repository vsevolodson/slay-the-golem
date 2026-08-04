using Game.Core.Data;
using TMPro;
using UnityEngine;

namespace Game.Unity.View
{
    public sealed class IntentView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;

        public void Show(EnemyIntent intent)
        {
            if (intent.Damage > 0 && intent.Block > 0)
                _label.text = $"Atack {intent.Damage} & Block {intent.Block}";
            else if (intent.Damage > 0)
                _label.text = $"Atack {intent.Damage}";
            else
                _label.text = $"Block {intent.Block}";
        }
    }
}