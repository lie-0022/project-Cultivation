using UnityEngine;
using UnityEngine.InputSystem;

namespace Cultivation.Systems
{
    /// <summary>
    /// 매 0.1초마다 가장 가까운 IInteractable을 갱신하고 HUD InteractionPrompt를 표시.
    /// E키 입력 시 현재 후보의 OnInteract(gameManager)를 호출.
    /// </summary>
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private float _refreshInterval = 0.1f;

        private IInteractable _current;
        private float _timer;

        public IInteractable Current => _current;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _refreshInterval)
            {
                _timer = 0f;
                RefreshNearest();
            }

            bool uiActive = _gameManager != null && _gameManager.IsUIModeActive;
            if (uiActive)
            {
                _gameManager.UI?.HUD?.HidePrompt();
                return;
            }

            var kb = Keyboard.current;
            if (kb != null && kb.eKey.wasPressedThisFrame && _current != null && _current.CanInteract)
            {
                _current.OnInteract(_gameManager);
            }
        }

        private void RefreshNearest()
        {
            if (_player == null) return;

            Vector3 pos = _player.position;
            IInteractable nearest = null;
            float nearestDist = float.MaxValue;

            var all = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] is IInteractable interactable && interactable.CanInteract)
                {
                    float d = Vector3.Distance(pos, interactable.Position);
                    if (d <= interactable.InteractionRange && d < nearestDist)
                    {
                        nearest = interactable;
                        nearestDist = d;
                    }
                }
            }

            if (!ReferenceEquals(nearest, _current))
            {
                _current = nearest;
                var hud = _gameManager?.UI?.HUD;
                if (hud != null)
                {
                    if (_current != null) hud.ShowPrompt(_current.PromptText);
                    else hud.HidePrompt();
                }
            }
        }
    }
}
