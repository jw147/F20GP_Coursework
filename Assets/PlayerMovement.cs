using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    //Character Controller component
    public CharacterController controller;

    public GameObject Camera;
    public GameObject Player;
    
    //Constants needed for movement
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    Vector3 velocity;
    
    //Objects and constants needed for jumping and falling
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    //Object displaying user health and constant for user's starting health
    public Text health;
    public int CharacterHealth;

   
    public Image Death;
    public Sprite deathSprite;

    public bool win = false;

    public bool lose = false;



    //When the game starts, set the HUD text to the correct value
    void Start(){
        CharacterHealth = 100;
        health.text = CharacterHealth.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        if(win == false && lose == false){
            health.text = CharacterHealth.ToString();
            //Check to see if the player is Grounded
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                //if the player is grounded set the velocity to -2
                velocity.y = -2f;
            }

            //get the direction the player is facing and move them in that direction
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

            // if the player is grounded and presses the jump button, propell the player into the air dependent on the set jumpHeight,
            // a constant of -2 and gravity
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            //keep the player grounded by pulling them towards the ground with gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        if(CharacterHealth <= 0){
            //update the UI
            health.text = 0.ToString();
            //set the game as a loss
            lose = true;
            //load the Loss sprite into the Image
            Death.sprite = deathSprite;
            Death.color = Color.white;
            //tell the other scripts that the game is lost
            var Lose = Player.GetComponent<GunController>();
            Lose.Lose();
            var Lose2 = Camera.GetComponent<MouseLook>();
            Lose2.Lose();
        }
    }

    public void Win(){
        //called by another script to determine the state of the game as a win
        win = true;
    }

    public void Lose(){
        //function that determines the state of the game as a loss
        lose = true;
    }

    public void getHit(){
        //called by another script to decrement the player's health by 2
        CharacterHealth = CharacterHealth - 2;
    }
}
