using UnityEngine;

// * Enumeration values for the 6 movement directions of the camera.
public enum CameraButton {
    Up,
    Down,
    Left,
    Right,
    In,
    Out,
    Null
}

// Class which controls the movement of the camera in the simulation.
public class CameraMovement : MonoBehaviour
{

    public float speed;         // Current pan speed of the camera.
    public Transform map;       // Dimentions of the map.

    public float zLimits;       // Top and Bottom limits.
    public float xLimits;       // Left and Right limits.
    public float inLimit;       // Zoom in limit.
    public float outLimit;      // Zoom out limit.
    public float defaultZoom;   // Starting zoom (y) position of the camera.
    public float defaultSpeed;  // Starting pan speed of the camera.

    public CameraButton mode;   // Current button pressed.

    // Start is called at the start of the game.
    private void Start() {
        // Set the pan limits of the camera.
        zLimits = (map.localScale.z / 2);
        xLimits = (map.localScale.x / 2);
        inLimit = transform.position.y / 2;
        outLimit = transform.position.y * 4;
        // Set starting values of the camera.
        defaultZoom = transform.position.y;
        defaultSpeed = speed;
        // Set the starting camera input to nothing.
        mode = CameraButton.Null;
    }

    // Update is called once per frame
    void Update()
    {
        // Change camera speed depending on current zoom.
        speed = (transform.position.y / defaultZoom) * defaultSpeed;
        // Limit the movement to within the bounds of the map.
        switch (mode) {
            case CameraButton.Up: MoveUp(); if (Mathf.Abs(transform.position.z) + speed >= zLimits) { MoveDown(); } break;
            case CameraButton.Down: MoveDown(); if (Mathf.Abs(transform.position.z) + speed >= zLimits) { MoveUp(); } break;
            case CameraButton.Left: MoveLeft(); if (Mathf.Abs(transform.position.x) + speed >= xLimits) { MoveRight(); } break;
            case CameraButton.Right: MoveRight(); if (Mathf.Abs(transform.position.x) + speed >= xLimits) { MoveLeft(); } break;
            case CameraButton.In: MoveIn(); if (Mathf.Abs(transform.position.y) + speed <= inLimit) { MoveOut(); } break;
            case CameraButton.Out: MoveOut(); if (Mathf.Abs(transform.position.y) + speed >= outLimit) { MoveIn(); } break;
        }
    }

    // Eventlistener for each button.
    public void UpPressed() { mode = CameraButton.Up; }
    public void DownPressed() { mode = CameraButton.Down; }
    public void LeftPressed() { mode = CameraButton.Left; }
    public void RightPressed() { mode = CameraButton.Right; }
    public void InPressed() { mode = CameraButton.In; }
    public void OutPressed() { mode = CameraButton.Out; }
    // Reset mode upon button release.
    public void ButtonRelease() { mode = CameraButton.Null; }

    // Move the camera north.
    public void MoveUp() { transform.position = transform.position - new Vector3(0, 0, speed); }
    // Move the camera south.
    public void MoveDown() { transform.position = transform.position + new Vector3(0, 0, speed); }
    // Move the camera west.
    public void MoveLeft() { transform.position = transform.position + new Vector3(speed, 0, 0); }
    // Move the camera east.
    public void MoveRight() { transform.position = transform.position - new Vector3(speed, 0, 0); }
    // Zoom the camera in.
    public void MoveIn() { transform.position = transform.position - new Vector3(0, speed, 0); }
    // Zoom the camera out.
    public void MoveOut() { transform.position = transform.position + new Vector3(0, speed, 0); }

}
