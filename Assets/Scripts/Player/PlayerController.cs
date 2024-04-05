using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public settings, will be adjustable from the game settings
    public Vector2 mouseSensitivity;
    public Vector2 yClamp;
    
    [SerializeField] new Transform camera;

    Vector3 playerEuler, cameraEuler;

    // Start is called before the first frame update
    void Start()
    {
        playerEuler = transform.localEulerAngles;
        cameraEuler = camera.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        playerEuler.y += Input.GetAxis("Mouse X") * mouseSensitivity.x * Time.deltaTime;

        cameraEuler.x += Input.GetAxis("Mouse Y") * mouseSensitivity.y * Time.deltaTime;
        cameraEuler.x = Mathf.Clamp(cameraEuler.x, yClamp.x, yClamp.y);

        transform.localEulerAngles = playerEuler;
        camera.localEulerAngles = cameraEuler;
    }
}