using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WizardController : MonoBehaviour
{
    //Start() vars
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxColl;
    private CircleCollider2D circleColl;
    private CircleCollider2D groundCheck;
    public CharacterController2D controller;

    //Finite State Machine
    public enum State { idle, walking, jumping, falling, hurt, attacking, dead, victory, plant, water, fire }
    public State state = State.idle;

    //Inspector variables
    [SerializeField] private LayerMask ground;
    //private float speed = 20f;
    [SerializeField] private float jumpForce = 36f;
    private bool coyote;
    private bool jumped;

    //CheckPoint System
    [SerializeField] GameObject checkPts;
    private Vector3[] checkPoints;
    [SerializeField] int checkPoint = 0;

    //Health vars
    [SerializeField] private Text livesText;
    private int lives = 3;
    private float health;
    private int mana = 0;
    [SerializeField] private Slider healthSlider;

    //Attack vars
    [SerializeField] GameObject basicAttack;
    [SerializeField] Transform shotPoint;
    private float timeBtwBasics = 0.7f;
    private float timestampBasic;
    private bool fromLeft = false;

    //Special Attack Vars
    [SerializeField] bool plantOrb = false;
    [SerializeField] GameObject plantAttack;
    private float timeBtwPlants = 5f;
    private float timestampPlant;

    [SerializeField] bool waterOrb = false;
    [SerializeField] GameObject waterAttack;
    private float timeBtwWaters = 2f;
    private float timestampWater;

    [SerializeField] bool fireOrb = false;
    [SerializeField] GameObject fireAttack;
    private float timeBtwFires = 3f;
    private float timestampFire;


    float horizontalMove = 0f;
    [SerializeField] float runSpeed = 72f;
    private bool dontPush = false;
    private bool dotOn = false;

    //Sound effects
    [SerializeField] AudioSource steps;
    [SerializeField] AudioSource attack;
    [SerializeField] AudioSource hurt;
    [SerializeField] AudioSource jump;
    [SerializeField] AudioSource dead;

    [SerializeField] AudioSource plantSkill;
    [SerializeField] AudioSource fireSkill;
    [SerializeField] AudioSource waterSkill;
    [SerializeField] AudioSource potionPickup;
    [SerializeField] AudioSource victory;

    [SerializeField] bool isGrounded;
    [SerializeField] GameObject witch;
	bool yippee;

    // Start is called before the first frame update
    private void Start()
    {
        checkPoints = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            checkPoints[i] = checkPts.transform.GetChild(i).position;
            checkPoints[i].z = transform.position.z;
        }
        //transform.position = checkPoints[checkPoint];
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxColl = GetComponent<BoxCollider2D>();
        circleColl = GetComponent<CircleCollider2D>();
        //groundCheckPosition = transform.GetChild(0).position;
        //print(groundCheckPosition);
        groundCheck = GetComponentInChildren<CircleCollider2D>();
        health = healthSlider.value;
		yippee =false;
    }

    // Update is called once per frame
    private void Update()
    {
        livesText.text = "x " + lives.ToString();
		if (plantOrb && waterOrb && fireOrb && witch == null) {
			state = State.victory;
			yippee = true;
		}
        if (checkPoint < 5 && transform.position.x > checkPoints[checkPoint + 1].x)
        {
            checkPoint++;
        }
        if (state != State.hurt && state != State.dead && !yippee)
        {
            //print("My state is not hurt: " + state);
            if (Input.GetKeyDown(KeyCode.F) && Time.time >= timestampBasic)
            {
				StartCoroutine(pewpew());
            }
            else if (Input.GetKeyDown(KeyCode.A) && plantOrb && Time.time >= timestampPlant) //plant orb
            {
                plantSkill.Play();
				state = State.plant;
                Projectile bullet = Instantiate(plantAttack, transform.position, transform.rotation).GetComponent<Projectile>();
                if (health < 10)
                {
                    if (health < 7)
                    {
                        health += 3;
                    }
                    else
                    {
                        health = 10;
                    }
                }
                timestampPlant = Time.time + timeBtwPlants;
            }
            else if (Input.GetKeyDown(KeyCode.S) && waterOrb && Time.time >= timestampWater) //water orb
            {
                StartCoroutine(aquabolt());

                //Projectile bullet = Instantiate(basicAttack, shotPoint.position, transform.rotation).GetComponent<Projectile>();
                //bullet.dir = new Vector2(transform.localScale.x, 0);
                //bullet.transform.localScale = new Vector3(transform.localScale.x, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) && fireOrb && Time.time >= timestampFire) //fire orb
            {
                //Vector2 direction = witch.transform.position - transform.position;
                //Projectile bullet = Instantiate(fireAttack, shotPoint.position, transform.rotation).GetComponent<Projectile>();
                //Physics2D.IgnoreCollision(bullet.gameObject.GetComponent<BoxCollider2D>(), boxColl);

                //bullet.GetComponent<Rigidbody2D>().velocity = direction * 2f;

                //float angle = Mathf.Atan2(bullet.GetComponent<Rigidbody2D>().velocity.y, bullet.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
                //bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                StartCoroutine(firebolt());
            }
            else
            {
                Movement();
            }
            //else
            //{
            //    timeBtwBasics -= Time.deltaTime;
            //}

        }
        //if ((/*transform.position.y <= -25 || */transform.position.x >= 803.5) && state != State.dead)
        //{
        //    //yield return new WaitForSeconds(0f);
        //    //state = State.hurt;
        //    state = State.dead;
        //    StartCoroutine(Die());
        //}

        //var layermask = LayerMask.GetMask("Ground");
        //var collision = Physics2D.OverlapCircle(groundCheckPosition, 1f);
        //print(collision?.name);
        //if (collision)
        //{
        //    isGrounded = true;
        //}
        //else
        //{
        //    isGrounded = false;
        //}
		if (yippee) {
			state = State.victory;
		}
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        StartCoroutine(AnimationState());
        anim.SetInteger("state", (int)state);
        healthSlider.value = health;
    }

	private IEnumerator aquabolt() {
		state = State.water;
		yield return new WaitForSeconds(0.1f);
		waterSkill.Play();
		Projectile bullet = Instantiate(waterAttack, shotPoint.position, transform.rotation).GetComponent<Projectile>();
		bullet.dir = new Vector2(transform.localScale.x, 0);
		bullet.transform.localScale = new Vector3(transform.localScale.x, 1);
		timestampWater = Time.time + timeBtwWaters;
	}

	private IEnumerator firebolt() {
		state = State.fire;
		yield return new WaitForSeconds(0.1f);
		fireSkill.Play();
		Projectile bullet = Instantiate(fireAttack, shotPoint.position, transform.rotation).GetComponent<Projectile>();
		bullet.dir = new Vector2(transform.localScale.x, 0);
		bullet.transform.localScale = new Vector3(transform.localScale.x, 1);
		timestampFire = Time.time + timeBtwFires;
	}

    public IEnumerator pewpew()
    {
		state = State.attacking;
		yield return new WaitForSeconds(0.05f);
		attack.Play();
        Projectile bullet = Instantiate(basicAttack, shotPoint.position, transform.rotation).GetComponent<Projectile>();
        bullet.dir = new Vector2(transform.localScale.x, 0);
        bullet.transform.localScale = new Vector3(transform.localScale.x, 1);
		timestampBasic = Time.time + timeBtwBasics;
    }

    /*
	private void OnTriggerStay2D(Collider2D collision) {
		if(health > 0)
		{
            if (collision.tag == "Thorns")
            {
				state = State.hurt;
                health -= 1;
            }
        }
	}*/
    private IEnumerator snared()
    {
		runSpeed = 18f;
		jumpForce = 18f;
		yield return new WaitForSeconds(3f);
		jumpForce = 36f;
		runSpeed = 72f;
    }

    private IEnumerator damageOverTime()
    {
        while (dotOn)
        {
            state = State.hurt;
            health -= 1;
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        dotOn = false;
        StopCoroutine(damageOverTime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DP")
        {
            health = 0;
        }
        if (collision.tag == "HP")
        {
            potionPickup.Play();
            if (health < 10)
            {
                Destroy(collision.gameObject);
                if (health < 5)
                {
                    health += 5;
                }
                else
                {
                    health = 10;
                }
            }
        }
        if (collision.tag == "Mana")
        {
            if (mana < 10)
            {
                Destroy(collision.gameObject);
                if (mana < 5)
                {
                    mana += 5;
                }
                else
                {
                    mana = 10;
                }
            }
        }

        if (health > 0)
        {
            if (state != State.hurt && state != State.dead)
            {
                if (collision.tag == "ZombieBasicAttack" || collision.tag == "PlantBasicAttack" || collision.tag == "BatBasicAttack" ||
					collision.tag == "LavaBasicAttack" || collision.tag == "LavaPunch" || collision.tag == "WitchBasicAttack" || collision.tag == "WitchTriAttack")
                {
                    health -= 1;
                    state = State.hurt;

                    if (collision.gameObject.transform.position.x > transform.position.x)
                    {
                        fromLeft = false;
                    }
                    else
                    {
                        fromLeft = true;
                    }
                    Destroy(collision.gameObject);
                }
				if (collision.tag == "WitchTriAttack") {
					StartCoroutine(snared());
				}
                if (collision.tag == "Thorns")
                {
                    dotOn = true;
                    StartCoroutine(damageOverTime());
                    //health -= 1;
                    //state = State.hurt;
                    //if (health == 0)
                    //{
                    //    state = State.dead;
                    //    StartCoroutine(Die());
                    //}
                }
            }
        }

        if (collision.tag == "Orb")
        {
            state = State.victory;
            victory.Play();
            print("state: " + state);
            Destroy(collision.gameObject);
        }

        //if (health <= 0 && state != State.dead)
        //{
        //    //transform.position = checkPoint.position;
        //    state = State.dead;
        //    StartCoroutine(Die());
        //}

    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            StartCoroutine(coyoteEffect());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //print("Game Tag: " + other.gameObject.tag);
        //if(other.gameObject.tag == "GroundCheck")
        //{
        //    isGrounded = true;
        //}
        //else
        //{
        //    isGrounded = false;
        //}
        if (other.gameObject.tag == "Ground")
        {
            jumped = false;
        }
        if (state != State.hurt && state != State.dead)
        {
            if (other.gameObject.tag == "ZombieB" || other.gameObject.tag == "ZombieA" || other.gameObject.tag == "PlantBoss" || 
					other.gameObject.tag == "BatBoss" || other.gameObject.tag == "LavaBoss" || other.gameObject.tag == "WitchBoss")
            {
                //if (state == State.falling)
                //{
                //    Destroy(other.gameObject);
                //    Jump();
                //}
                //else
                //{
                //print("I'm hurt\n");
                state = State.hurt;
                health -= 1;

                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy on my right, so take damage and move left
                    fromLeft = false;
                }
                else
                {
                    //Enemy on my left, so take damage and move right
                    fromLeft = true;
                }
                //}
            }
            //if (health <= 0 && state != State.dead)
            //{
            //    state = State.dead;
            //    StartCoroutine(Die());
            //}
        }
    }

    private void FixedUpdate()
    {
        //Move our char
        if (state != State.dead)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
        }
    }

    private void Movement()
    {
        //float hdirection = Input.GetAxis("Horizontal");
        //Moving Left
        //if (hdirection < 0)
        //{
        //    rb.velocity = new Vector2(-speed, rb.velocity.y);
        //    transform.localScale = new Vector2(-1, 1);
        //}
        //Moving right
        //else if (hdirection > 0)
        //{
        //    rb.velocity = new Vector2(speed, rb.velocity.y);
        //    transform.localScale = new Vector2(1, 1);
        //}
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        //Jumping
        if (Input.GetButtonDown("Jump") && !jumped)
        {
            //groundCheck.IsTouchingLayers(ground)
            if (circleColl.IsTouchingLayers(ground) || boxColl.IsTouchingLayers(ground) || coyote)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        jumped = true;
        jump.Play();
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private IEnumerator coyoteEffect()
    {
        coyote = true;
        yield return new WaitForSeconds(0.3f);
        coyote = false;
    }

    private IEnumerator AnimationState()
    {
        if (state != State.walking)
        {
            steps.Stop();
        }
        if(state == State.victory)
        {
            yield return new WaitForSeconds(1f);
            state = State.idle;
        }
        else if (state != State.dead)
        {
            if (state == State.jumping)
            {
                if (rb.velocity.y < 0.1f)
                {
                    state = State.falling;
                }
            }
            else if (state == State.falling)
            {
                //circleColl
                if (circleColl.IsTouchingLayers(ground))
                {
                    state = State.idle;
                }
            }
            else if (state == State.victory)
            {
               
            }
            else if (state == State.hurt)
            {
                if (health <= 0)
                {
                    //state = State.dead;
                    StartCoroutine(Die());
                }
                else
                {
                    hurt.Play();
                    yield return new WaitForSeconds(0.2f);
                    if (!dontPush)
                    {
                        if (fromLeft)
                        {
                            rb.velocity = new Vector2(20f, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-20f, rb.velocity.y);
                        }
                    }
                    state = State.idle;
                }

            }
            else if (Mathf.Abs(rb.velocity.x) > 2f)
            {
                //Moving
                state = State.walking;
            }
			else if (state == State.water || state == State.fire) {
				yield return new WaitForSeconds(0.5f);
                state = State.idle;
			}
			else if (state == State.plant) {
				yield return new WaitForSeconds(1f);
                state = State.idle;
			}
            else if (state == State.attacking)
            {
                yield return new WaitForSeconds(0.33f);
                state = State.idle;
            }
            else if (state == State.dead)
            {
                //StartCoroutine(Die());
                //yield return new WaitForSeconds(0.33f);
                //state = State.idle;
            }
            else
            {
                state = State.idle;
            }
        }
        else
        {

        }
    }

    //multiple scenes running at the same time
    //create scriptable obj (Unity feature) 
    private IEnumerator Die()
    {
        state = State.dead;
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator respawn()
    {
        lives--;
        dead.Play();
        yield return new WaitForSeconds(1.5f);
        
        if (lives == 0)
        {
            restart();
        }
        else
        {
            health = 10;
            state = State.idle;
            transform.position = checkPoints[checkPoint];
        }
    }

    private void restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Moving()
    {
        steps.Play();
    }
}