using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject panelToDisable;
    public GameObject panelToEnable;

    public void SwitchPanels()
    {
        if (panelToDisable != null) 
            panelToDisable.SetActive(false);
        
        if (panelToEnable != null) 
            panelToEnable.SetActive(true);
    }
}
