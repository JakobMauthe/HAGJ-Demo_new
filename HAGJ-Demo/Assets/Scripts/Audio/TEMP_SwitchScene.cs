using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TEMP_SwitchScene : MonoBehaviour
{
    


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Main Menu"))
            {
                SceneManager.LoadScene("Audio Lab 23-6");
            }
            else if (SceneManager.GetActiveScene().name.StartsWith("Audio Lab"))
            {
                SceneManager.LoadScene("Main Menu (Audio Test)");
            }



        }
    }
}
