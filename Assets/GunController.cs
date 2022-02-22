using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunController : MonoBehaviour
{

    private AudioSource shoot;
    Animator animGun;

    [Header("Gong Settings")]
    public GameObject Gong;
    Animator animGong;
    private AudioSource soundGong;


    int fireHash = Animator.StringToHash("Fire");
    int idleHash = Animator.StringToHash("IdleGun");

    int hitGongHash = Animator.StringToHash("Hit");

    [Header("Gun Settings")]
    public float fireRate = 0.5f;
    public int clipSize = 12;


    // Variables controlling the player's ammo and whether or not they can shoot
    bool _canShoot;
    int _currentAmmoInClip;

    // Muzzle Flash
    [Header("Muzzle Flash Settings")]
    public Image muzzleFlashImage;
    public Sprite[] flashes;

    // HUD
    [Header("HUD Settings")]
    public Text ammoText;
    public Text reserveAmmoText;


    public void Start(){
        // When the game starts, set the current ammo in the player's clip to clipSize and allow them to shoot
        _currentAmmoInClip = clipSize;
        _canShoot = true;
        ammoText.text = _currentAmmoInClip.ToString();
        reserveAmmoText.text = "/0";

        shoot = GetComponent<AudioSource>();
        animGun = GetComponent<Animator>();
        animGong = Gong.GetComponent<Animator>();
        soundGong = Gong.GetComponent<AudioSource>();
        
    }

    private void Update(){
        // If the player clicks the left mouse button once, _canShoot is true and there is ammo in the player's clip:
        if(Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0){
            // Set _canShoot to false, decrement the amount of ammo in the clip by one and call a Coroutine called ShootGun
            _canShoot = false;
            _currentAmmoInClip--;
            ammoText.text = _currentAmmoInClip.ToString();
            StartCoroutine(ShootGun());
            shoot.Play();
        }else{}
    }
    
    void RayCast(){
        RaycastHit hit;

        if(Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, Mathf.Infinity)){
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Gong")){
                Debug.Log("Gong");
                animGong.SetTrigger(hitGongHash);
                soundGong.Play();
            }
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy")){
                Debug.Log("Zombie");
                ScriptType script = WhatIHit.collider.GetComponent<ScriptType>();
                if(script!=null){
                    script.isDead = true;
                }
            }
        }
        
        //If Enemy is hit
        // if(Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, 1 << LayerMask.NameToLayer("Enemy"))){
        //     try{
        //         Debug.Log("Hit an enemy");
        //         Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
        //         rb.constraints = RigidbodyConstraints.None;
        //         rb.AddForce(transform.parent.transform.forward * 500);
        //     }catch{            }
        // }
        // if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Gong")){
        //     try{
        //         Debug.Log("Hit gong");
        //         animGong.SetTrigger(hitGongHash);
        //         // Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
        //         // rb.constraints = RigidbodyConstraints.None;
        //         // rb.AddForce(transform.parent.transform.forward * 500);
        //     }catch{            }
        // }
        
    }

    void RayCastForGong(){
        RaycastHit hit;
        
    }

    IEnumerator ShootGun(){

        animGun.SetTrigger(fireHash);
        StartCoroutine(MuzzleFlash());

        RayCast();
        
        // Wait the amount of seconds the fireRate is set to before allowing the user to shoot again
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }

    IEnumerator MuzzleFlash(){
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
    }
}
