using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] new Transform camera;
    [SerializeField][Range(0f, 1f)] float cameraFollowSpeed = 0.5f;
    public KeyCode flashlightKey = KeyCode.Mouse1;
    bool lightOpen = true;
    [SerializeField] float xOffset, yOffset, zOffset;
    new Light light;

    // Start is called before the first frame update
    void Awake()
    {
        light = GetComponentInChildren<Light>();
        //posOffset = transform.position - (camera.position + camera.forward);

        TurnLight(lightOpen);
    }

    // Update is called once per frame
    void Update()
    {
        TrackCamera();

        // Turn on or off
        if (Input.GetKeyDown(flashlightKey))
        {
            TurnLight(!lightOpen);
        }
    }

    void TrackCamera()
    {
        // Follow the camera
        // This is not the correct way to use the Lerp function but whatever, it works as intended
        transform.rotation = Quaternion.Lerp(transform.rotation, camera.rotation, cameraFollowSpeed);
        
        transform.position = Vector3.Lerp(transform.position, camera.position + (camera.forward * zOffset + camera.up * yOffset + camera.right * xOffset), cameraFollowSpeed);
    }

    public void TurnLight(bool state)
    {
        light.enabled = state;
        lightOpen = state;
    }
}