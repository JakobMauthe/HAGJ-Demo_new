using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    public void PlayGame() {
        //levelLoader.LoadSpecificLevel(2);
        Debug.Log("No level 1 yet!");
    }
    public void TestLevel() {
        Loader.Load(Loader.Scene.TestingLevel);
    }

    public void Quit() {
        Application.Quit();
    }
}