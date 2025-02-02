using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InGameHandler : UiHandler
{
    public static InGameHandler Instance;

    [Header("InGame UI Variables ************** ")]
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
    private int _CurrentIndex = 0;
    private bool _CanMove = true;

    [Header("Slot Variables ***************** ")]
    [SerializeField] private SlotPosition[] SlotPositions;
    [SerializeField] private RectTransform[] DummySlotParents;
    [SerializeField] private ReelHandler[] ReelHandlers;
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
        MinusBtn.interactable = (val > 1f);
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
        ResetSlotManagers();
        CheckUIButtons();
        SpinAnimation(true);
        AudioController.Instance.PlayReelSpin();
        CancelInvoke(nameof(OnClickStop));             // For Auto Spin Stop ....... if need we can use ...........
        Invoke(nameof(OnClickStop), 5);
    }

    private void SpinAnimation(bool canSpin)
    {
        foreach (ReelHandler reel in ReelHandlers)
        {
            reel.HandleSpin(canSpin);
        }
    }

    private void OnClickStop()
    {
        if (_GameManager.IsSpinStarted)
        {
            _GameManager.IsSpinStarted = false;
            StopBtn.interactable = false;
            SetSlotData();
            StopCoroutine(nameof(OnStoSpin));
            StartCoroutine(nameof(OnStoSpin));
        }
    }

    public void CheckUIButtons()
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

    private IEnumerator OnStoSpin()
    {
        int slotHolderIndex = 0;
        while (slotHolderIndex <= SlotManagers.Length - 1)
        {
            ReelHandlers[slotHolderIndex].HandleSpin(false);
            if (SlotManagers[slotHolderIndex].Slots.Count > 0)
            {
                for (int i = 0; i < SlotManagers[slotHolderIndex].Slots.Count; i++)
                {
                    SlotManagers[slotHolderIndex].Slots[i].transform.SetParent(SlotPositions[slotHolderIndex].Slots[i].transform);
                    SlotManagers[slotHolderIndex].Slots[i].transform.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.InOutBounce);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            yield return new WaitForSeconds(0.5f);
            slotHolderIndex += 1;
        }
        AudioController.Instance.PlayReelStops();
        CheckForWinPattern();
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
            CancelInvoke(nameof(ResetHUD));
            Invoke(nameof(ResetHUD), 2f);
        }
        _GameManager.MatchedColoumnsCount = 0;
    }

    private void ResetHUD()
    {
        CheckUIButtons();
        CheckPlusMinus();
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
