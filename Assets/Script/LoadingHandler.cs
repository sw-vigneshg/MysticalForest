using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHandler : UiHandler
{
    [Header("Game Panels ***************** ")]
    public Image GameLogo;
    public GameObject LoadingPanel;

    [Header("Loading Info **************** ")]
    [SerializeField] private Image FillImage;

    [Header("Script Reference ************ ")]
    private UIController _UIController;

    private void Start()
    {
        _UIController = UIController.Instance;
    }

    public override void ShowMe()
    {
        base.ShowMe();
        ResetAll();
        OnShowMe();
    }
    public override void HideMe()
    {
        base.HideMe();
        OnHideMe();
    }

    private void ResetAll()
    {
        GameLogo.color = new Color(1, 1, 1, 0.5f);
        GameLogo.gameObject.SetActive(true);

        LoadingPanel.SetActive(false);
        FillImage.fillAmount = 0;
    }

    private void OnShowMe()
    {
        GameLogo?.DOKill();
        GameLogo.DOFade(1, 0.5f).OnComplete(() =>
        {
            GameLogo.DOFade(0, 2f).OnComplete(() =>
            {
                GameLogo.gameObject?.SetActive(false);
                LoadingPanel.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250);
                LoadingPanel.SetActive(true);
                LoadingPanel.transform.GetChild(2).GetComponent<RectTransform>().DOAnchorPosY(110, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
                {
                    LoadingPanel.transform.GetChild(1).transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.7f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                    StopCoroutine(nameof(InitLoading));
                    StartCoroutine(nameof(InitLoading));
                });
            });
        });
    }

    private void OnHideMe()
    {
        _UIController.InGameHandler.ShowMe();
        ResetAll();
    }

    private IEnumerator InitLoading()
    {
        double count = 5;
        while (count > 0)
        {
            count -= Time.deltaTime;
            SetFillValue(count);
            yield return new WaitForEndOfFrame();
        }
        HideMe();
    }

    private void SetFillValue(double value)
    {
        FillImage.fillAmount = Mathf.InverseLerp(5, 0, (float)value);
    }
}
