using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 상점 빌딩 시설물. OnInteract 시 작물/크리처 판매 UI 진입 예정(Phase 5).
    /// </summary>
    public class ShopInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private float _interactionRange = 2.5f;

        public Vector3 Position => transform.position;
        public float InteractionRange => _interactionRange;
        public string PromptText => "상점";
        public bool CanInteract => true;

        public void OnInteract(GameManager gm)
        {
            Debug.Log("[Shop] 상호작용 (Phase 5에서 판매 UI 연결 예정).");
            if (gm != null) gm.SetUIModeActive(true);
        }
    }
}
