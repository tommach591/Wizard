using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantBoss : MonoBehaviour
{
    //Start() vars
    private Animator anim;
    private Collider2D coll;
    [SerializeField] GameObject toDisable;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask ground;

    //Finite State Machine
    private enum State { idle, walking, attacking, hurt, dead, jumping, falling }
    private State state = State.idle;

    //Attack vars
    [SerializeField] GameObject wizard;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shotPoint;
    private float timeBtwShots = 3f;
    private float timestamp;

    //Health vars
    [SerializeField] Canvas healthBar;
    private float health;
    [SerializeField] private Slider healthSlider;

    private bool facingLeft = true;
    private bool inView = false;

    private float leftCap;
    private float rightCap;
    private float walkLength = 5;

	private float moveSpeed = 0f;
	private float jumpHeight = 85f;
    private float timeBtwJumps = 7f;
    private float jumpstamp;
	bool resting;

	private Vector2 spawn;

    //Sound effects
    [SerializeField] AudioSource attack;
    [SerializeField] AudioSource hurt;
    [SerializeField] AudioSource dead;


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = healthSlider.value;
		spawn = transform.position;
		resting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inView)
        {
            setDirection();
            if (state != State.hurt && state != State.dead);
            {
                if (Time.time >= timestamp && state != State.jumping && !resting)
                {
                    StartCoroutine(Attack());
                }
				else if (wizard.transform.position.y > transform.position.y && coll.IsTouchingLayers(ground) && Time.time >= jumpstamp && moveSpeed != 0 && !resting) 
				{
					state = State.jumping;
					rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
					jumpstamp = Time.time + timeBtwJumps;
				}
				else if (!resting){
					walkingAround();
				}
				if (!coll.IsTouchingLayers(ground)) 
				{
					if (state == State.walking || state == State.idle) {
						state = State.falling;
					}
				}
            }
        }
        if (health <= 3)
        {
            moveSpeed = 10f;
        }
        if (health <= 7)
        {
            moveSpeed = 7f;
        }
        else if (health <= 10)
        {
            moveSpeed = 3f;
        }
        else if (health <= 17)
        {
            moveSpeed = 1f;
			timeBtwJumps = 4f;
        }
        if (health <= 0)
        {
            timeBtwShots = 10f;
            state = State.dead;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
            Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);

            StartCoroutine(ifDead());
        }

		if (!inView) {
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(spawn.x, spawn.y, transform.position.z), moveSpeed * Time.deltaTime);
		}

        StartCoroutine(AnimationState());
        anim.SetInteger("state", (int)state);
        healthSlider.value = health;
    }

	void walkingAround() {
		if (state != State.attacking && state != State.hurt && state != State.dead) {
			if (moveSpeed == 0) {
				state = State.idle;
			}
			else {
				state = State.walking;
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(wizard.transform.position.x, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);
			}
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

    private IEnumerator Attack()
    {
        for (int i = 0; i < 3; i++)
        {
            if (state != State.dead && state != State.hurt)
            {
				state = State.attacking;
				attack.Play();
                Projectile bullet = Instantiate(projectile, shotPoint.position, transform.rotation).GetComponent<Projectile>();
				bullet.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y);
                if (wizard.transform.position.x < transform.position.x)
                {
					bullet.dir = new Vector2(-Mathf.Abs(wizard.transform.localScale.x), 0);
                }
                else
                {
					bullet.dir = new Vector2(Mathf.Abs(wizard.transform.localScale.x), 0);
                }
                timestamp = Time.time + timeBtwShots;
                yield return new WaitForSeconds(0.3f);
            }
        }
		resting = true;
		yield return new WaitForSeconds(0.5f);
		resting = false;
    }

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "ground") {
			state = State.idle;
		}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBasicAttack")
        {
            hurt.Play();
            health -= 1;
            state = State.hurt;
            Destroy(collision.gameObject);
        }
        if (health <= 0)
        {
            timeBtwShots = 10f;
            state = State.dead;
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
            //GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(ifDead());
        }
    }

    private IEnumerator ifDead()
    {
        dead.Play();
        state = State.dead;
        yield return new WaitForSeconds(2.3f);
        Destroy(gameObject);
    }

    private IEnumerator AnimationState()
    {
        if (state == State.hurt)
        {
            yield return new WaitForSeconds(1f);
            state = State.idle;
        }
        else if (state == State.attacking)
        {
            yield return new WaitForSeconds(0.9f);
            state = State.idle;
        }
		else if (state == State.jumping) {
		    yield return new WaitForSeconds(0.1f);
            state = State.idle;
		}
        else if (state == State.dead)
        {
            StartCoroutine(ifDead());
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
		timestamp = Time.time + timeBtwShots;
		jumpstamp = Time.time + timeBtwJumps;
        inView = true;
    }
}
