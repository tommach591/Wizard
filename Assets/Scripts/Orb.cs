using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Orb : MonoBehaviour
{
    [SerializeField] float timeBtwBasics;
    private float timestampBasic;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(image.name == "LeafOrb" && Input.GetKeyDown(KeyCode.A))
        {
            Color c = image.color;
            c.a = 0.5f;
            image.color = c;
            timestampBasic = Time.time + timeBtwBasics;
        }
        else if (image.name == "WaterOrb" && Input.GetKeyDown(KeyCode.S))
        {
            Color c = image.color;
            c.a = 0.5f;
            image.color = c;
            timestampBasic = Time.time + timeBtwBasics;
        }
        else if (image.name == "FireOrb" && Input.GetKeyDown(KeyCode.D))
        {
            Color c = image.color;
            c.a = 0.5f;
            image.color = c;
            timestampBasic = Time.time + timeBtwBasics;
        }
        if(Time.time >= timestampBasic)
        {
            Color c = image.color;
            c.a = 1;
            image.color = c;
            timestampBasic = Time.time + timeBtwBasics;
        }
    }
}
