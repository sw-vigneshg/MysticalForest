using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultHandler : UiHandler
{
    [Header("Result Variables ************* ")]
    [SerializeField] private List<Sprite> Numbers;
    [SerializeField] private Image[] WinAmountImages;
    //[SerializeField] private GameObject GlowImage;
    [SerializeField] private GameObject Popup;

    [Header("Script Reference ************ ")]
    private GameManager _GameManager;

    private void Start()
    {
        _GameManager = GameManager.Instance;

        if (_GameManager != null)
        {
            _GameManager.WinAmountAction += ShowWinAmount;
        }
    }

    public override void ShowMe()
    {
        base.ShowMe();
        OnShowMe();
    }
    public override void HideMe()
    {
        base.HideMe();
        OnHideMe();
    }

    private void OnShowMe()
    {
        //GlowImage.SetActive(false);
        AudioController.Instance.PlayWinning();
        Popup.transform.localScale = new Vector3(1,0,1);
        Popup.transform?.DOScaleY(1,0.5f).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            //GlowImage.transform.localScale = Vector3.zero;
            //GlowImage.SetActive(true);
            //GlowImage.transform.DOScale(new Vector3(2f,2f,2f),1).SetEase(Ease.InOutBounce);
            //GlowImage.transform.DOLocalRotate(new Vector3(0,0,-360),5 , RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
            //GlowImage.transform.DOScale(new Vector3(1.7f,1.7f,1.7f),1).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear);
        });
    }

    private void OnHideMe()
    {
        Popup.transform?.DOKill();
        //GlowImage.transform?.DOKill();
    }

    private void ShowWinAmount(float amount)
    {
        Debug.Log($"Win amount is : {amount}");
        string amountString = amount.ToString("F2");
        foreach (Image image in WinAmountImages)
        {
            image.gameObject.SetActive(false);
        }

        for (int i = 0; i < amountString.Length; i++)
        {
            char c = amountString[i];
            WinAmountImages[i].sprite = c == '.' ? Numbers.Find(x => x.name == "Dot") : Numbers.Find(x => x.name == c.ToString());
            WinAmountImages[i].gameObject.SetActive(true);
        }
    }
}
