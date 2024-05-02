using UnityEngine;
using UnityEngine.Events;

public class Flashlight : MonoBehaviour
{
    [SerializeField] new Transform camera;
    [SerializeField][Range(0f, 1f)] float cameraFollowSpeed = 0.5f;
    public KeyCode flashlightKey = KeyCode.Mouse1;
    public bool lightOpen { get { return light.enabled; } }
    public bool controlEnabled { get; set; } = true;
    [SerializeField] float xOffset, yOffset, zOffset;
    new Light light;

    public UnityAction onTurnOn, onTurnOff;

    void Awake()
    {

        light = GetComponentInChildren<Light>();
    }

    void Start()
    {
        TurnLight(lightOpen);
    }

    void Update()
    {
        TrackCamera();

        // Turn on or off
        if (controlEnabled && Input.GetKeyDown(flashlightKey))
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

        if (onTurnOn != null && onTurnOff != null)
        {
            if (state) onTurnOn.Invoke();
            else onTurnOff.Invoke();
        }
    }
}