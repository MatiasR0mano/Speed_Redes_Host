using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : NetworkBehaviour
{
    public void Salir_partida()
    {
        SceneManager.LoadSceneAsync("Tittle_Screen");
    }

    public void Salir() => Application.Quit();


}
