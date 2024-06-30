using UnityEngine;

public class UiHandler : MonoBehaviour
{
    public virtual void ShowMe()
    {
        if (!UIController.Instance.ActivePanels.Contains(this))
            UIController.Instance.ActivePanels.Add(this);
        this.gameObject.SetActive(true);
    }

    public virtual void HideMe()
    {
        if (UIController.Instance.ActivePanels.Contains(this))
            UIController.Instance.ActivePanels.Remove(this);
        this.gameObject.SetActive(false);
    }
}
