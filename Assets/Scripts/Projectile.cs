using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 14.0f;
    public float lifeTime = 1.5f;
    private float damage = 1.0f;
    [SerializeField] private LayerMask ground;
    private BoxCollider2D boxColl;

    private enum State{ idle, dead };
    private State state = State.idle;
    public GameObject toHit;
    //public bool hit = false;
    public Vector2 dir;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
        try
        {
            boxColl = GetComponent<BoxCollider2D>();
        }
        catch
        {
            //plant heal don't have collider
        }
        state = State.idle;
        //print("state: " + state);
    }

    // Update is called once per frame
    void Update()
    {
        if(state != State.dead)
        {
            //if (toHit)
            //{
            //    dir.x = toHit.transform.position.x;
            //    dir.y = toHit.transform.position.y;
            //}
            if (boxColl && boxColl.IsTouchingLayers(ground))
            {
                //print("that's what's up");
                state = State.dead;
                //StartCoroutine(ifDead());
                ifDead();
            }
            else
            {
                transform.Translate(dir * speed * Time.deltaTime);
            }
        }
        else
        {
            //print("we are dying");
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
    
    private void ifDead()
    {
        state = State.dead;
        Destroy(gameObject);
    }
}
