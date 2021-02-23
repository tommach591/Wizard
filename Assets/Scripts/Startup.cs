using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    public string newGameScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
     public void Intro()
     {
        SceneManager.LoadScene(newGameScene);
     }
}
