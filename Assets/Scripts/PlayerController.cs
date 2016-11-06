using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    Animator anim;
    Vector2 flyingVelocity;
    int speed;
    Rigidbody2D body;
    bool flying, wallJump, grounded;
    int jumpCount;

	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        flying = false;
        grounded = true;
        wallJump = false;
        jumpCount = 0;
        speed = 0;
        flyingVelocity = new Vector2(2.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        //speed = new Vector2( Mathf.Clamp(speed.x * 0.1f, 0, 2f), Mathf.Clamp( speed.y * 0.1f, 0, 2f));
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpCount++;
            flying = (jumpCount > 1) ? true : false;
            grounded = false;

            if (!flying)
            {
                anim.SetBool("walk", false);
                body.velocity += Vector2.up * 3.5f;
            }
            else
            {
                jumpCount = 0;
                body.velocity = flyingVelocity;
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!flying && body.velocity.magnitude < 2f)
            {
                body.velocity += Vector2.right;
                grounded = true;
                anim.SetBool("walk", true);
            }
            else if (Camera.main.WorldToScreenPoint(transform.position).x <= Screen.width / 2 && wallJump)
            {
                transform.Rotate(Vector3.up * 180);
                grounded = false;
                flying = true;
                flyingVelocity.Set(-flyingVelocity.x, flyingVelocity.y);
                body.velocity = flyingVelocity;
            }
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (!flying && body.velocity.magnitude < 2f)
            {
                grounded = true;
                anim.SetBool("walk", true);
                body.velocity += Vector2.left;
            }
            else if (Camera.main.WorldToScreenPoint(transform.position).x > Screen.width / 2 && wallJump)
            {
                transform.Rotate(Vector3.up * 180);
                flying = true;
                grounded = false;
                flyingVelocity.Set(-flyingVelocity.x, flyingVelocity.y);
                body.velocity = flyingVelocity;
            }
        }

        if (body.velocity.SqrMagnitude() < 0.2f)
            anim.SetBool("walk", false);

        anim.SetBool("jump", !grounded);
        body.gravityScale = (flying) ? 0 : 1;

        print(grounded);
        //print((Screen.width / 2) + "|" + ;
        //print("Flying: " + flying + "\nCalc: " + (transform.position.x <= Screen.width / 2) + "\nWallJump: " + wallJump);
        //print("Velocity: " + body.velocity + "\nGravity: " + body.gravityScale + "\nFlying: " + flying + "\nCalc: " + (transform.position.x > Screen.width / 2));
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name.Contains("Wall"))
        {
            flying = false;
            body.velocity = Vector2.zero;
        }

        if (coll.gameObject.tag.Equals("Obstacle"))
        {
            jumpCount = 0;
            flyingVelocity.Set(2.5f, 0.5f);
            grounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        print("<color=green>" + coll.name + "</color>");
        if (coll.name.Equals("Wall"))
            wallJump = true;
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.name.Equals("Wall"))
            wallJump = false;
    }
}
