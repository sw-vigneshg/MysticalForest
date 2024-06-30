using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("UI Handlers ***************** ")]
    public UiHandler LoadingHandler;
    public UiHandler InGameHandler;
    public UiHandler ResultHandler;
    public List<UiHandler> ActivePanels = new List<UiHandler>();

    private void Awake()
    {
        Instance = this;
    }
}
