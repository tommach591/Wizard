using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPunch : MonoBehaviour
{
	[SerializeField] AudioSource punchFire;
	private BoxCollider2D col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void startHitBox() {
		col.offset = new Vector2(0f, -3f);
		col.size = new Vector2(4f, 1f);
	}

	void updateHitbox() {
		col.offset = new Vector2(col.offset.x, col.offset.y + 1f);
		col.size = new Vector2(col.size.x, col.size.y + 3f);
	}

	void endMe() {
		Destroy(gameObject);
	}

	public void PlayPunchFire()
	{
		punchFire.Play();
	}
}
