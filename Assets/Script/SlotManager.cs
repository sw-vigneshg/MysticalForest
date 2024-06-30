using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private SlotRank MySlotData;
    [SerializeField] private Image SlotImage;

    public void SetSlotData(SlotRank data,Sprite sprite)
    {
        MySlotData = data;
        SlotImage.sprite = sprite;
    }

    public SlotRank GetRank()
    {
        return MySlotData;
    }
}
