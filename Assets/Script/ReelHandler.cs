using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class ReelHandler : MonoBehaviour
{
    [SerializeField] private bool IsAnimating = false;
    [SerializeField] private int ReelIndex;
    [SerializeField] private RectTransform[] SpinAnimation;
    private int _Speed = 3000;

    public void HandleSpin(bool animate)
    {
        IsAnimating = animate;
        foreach (RectTransform rect in SpinAnimation)
        {
            rect.gameObject.SetActive(animate);
        }
        if(animate)
        {
            StopCoroutine(nameof(Spin));
            StartCoroutine(nameof(Spin));
        }
    }

    private IEnumerator Spin()
    {
        SpinAnimation[0].anchoredPosition = Vector2.zero;
        SpinAnimation[1].anchoredPosition = new Vector2(0, 645);
        while (IsAnimating)
        {
            foreach (RectTransform rect in SpinAnimation)
            {
                rect.Translate(Vector2.down * _Speed * Time.deltaTime);
                if (rect.anchoredPosition.y <= -645)
                {
                    rect.anchoredPosition = new Vector2(0, 645);
                }
            }
            yield return null;
        }
    }

}
