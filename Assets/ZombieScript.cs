using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    public float speed = 2f;
    public float wanderTime;
    public CharacterController controller;
    public float gravity = -9.81f;
    public bool isDead;
    public float arms;
    public Animator animZombie;
    
    // Start is called before the first frame update
    void Start()
    {
        animZombie = GetComponent<Animator>();
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == false){
            if(wanderTime > 0){
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                wanderTime -= Time.deltaTime;
            }else{
                wanderTime = Random.Range(5.0f, 10.0f);
                Wander();
            }
        }
        else{
            animZombie.SetTrigger("Dead");
        }
    }

    void Wander(){
        arms = Random.Range(0f, 1f);
        if(arms < 0.5f){
            animZombie.SetTrigger("WalkArmsDown");
        }else{
            animZombie.SetTrigger("WalkArmsUp");
        }
        transform.eulerAngles = new Vector3 (0, Random.Range (0, 180), 0);
    }

    private void OnTriggerEnter(Collider zombie){
        transform.eulerAngles = new Vector3 (0, Random.Range (0, 90), 0);
    }
}
