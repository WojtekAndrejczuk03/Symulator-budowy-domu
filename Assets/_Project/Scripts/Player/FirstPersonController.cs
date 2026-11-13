using UnityEngine;
using UnityEngine.InputSystem;

namespace HouseBuilder.Player
{
    /// <summary>
    /// First-person movement: walk, sprint, jump and mouse look.
    /// Reads the "Player" action map (Move, Look, Jump, Sprint) from the input actions asset.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputActionAsset _actions;
        [SerializeField] private Transform _cameraPivot;

        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 7f;
        [SerializeField] private float _jumpHeight = 1.1f;
        [SerializeField] private float _gravity = -20f;

        [Header("Look")]
        [SerializeField] private float _lookSensitivity = 0.1f;
        [SerializeField] private float _maxPitch = 85f;

        // Small downward speed while grounded keeps the controller snapped to the floor.
        private const float GroundedStickVelocity = -2f;

        private CharacterController _controller;
        private InputActionMap _playerMap;
        private InputAction _move;
        private InputAction _look;
        private InputAction _jump;
        private InputAction _sprint;

        private float _pitch;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _playerMap = _actions.FindActionMap("Player", throwIfNotFound: true);
            _move = _playerMap.FindAction("Move", throwIfNotFound: true);
            _look = _playerMap.FindAction("Look", throwIfNotFound: true);
            _jump = _playerMap.FindAction("Jump", throwIfNotFound: true);
            _sprint = _playerMap.FindAction("Sprint", throwIfNotFound: true);
        }

        private void OnEnable()
        {
            _playerMap.Enable();
            SetCursorLocked(true);
        }

        private void OnDisable()
        {
            _playerMap.Disable();
            SetCursorLocked(false);
        }

        private void Update()
        {
            HandleCursorLock();

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Look();
            }

            Move();
        }

        private void Look()
        {
            Vector2 delta = _look.ReadValue<Vector2>() * _lookSensitivity;

            transform.Rotate(0f, delta.x, 0f);

            _pitch = Mathf.Clamp(_pitch - delta.y, -_maxPitch, _maxPitch);
            _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void Move()
        {
            Vector2 input = _move.ReadValue<Vector2>();
            Vector3 direction = transform.right * input.x + transform.forward * input.y;
            float speed = _sprint.IsPressed() ? _sprintSpeed : _walkSpeed;

            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = GroundedStickVelocity;
            }

            if (_controller.isGrounded && _jump.WasPressedThisFrame())
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            _verticalVelocity += _gravity * Time.deltaTime;

            Vector3 velocity = direction * speed + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        // Temporary: Esc frees the mouse, a click captures it again.
        // Will move to the pause menu later.
        private static void HandleCursorLock()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetCursorLocked(false);
            }
            else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
                     && Cursor.lockState != CursorLockMode.Locked)
            {
                SetCursorLocked(true);
            }
        }

        private static void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
