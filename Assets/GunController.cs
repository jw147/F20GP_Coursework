using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunController : MonoBehaviour
{

    // Gun animation and audio variables
    AudioSource shoot;
    Animator animGun;
    
    // Gong GameObject, animation and audio variables
    [Header("Gong Settings")]
    public GameObject Gong;
    Animator animGong;
    private AudioSource soundGong;


    // Gun animation variables
    int fireHash = Animator.StringToHash("Fire");
    int idleHash = Animator.StringToHash("IdleGun");

    int hitGongHash = Animator.StringToHash("Hit");

    // Variables controlling the player's ammo and whether or not they can shoot
    [Header("Gun Settings")]
    public float fireRate = 0.5f;
    public int clipSize = 12;
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

    bool getShot = true;


    public void Start(){
        // When the game starts, set the current ammo in the player's clip to clipSize and allow them to shoot
        _currentAmmoInClip = clipSize;
        _canShoot = true;
        ammoText.text = _currentAmmoInClip.ToString();
        reserveAmmoText.text = "/0";

        // Assigning animation and audio source variables to their respective components
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
        // New RaycastHit variable called hit
        RaycastHit hit;

        
        if(Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, Mathf.Infinity)){
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Gong")){
                //if Raycast hits a gameObject that is a layer called "Gong", play the gong animation and sound
                animGong.SetTrigger(hitGongHash);
                soundGong.Play();
            }
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy")){
                Debug.Log("Zombie");
                //if Raycast hits a gameObject that is a layer called "Enemy", send the gameObject's script a call to the getShot() function
                var EH = hit.collider.gameObject.GetComponent<ZombieScript>();
                EH.getShot();
            }
        }        
    }


    IEnumerator ShootGun(){
        // set the animation trigger to fire the gun
        animGun.SetTrigger(fireHash);
        // call the MuzzleFlash CoRoutine
        StartCoroutine(MuzzleFlash());
        //Call the RayCast() function
        RayCast();
        
        // Wait the amount of seconds the fireRate is set to before allowing the user to shoot again
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }

    IEnumerator MuzzleFlash(){
        //flash the muzzleFlash Image
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        //wait 0.05 seconds
        yield return new WaitForSeconds(0.05f);
        //reset the muzzleFlash image to a clear image with no colour
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
    }
}
