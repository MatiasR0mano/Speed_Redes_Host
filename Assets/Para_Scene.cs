using Fusion;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class Para_Scene : NetworkBehaviour
{
    public static Para_Scene instance { get; private set; }
    void Awake()
    {
        instance = this;
    }

    public void Cambio()
    {
        Runner.LoadScene("Tittle_Screen");
    }
}
