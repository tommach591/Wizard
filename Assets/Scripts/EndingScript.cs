using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.loopPointReached += Loadnextscene;
    }

    void Loadnextscene(VideoPlayer Intro)
    {
        SceneManager.LoadScene("Startup");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            videoPlayer.Stop();
            SceneManager.LoadScene("Startup");
        }
    }

    private void Awake()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Ending.mp4");
    }
}
