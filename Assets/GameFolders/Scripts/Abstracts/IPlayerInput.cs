namespace Abstracts.Input
{
    public interface IPlayerInput
    {
        float HorizontalAxis { get; }
        bool IsJumpButtonDown { get; }
        bool IsJumpButton { get; }
        bool IsDownButton { get; }
        bool IsInteractButton { get; }
        bool IsExitButton { get; }

        // 🟩 For both PC (tap-based) and Android (hold-based)
        bool IsGrowButtonDown { get; }
        bool IsShrinkButtonDown { get; }

        // 🟩 For hold detection on mobile
        bool IsGrowButtonHeld { get; }
        bool IsShrinkButtonHeld { get; }
    }
}
