using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    // Zombie speed
    public float speed = 1f;
    //Amount of time zombie wanders in one direction before changing
    public float wanderTime;
    // Bool to check if the zombie is dead
    public bool isDead;
    //float to determine between one of two walking animations for zombies
    public float arms;
    // Variable to get Animator component of Zombie
    public Animator animZombie;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get the animator component of the zombie and set isDead to false
        animZombie = GetComponent<Animator>();
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == false){
            if(wanderTime > 0){
                //if the zombie is alive and wanderTime is > 0, walk forward at a constant speed and decrement wanderTime by Time.deltaTime
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                wanderTime -= Time.deltaTime;
            }else{
                //if wanderTime = 0, set wanderTime to a random number between 5.0 and 10.0
                wanderTime = Random.Range(5.0f, 10.0f);
                //call the Wander function
                Wander();
            }
        }
        else{
            //if Zombie isDead == true, Trigger the Dead animation for this zombie 
            animZombie.SetTrigger("Dead");
        }
    }
 
    public void getShot(){
        //Function that is called by the Gun Controller that sets isDead to true if hit
        isDead = true;
    }
    void Wander(){
        //Set the wander animation to one of these two randomly
        arms = Random.Range(0f, 1f);
        if(arms < 0.5f){
            animZombie.SetTrigger("WalkArmsDown");
        }else{
            animZombie.SetTrigger("WalkArmsUp");
        }
        //rotate the zombie in any direction in 360 of the y axis
        transform.eulerAngles = new Vector3 (0, Random.Range (0, 360), 0);
    }

    private void OnTriggerEnter(Collider zombie){
        //if the Trigger Collision box Enters another gameObject, rotate any direction within 180 degrees of the y axis
        transform.eulerAngles = new Vector3 (0, Random.Range (0, 180), 0);
    }
}
