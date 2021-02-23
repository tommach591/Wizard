using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    //Start() vars
    private Animator anim;
    private Collider2D coll;
    public Collider2D toDisable;
    private Rigidbody2D rb;

    //Finite State Machine
    public enum State { idle, walking, attacking, hurt, dead }
    public State state;
	public State startingState;

    //Attack vars
    public GameObject wizard;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shotPoint;
    private float timeBtwShots = 2f;
    private float timestamp;

    //Health vars
    [SerializeField] Canvas healthBar;
    private float health;
    [SerializeField] private Slider healthSlider;
	private float moveSpeed = 2f;

    private bool facingLeft = true;
    private bool inView = false;

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
		startingState = state;

        health = healthSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        //print("state: " + state);
        if (inView)
        {
            setDirection();
            if (state != State.hurt && state != State.dead)
            {
                if (Time.time >= timestamp)
                {
					state = State.attacking;
                }
				if (state != State.attacking && startingState == State.walking) {
					transform.position = Vector3.MoveTowards(transform.position, new Vector3(wizard.transform.position.x, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);
				}
                else
                {
                    Wait();
                }
                //else
                //{
                //    timeBtwShots -= Time.deltaTime;
                //}
            }
            
        }
        if (health <= 0)
        {
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<BoxCollider2D>(), coll);
			Physics2D.IgnoreCollision(wizard.gameObject.GetComponent<CircleCollider2D>(), coll);
            Physics2D.IgnoreCollision(toDisable.GetComponent<CircleCollider2D>(), coll);

            state = State.dead;
            //timeBtwShots = 10f;
            //GetComponent<BoxCollider2D>().enabled = false;
            //StartCoroutine(ifDead());
        }
        StartCoroutine(AnimationState());
        anim.SetInteger("state", (int)state);
        healthSlider.value = health;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.8f);
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

    public void Attack()
    {
        Projectile bullet = Instantiate(projectile, shotPoint.position, transform.rotation)
                        .GetComponent<Projectile>();
        if(wizard.transform.position.x < transform.position.x)
        {
            bullet.transform.localScale = new Vector3(transform.localScale.x, 1);
            bullet.dir = new Vector2(-Mathf.Abs(wizard.transform.localScale.x), 0);
        }
        else
        {
            bullet.transform.localScale = new Vector3(transform.localScale.x, 1);
            bullet.dir = new Vector2(Mathf.Abs(wizard.transform.localScale.x), 0);
        }
        //print("wizard.transform.localScale.x: " + wizard.transform.localScale.x);
        //Instantiate(projectile, shotPoint.position, transform.rotation);
        //state = State.attacking;
        timestamp = Time.time + timeBtwShots;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBasicAttack")
        {
            health -= 1;
            state = State.hurt;
            Destroy(collision.gameObject);
        }
		else if (collision.tag == "PlayerWaterAttack" || collision.tag == "PlayerFireAttack")
		{
			health -= 2f;
			Destroy(collision.gameObject);
		}
    }

	public void destroyZombie() {
		Destroy(gameObject);
	}

    private IEnumerator ifDead()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }

    private IEnumerator AnimationState()
    {
        if (state == State.hurt)
        {
            yield return new WaitForSeconds(0.5f);
			state = startingState;
        }
        else if (state == State.attacking)
        {
            yield return new WaitForSeconds(0.3f);
			state = startingState;
        }
        else if (state == State.dead)
        {
            //StartCoroutine(ifDead());
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

    public void PlayAttack() //
    {
        attack.Play();
    }
    public void PlayHurt()//
    {
        hurt.Play();
    }
    public void PlayDead()//
    {
        dead.Play();
    }
}
