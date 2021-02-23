using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public Transform player;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    //private void Update()
    //{
    //    transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    //}

    [SerializeField] public GameObject player;
    [SerializeField] public GameObject boss;
    [SerializeField] public Transform highWall;
    [SerializeField] public Transform lowWall;
    [SerializeField] public Transform rightWall01;
    [SerializeField] public Transform rightWall02;
    [SerializeField] public Transform leftWall;

	private DeathZones[] DeathScript;
	[SerializeField] GameObject[] DeathObjects;
	private int doISeeDeathZone;
	private bool deathInSight;
	private WizardController PlayerScript;
	private Vector2 stopHere;

    private float yMax;
    private float yMin;
    private float xMax;
    private float xMin;

    // Use this for initialization
    void Start()
    {
        yMax = highWall.transform.position.y;
        //yMin = lowWall.transform.position.y;
        xMax = rightWall01.transform.position.x;
        xMin = leftWall.transform.position.x;

		DeathScript = new DeathZones[DeathObjects.Length];
		for (int i = 0; i < DeathObjects.Length; i++) {
			DeathScript[i] = DeathObjects[i].GetComponent<DeathZones>();
		}
		PlayerScript = player.GetComponent<WizardController>();
    }

    // Update is called once per frame
    void Update()
    {
		doISeeDeathZone = 0;
		for (int i = 0; i < DeathObjects.Length; i++) {
			if(DeathScript[i].inView) {
				doISeeDeathZone += 1;
			}
			else {
				doISeeDeathZone += 0;
			}
		}
		if (doISeeDeathZone != 0) {
			deathInSight = true;
		}
		else {
			deathInSight = false;
		}

		if (deathInSight || player.transform.position.y > yMax) {
			stopHere = new Vector3(transform.position.x, transform.position.y);
			if (player.transform.position.y > stopHere.y) {
				transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -110.0f);
			}
			else {
				if (player.transform.position.x > xMax || player.transform.position.x < xMin) {
					transform.position = new Vector3(stopHere.x, stopHere.y, -110.0f);
				}
				transform.position = new Vector3(player.transform.position.x, stopHere.y, -110.0f);
			}
		}
		else if (player.transform.position.x > xMax || player.transform.position.x < xMin) {
			transform.position = new Vector3(transform.position.x, player.transform.position.y, -110.0f);
		}
		else {
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -110.0f);
		}

		if(boss == null)
		{
			xMax = rightWall02.transform.position.x;
		}

		if (PlayerScript.state == WizardController.State.dead) {
			StartCoroutine(repositionAfterDeath());
		}
		/*
		//if within the bounds, camera locks onto player
		if (player.transform.position.y < yMax && player.transform.position.y > yMin)
		{
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -110.0f);
		}
		//if player is above/below the y axis binders, camera locks to player on xAxis and stays stationary 
		//on yAxis
		if (player.transform.position.y > yMax)
		{
			transform.position = new Vector3(player.transform.position.x, yMax, -110.0f);
		}
		else if (player.transform.position.y < yMin)
		{
			transform.position = new Vector3(player.transform.position.x, yMin, -110.0f);
		}

		//if player is right/left of the xAxis binders, camera locks to player on yAxis and stays stationary 
		//on xAxis
		if (player.transform.position.x > xMax)
		{
			transform.position = new Vector3(xMax, player.transform.position.y, -110.0f);
		}
		else if (player.transform.position.x < xMin)
		{
			transform.position = new Vector3(xMin, player.transform.position.y, -110.0f);
		}

		//if player is above the yAxis binder, and to the right of the xAxis, the camera stays stationary
		if (player.transform.position.y > yMax && player.transform.position.x > xMax)
		{
			transform.position = new Vector3(xMax, yMax, -110.0f);
		}
		//if player is above the yAxis binder, and to the left of the xAxis, the camera stays stationary
		if (player.transform.position.y > yMax && player.transform.position.x < xMin)
		{
			transform.position = new Vector3(xMin, yMax, -110.0f);
		}
		//if player is below the yAxis binder, and to the right of the xAxis, the camera stays stationary
		if (player.transform.position.y < yMin && player.transform.position.x > xMax)
		{
			transform.position = new Vector3(xMax, yMin, -110.0f);
		}
		//if player is below the yAxis binder, and to the left of the xAxis, the camera stays stationary
		if (player.transform.position.y < yMin && player.transform.position.x < xMin)
		{
			transform.position = new Vector3(xMin, yMin, -110.0f);
		}

		if(boss == null)
		{
			xMax = rightWall02.transform.position.x;
		}

		*/
    }

	private IEnumerator repositionAfterDeath() {
		yield return new WaitForSeconds(2.4f);
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -110.0f);
	}

}

