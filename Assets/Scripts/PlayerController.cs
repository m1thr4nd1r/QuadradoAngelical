using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    Text ui;
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
        ui = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<Text>();
		print(ui);
        flying = false;
        grounded = true;
        wallJump = false;
        jumpCount = 0;
        speed = 0;
        flyingVelocity = new Vector2(2.5f, 1.5f);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(0);
			Time.timeScale = 1;
		}

		//speed = new Vector2( Mathf.Clamp(speed.x * 0.1f, 0, 2f), Mathf.Clamp( speed.y * 0.1f, 0, 2f));
		ui.text = "Tempo: " + Time.timeSinceLevelLoad;

		Vector3 camPos = new Vector3(transform.position.x,
									 transform.position.y + 0.8f,
									 -10f);
		Camera.main.gameObject.transform.position = camPos;

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
            transform.localScale = Vector3.one;

            if (!flying && body.velocity.magnitude < 2f)
            {
                body.velocity += Vector2.right;
                grounded = true;
                anim.SetBool("walk", true);
            }
            else if (Camera.main.WorldToScreenPoint(transform.position).x <= Screen.width / 2 && wallJump)
            {
                grounded = false;
                flying = true;
                flyingVelocity.Set(-flyingVelocity.x, flyingVelocity.y);
                body.velocity = flyingVelocity;
            }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localScale = (Vector3.right * -2) + Vector3.one;
			print(wallJump + "|" + Camera.main.WorldToScreenPoint(transform.position).x + "|" +  Screen.width / 2);

            if (!flying && body.velocity.magnitude < 2f)
            {
                grounded = true;
                anim.SetBool("walk", true);
                body.velocity += Vector2.left;
            }
            else if (Camera.main.WorldToScreenPoint(transform.position).x >= Screen.width / 2 && wallJump)
            {
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

        //print(grounded);
        //print((Screen.width / 2) + "|" + ;
        //print("Flying: " + flying + "\nCalc: " + (transform.position.x <= Screen.width / 2) + "\nWallJump: " + wallJump);
        //print("Velocity: " + body.velocity + "\nGravity: " + body.gravityScale + "\nFlying: " + flying + "\nCalc: " + (transform.position.x > Screen.width / 2));
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name.Contains("Collision"))
        {
            flying = false;
            flyingVelocity.Set(2.5f, 0.5f);
            body.velocity = Vector2.zero;
        }

        if (coll.gameObject.tag.Equals("Enemy"))
        {
            Time.timeScale = 0;
            //jumpCount = 0;
            //flyingVelocity.Set(2.5f, 0.5f);
            //grounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        print("<color=green>" + coll.name + "</color>");
        if (coll.name.Equals("Collision"))
            wallJump = true;
        if (coll.transform.tag.Equals("End"))
            Time.timeScale = 0;
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.name.Equals("Collision"))
            wallJump = false;
    }
}
