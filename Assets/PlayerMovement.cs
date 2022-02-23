using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    //Character Controller component
    public CharacterController controller;
    
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
    public int CharacterHealth = 100;

    //When the game starts, set the HUD text to the correct value
    void Start(){
        health.text = CharacterHealth.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        //Check to see if the player is Grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0){
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
        if(Input.GetButtonDown("Jump") && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //keep the player grounded by pulling them towards the ground with gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
