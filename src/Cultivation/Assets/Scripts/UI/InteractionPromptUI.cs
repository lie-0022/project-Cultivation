using UnityEngine;
using UnityEngine.UI;

namespace Cultivation.UI
{
    /// <summary>
    /// 시설물 접근 시 "[E] 상호작용" 프롬프트를 표시하는 단순 UI. MVP는 legacy UI Text 사용.
    /// </summary>
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Text _label;

        private void Awake()
        {
            Hide();
        }

        public void Show(string message)
        {
            if (_root != null) _root.SetActive(true);
            if (_label != null) _label.text = message;
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }
    }
}
