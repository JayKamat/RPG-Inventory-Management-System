using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitApplication : MonoBehaviour {

    public void Exit()
    {
        GameManager.GlobalClickSFX.Play();
        Application.Quit();
    }

}
