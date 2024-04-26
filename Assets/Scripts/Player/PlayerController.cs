using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public bool isOn {  get; private set; }

    // Public settings, will be adjustable from the game settings
    public Vector2 mouseSensitivity;
    public Vector2 yClamp;
    
    [SerializeField] new Transform camera;
    [SerializeField] Flashlight flashlight;

    Vector3 playerEuler, cameraEuler;

    [Header("Eyelids")]
    [SerializeField] Animator eyelidsAnimator;

    public Place currentPlace {  get; private set; }
    PlaceTrigger currentPlaceTrigger;
    Door currentDoor;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetControlState(true);

        playerEuler = transform.localEulerAngles;
        cameraEuler = camera.localEulerAngles;

        flashlight.onTurnOn += () => 
        { 
            //Debug.Log("Flashlight turned on");

            if (currentPlace != null && currentDoor != null && currentPlace.animatronic != null)
            //if (currentPlace.Name != "Default")
            {
                if (currentDoor.isOpen)
                {
                    if (currentPlace.animatronic.atLastPhase)
                    {
                        currentPlace.animatronic.Jumpscare();
                    }
                    else
                    {
                        currentPlace.animatronic.ResetJumpscare();
                    }
                }
            }
            //else
            //{
            //    Debug.Log("Can't");
            //}
        };

        flashlight.onTurnOff += () =>
        {
            //Debug.Log("Flashlight turned off");
        };

        //Time.timeScale = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            MouseControl();
            EyelidsControl();
            Interact();
        }
    }

    public void SetControlState(bool state)
    {
        isOn = state;
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
                            if (currentPlaceTrigger != null) currentPlaceTrigger.gameObject.SetActive(true);

                            if (currentDoor != null) currentDoor.Toggle(true);
                            currentDoor = null;

                            currentPlace = place;
                            currentPlaceTrigger = trigger;
                            currentPlaceTrigger.gameObject.SetActive(false);

                            MoveToPlace(place);
                            break;
                        }
                    }
                }
                else if (hit.collider.TryGetComponent(out Door door))
                {
                    currentDoor = door;
                    door.Toggle(!door.isOpen);

                    if (!door.isOpen && currentPlace.animatronic.atLastPhase)
                    {
                        currentPlace.animatronic.ResetJumpscare();
                    }
                    //Debug.Log("Interact with door");
                }
            }  
        }
    }

    void MoveToPlace(Place place)
    {
        StopAllCoroutines();
        StartCoroutine(IEMoveToPlace(place));
    }

    IEnumerator IEMoveToPlace(Place place, float waitTime = 1f)
    {
        SetControlState(false);

        flashlight.TurnLight(false);

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;

            // Animation will be here, replace this stuff
            UIManager.Instance.playerActionPanel.SetActive(true);
            UIManager.Instance.playerActionText.text = "Moving to " + place.Name + ", animation playing"; 

            yield return null;
        }

        transform.position = place.Transform.position;

        UIManager.Instance.playerActionPanel.SetActive(false);
        UIManager.Instance.playerActionText.text = "";

        SetControlState(true);
    }

    public void FlashlightJumpscare()
    {
        

        //if (currentPlace != null && currentPlace.animatronic != null && currentPlace.animatronic.atLastPhase) // At the door or under the bed, whatever it's doing
        //{
        //    currentPlace.animatronic.Jumpscare();
        //}
    }
}