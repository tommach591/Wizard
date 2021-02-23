using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTop : MonoBehaviour
{

	[SerializeField] GameObject lavaBoss;
	[SerializeField] GameObject projectile;
	[SerializeField] GameObject ground;
	private LavaBoss script;
	private BoxCollider2D coll;
	private Collider2D groundcol;
	private float hitX;

    // Start is called before the first frame update
    void Start()
    {
		script = lavaBoss.GetComponent<LavaBoss>();
		coll = GetComponent<BoxCollider2D>();
		groundcol = ground.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

	private IEnumerator shootProjectile() {
		float x = hitX;
		
		yield return new WaitForSeconds(1f);
		Projectile bullet = Instantiate(projectile, new Vector2(x, transform.position.y + 5f), transform.rotation).GetComponent<Projectile>();
		Physics2D.IgnoreCollision(bullet.gameObject.GetComponent<BoxCollider2D>(), groundcol);
		Physics2D.IgnoreCollision(bullet.gameObject.GetComponent<BoxCollider2D>(), coll);
		bullet.lifeTime = 10f;
		bullet.transform.localScale = new Vector3(1.5f, 1.5f, 0f);
		bullet.GetComponent<BoxCollider2D>().size *= new Vector2(1.65f, 1.5f);
		bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -9 * 2f);

		float angle = Mathf.Atan2(bullet.GetComponent<Rigidbody2D>().velocity.y,bullet.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
		bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "LavaBasicAttack" && script.biggerer)
        {
			hitX = collision.gameObject.transform.position.x;
			StartCoroutine(shootProjectile());
        }
    }
}
