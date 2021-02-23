using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatBoss : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;
	[SerializeField] Collider2D toDisable;
	private Rigidbody2D rb;

    private enum State { idle, flying, shooting, swoop, hurt, dead }
    private State state = State.idle;

    [SerializeField] Canvas healthBar;
    private float health;
    [SerializeField] private Slider healthSlider;

	[SerializeField] Transform rotationCenter;
    [SerializeField] GameObject wizard;
    private WizardController wizScript;
    [SerializeField] GameObject platform;
    [SerializeField] GameObject ground;
    [SerializeField] GameObject projectile;

	private bool seen = false;
	[SerializeField] float rotationRadius;
	[SerializeField] float angularSpeed;
	[SerializeField] float stretchx;
	[SerializeField] float stretchy;
	private float posx;
	private float posy;
	private float angle;
	private float speed = 10f;

    private bool facingLeft = true;
    private bool inView = false;

    private float timeBtwDives = 9f;
    private float divestamp;
    private float timeBtwShots = 4f;
    private float shootstamp;
	private bool dove = false;
	private Vector2 lockedOn;
	private bool angryMode = false;

	private Vector2 spawn;


	//Sound effects
	[SerializeField] AudioSource sonicWave;
	[SerializeField] AudioSource hurt;
	[SerializeField] AudioSource swoosh;
	[SerializeField] AudioSource flapping;

	// Start is called before the first frame update
	void Start()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
		wizScript = wizard.GetComponent<WizardController>();
		Physics2D.IgnoreCollision(platform.GetComponent<Collider2D>(), coll);
		Physics2D.IgnoreCollision(ground.GetComponent<Collider2D>(), coll);
		health = healthSlider.value;
		spawn = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		if (inView) {
			setDirection();
			if (state != State.hurt && state != State.dead) {
				if (Time.time >= divestamp && !dove && seen)			// Setup Dive Attack
				{
					StartCoroutine(diveAttack());
				}
				else if (Time.time >= shootstamp && !dove && seen) {	// Shoot Attack
					StartCoroutine(shootProjectile());
				}
				else if (!dove) {								// Chilling
					hoveringAround();
				}
				if (dove) {										// Dive Attack
					transform.position = Vector3.MoveTowards(transform.position, new Vector3(lockedOn.x, lockedOn.y, transform.position.z), speed * 3 * Time.deltaTime);
				}

				if (wizScript.state == WizardController.State.dead) {
					seen = false;
				}

			}
		}
		if (health <= 7 && !angryMode) {		// Angwy mode >:C
			// Play sound!
			angryMode = true;
		}
        if (angryMode) {
			timeBtwShots = 2f;
			timeBtwDives = 7f;
			speed = 15f;
			angularSpeed = 2.75f;
		}
        if (health <= 0)
        {
			timeBtwDives = 10f;
			timeBtwShots = 10f;
            state = State.dead;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
			Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);
			StartCoroutine(ifDead());
        }
		if (!seen && !inView) {
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(spawn.x, spawn.y, transform.position.z), speed * Time.deltaTime);
		}

		StartCoroutine(AnimationState());
        anim.SetInteger("state", (int)state);
		healthSlider.value = health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBasicAttack")
        {
            health -= 1f;
            state = State.hurt;
            Destroy(collision.gameObject);
        }
        if (health <= 0)
        {
			timeBtwDives = 10f;
			timeBtwShots = 10f;
            state = State.dead;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
			Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);

			StartCoroutine(ifDead());
        }
    }

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			dove = false;
			seen = false;
		}
	}

	void hoveringAround() {
		if (state != State.shooting && state != State.swoop) {
			state = State.flying;
		}
		posx = rotationCenter.position.x + Mathf.Cos(angle) * rotationRadius * stretchx;
		posy = rotationCenter.position.y + Mathf.Sin(angle) * rotationRadius * stretchy;
		if (!seen) {
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(posx, posy, transform.position.z), speed * Time.deltaTime);
			if (transform.position.y == posy && transform.position.x == posx) {
				seen = true;
			}
		}
		else {
			transform.position = new Vector2(posx, posy);
			angle = angle + Time.deltaTime * angularSpeed;
			if (angle >= 360f) {
				angle = 0f;
			}
		}
	}

	private IEnumerator shootProjectile() {
		state = State.shooting;
		Vector2 direction = wizard.transform.position - transform.position;
		for (int i = 0; i < 2; i++) {
			Projectile bullet = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Projectile>();
	        if (angryMode) {
				bullet.GetComponent<Rigidbody2D>().velocity = direction * 2f;
			}
			else {
				bullet.GetComponent<Rigidbody2D>().velocity = direction * 1f;
			}
			bullet.GetComponent<Rigidbody2D>().velocity = direction * 2f;
			float angle = Mathf.Atan2(bullet.GetComponent<Rigidbody2D>().velocity.y,bullet.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
			bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			shootstamp = Time.time + timeBtwShots;
	        if (angryMode) {
				yield return new WaitForSeconds(0.3f);
			}
			else {
				yield return new WaitForSeconds(0.6f);
			}
		}
	}

	private IEnumerator diveAttack() {
		state = State.swoop;
		lockedOn = new Vector2(wizard.transform.position.x, wizard.transform.position.y);
		yield return new WaitForSeconds(0.7f);
		dove = true;
		yield return new WaitForSeconds(4f);
		dove = false;
		seen = false;
		divestamp = Time.time + timeBtwDives;
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
        inView = false;
		seen = false;
    }

    // ...and enable it again when it becomes visible.
    void OnBecameVisible()
    {
        inView = true;
    }

    private IEnumerator ifDead()
    {
        state = State.dead;
        yield return new WaitForSeconds(1.85f);
        Destroy(gameObject);
    }

    private IEnumerator AnimationState()
    {
        if (state == State.hurt)
        {
            yield return new WaitForSeconds(0.5f);
            state = State.idle;
        }
        else if (state == State.swoop)
        {
            yield return new WaitForSeconds(0.9f);
            state = State.idle;
        }
        else if (state == State.shooting)
        {
            yield return new WaitForSeconds(0.9f);
            state = State.idle;
        }
        else if (state == State.dead)
        {
            StartCoroutine(ifDead());
        }
    }

	//Sounds
	public void PlaySonicWave()
    {
		sonicWave.Play();
    }

	public void PlayHurt()
	{
		hurt.Play();
	}

	public void PlaySwoosh()
	{
		swoosh.Play();
	}

	public void PlayFlap()
	{
		if (inView)
		{
			flapping.Play();
		}
	}
}
