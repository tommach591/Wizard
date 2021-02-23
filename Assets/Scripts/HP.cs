using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    private enum State { idle, used }
    private State state = State.idle;

    private Animator anim;
    private Collider2D coll;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.used)
        {
            //Destroy(this);
        }
        anim.SetInteger("state", (int)state);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "Player")
        {
            state = State.used;
        }
    }
}
