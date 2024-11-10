using UnityEngine;
using UnityEngine.UI;

public class ScreenTest : MonoBehaviour, IScreen
{
    public ScreenTest nextScreen;
    public void BTN_Active()
    {
        Cursor.visible = true;
        if (nextScreen != null) ScreenManager.instance.ActiveScreen(nextScreen);
    }
    public void BTN_Active_Conpausa()
    {
        Cursor.visible = true;
        if (nextScreen != null) ScreenManager.instance.ActiveScreen(nextScreen);
        //Gamemanager.instance.Pausa = true;
    }

    public void BTN_Close()
    {
        Cursor.visible = false;
        ScreenManager.instance.DesactiveScreen();
    }

    public void BTN_Close_Conpausa()
    {
        Cursor.visible = false;
        //Gamemanager.instance.Fuerapausa_Game();
    }

    public void BTN_Close_hab()
    {
        ScreenManager.instance.DesactiveScreen();
        //Gamemanager.instance.Pausa = false;
        Cursor.visible = false;
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        foreach (var item in GetComponentsInChildren<Button>())
            item.interactable = true;
    }

    public void Desactivate() => gameObject.SetActive(false);


}
