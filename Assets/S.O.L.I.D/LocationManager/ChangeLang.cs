using UnityEngine;

public class ChangeLang : MonoBehaviour
{
    public void ChangeLangSpa()
    {
        LocalizationManager.instance.Spanish();
    }
    public void ChangeLangEng()
    {
        LocalizationManager.instance.English();
    }
}
