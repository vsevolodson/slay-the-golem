using Game.Core.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Unity.View
{
    public sealed class CombatantView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _health;
        [SerializeField] private TextMeshProUGUI _block;
        [SerializeField] private TextMeshProUGUI _statuses;

        public void Show(string title, Sprite portrait, Combatant combatant)
        {
            _title.text = title;

            if (_portrait != null)
            {
                _portrait.enabled = portrait != null;
                _portrait.sprite = portrait;
            }
            
            _health.text = $"{combatant.Health}/{combatant.MaxHealth}";
            _block.text = combatant.Block > 0 ? $"Block {combatant.Block}" : string.Empty;
            _statuses.text = Statuses(combatant);
        }

        private static string Statuses(Combatant combatant)
        {
            if (combatant.Poison > 0 && combatant.Vulnerable > 0)
                return $"Poison {combatant.Poison}   Vulnerable {combatant.Vulnerable}";

            if (combatant.Poison > 0)
                return $"Poison {combatant.Poison}";

            if (combatant.Vulnerable > 0)
                return $"Vulnerable {combatant.Vulnerable}";

            return string.Empty;
        }
    }
}