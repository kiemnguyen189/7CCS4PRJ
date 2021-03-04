using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{

    public float speed;
    public Transform map;

    public float zLimits;   // Top and Bottom limits.
    public float xLimits;   // Left and Right limits.
    public float inLimit;   // Zoom in limit.
    public float outLimit;  // Zoom out limit.

    public bool upPressed = false;
    public bool downPressed = false;
    public bool leftPressed = false;
    public bool rightPressed = false;

    public bool inPressed = false;
    public bool outPressed = false;

    private void Start() {
        zLimits = (map.localScale.z / 2);
        xLimits = (map.localScale.x / 2);
        inLimit = transform.position.y / 2;
        outLimit = transform.position.y * 4;
    }

    // Update is called once per frame
    void Update()
    {

        // Limit the movement to within the bounds of the map.
        if (upPressed) { 
            MoveUp(); 
            if (Mathf.Abs(transform.position.z) + speed >= zLimits) { MoveDown(); }
        }
        if (downPressed) { 
            MoveDown(); 
            if (Mathf.Abs(transform.position.z) + speed >= zLimits) { MoveUp(); }
        }
        if (leftPressed) { 
            MoveLeft(); 
            if (Mathf.Abs(transform.position.x) + speed >= xLimits) { MoveRight(); }
        }
        if (rightPressed) { 
            MoveRight(); 
            if (Mathf.Abs(transform.position.x) + speed >= xLimits) { MoveLeft(); }
        }
        if (inPressed) { 
            MoveIn(); 
            if (Mathf.Abs(transform.position.y) + speed <= inLimit) { MoveOut(); }
        }
        if (outPressed) { 
            MoveOut(); 
            if (Mathf.Abs(transform.position.y) + speed >= outLimit) { MoveIn(); }
        }

        
    }

    public void UpPressed() { upPressed = true; }
    public void UpReleased() { upPressed = false; }

    public void DownPressed() { downPressed = true; }
    public void DownReleased() { downPressed = false; }

    public void LeftPressed() { leftPressed = true; }
    public void LeftReleased() { leftPressed = false; }

    public void RightPressed() { rightPressed = true; }
    public void RightReleased() { rightPressed = false; }

    public void InPressed() { inPressed = true; }
    public void InReleased() { inPressed = false; }

    public void OutPressed() { outPressed = true; }
    public void OutReleased() { outPressed = false; }

    public void MoveUp() { transform.position = transform.position - new Vector3(0, 0, speed); }

    public void MoveDown() { transform.position = transform.position + new Vector3(0, 0, speed); }

    public void MoveLeft() { transform.position = transform.position + new Vector3(speed, 0, 0); }

    public void MoveRight() { transform.position = transform.position - new Vector3(speed, 0, 0); }

    public void MoveIn() { transform.position = transform.position - new Vector3(0, speed, 0); }

    public void MoveOut() { transform.position = transform.position + new Vector3(0, speed, 0); }

}
