using UnityEngine;
using Abstracts.Input;
using Movements;
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
            // ✅ Use Joystick input on Android
            _joystick = FindObjectOfType<JoystickController>();
            if (_joystick != null)
            {
                _input = _joystick;
                Debug.Log("🎮 Using JoystickController for Android input");

                // Boost player feel
                _rb.SetPrivateSpeedMultiplier(androidMoveSpeedMultiplier);
                _rb.SetPrivateJumpMultiplier(androidJumpMultiplier);
            }
            else
            {
                Debug.LogWarning("⚠️ No JoystickController found — fallback to PC input.");
                _input = new PcInput();
            }
#else
            // ✅ Use keyboard on PC/Web/Editor
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

            // 🔹 Handle pause toggle
            if (_input.IsExitButton)
            {
                SoundManager.Instance.PlaySound(2);
                if (_isPaused) GameManager.Instance.UnpauseGame();
                else GameManager.Instance.PauseGame();
            }

            if (_isPaused) return;

            // 🔹 Movement input
            _horizontalAxis = _input.HorizontalAxis;

            // 🔹 Walk sound
            if (_horizontalAxis != 0 && _groundCheck.IsOnGround)
                SoundManager.Instance.PlaySound(1);
            else
                SoundManager.Instance.StopSound(1);

            // 🔹 Jump
            if (_input.IsJumpButtonDown && _groundCheck.IsOnGround)
                _isJumped = true;

            // 🔹 Drop through platforms
            if (_input.IsDownButton)
                _platform.DisableCollider();

            // 🔹 Interact
            if (_input.IsInteractButton)
                _interact.Interact();

            // 🔹 Handle Grow / Shrink (hold-based)
            HandleSizeControl();

            // 🔹 Animation handling
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

        // 🔹 Centralized size control logic
        private void HandleSizeControl()
        {
            if (_sizeControl == null) return;

            bool growHeld = _input.IsGrowButtonHeld || Input.GetKey(KeyCode.G);
            bool shrinkHeld = _input.IsShrinkButtonHeld || Input.GetKey(KeyCode.H);

            // Grow
            if (growHeld && !shrinkHeld)
            {
                _sizeControl.OnGrowPressed(true);
                _sizeControl.OnShrinkPressed(false);
            }
            // Shrink
            else if (shrinkHeld && !growHeld)
            {
                _sizeControl.OnShrinkPressed(true);
                _sizeControl.OnGrowPressed(false);
            }
            // Neither
            else
            {
                _sizeControl.OnGrowPressed(false);
                _sizeControl.OnShrinkPressed(false);
            }
        }

        private void HandleGameUnpaused() => _isPaused = false;
        private void HandleGamePaused() => _isPaused = true;
    }
}
