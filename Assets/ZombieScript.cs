using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour
{

    // GameObjects needed for Script manipulation
    public GameObject PlayerCollision;
    public GameObject Player;
    public GameObject GC;

    // Zombie speed
    public float speed = 1f;
    //Amount of time zombie wanders in one direction before changing or idles in the same position
    public float wanderTime;
    public float idleTime;
    // Bool to check if the zombie is dead and if the game has started
    public bool isDead;

    public bool gameStart;
    //float to determine between one of two walking animations for zombies
    public float arms;
    public float zombieRunChance;
    // Variable to get Animator component of Zombie
    public Animator animZombie;
    // Variable to determine whether or not the Zombie wanders or idles before game start
    public float WanderorIdle;
    //Boolean determining whether or not the zombies chase the player
    public bool moveTowards;
    // Zombie health variable
    public int health = 100;

    

    // Start is called before the first frame update
    void Start()
    {
        
        // Get the animator component of the zombie and set isDead, moveTowards and gameStart to false
        animZombie = GetComponent<Animator>();
        isDead = false;
        gameStart = false;
        moveTowards = false;
        //assign xCoord and zCoord to random variables within the map boundaries
        float xCoord = Random.Range(-30.0f, 11.0f);
        float zCoord = Random.Range(25, 38);
        //randomly spawn each zombie within the map
        transform.position = new Vector3(xCoord, 3, zCoord);
        // set wander or idle for each zombie to a random number between 0 and 1
        WanderorIdle = Random.Range(0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // if the zombie is alive and the game has not started
        if(isDead == false && gameStart == false){
            // 80% chance the zombie wanders
            if(WanderorIdle > 0.2){
                if (wanderTime > 0){
                    //50% chance the zombie animation is walking with arms up or down
                    if (arms < 0.5f)
                    {
                        animZombie.SetTrigger("WalkArmsDown");
                    }
                    else
                    {
                        animZombie.SetTrigger("WalkArmsUp");
                    }
                    //if the zombie is alive and wanderTime is > 0, walk forward at a constant speed and decrement wanderTime by Time.deltaTime
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    wanderTime -= Time.deltaTime;
                }
                else{
                    //reset wander or idle to another random number between 0 and 1
                    WanderorIdle = Random.Range(0.0f, 1.0f);
                    if(WanderorIdle > 0.2){
                        //if wanderTime = 0, set wanderTime to a random number between 5.0 and 10.0
                        wanderTime = Random.Range(5.0f, 10.0f);

                        //call the Wander function
                        Wander();
                    }
                }
            }else{
                //if the idleTime is more than 0, do nothing except decrease idletime by the current time in seconds
                if (idleTime > 0){
                    idleTime -= Time.deltaTime;
                    animZombie.SetTrigger("Idle");
                }else{
                    //reset wander or idle to another random number between 0 and 1
                    WanderorIdle = Random.Range(0.0f, 1.0f);
                    if(WanderorIdle <= 0.2){
                        //set idletime to a random time between 4 and 8 seconds
                        idleTime = Random.Range(4.0f, 8.0f);
                        //call the idle function
                        Idle();
                    }
                }
            }
        }
        else if(gameStart == true && isDead == true){
            //if Zombie isDead == true and the game has started, Trigger the Dead animation for this zombie 
            animZombie.SetTrigger("Dead");
        }
        else if(gameStart == true && isDead == false){
            //if the game has started and the zombie is alive, set the zombies runchance to a random number between 0 and 1
            zombieRunChance = Random.Range(0.0f, 1.0f);
            //call the zombieRun function
            zombieRun();

        }
    }
 
    public void getShot(){
        // minus the current health by 25 
        health = health - 25;
        //if the zombies health is 0, set the zombie to dead and tell the GunController script to minus a zombie
        if(health == 0){
            isDead = true;
            var DZ = GC.GetComponent<GunController>();
            DZ.minusZombie();
        }
    }

    public void startGame(){
        //function called by another script to start the game when the Gong is shot
        gameStart = true;
        moveTowards = true;
    }

    void zombieRun(){
        //algorithm to make the zombie chase the player

        //set the zombie animation to the run animation
        animZombie.SetTrigger("Run");
        //create a new Vector3 setting the target direction to be the difference between the player's position and the zombie's position
        Vector3 targetDirection = PlayerCollision.transform.position - transform.position;
        //if the moveTowards function has been set to true
        if(moveTowards == true){
        //make the zombie object face the player object
        transform.LookAt(PlayerCollision.transform);
            //set the speed of the zombie to any of these speeds 33% chance of each
            if (zombieRunChance <= 0.33)
            {
                transform.position += transform.forward * 3.0f * Time.deltaTime;
            }
            else if (zombieRunChance > 0.33 && zombieRunChance <= 0.66)
            {
                transform.position += transform.forward * 4.0f * Time.deltaTime;
            }
            else
            {
                transform.position += transform.forward * 5.0f * Time.deltaTime;
            }
        }
    }

    void Wander(){
        //Set the wander animation to one of these two randomly with a 50% chance of each
        arms = Random.Range(0f, 1f);
        if(arms < 0.5f){
            animZombie.SetTrigger("WalkArmsDown");
        }else{
            animZombie.SetTrigger("WalkArmsUp");
        }
        //rotate the zombie in any direction in 360 of the y axis
        transform.eulerAngles = new Vector3 (0, Random.Range (0, 360), 0);
        
    }

    void Idle(){
        //Set the wander animation to one of these two randomly
        animZombie.SetTrigger("Idle");
        //rotate the zombie in any direction in 360 of the y axis
        transform.eulerAngles = new Vector3 (0, Random.Range (0, 360), 0);
    }

    private void OnTriggerEnter(Collider other){
        //if the Trigger Collision box enters the Player's gameobject, set moveTowards as false and call the attack function
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            StartCoroutine(Attack());
            moveTowards = false;
        }
        else if(other.gameObject.layer != LayerMask.NameToLayer("Ground") && other.gameObject.layer != LayerMask.NameToLayer("Enemy")){
            //if the zombie collides with objects that are not the ground, other enemies or the player, rotate in a random direction by a degree of 0-180 in the y axis
            //(this does not effect the zombie when running towards the player. Used for obstacle avoidance when game is in lobby mode)
            transform.eulerAngles = new Vector3 (0, Random.Range(0,180), 0);
        }
    }

    private void OnTriggerExit(Collider other){
        //if the zombie is no longer colliding with the player, set moveTowards as true
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            moveTowards = true;
        }
    }

    IEnumerator Attack(){
        //do not move towards the player
        moveTowards = false;
        //trigger the attack animation
        animZombie.SetTrigger("Attack");
        //call the getHit() function from playermovement
        var getHit = Player.GetComponent<PlayerMovement>();
        getHit.getHit();
        //wait for 1 second
        yield return new WaitForSeconds(1.0f);
    }
}
