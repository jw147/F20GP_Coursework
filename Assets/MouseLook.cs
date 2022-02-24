using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    // set mouse sensitivity
    public float mouseSensitivity = 100f;
    // link camera movement to the player object
    public Transform playerBody;

    float xRotation = 0f;

    public bool win = false;
    public bool lose = false;

    // Start is called before the first frame update
    void Start()
    {
        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(win == false && lose == false){
            // Take mouse input by setting mouseX and mouseY variables to the current x and y location of the mouse * the sensitivity *
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            // Rotate the camera up and down
            xRotation -= mouseY;
            //do not allow the player to look behind itself from up and down rotations by Clamping
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Rotate left and right
            playerBody.Rotate(Vector3.up * mouseX);
        }

    }
    public void Win(){
        //called by another script to determine the state of the game as a win
        win = true;
    }

    public void Lose(){
        //called by another script to determine the state of the game as a loss
        lose = true;
    }
}
