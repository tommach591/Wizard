using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class UI : MonoBehaviour
{
    //Fade time in seconds
    private float fadeOutTime = 1.5f;
    [SerializeField] GameObject player;
    [SerializeField] Text introText;
    [SerializeField] Text movementText;
    [SerializeField] RectTransform disappearPoint;
    private bool done = false;
    private bool done2 = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
		try
        {
			introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 0);
			movementText.color = new Color(movementText.color.r, movementText.color.g, movementText.color.b, 0);
		}
		catch(Exception x) {}
        StartCoroutine(FadeTextToFullAlpha(fadeOutTime, introText));
        yield return new WaitForSeconds(6f);
        StartCoroutine(FadeTextToFullAlpha(fadeOutTime, movementText));
    }

    // Update is called once per frame
    void Update()
    {
        if (!done2)
        {
            StartCoroutine(FadeIntro());
            done2 = true;
        }
        //print("player: " + player.transform.position.x);
        //print("dis: " + disappearPoint.position.x);
        if (!done)// && done == false)
        {
            StartCoroutine(FadeMove());
            done = true;
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeIntro()
    {
        yield return new WaitForSeconds(5f); //time first text on screen
        StartCoroutine(FadeTextToZeroAlpha(fadeOutTime, introText));
        done2 = true;
    }

    public IEnumerator FadeMove()
    {
        yield return new WaitForSeconds(15f); //time second text on screen
        StartCoroutine(FadeTextToZeroAlpha(fadeOutTime, movementText));
        done = true;
    }
}
