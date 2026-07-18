using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Shared helper replacing the identical AutoDisableImage/AutoDisableText
    /// copies that previously lived in WheelSliceView, RewardDisplayItemView
    /// and ZoneIndicatorItemView. Works for both Image and TextMeshProUGUI
    /// since both derive from MaskableGraphic.
    /// </summary>
    public static class UIGraphicUtility
    {
        public static void MakeNonInteractive(MaskableGraphic graphic)
        {
            if (graphic == null)
                return;

            graphic.raycastTarget = false;
            graphic.maskable = false;
        }
    }
}
