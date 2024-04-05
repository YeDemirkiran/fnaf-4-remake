using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public settings, will be adjustable from the game settings
    public Vector2 mouseSensitivity;
    public Vector2 yClamp;
    
    [SerializeField] new Transform camera;

    Vector3 playerEuler, cameraEuler;

    [Header("Eyelids")]
    [SerializeField] Animator eyelidsAnimator;
    bool eyelidsShut = false;

    // Start is called before the first frame update
    void Start()
    {
        playerEuler = transform.localEulerAngles;
        cameraEuler = camera.localEulerAngles;

        //Time.timeScale = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        MouseControl();
        EyelidsControl();
    }

    void MouseControl()
    {
        playerEuler.y += Input.GetAxis("Mouse X") * mouseSensitivity.x * Time.deltaTime;

        cameraEuler.x += Input.GetAxis("Mouse Y") * mouseSensitivity.y * Time.deltaTime;
        cameraEuler.x = Mathf.Clamp(cameraEuler.x, yClamp.x, yClamp.y);

        transform.localEulerAngles = playerEuler;
        camera.localEulerAngles = cameraEuler;
    }

    void EyelidsControl()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            eyelidsShut = true;
            eyelidsAnimator.SetBool("eyelidsShut", true);

            Debug.Log("Eyelids shut!");
        }
        else if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            eyelidsShut = false;

            Debug.Log("Eyelids open!");
            eyelidsAnimator.SetBool("eyelidsShut", false);
        }

        //eyelidsShut = Input.GetKey(KeyCode.Mouse2);
        //eyelidsAnimator.SetBool("eyelidsShut", eyelidsShut);
    }
}