using DG.Tweening;
using System;
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

    Place currentPlace;

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
        Interact();
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
            eyelidsAnimator.SetBool("eyelidsShut", true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            eyelidsAnimator.SetBool("eyelidsShut", false);
        }

        //eyelidsShut = Input.GetKey(KeyCode.Mouse2);
        //eyelidsAnimator.SetBool("eyelidsShut", eyelidsShut);
    }

    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.TryGetComponent(out PlaceTrigger trigger))
                {
                    foreach (Place place in GameManager.Places)
                    {
                        if (place.Name == trigger.placeName)
                        {
                            currentPlace = place;
                            MoveToPlace(place);
                            break;
                        }
                    }
                }
            }  
        }
    }

    void MoveToPlace(Place place)
    {
        transform.position = place.Position;
    }
}