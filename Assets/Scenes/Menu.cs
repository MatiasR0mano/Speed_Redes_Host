using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Nueva_partida(string nombreNivel) => SceneManager.LoadScene(nombreNivel);

    public void Salir() => Application.Quit();
}
