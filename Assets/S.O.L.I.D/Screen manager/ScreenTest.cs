using UnityEngine;
using UnityEngine.UI;

public class ScreenTest : MonoBehaviour, IScreen
{
    public ScreenTest nextScreen;
    public void BTN_Active()
    {
        if (nextScreen != null) ScreenManager.instance.ActiveScreen(nextScreen);
    }

    public void BTN_Close()
    {
        ScreenManager.instance.DesactiveScreen();
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        foreach (var item in GetComponentsInChildren<Button>())
            item.interactable = true;
    }

    public void Desactivate() => gameObject.SetActive(false);
}