using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriAttack : MonoBehaviour
{
	public GameObject wizard;
	private float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(wizard.transform.position.x, wizard.transform.position.y), speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBasicAttack" || collision.tag == "PlayerWaterAttack" || collision.tag == "PlayerFireAttack")
        {
            Destroy(collision.gameObject);
			Destroy(gameObject);
        }
	}
}
