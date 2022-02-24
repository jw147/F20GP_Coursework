using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunController : MonoBehaviour
{
    // gameStart boolean
    public bool gameStart = false;

    // GameObjects and AudioSources necessary for accessing scripts and playing the correct audio
    public GameObject Camera;
    public GameObject Player;
    private AudioSource soundPlayer;

    public GameObject StartMusic;
    private AudioSource soundMusic;

    public GameObject LobbyMusic;
    private AudioSource soundLobby;

    // a list of GameObjects that will be filled with zombies hit with RayCasts
    public List<GameObject> objectList = new System.Collections.Generic.List<GameObject>();
    

    // Gun animation and audio variables
    AudioSource shoot;
    Animator animGun;
    
    // Gong GameObject, animation and audio variables
    [Header("Gong Settings")]
    public GameObject Gong;
    Animator animGong;
    private AudioSource soundGong;


    // Animation hash variables
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

    // Variables necessary for the Victory screen
    public Image Victory;
    public Sprite victorySprite;
    
    //Zombie left count
    int zombiesLeft = 15;
    // Booleans containing the win and lose states
    bool win = false;
    bool lose = false;


    public void Start(){
        // call the FindAllZombies() function on start
        FindAllZombies();

        // When the game starts, set the current ammo in the player's clip to clipSize and allow them to shoot
        _currentAmmoInClip = clipSize;
        _canShoot = true;
        // Edit the UI with the correct information
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
        // play lobby music
        soundLobby.Play();
    }

    private void Update(){
        //if the game has not been won or lost
        if(win == false && lose == false){
            //if the game has started
            if (gameStart == true)
            {
                //Clear start prompt and show the amount of Zombies left in the area
                startPromptText.text = "";
                zombiesLeftText.text = "Zombies Left: " + zombiesLeft.ToString();
            }
            // If the player clicks the left mouse button once, _canShoot is true and there is ammo in the player's clip:
            if (Input.GetMouseButton(0) && _canShoot && _currentAmmoInClip > 0)
            {
                // Set _canShoot to false, decrement the amount of ammo in the clip by one, display the current ammo to the UI and call a Coroutine called ShootGun
                _canShoot = false;
                _currentAmmoInClip--;
                ammoText.text = _currentAmmoInClip.ToString();
                StartCoroutine(ShootGun());
                shoot.Play();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                //If the R key is pressed, call the reload Coroutine
                StartCoroutine(Reload());
            }

        }
        //if there are no zombies left
        if(zombiesLeft == 0){
            //display to the user the zombies left are 0
            zombiesLeftText.text = "Zombies Left: " + zombiesLeft.ToString();
            // set the game as won
            win = true;
            //set the Victory image to the sprite assigned to this object
            Victory.sprite = victorySprite;
            Victory.color = Color.white;
            //stop the music
            soundMusic.Stop();
            //tell the other scripts that the game is won disabling movement and looking
            var Win = Player.GetComponent<PlayerMovement>();
            Win.Win();
            var Win2 = Camera.GetComponent<MouseLook>();
            Win2.Win();
        }
    }
    
    void RayCast(){
        // New RaycastHit array holding all of the RayCast hits
        RaycastHit[] hits;
        //set hits to all objects hit infront of the player with an infinite distance
        hits = Physics.RaycastAll(transform.parent.position, transform.parent.forward, Mathf.Infinity);
        //zombiesHit variable
        int zombiesHit = 0;
        //for every hit
        for(int x = 0; x < hits.Length; x++){
            //set hit to be this instance of hits
            RaycastHit hit = hits[x];
            //if the player has shot the gong
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
                //this is limited to 3 zombies at once
                var EH = hit.collider.gameObject.GetComponent<ZombieScript>();
                EH.getShot();
                zombiesHit++;
            }else{}
        } 
    }

    public void minusZombie(){
        //function called by another script to decrement the amount of zombies in this script
        zombiesLeft--;
    }
    public void Lose(){
        //function called by another script to set the state of the game to a loss
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
        //play the reload sound
        soundReload.Play();
        //do not allow the user to shoot
        _canShoot = false;
        //calculate the amount of ammo to take away from the reserve ammunition and to put into the current clip
        int maxClipSize = 12;
        reserveAmmo = reserveAmmo - (maxClipSize - _currentAmmoInClip);
        _currentAmmoInClip = maxClipSize;
        //wait 3 seconds
        yield return new WaitForSeconds(3f);
        //update the UI and allow the user to shoot
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
