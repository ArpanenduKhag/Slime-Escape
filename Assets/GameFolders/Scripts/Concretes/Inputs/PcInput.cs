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

        // 🟩 New controls for slime size
        public bool IsGrowButtonDown => Input.GetKeyDown(KeyCode.Z);  // Grow slime
        public bool IsShrinkButtonDown => Input.GetKeyDown(KeyCode.X); // Shrink slime
    }
}
