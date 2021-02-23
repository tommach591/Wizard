using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TransitionScript : MonoBehaviour
{
    public string newGameScene;
    public GameObject Wizard;

    bool incomplete = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if(incomplete)
        {
            if (Wizard.transform.position.x >= transform.position.x)
            {
                //print("Before you wreck yourself: " + pos);
                incomplete = false;
                SceneManager.LoadScene(newGameScene);
            }
        }
    }
}
