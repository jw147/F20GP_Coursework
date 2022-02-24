using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{

    public GameObject PlayerCollision;
    public GameObject Player;
    public GameObject GC;

    // Zombie speed
    public float speed = 1f;
    //Amount of time zombie wanders in one direction before changing or idles in the same position
    public float wanderTime;
    public float idleTime;
    // Bool to check if the zombie is dead
    public bool isDead;

    public bool gameStart;
    //float to determine between one of two walking animations for zombies
    public float arms;
    public float zombieRunChance;
    // Variable to get Animator component of Zombie
    public Animator animZombie;

    public float WanderorIdle;

    public bool moveTowards;

    public int health = 100;

    
    

    // Start is called before the first frame update
    void Start()
    {
        // Get the animator component of the zombie and set isDead and gameStart to false
        animZombie = GetComponent<Animator>();
        isDead = false;
        gameStart = false;
        moveTowards = false;
        //assign xCoord and zCoord to random variables within the map boundaries
        float xCoord = Random.Range(-40.0f, 21.0f);
        float zCoord = Random.Range(20, 48);
        //randomly spawn each zombie within the map
        transform.position = new Vector3(xCoord, 3, zCoord);
        WanderorIdle = Random.Range(0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == false && gameStart == false){
            if(WanderorIdle > 0.2){
                if (wanderTime > 0){
                    //if the zombie is alive and wanderTime is > 0, walk forward at a constant speed and decrement wanderTime by Time.deltaTime
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    wanderTime -= Time.deltaTime;
                }
                else{
                    if(WanderorIdle > 0.2){
                        //if wanderTime = 0, set wanderTime to a random number between 5.0 and 10.0
                        wanderTime = Random.Range(5.0f, 10.0f);

                        //call the Wander function
                        Wander();
                    }
                }
            }else{
                if (idleTime > 0){
                    idleTime -= Time.deltaTime;
                }else{
                    WanderorIdle = Random.Range(0.0f, 1.0f);
                    if(WanderorIdle <= 0.2){
                        idleTime = Random.Range(4.0f, 8.0f);
                        Idle();
                    }
                }
            }
        }
        else if(gameStart == true && isDead == true){
            //if Zombie isDead == true, Trigger the Dead animation for this zombie 
            animZombie.SetTrigger("Dead");
        }
        else if(gameStart == true && isDead == false){
            //animZombie.SetTrigger("Dead");
            zombieRunChance = Random.Range(0.0f, 1.0f);
            zombieRun();

        }
    }
 
    public void getShot(){
        health = health - 25;
        if(health == 0){
            //Function that is called by the Gun Controller that sets isDead to true if hit
            isDead = true;
            var DZ = GC.GetComponent<GunController>();
            DZ.minusZombie();
        }
    }

    public void startGame(){
        
        //Function that is called by the Gun Controller that sets isDead to true if hit
        gameStart = true;
        moveTowards = true;
    }

    void zombieRun(){
        animZombie.SetTrigger("Run");
        Vector3 playerPos = PlayerCollision.transform.position;
        Vector3 targetDirection = PlayerCollision.transform.position - transform.position;
        transform.LookAt(PlayerCollision.transform);
        if(moveTowards == true){

        if(zombieRunChance <= 0.33){
            transform.position += transform.forward * 3.5f * Time.deltaTime;
        }
        else if(zombieRunChance > 0.33 && zombieRunChance <= 0.66){
            transform.position += transform.forward * 4.0f * Time.deltaTime;
        }else{
            transform.position += transform.forward * 4.5f * Time.deltaTime;
        }
        }
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

    void Idle(){
        //Set the wander animation to one of these two randomly
        animZombie.SetTrigger("Idle");
        //rotate the zombie in any direction in 360 of the y axis
        transform.eulerAngles = new Vector3 (0, Random.Range (0, 360), 0);
        
    }

    private void OnTriggerEnter(Collider other){
        //if the Trigger Collision box Enters another gameObject, rotate any direction within 180 degrees of the y axis
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            StartCoroutine(Attack());
        }
        else{
            Debug.Log("here");
            transform.eulerAngles = new Vector3 (0, Random.Range (0, 180), 0);
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            moveTowards = true;
        }
    }

    IEnumerator Attack(){
        moveTowards = false;
        animZombie.SetTrigger("Attack");
        yield return new WaitForSeconds(1.0f);
        var getHit = Player.GetComponent<PlayerMovement>();
        getHit.getHit();
    }

    // private float Distance(){
    //     return Vector3.Distance(zombie.position, player.position);
    // }
}
