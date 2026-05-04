using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 사육장 시설물. OnInteract 시 BarnPanel을 엽니다.
    /// 슬롯 인덱스와 무관하게 BarnPanel 전체를 열어 크리처/교배/판매를 모두 처리합니다.
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
            gm?.UI?.OpenBarnPanel(_slotIndex);
        }
    }
}
