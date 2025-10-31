using Abstracts.Input;
using Movements;
using UnityEngine;
using Inputs;
using Animations;
using Mechanics;
using Managers;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        bool _isJumped;
        float _horizontalAxis;
        IPlayerInput _input;
        CharacterAnimation _anim;
        RbMovement _rb;
        Flip _flip;
        GroundCheck _groundCheck;
        PlatformHandler _platform;
        InteractHandler _interact;
        PlayerSizeControl _sizeControl;

        private bool _isPaused;
        private JoystickController _joystick;

        // 🔹 Jump multiplier for Android
        [Header("Platform Settings")]
        [SerializeField] private float androidJumpMultiplier = 1.35f;
        [SerializeField] private float androidMoveSpeedMultiplier = 1.1f;

        private void Awake()
        {
            _rb = GetComponent<RbMovement>();
            _anim = GetComponent<CharacterAnimation>();
            _flip = GetComponent<Flip>();
            _groundCheck = GetComponent<GroundCheck>();
            _platform = GetComponent<PlatformHandler>();
            _interact = GetComponent<InteractHandler>();
            _sizeControl = GetComponent<PlayerSizeControl>();

#if UNITY_ANDROID && !UNITY_EDITOR
            // On Android → Use joystick input
            _joystick = FindObjectOfType<JoystickController>();
            if (_joystick != null)
            {
                _input = _joystick;
                Debug.Log("🎮 Using JoystickController for Android input");

                // Boost movement feel for touch devices
                _rb.SetPrivateSpeedMultiplier(androidMoveSpeedMultiplier);
                _rb.SetPrivateJumpMultiplier(androidJumpMultiplier);
            }
            else
            {
                Debug.LogWarning("⚠️ No JoystickController found, falling back to PC input.");
                _input = new PcInput();
            }
#else
            // On PC / Web / Editor
            _input = new PcInput();
            Debug.Log("⌨️ Using PcInput for Editor/PC input");
#endif
        }

        private void OnEnable()
        {
            GameManager.Instance.OnGamePaused += HandleGamePaused;
            GameManager.Instance.OnGameUnpaused += HandleGameUnpaused;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGamePaused -= HandleGamePaused;
            GameManager.Instance.OnGameUnpaused -= HandleGameUnpaused;
        }

        private void Update()
        {
            if (_input == null) return;

            // 🔹 Pause toggle
            if (_input.IsExitButton)
            {
                SoundManager.Instance.PlaySound(2);
                if (_isPaused) GameManager.Instance.UnpauseGame();
                else GameManager.Instance.PauseGame();
            }

            if (_isPaused) return;

            // 🔹 Horizontal input
            _horizontalAxis = _input.HorizontalAxis;

            // 🔹 Walk sound
            if (_horizontalAxis != 0 && _groundCheck.IsOnGround)
                SoundManager.Instance.PlaySound(1);
            else
                SoundManager.Instance.StopSound(1);

            // 🔹 Jump
            if (_input.IsJumpButtonDown && _groundCheck.IsOnGround)
            {
                _isJumped = true;
            }

            // 🔹 Drop down through platforms
            if (_input.IsDownButton)
                _platform.DisableCollider();

            // 🔹 Interact
            if (_input.IsInteractButton)
                _interact.Interact();

            // 🔹 Grow / Shrink (Mobile)
            if (_input.IsGrowButtonDown && _sizeControl != null)
                _sizeControl.Grow();
            if (_input.IsShrinkButtonDown && _sizeControl != null)
                _sizeControl.Shrink();

            // 🔹 Animations
            _anim.JumpAnFallAnim(_groundCheck.IsOnGround, _rb.VelocityY);
            _anim.HorizontalAnim(_horizontalAxis);
            _flip.FlipCharacter(_horizontalAxis);
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;

            _rb.HorizontalMove(_horizontalAxis);

            if (_isJumped)
            {
                SoundManager.Instance.PlaySound(0);
                _rb.Jump();
                _isJumped = false;
            }
        }

        private void HandleGameUnpaused() => _isPaused = false;
        private void HandleGamePaused() => _isPaused = true;
    }
}
