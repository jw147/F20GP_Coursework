using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunController : MonoBehaviour
{
    // gameStart boolean
    public bool gameStart = false;

    public GameObject Camera;
    public GameObject Player;
    private AudioSource soundPlayer;

    public GameObject StartMusic;
    private AudioSource soundMusic;

    public GameObject LobbyMusic;
    private AudioSource soundLobby;


    public List<GameObject> objectList = new System.Collections.Generic.List<GameObject>();
    

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
    public int reserveAmmo = 36;
    bool _canShoot;
    int _currentAmmoInClip;
    private AudioSource soundReload;
    public GameObject reload;

    // Muzzle Flash
    [Header("Muzzle Flash Settings")]
    public Image muzzleFlashImage;
    public Sprite[] flashes;

    // HUD
    [Header("HUD Settings")]
    public Text ammoText;
    public Text reserveAmmoText;
    public Text zombiesLeftText;
    public Text startPromptText;

     public Image Victory;
    public Sprite victorySprite;
    

    int zombiesLeft = 15;
    bool win = false;
    bool lose = false;


    public void Start(){
        FindAllZombies();

        // When the game starts, set the current ammo in the player's clip to clipSize and allow them to shoot
        _currentAmmoInClip = clipSize;
        _canShoot = true;
        ammoText.text = _currentAmmoInClip.ToString();
        reserveAmmoText.text = "/" + reserveAmmo.ToString();
        zombiesLeftText.text = "";
        startPromptText.text = "Shoot The Gong To Start";

        // Assigning animation and audio source variables to their respective components
        shoot = GetComponent<AudioSource>();
        animGun = GetComponent<Animator>();
        animGong = Gong.GetComponent<Animator>();
        soundGong = Gong.GetComponent<AudioSource>();
        soundPlayer = Player.GetComponent<AudioSource>();
        soundReload = reload.GetComponent<AudioSource>();
        soundMusic = StartMusic.GetComponent<AudioSource>();
        soundLobby = LobbyMusic.GetComponent<AudioSource>();
        soundLobby.Play();
    }

    private void Update(){
        if(win == false && lose == false){
            if (gameStart == true)
            {
                startPromptText.text = "";
                zombiesLeftText.text = "Zombies Left: " + zombiesLeft.ToString();
            }
            // If the player clicks the left mouse button once, _canShoot is true and there is ammo in the player's clip:
            if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
            {
                // Set _canShoot to false, decrement the amount of ammo in the clip by one and call a Coroutine called ShootGun
                _canShoot = false;
                _currentAmmoInClip--;
                ammoText.text = _currentAmmoInClip.ToString();
                StartCoroutine(ShootGun());
                shoot.Play();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }

        }
        if(zombiesLeft == 0){
            zombiesLeftText.text = "Zombies Left: " + zombiesLeft.ToString();
            win = true;
            Victory.sprite = victorySprite;
            Victory.color = Color.white;
            soundMusic.Stop();
            var Win = Player.GetComponent<PlayerMovement>();
            Win.Win();
            var Win2 = Camera.GetComponent<MouseLook>();
            Win2.Win();
        }
    }
    
    void RayCast(){
        // New RaycastHit variable called hit
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.parent.position, transform.parent.forward, Mathf.Infinity);
        int zombiesHit = 0;

        for(int x = 0; x < hits.Length; x++){
            RaycastHit hit = hits[x];

            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Gong")){
                //if Raycast hits a gameObject that is a layer called "Gong", play the gong animation and sound
                animGong.SetTrigger(hitGongHash);
                soundGong.Play();
                //if the gong is shot and the game has not started, start the game, iterate 
                //through the list of zombies and call the startGame() script within each object
                if(gameStart==false){
                    zombiesLeftText.text = "Zombies Left: "+zombiesLeft.ToString();
                    soundMusic.Play();
                    soundLobby.Stop();
                    soundPlayer.Play();
                    gameStart = true;
                    for (var i = 0; i < objectList.ToArray().Length; i++){
                        var start = objectList[i].GetComponent<ZombieScript>();
                        start.startGame();
                        
                    }
                }
            }
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") && gameStart == true && zombiesHit <= 2){
                //if Raycast hits a gameObject that is a layer called "Enemy", send the gameObject's script a call to the getShot() function
                var EH = hit.collider.gameObject.GetComponent<ZombieScript>();
                EH.getShot();
                zombiesHit++;
            }else{}
        }
        
        // if(Physics.RaycastAll(transform.parent.position, transform.parent.forward, out hit, Mathf.Infinity)){
        //     if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Gong")){
        //         //if Raycast hits a gameObject that is a layer called "Gong", play the gong animation and sound
        //         animGong.SetTrigger(hitGongHash);
        //         soundGong.Play();
        //         soundPlayer.Play();
        //         //if the gong is shot and the game has not started, start the game, iterate 
        //         //through the list of zombies and call the startGame() script within each object
        //         if(gameStart==false){
        //             gameStart = true;
        //             for (var i = 0; i < objectList.ToArray().Length; i++){
        //                 var start = objectList[i].GetComponent<ZombieScript>();
        //                 start.startGame();
                        
        //             }
        //         }
        //     }
        //     if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") && gameStart == true){
        //         //if Raycast hits a gameObject that is a layer called "Enemy", send the gameObject's script a call to the getShot() function
        //         var EH = hit.collider.gameObject.GetComponent<ZombieScript>();
        //         EH.getShot();
        //     }
        // }        
    }

    public void minusZombie(){
        zombiesLeft--;
    }
    public void Lose(){
        lose = true;
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

    IEnumerator Reload(){
        soundReload.Play();
        _canShoot = false;
        int maxClipSize = 12;
        reserveAmmo = reserveAmmo - (maxClipSize - _currentAmmoInClip);
        _currentAmmoInClip = maxClipSize;
        //wait 0.05 seconds
        yield return new WaitForSeconds(3f);
        reserveAmmoText.text = "/" + reserveAmmo.ToString();
        ammoText.text = _currentAmmoInClip.ToString();
        _canShoot = true;
    }

    void FindAllZombies(){
        //create a new array called objectArray and set it equal to all GameObjects
        GameObject[] objectArray = FindObjectsOfType<GameObject>();
        for(var i = 0; i < objectArray.Length; i++){
            //for every GameObject in objectArray, add the GameObjects with the layer "Enemy" to the ObjectList
            if(objectArray[i].layer == LayerMask.NameToLayer("Enemy")){
                objectList.Add(objectArray[i]);
            }
        }
    }
}
