using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class WheelVisualsView : MonoBehaviour
    {
        [SerializeField] private Image wheelBaseImage;
        [SerializeField] private Image pointerImage;
        [SerializeField] private Image backplateImage;

        public void Apply(WheelConfig config)
        {
            if (config == null)
                return;

            if (wheelBaseImage != null && config.WheelSprite != null)
                wheelBaseImage.sprite = config.WheelSprite;

            if (pointerImage != null && config.PointerSprite != null)
                pointerImage.sprite = config.PointerSprite;

            if (backplateImage != null && wheelBaseImage != null)
                backplateImage.sprite = wheelBaseImage.sprite;
        }

        public void PlayResultFeedback()
        {
            if (pointerImage != null)
            {
                pointerImage.transform.DOKill();
                pointerImage.transform.localScale = Vector3.one;
                pointerImage.transform.DOPunchScale(Vector3.one * 0.35f, 0.5f, 8, 0.4f)
                    .SetLink(pointerImage.gameObject);
            }

            if (wheelBaseImage != null)
            {
                wheelBaseImage.transform.DOPunchScale(Vector3.one * 0.04f, 0.35f, 4, 0.3f)
                    .SetLink(wheelBaseImage.gameObject);
            }
        }
    }
}
