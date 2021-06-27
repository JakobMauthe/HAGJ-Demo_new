using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {
    
    public enum Scene {  //All Scenes must be registered here
        Quote,
        MainMenu,
        Loading,
        TestingLevel,
        Intro,
        Level1,
        Cutscene_lvl1_to_lvl2,
        Level2,        
    }

    private static Action onLoaderCallback;

    public static void Load(Scene scene) {
        //Set the loader callback action to load the target scene
        onLoaderCallback = () => {
            SceneManager.LoadScene(scene.ToString());
        };
        //Load the loading scene
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoadNextScene() {
        Load((Scene)SceneManager.GetActiveScene().buildIndex + 1);        
    }

    public static void LoadCurrentScene() {        
        Load((Scene)SceneManager.GetActiveScene().buildIndex);
    }

    public static void LoaderCallback() {
        //Triggerd after the first Update which lets the screen refresh
        //Execute the loader callback action which will load the target scene
        if (onLoaderCallback != null) {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}