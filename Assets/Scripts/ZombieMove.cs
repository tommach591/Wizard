using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieMove : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;
    private Rigidbody2D rb;
    public CharacterController2D controller;

    private float leftCap;
    private float rightCap;
    private float walkLength = 100;

    private enum State { idle, walking, attacking, hurt, dead }
    private State state = State.idle;

    public Canvas healthBar;
    private float health;
    [SerializeField] private Slider healthSlider;

    private bool facingLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        health = healthSlider.value;
        leftCap = transform.position.x - 3;
        rightCap = transform.position.x + 3;
    }

    // Update is called once per frame
    void Update()
    {

        //if(transform.position.x > leftCap)
        //{
            //transform.localScale = new Vector3(1, 1);
        //rb.velocity = new Vector2(-walkLength, rb.velocity.y);
        controller.Move(walkLength * Time.fixedDeltaTime, false, false) ;
        //}
        //else
        //{
        //    transform.localScale = new Vector3(-1, 1);
        rb.velocity = new Vector2(walkLength, rb.velocity.y);
        //}

        //if (facingLeft)
        //{
        //    //Move to the left
        //    if (transform.position.x > leftCap)
        //    {
        //        //Make sure sprite facing right dir
        //        if (transform.localScale.x != 1)
        //        {
        //            transform.localScale = new Vector3(1, 1);
        //        }
        //        rb.velocity = new Vector2(-walkLength, rb.velocity.y);
        //    }
        //    else
        //    {
        //        facingLeft = false;
        //    }
        //}
        //else
        //{
        //    if (transform.position.x < rightCap)
        //    {
        //        //Make sure sprite facing left dir
        //        if (transform.localScale.x != -1)
        //        {
        //            transform.localScale = new Vector3(-1, 1);
        //        }
        //        rb.velocity = new Vector2(walkLength, rb.velocity.y);
        //    }
        //    else
        //    {
        //        facingLeft = true;
        //    }
        //}
        StartCoroutine(AnimationState());
        anim.SetInteger("state", (int)state);
        healthSlider.value = health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBasicAttack")
        {
            health -= 1;
            state = State.hurt;
            Destroy(collision.gameObject);
        }
        if (health == 0)
        {
            state = State.dead;
            //GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(ifDead());
        }
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
            yield return new WaitForSeconds(0.7f);
            state = State.idle;
        }
    }
}
