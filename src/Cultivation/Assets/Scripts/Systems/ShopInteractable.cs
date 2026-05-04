using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 상점 빌딩 시설물. OnInteract 시 ShopPanel을 엽니다.
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
            gm?.UI?.OpenShopPanel();
        }
    }
}
