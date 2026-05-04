using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 상호작용 시스템 검증용 스텁. 본 시설물(가챠/밭/사육장/상점)이 추가되면 제거 또는 비활성화 권장.
    /// </summary>
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _promptText = "테스트";
        [SerializeField] private float _interactionRange = 2f;
        [SerializeField] private bool _toggleUIModeOnInteract = true;

        public Vector3 Position => transform.position;
        public float InteractionRange => _interactionRange;
        public string PromptText => _promptText;
        public bool CanInteract => true;

        public void OnInteract(GameManager gm)
        {
            Debug.Log($"[TestInteractable] '{_promptText}' 상호작용 발생!");
            if (_toggleUIModeOnInteract && gm != null)
            {
                gm.SetUIModeActive(true);
            }
        }
    }
}
