using UnityEngine;
using Abstracts.Input;

namespace Inputs
{
    public class PcInput : IPlayerInput
    {
        public float HorizontalAxis => Input.GetAxis("Horizontal");
        public bool IsJumpButtonDown => Input.GetButtonDown("Jump");
        public bool IsJumpButton => Input.GetButton("Jump");
        public bool IsDownButton => Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        public bool IsInteractButton => Input.GetKeyDown(KeyCode.E);
        public bool IsExitButton => Input.GetKeyDown(KeyCode.Escape);

        // 🟩 Tap-based (for PC)
        public bool IsGrowButtonDown => Input.GetKey(KeyCode.Z);   // Hold or tap to grow
        public bool IsShrinkButtonDown => Input.GetKey(KeyCode.X); // Hold or tap to shrink

        // 🟩 Hold-based (same for PC — mirrors Down for compatibility)
        public bool IsGrowButtonHeld => Input.GetKey(KeyCode.Z);
        public bool IsShrinkButtonHeld => Input.GetKey(KeyCode.X);
    }
}
