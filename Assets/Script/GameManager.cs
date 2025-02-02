using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("InGame Variables ************ ")]
    [SerializeField] private float PlayerBalance = 0;
    [SerializeField] private float BetAmount = 0;
    [SerializeField] private float WinAmount = 0;
    public Action<float> PlayerBalanceAction;
    public Action<float> BetAmountAction;
    public Action<float> WinAmountAction;
    public bool IsSpinStarted = false;

    [Header("Slot Variables ************* ")]
    public Sprite[] SoltSprites;
    public List<PayTable> PayTableValues;
    public int MatchedColoumnsCount = 0;
    public float MultiplierValue = 0;

    [Header("Script Reference ************ ")]
    public UIController _UIController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (_UIController != null)
        {
            _UIController.LoadingHandler.ShowMe();
            _UIController.InGameHandler.HideMe();
            _UIController.ResultHandler.HideMe();
        }
        SetPlayerBalance();
    }

    private void SetPlayerBalance()
    {
        PlayerBalance = 2000;
        PlayerBalanceAction?.Invoke(PlayerBalance);
        BetAmount = 1f;
        BetAmountAction?.Invoke(BetAmount);
        WinAmount = 0;
        WinAmountAction?.Invoke(WinAmount);
    }

    public void UpdateBalanceOnBet()
    {
        PlayerBalance -= BetAmount;
        PlayerBalanceAction?.Invoke(PlayerBalance);
    }

    public void UpdateBalanceOnWinning(float amount)
    {
        PlayerBalance += amount;
        PlayerBalanceAction?.Invoke(PlayerBalance);
    }

    public void OnIncreseBetAmount()
    {
        if ((BetAmount + 1f) <= 25)
            BetAmount += 1f;
        BetAmountAction?.Invoke(BetAmount);
    }

    public void OnDecreseBetAmount()
    {
        if ((BetAmount - 1f) > 1f)
            BetAmount -= 1f;
        BetAmountAction?.Invoke(BetAmount);
    }

    public void SetWinAmount(float amount)
    {
        WinAmount += amount;
        WinAmountAction?.Invoke(WinAmount);
    }

    public void WinAmountCalculation(SlotRank matchedRank)
    {
        if (PayTableValues.Exists(x => x.SlotRank == matchedRank))
        {
            PayTable GetPayValue = PayTableValues.Find(x => x.SlotRank == matchedRank);

            if (GetPayValue != null)
            {
                switch (MatchedColoumnsCount)
                {
                    case 3:
                        MultiplierValue = GetPayValue.ThreeRows;
                        break;
                    case 4:
                        MultiplierValue = GetPayValue.FourRows;
                        break;
                    case 5:
                        MultiplierValue = GetPayValue.FiveRows;
                        break;
                }
                float winAmount = (BetAmount + (BetAmount * MultiplierValue * MatchedColoumnsCount));
                Debug.Log($"Multiplier value : {MultiplierValue} , WinAmount : {winAmount} , Rank : {matchedRank.ToString()}");
                UpdateBalanceOnWinning(winAmount);
                _UIController.ResultHandler.ShowMe();
                SetWinAmount(winAmount);
                CancelInvoke(nameof(ResetHandler));
                Invoke(nameof(ResetHandler), 3);
            }
        }
    }

    public void ResetHandler()
    {
        _UIController.ResultHandler.HideMe();
        InGameHandler.Instance.CheckUIButtons();
        InGameHandler.Instance.CheckPlusMinus();
        _UIController.InGameHandler.ShowMe();
    }

    public Sprite GetSlotImage(SlotRank rank)
    {
        if ((int)rank == 9)
        {
            if (rank == SlotRank.WILD)
            {
                return SoltSprites[9];
            }
            else if (rank == SlotRank.SCATTER)
            {
                return SoltSprites[10];
            }
            else if (rank == SlotRank.BONUS)
            {
                return SoltSprites[11];
            }
        }
        else
        {
            return SoltSprites[(int)rank];
        }
        return null;
    }

    public float GetBetAmount()
    {
        return BetAmount;
    }
}

[System.Serializable]
public class SlotPosition
{
    public RectTransform[] Slots;
}

[Serializable]
public class SlotManagers
{
    public List<SlotManager> Slots = new List<SlotManager>();
}

[Serializable]
public class SlotDetails
{
    public List<SlotData> SlotData = new List<SlotData>();
}

[Serializable]
public class PayTable
{
    public SlotRank SlotRank;
    public float ThreeRows;
    public float FourRows;
    public float FiveRows;

}

[Serializable]
public class SlotData
{
    public SlotRank Rank;
}

[Serializable]
public enum SlotRank
{
    TEN = 0,
    JACK = 1,
    QUEEN = 2,
    KING = 3,
    ACE = 4,
    HIGH1 = 5,
    HIGH2 = 6,
    HIGH3 = 7,
    HIGH4 = 8,
    WILD = 9,
    SCATTER = 9,
    BONUS = 9
}