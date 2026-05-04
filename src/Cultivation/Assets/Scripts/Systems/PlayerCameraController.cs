using UnityEngine;
using UnityEngine.InputSystem;

namespace Cultivation.Systems
{
    /// <summary>
    /// 3인칭 마우스 룩 카메라. 플레이어 주변을 yaw/pitch로 회전하며 LateUpdate에서 위치/회전을 동기화한다.
    /// UI 모드일 때 마우스 입력 차단(커서 잠금 해제는 GameManager 책임).
    /// </summary>
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _height = 1.6f;
        [SerializeField] private float _sensitivity = 2f;
        [SerializeField] private float _minPitch = -30f;
        [SerializeField] private float _maxPitch = 60f;
        [SerializeField] private float _initialPitch = 15f;

        private float _yaw;
        private float _pitch;

        public float Yaw => _yaw;

        private void Start()
        {
            _pitch = _initialPitch;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            bool blockInput = _gameManager != null && _gameManager.IsUIModeActive;
            if (!blockInput)
            {
                var mouse = Mouse.current;
                if (mouse != null)
                {
                    Vector2 d = mouse.delta.ReadValue();
                    _yaw += d.x * _sensitivity * 0.1f;
                    _pitch -= d.y * _sensitivity * 0.1f;
                    _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
                }
            }

            Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 offset = rot * new Vector3(0f, 0f, -_distance);
            Vector3 lookAt = _target.position + Vector3.up * _height;
            transform.position = lookAt + offset;
            transform.LookAt(lookAt);
        }
    }
}
