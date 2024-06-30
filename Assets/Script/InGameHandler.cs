using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InGameHandler : UiHandler
{
    public static InGameHandler Instance;

    [Header("InGame UI Variables ************** ")]
    [SerializeField] private RectTransform SpinAnimGameObject1;
    [SerializeField] private RectTransform SpinAnimGameObject2;
    [SerializeField] private GameObject WelcomPanel;
    [SerializeField] private Toggle WelcomeToggle;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button SpinBtn;
    [SerializeField] private Button StopBtn;
    [SerializeField] private Button PlusBtn;
    [SerializeField] private Button MinusBtn;
    [SerializeField] private Button InfoBtn;
    [SerializeField] private Button NextBtn;
    [SerializeField] private Button PreviousBtn;
    [SerializeField] private Button CloseBtn;
    [SerializeField] private GameObject InfoPanel;
    [SerializeField] private RectTransform[] InfoPanels;
    public float Speed;
    private int _CurrentIndex = 0;
    private bool _CanMove = true;

    [Header("Slot Variables ***************** ")]
    [SerializeField] private RectTransform[] Coloumn1Holder;
    [SerializeField] private RectTransform[] Coloumn2Holder;
    [SerializeField] private RectTransform[] Coloumn3Holder;
    [SerializeField] private RectTransform[] Coloumn4Holder;
    [SerializeField] private RectTransform[] Coloumn5Holder;
    [SerializeField] private RectTransform[] DummySlotParents;
    public SlotManagers[] SlotManagers;

    [Header("Betting Variables *********** ")]
    [SerializeField] private TMP_Text PlayerBalanaceText;
    [SerializeField] private TMP_Text WinAmountText;
    [SerializeField] private TMP_Text BetAmountText;

    [Header("Script Reference ************* ")]
    private GameManager _GameManager;

    private void Awake()
    {
        Instance = this;
        OnWelcomeToggleSelection(false);
        ButtonActions();
    }

    private void ButtonActions()
    {
        WelcomeToggle.onValueChanged.AddListener(delegate { OnWelcomeToggleSelection(WelcomeToggle.isOn); });
        ContinueBtn.onClick.AddListener(delegate { OnClickContinue(); });
        SpinBtn.onClick.AddListener(delegate { OnClickSpin(); });
        StopBtn.onClick.AddListener(delegate { OnClickStop(); });
        PlusBtn.onClick.AddListener(delegate { OnClickPlus(); });
        MinusBtn.onClick.AddListener(delegate { OnClickMinus(); });
        InfoBtn.onClick.AddListener(delegate { OnClickInfo(true); });
        CloseBtn.onClick.AddListener(delegate { OnClickInfo(false); });
        NextBtn.onClick.AddListener(delegate { OnClickNext(); });
        PreviousBtn.onClick.AddListener(delegate { OnClickPrevious(); });
    }

    private void Start()
    {
        _GameManager = GameManager.Instance;
        _GameManager.PlayerBalanceAction += UpdatePlayerAmount;
        _GameManager.BetAmountAction += UpdateBetAmount;
        _GameManager.WinAmountAction += UpdateWinAmount;
    }

    private void OnWelcomeToggleSelection(bool isOn)
    {
        WelcomeToggle.isOn = isOn;
        PlayerPrefs.SetInt("Toggle", isOn ? 1 : 0);
    }

    public override void ShowMe()
    {
        base.ShowMe();
        OnShowMe();
    }
    public override void HideMe()
    {
        base.HideMe();
        OnShowMe();
    }

    private void OnShowMe()
    {
        //int val = PlayerPrefs.GetInt("Toggle");
        CheckPlusMinus();
        WelcomPanel.SetActive(WelcomeToggle.isOn);
    }

    public void UpdatePlayerAmount(float amount)
    {
        Debug.Log("Player Balance ******* " + amount);
        PlayerBalanaceText.text = $"$ {amount}";
    }

    public void UpdateBetAmount(float amount)
    {
        Debug.Log("Bet Amount ******* " + amount);
        BetAmountText.text = $"$ {amount.ToString("F2")}";
    }

    public void UpdateWinAmount(float amount)
    {
        Debug.Log("Win Amount ******* " + amount);
        WinAmountText.text = $"$ {amount}";
    }

    public void CheckPlusMinus()
    {
        float val = _GameManager.GetBetAmount();
        PlusBtn.interactable = (val < 25);
        MinusBtn.interactable = (val > 0.1f);
    }

    private void OnClickPlus()
    {
        _GameManager.OnIncreseBetAmount();
        CheckPlusMinus();
    }

    private void OnClickMinus()
    {
        _GameManager.OnDecreseBetAmount();
        CheckPlusMinus();
    }

    private void OnClickInfo(bool canEnable)
    {
        _CurrentIndex = 0;
        foreach (RectTransform panel in InfoPanels)
        {
            panel.anchoredPosition = new Vector2(2500, 0);
        }
        InfoPanels[_CurrentIndex].anchoredPosition = Vector2.zero;
        _CanMove = canEnable;
        InfoPanel.SetActive(canEnable);
    }

    private void OnClickNext()
    {
        if (InfoPanel.activeSelf)
        {
            if (_CanMove)
            {
                if (_CurrentIndex >= 3) return;
                _CanMove = false;
                InfoPanels[_CurrentIndex].DOAnchorPosX(-2500, 1).SetEase(Ease.InOutFlash);
                _CurrentIndex += 1;
                InfoPanels[_CurrentIndex].DOAnchorPosX(0, 1).SetEase(Ease.InOutFlash).OnComplete(() =>
                {
                    _CanMove = true;
                });
            }
        }
    }

    private void OnClickPrevious()
    {
        if (InfoPanel.activeSelf)
        {
            if (_CanMove)
            {
                if (_CurrentIndex <= 0) return;
                _CanMove = false;
                InfoPanels[_CurrentIndex].DOAnchorPosX(2500, 1).SetEase(Ease.InOutFlash);
                _CurrentIndex -= 1;
                InfoPanels[_CurrentIndex].DOAnchorPosX(0, 1).SetEase(Ease.InOutFlash).OnComplete(() =>
                {
                    _CanMove = true;
                });
            }
        }
    }

    private void OnClickContinue()
    {
        if (WelcomPanel.activeSelf)
        {
            WelcomPanel.SetActive(false);
        }
    }

    private void OnClickSpin()
    {
        _GameManager.IsSpinStarted = true;
        _GameManager.UpdateBalanceOnBet();
        EnableSpin(true);
        ResetSlotManagers();
        CheckUIButtons();
        StopCoroutine(nameof(SpinAnimation));
        StartCoroutine(nameof(SpinAnimation));

        CancelInvoke(nameof(OnClickStop));             // For Auto Spin Stop ....... if need we can use ...........
        Invoke(nameof(OnClickStop), 5);
    }

    private IEnumerator SpinAnimation()
    {
        Vector3 targetPos = new Vector3(SpinAnimGameObject1.anchoredPosition.x, -615f, 0);
        Vector3 startingPos = new Vector3(SpinAnimGameObject1.anchoredPosition.x, 675f, 0);

        while (_GameManager.IsSpinStarted)
        {
            SpinAnimGameObject1.transform.Translate(Vector3.down * Speed * Time.deltaTime);
            SpinAnimGameObject2.transform.Translate(Vector3.down * Speed * Time.deltaTime);

            if (SpinAnimGameObject1.transform.localPosition.y <= -620f)
            {
                SpinAnimGameObject1.transform.localPosition = startingPos;
            }

            if (SpinAnimGameObject2.transform.localPosition.y <= -620f)
            {
                SpinAnimGameObject2.transform.localPosition = startingPos;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void EnableSpin(bool canEnable)
    {
        SpinAnimGameObject1.gameObject.SetActive(canEnable);
        SpinAnimGameObject2.gameObject.SetActive(canEnable);
    }

    private void OnClickStop()
    {
        if (_GameManager.IsSpinStarted)
        {
            _GameManager.IsSpinStarted = false;
            StopBtn.interactable = false;
            EnableSpin(false);
            SpinAnimGameObject1.transform.localPosition = new Vector3(SpinAnimGameObject1.anchoredPosition.x, 30f, 0);
            SpinAnimGameObject2.transform.localPosition = new Vector3(SpinAnimGameObject1.anchoredPosition.x, 675f, 0);
            SetSlotData();
            StopCoroutine(nameof(OnStoSpin));
            StartCoroutine(nameof(OnStoSpin));
        }
    }

    private void CheckUIButtons()
    {
        StopBtn.gameObject.SetActive(_GameManager.IsSpinStarted);
        StopBtn.interactable = true;
        PlusBtn.interactable = !_GameManager.IsSpinStarted;
        MinusBtn.interactable = !_GameManager.IsSpinStarted;
        InfoBtn.interactable = !_GameManager.IsSpinStarted;
    }

    public void ResetSlotManagers()
    {
        for (int i = 0; i < SlotManagers.Length; i++)
        {
            for (int j = 0; j < SlotManagers[i].Slots.Count; j++)
            {
                SlotManagers[i].Slots[j].transform.SetParent(DummySlotParents[i].transform);
                SlotManagers[i].Slots[j].transform.localPosition = Vector2.zero;
            }
        }
    }

    public IEnumerator OnStoSpin()
    {
        for (int i = 0; i < SlotManagers.Length; i++)
        {
            for (int j = 0; j < SlotManagers[i].Slots.Count; j++)
            {
                yield return new WaitForSeconds(0.1f);
                AudioController.Instance.PlayLanding();
                if (i == 0)
                {
                    SlotManagers[i].Slots[j].transform.SetParent(Coloumn1Holder[j].transform);
                }
                else if (i == 1)
                {
                    SlotManagers[i].Slots[j].transform.SetParent(Coloumn2Holder[j].transform);
                }
                else if (i == 2)
                {
                    SlotManagers[i].Slots[j].transform.SetParent(Coloumn3Holder[j].transform);
                }
                else if (i == 3)
                {
                    SlotManagers[i].Slots[j].transform.SetParent(Coloumn4Holder[j].transform);
                }
                else if (i == 4)
                {
                    SlotManagers[i].Slots[j].transform.SetParent(Coloumn5Holder[j].transform);
                }
                SlotManagers[i].Slots[j].transform.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.InOutBounce);
            }
        }
        yield return new WaitForSeconds(2f);
        CheckForWinPattern();
        CheckUIButtons();
        CheckPlusMinus();
    }

    private void SetSlotData()
    {
        foreach (SlotManagers slots in SlotManagers)
        {
            foreach (SlotManager slot in slots.Slots)
            {
                int random = Random.Range(0, 10);
                slot.SetSlotData((SlotRank)random, _GameManager.GetSlotImage((SlotRank)random));
            }
        }
    }

    private SlotRank _MatchedRank;
    private List<SlotManager> MatchedSlots = new List<SlotManager>();

    private void CheckForWinPattern()
    {
        if (SlotManagers.Length <= 0)
        {
            Debug.LogError("Slot Manager data is null");
            return;
        }

        Debug.Log($"Matched Row count Before : {_GameManager.MatchedColoumnsCount}");

        if (MatchedSlots.Count > 0) MatchedSlots.Clear();

        for (int i = 0; i < SlotManagers[0].Slots.Count; i++)
        {
            if (MoveToNextColoumn(SlotManagers[0].Slots[i].GetRank()))
            {
                _MatchedRank = SlotManagers[0].Slots[i].GetRank();
                if (!MatchedSlots.Contains(SlotManagers[0].Slots[i]))
                    MatchedSlots.Add(SlotManagers[0].Slots[i]);

            }
        }
        Debug.Log($"Matched Row count After : {_GameManager.MatchedColoumnsCount}");
        if (_GameManager.MatchedColoumnsCount >= 3)
        {
            foreach (SlotManager slot in MatchedSlots)
            {
                slot.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.3f).SetEase(Ease.InOutExpo).SetLoops(5);
            }
            CancelInvoke(nameof(SetGlowAnimation));
            Invoke(nameof(SetGlowAnimation), 1);
        }
        else
        {
            AudioController.Instance.PlayError();
        }
        _GameManager.MatchedColoumnsCount = 0;
    }

    private void SetGlowAnimation()
    {
        _GameManager.WinAmountCalculation(_MatchedRank);
    }

    private bool MoveToNextColoumn(SlotRank rank)
    {
        if (MatchedSlots.Count > 0) MatchedSlots.Clear();

        if (SlotManagers[4].Slots.Exists(x => x.GetRank() == rank))
        {
            if (!MatchedSlots.Contains(SlotManagers[4].Slots.Find(x => x.GetRank() == rank)))
                MatchedSlots.Add(SlotManagers[4].Slots.Find(x => x.GetRank() == rank));

            int count = 0;
            for (int i = 1; i < SlotManagers.Length - 2; i++)
            {
                if (SlotManagers[i].Slots.Exists(x => x.GetRank() == rank) || SlotManagers[i].Slots.Exists(x => x.GetRank() == SlotRank.WILD))
                {
                    count += 1;
                    Debug.Log("Count ***** " + count);
                    if (!MatchedSlots.Contains(SlotManagers[i].Slots[i]))
                        MatchedSlots.Add(SlotManagers[i].Slots[i]);
                }
            }
            if (count == 3)
            {
                _GameManager.MatchedColoumnsCount = 5;
                return true;
            }
        }

        if (MatchedSlots.Count > 0) MatchedSlots.Clear();

        if (SlotManagers[3].Slots.Exists(x => x.GetRank() == rank))
        {
            if (!MatchedSlots.Contains(SlotManagers[3].Slots.Find(x => x.GetRank() == rank)))
                MatchedSlots.Add(SlotManagers[3].Slots.Find(x => x.GetRank() == rank));

            int count = 0;
            for (int i = 1; i < SlotManagers.Length - 3; i++)
            {
                if (SlotManagers[i].Slots.Exists(x => x.GetRank() == rank) || SlotManagers[i].Slots.Exists(x => x.GetRank() == SlotRank.WILD))
                {
                    count += 1;
                    Debug.Log("Count ***** " + count);
                    if (!MatchedSlots.Contains(SlotManagers[i].Slots[i]))
                        MatchedSlots.Add(SlotManagers[i].Slots[i]);
                }
            }
            if (count == 2)
            {
                _GameManager.MatchedColoumnsCount = 4;
                return true;
            }
        }

        if (MatchedSlots.Count > 0) MatchedSlots.Clear();

        if (SlotManagers[2].Slots.Exists(x => x.GetRank() == rank))
        {
            if (!MatchedSlots.Contains(SlotManagers[2].Slots.Find(x => x.GetRank() == rank)))
                MatchedSlots.Add(SlotManagers[2].Slots.Find(x => x.GetRank() == rank));

            if (SlotManagers[1].Slots.Exists(x => x.GetRank() == SlotRank.WILD) || SlotManagers[1].Slots.Exists(x => x.GetRank() == rank))
            {
                _GameManager.MatchedColoumnsCount = 3;
                if (!MatchedSlots.Contains(SlotManagers[1].Slots.Find(x => x.GetRank() == SlotRank.WILD || x.GetRank() == rank)))
                    MatchedSlots.Add(SlotManagers[1].Slots.Find(x => x.GetRank() == SlotRank.WILD || x.GetRank() == rank));
                return true;
            }
        }
        return false;
    }
}
