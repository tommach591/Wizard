using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchBoss : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D coll;
	[SerializeField] Collider2D toDisable;
	private CircleCollider2D range;
    private Rigidbody2D rb;

    private enum State { idle, walking, basic, tri, summoning, 
						jumping, falling, plant, water, fire,
						teleport, reappear, hurt, dead }

    private State state = State.idle;

    [SerializeField] Canvas healthBar;
    private float health;
    [SerializeField] private Slider healthSlider;

    [SerializeField] GameObject wizard;
    private WizardController wizScript;
    [SerializeField] GameObject platform;
    [SerializeField] GameObject ground;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject plantBossAttack;
    [SerializeField] GameObject batBossAttack;
    [SerializeField] GameObject lavaBossAttack;
    [SerializeField] GameObject triAttack;
    [SerializeField] GameObject zombieB;
    [SerializeField] GameObject zombieA;
	

    private bool facingLeft = true;
    private bool inView = false;
	private Vector2 spawn;
	private float moveSpeed = 4f;
	private float jumpHeight = 85f;

	private Vector2 lockedOn;
    private float timeBtwSums = 20f;
    private float sumstamp;
    private float timeBtwTri = 20f;
    private float tristamp;
    private float timeBtwTP = 5f;
    private float tpstamp;
    private float timeBtwShots = 2f;
    private float shootstamp;

	int whichAttack;
	int	whichAttackRange;

	int numberOfTri;
	bool doodooingSomething;
	bool angryMode;
	bool angrierMode;
	public bool witchisdead;

	//Sound Effects
	//[SerializeField] AudioSource steps;
	[SerializeField] AudioSource laugh;
	[SerializeField] AudioSource batAttack;
	[SerializeField] AudioSource plantAttack;
	[SerializeField] AudioSource lavaAttack;

	// Start is called before the first frame update
	void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
		wizScript = wizard.GetComponent<WizardController>();
		health = healthSlider.value;
		spawn = transform.position;
		doodooingSomething = false;
		angryMode = false;
		angrierMode = false;
		numberOfTri = 1;
		whichAttackRange = 0;
		witchisdead = false;
    }

    // Update is called once per frame
    void Update()
    {
        setDirection();

		if (inView && state != State.hurt && state != State.dead) {
			if (health <= 10f && angryMode && !angrierMode) {
				angrierMode = true;
				health = 20f;
				timeBtwTP = 3f;
				numberOfTri = 2;
			}
			if (health <= 10f && !angryMode) {
				angryMode = true;
				health = 20f;
				timeBtwShots = 4f;
				whichAttackRange = 4;
				moveSpeed = 8f;
			}
			if (Time.time >= tristamp && !doodooingSomething) {
				StartCoroutine(snare());
			}
			else if (Time.time >= sumstamp && !doodooingSomething && angryMode) {
				StartCoroutine(summonZombies());
			}
			else if (Time.time >= tpstamp && !doodooingSomething) {
				StartCoroutine(teleport());
			}
			else if (Time.time >= shootstamp && !doodooingSomething) {
				StartCoroutine(shootProjectile());
			}
			else if (!doodooingSomething) {
				movement();
			}
		}
		healthSlider.value = health;
        if (health <= 0)
        {
			state = State.dead;
			witchisdead = true;
			doodooingSomething = true;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
			Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);
			DestroyAllZombies();
			StartCoroutine(ifDead());
        }
		if (wizard.GetComponent<WizardController>().state == WizardController.State.dead) {
			inView = false;
			state = State.teleport;
			transform.position = spawn;
		}
        anim.SetInteger("state", (int)state);
		StartCoroutine(AnimationState());
    }

	private void movement() {
		state = State.walking;
		transform.position = Vector2.MoveTowards(transform.position, new Vector2(wizard.transform.position.x, transform.position.y), moveSpeed * Time.deltaTime);
	}

	private IEnumerator shootProjectile() {
		shootstamp = Time.time + timeBtwShots;
		doodooingSomething = true;
		Vector2 direction;
		whichAttack = Random.Range(0, whichAttackRange);

		switch (whichAttack) {
			case 3:
				state = State.fire;
				yield return new WaitForSeconds(1f);
				for (int i = 0; i < 5; i++) {
					lavaAttack.Play();
					lockedOn = new Vector2(wizard.transform.position.x, -6.5f);
					yield return new WaitForSeconds(0.3f);
					Projectile bullet = Instantiate(lavaBossAttack, lockedOn, transform.rotation).GetComponent<Projectile>();
				}
				break;
			case 2:
				for (int i = 0; i < 2; i++) {
					batAttack.Play();
					state = State.water;
					direction = wizard.transform.position - transform.position;
					Projectile bullet = Instantiate(batBossAttack, transform.position, transform.rotation).GetComponent<Projectile>();
					bullet.GetComponent<Rigidbody2D>().velocity = direction * 3f;
					float angle = Mathf.Atan2(bullet.GetComponent<Rigidbody2D>().velocity.y,bullet.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
					bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

					yield return new WaitForSeconds(0.6f);
				}
				break;
			case 1:
				for (int i = 0; i < 3; i++) {
					plantAttack.Play();
					state = State.plant;
					direction = wizard.transform.position - transform.position;
					Projectile bullet = Instantiate(plantBossAttack, transform.position, transform.rotation).GetComponent<Projectile>();
					bullet.GetComponent<Rigidbody2D>().velocity = direction * 2f;
					float angle = Mathf.Atan2(bullet.GetComponent<Rigidbody2D>().velocity.y,bullet.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
					bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
					yield return new WaitForSeconds(0.3f);
				}
				break;
			default:
				for (int i = 0; i < 1; i++) {
					state = State.basic;
					direction = wizard.transform.position - transform.position;
					Projectile bullet = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Projectile>();
					Physics2D.IgnoreCollision(bullet.gameObject.GetComponent<BoxCollider2D>(), coll);
					bullet.GetComponent<Rigidbody2D>().velocity = direction * 2f;
					float angle = Mathf.Atan2(bullet.GetComponent<Rigidbody2D>().velocity.y,bullet.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
					bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

					yield return new WaitForSeconds(0.2f);
				}
				break;
		}
		doodooingSomething = false;
	}

	private IEnumerator teleport() {
		tpstamp = Time.time + timeBtwTP;
		doodooingSomething = true;
		float x;
		int LR = Random.Range(0, 2);
		if (LR == 0) {
			x = wizard.transform.position.x + Random.Range(-10f, -4f);
		}
		else {
			x = wizard.transform.position.x + Random.Range(4f, 10f);
		}
		if (x < 207f) {
			x = 208f;
		}
		if (x > 280f) {
			x = 279f;
		}
		float y = wizard.transform.position.y;
		if (angrierMode) {
			rb.gravityScale = 0;
			y = wizard.transform.position.y + Random.Range(4f, 8f);
		}
		state = State.teleport;
		yield return new WaitForSeconds(0.45f);
		transform.position = new Vector2(x, y);
		state = State.reappear;
		yield return new WaitForSeconds(1f);
		doodooingSomething = false;
	}

	private IEnumerator snare() {
		tristamp = Time.time + timeBtwTri;
		doodooingSomething = true;
		laugh.Play();
		state = State.tri;
		for (int i = 0; i < numberOfTri; i++) {
			Projectile bullet = Instantiate(triAttack, transform.position, transform.rotation).GetComponent<Projectile>();
			Physics2D.IgnoreCollision(bullet.gameObject.GetComponent<BoxCollider2D>(), coll);
			bullet.lifeTime = 20f;
			bullet.GetComponent<TriAttack>().wizard = wizard;
			yield return new WaitForSeconds(0.5f);
		}
		doodooingSomething = false;
	}

	private IEnumerator summonZombies() {
		sumstamp = Time.time + timeBtwSums;
		doodooingSomething = true;
		rb.gravityScale = 15f;
		if (transform.position.y > -5) {
			state = State.falling;
		}
		yield return new WaitForSeconds(1f);
			for (int i = 0; i < 3; i++) {
				state = State.summoning;
				Zombie zom;
				if (angrierMode) {
					zom = Instantiate(zombieA, new Vector2(transform.position.x, transform.position.y), transform.rotation).GetComponent<Zombie>();
					zom.toDisable = toDisable;
				}
				else {
					zom = Instantiate(zombieB, new Vector2(transform.position.x, transform.position.y), transform.rotation).GetComponent<Zombie>();
					zom.toDisable = toDisable;
				}
				Physics2D.IgnoreCollision(zom.gameObject.GetComponent<BoxCollider2D>(), coll);
				zom.wizard = wizard;
				zom.startingState = Zombie.State.walking;
				zom.state = zom.startingState;
				yield return new WaitForSeconds(1f);
			}
		yield return new WaitForSeconds(2f);
		doodooingSomething = false;
	}

	void DestroyAllZombies()
	{
		GameObject[] gameObjects;
		gameObjects = GameObject.FindGameObjectsWithTag("ZombieB");
     
		for(var i = 0 ; i < gameObjects.Length ; i ++)
		{
			Destroy(gameObjects[i]);
		}
		gameObjects = GameObject.FindGameObjectsWithTag("ZombieA");
     
		for(var i = 0 ; i < gameObjects.Length ; i ++)
		{
			Destroy(gameObjects[i]);
		}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBasicAttack")
        {
            health -= 1f;
			if (state == State.idle || state == State.walking) {
				state = State.hurt;
			}
            Destroy(collision.gameObject);
        }
		else if (collision.tag == "PlayerWaterAttack" || collision.tag == "PlayerFireAttack")
		{
			health -= 2f;
			if (state == State.idle || state == State.walking)
			{
				state = State.hurt;
			}
			Destroy(collision.gameObject);
		}
		if (health <= 0)
        {
			state = State.dead;
			witchisdead = true;
			doodooingSomething = true;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
			Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);
			DestroyAllZombies();
			StartCoroutine(ifDead());
        }
    }

    private void setDirection()
    {
        if (wizard.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1);
			healthBar.transform.localScale = new Vector3(Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);
		}
        else
        {
            transform.localScale = new Vector3(-1, 1);
			healthBar.transform.localScale = new Vector3(-Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);
		}

    }

    // Disable the behaviour when it becomes invisible...
    void OnBecameInvisible()
    {
        //inView = false;
    }

    // ...and enable it again when it becomes visible.
    void OnBecameVisible()
    {
        inView = true;
    }

    private IEnumerator ifDead()
    {
		rb.gravityScale = 15f;
        yield return new WaitForSeconds(0.78f);
        Destroy(gameObject);
    }

    private IEnumerator AnimationState()
    {
        if (state == State.hurt)
        {
            yield return new WaitForSeconds(0.3f);
            state = State.idle;
        }
        else if (state == State.basic || state == State.tri || state == State.plant || state == State.water || state == State.fire)
        {
            yield return new WaitForSeconds(2.5f);
            state = State.idle;
        }
        else if (state == State.summoning)
        {
            yield return new WaitForSeconds(2.5f);
            state = State.idle;
        }
        else if (state == State.teleport)
        {
            yield return new WaitForSeconds(0.45f);
            state = State.reappear;
        }
        else if (state == State.reappear)
        {
            yield return new WaitForSeconds(0.45f);
            state = State.idle;
        }
        else if (state == State.falling)
        {
            yield return new WaitForSeconds(0.4f);
            state = State.idle;
        }
        else if (state == State.dead)
        {
            StartCoroutine(ifDead());
        }
    }
}
