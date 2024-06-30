using DG.Tweening;
using UnityEngine;

public class PanelAnimation : MonoBehaviour
{
   private RectTransform PanelTransform;

    private void OnEnable()
    {
        PanelTransform = GetComponent<RectTransform>();
        PanelTransform.DOScale(Vector3.one,0.5f).From(0).SetEase(Ease.InOutBounce);
    }
}
