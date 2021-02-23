using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TransitionEnding : MonoBehaviour
{
    public string newGameScene;
    public GameObject Wizard;
    public GameObject Enemy;

    bool incomplete = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (incomplete)
        {
            if (Enemy == null)
            {
                //print("Before you wreck yourself: " + pos);
                StartCoroutine(Won());
            }
        }
    }

    private IEnumerator Won()
    {
        incomplete = false;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(newGameScene);
    }
}
