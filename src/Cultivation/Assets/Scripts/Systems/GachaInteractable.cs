using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 가챠 빌딩 시설물. OnInteract 시 가챠 UI 진입 예정(Phase 5).
    /// </summary>
    public class GachaInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private float _interactionRange = 2.5f;

        public Vector3 Position => transform.position;
        public float InteractionRange => _interactionRange;
        public string PromptText => "가챠";
        public bool CanInteract => true;

        public void OnInteract(GameManager gm)
        {
            Debug.Log("[Gacha] 상호작용 (Phase 5에서 가챠 UI 연결 예정).");
            if (gm != null) gm.SetUIModeActive(true);
        }
    }
}
