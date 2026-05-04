using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 사육장 슬롯 시설물. slotIndex로 BarnManager.Slots와 매핑된다.
    /// OnInteract는 사육장 UI(크리처화/교배/판매)를 여는 진입점이 될 예정(Phase 5).
    /// </summary>
    public class BarnSlotInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private int _slotIndex;
        [SerializeField] private float _interactionRange = 2f;

        public int SlotIndex => _slotIndex;
        public Vector3 Position => transform.position;
        public float InteractionRange => _interactionRange;
        public string PromptText => $"사육장 #{_slotIndex + 1}";
        public bool CanInteract => true;

        public void OnInteract(GameManager gm)
        {
            Debug.Log($"[BarnSlot {_slotIndex}] 상호작용 (Phase 5에서 사육장 UI 연결 예정).");
            if (gm != null) gm.SetUIModeActive(true);
        }
    }
}
