using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LavaBoss : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D coll;
	[SerializeField] Collider2D toDisable;
	private CircleCollider2D range;
    private Rigidbody2D rb;

    private enum State { idle, sliding, jumping, falling, shooting, punching, hurt, dead }
    private State state = State.idle;

    [SerializeField] Canvas healthBar;
    private float health;
    [SerializeField] private Slider healthSlider;

    [SerializeField] GameObject wizard;
    private WizardController wizScript;
    [SerializeField] GameObject wizardProj;
    [SerializeField] GameObject platform;
    [SerializeField] GameObject ground;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject punch;

    private bool facingLeft = true;
    private bool inView = false;
	private Vector2 spawn;
	private float moveSpeed = 5f;
	private float jumpHeight = 85f;

	private Vector2 lockedOn;
    private float timeBtwPunches = 20f;
    private float punchstamp;
    private float timeBtwShots = 3f;
    private float shootstamp;

	private bool punching;
	public bool biggerer;
	private int punches;

	//Sound effects
	[SerializeField] AudioSource punchSound;
	[SerializeField] AudioSource attack;
	[SerializeField] AudioSource move;
	[SerializeField] AudioSource hurt;
	[SerializeField] AudioSource dead;


	void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
		wizScript = wizard.GetComponent<WizardController>();
		health = healthSlider.value;
		spawn = transform.position;
		punching = false;
		biggerer = false;
		punches = 3;
    }

    // Update is called once per frame
    void Update()
    {
		setDirection();
		if (health <= 10 && !biggerer) {
			biggerer = true;
			StartCoroutine(getBig());
			moveSpeed = 10f;
		}
		if (inView && state != State.hurt && state != State.dead) {
				if (Time.time >= punchstamp) {
					StartCoroutine(land());
				}
				else if (Time.time >= shootstamp && !punching) {
					StartCoroutine(shootProjectile());
				}
				else if (!punching && state != State.shooting) {
					movement();
				}
		}

        anim.SetInteger("state", (int)state);
		StartCoroutine(AnimationState());
		healthSlider.value = health;
        if (health <= 0)
        {
			transform.localScale = new Vector3(1f, 1f, 1f);
			state = State.dead;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
			Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);
			StartCoroutine(ifDead());
        }
    }

	private IEnumerator getBig() {
		StartCoroutine(sink());
		for (int i = 0; i < 10; i++) {
			transform.localScale += new Vector3(0.01f, 0.01f, 0f);
			coll.size += new Vector2(0.01f, 0.01f);
		}
		moveSpeed = 7f;
		health = 20f;
		healthSlider.value = health;
		punches = 5;
		yield return new WaitForSeconds(1f);
		state = State.hurt;
		punching = false;
	}

	private IEnumerator land() {
		punchstamp = Time.time + timeBtwPunches;
		punching =  true;
		yield return new WaitForSeconds(1f);
		state = State.jumping;
		rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
		Physics2D.IgnoreCollision(platform.GetComponent<Collider2D>(), coll, false);
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(lavaPunch());
	}

	private IEnumerator sink() {
		state = State.falling;
		Physics2D.IgnoreCollision(platform.GetComponent<Collider2D>(), coll, true);
		shootstamp = Time.time + timeBtwShots;
		yield return new WaitForSeconds(0.3f);
		punching = false;
	}

	private IEnumerator lavaPunch() {
		yield return new WaitForSeconds(0.5f);
		state = State.punching;
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < punches; i++) {
			lockedOn = new Vector2(wizard.transform.position.x, transform.position.y - 1f);
			yield return new WaitForSeconds(0.3f);
			Projectile bullet = Instantiate(punch, lockedOn, transform.rotation).GetComponent<Projectile>();
		}
		yield return new WaitForSeconds(3.5f);
		StartCoroutine(sink());
	}

	private void movement() {
		state = State.sliding;
		transform.position = Vector2.MoveTowards(transform.position, new Vector2(wizard.transform.position.x, transform.position.y), moveSpeed * Time.deltaTime);
	}

	private IEnumerator shootProjectile() {
		shootstamp = Time.time + timeBtwShots;
		state = State.shooting;
		for (int i = 0; i < 3; i++) {
			Vector2 direction = wizard.transform.position - transform.position;
			Projectile bullet = Instantiate(projectile, new Vector2(transform.position.x, transform.position.y - 0.25f), transform.rotation).GetComponent<Projectile>();
			Physics2D.IgnoreCollision(bullet.gameObject.GetComponent<BoxCollider2D>(), coll);
			if (biggerer) {
				bullet.transform.localScale = new Vector3(1.5f, 1.5f, 0f);
				bullet.GetComponent<BoxCollider2D>().size *= new Vector2(1.65f, 1.5f);
				bullet.GetComponent<Rigidbody2D>().velocity = direction * 1.5f;
			}
			else {
				bullet.GetComponent<Rigidbody2D>().velocity = direction * 2f;
			}
			float angle = Mathf.Atan2(bullet.GetComponent<Rigidbody2D>().velocity.y,bullet.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
			bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			yield return new WaitForSeconds(0.25f);
		}
	}

    private void OnTriggerExit2D(Collider2D collision) {

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBasicAttack")
        {
            health -= 1f;
			if (state != State.punching && state != State.jumping && state != State.falling) {
				state = State.hurt;
			}
            Destroy(collision.gameObject);
        }
		else if (collision.tag == "PlayerWaterAttack")
		{
			health -= 2f;
			if (state != State.punching && state != State.jumping && state != State.falling)
			{
				state = State.hurt;
			}
			Destroy(collision.gameObject);
		}
		if (health <= 0)
        {
			state = State.dead;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
			Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);

			StartCoroutine(ifDead());
        }
    }

    private void setDirection()
    {
		if (biggerer) {
			if (wizard.transform.position.x < transform.position.x)
			{
				transform.localScale = new Vector3(1.1f, 1.1f);
				healthBar.transform.localScale = new Vector3(Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);
			}
			else {
				transform.localScale = new Vector3(-1.1f, 1.1f);
				healthBar.transform.localScale = new Vector3(-Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);
			}
		}
        else if (wizard.transform.position.x < transform.position.x && !biggerer)
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
    }

    // ...and enable it again when it becomes visible.
    void OnBecameVisible()
    {
        inView = true;
    }

    private IEnumerator ifDead()
    {
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
        else if (state == State.shooting)
        {
            yield return new WaitForSeconds(0.7f);
            state = State.idle;
        }
        else if (state == State.punching)
        {
            yield return new WaitForSeconds(2f);
            state = State.idle;
        }
        else if (state == State.falling)
        {
            yield return new WaitForSeconds(0.1f);
            state = State.idle;
        }
        else if (state == State.jumping)
        {
            yield return new WaitForSeconds(0.1f);
            state = State.idle;
        }
        else if (state == State.dead)
        {
            StartCoroutine(ifDead());
        }
    }

	//Sounds
	public void PlayPunch()//
	{
		move.Stop();
		punchSound.Play();
	}
	public void PlayAttack() //
	{
		move.Stop();
		attack.Play();
	}
	public void PlayHurt()//
	{
		move.Stop();
		hurt.Play();
	}
	public void PlayDead()//
	{
		move.Stop();
		dead.Play();
	}
	public void PlayMove()//
	{
		if (inView)
		{
			move.Play();
		}
	}
}
