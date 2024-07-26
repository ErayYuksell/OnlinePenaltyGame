using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void TapToSingleplayerButton()
    {
        SceneManager.LoadScene("Game");
    }
    public void TapToMultilayerButton()
    {
        SceneManager.LoadScene("LoadingScreen");
    }
}
