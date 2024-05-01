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
    Vector2 currentSensitivity;

    public Vector2 xClamp {  get; set; }
    public Vector2 yClamp { get; set; }

    public bool smoothClamp = true;
    [Range(0f, 1f)] public float smoothRatio = 0.10f; // 10 percent


    [SerializeField] new Transform camera;
    public Flashlight flashlight;

    Vector3 playerEuler, cameraEuler;

    [Header("Eyelids")]
    [SerializeField] Animator eyelidsAnimator;

    public Place currentPlace {  get; private set; }
    public Door currentDoor { get { return currentPlace.door; } }
    PlaceTrigger currentPlaceTrigger;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetControlState(true);

        currentPlace = GameManager.Places[0]; // Ensure that the default place is at index 0

        playerEuler = transform.localEulerAngles;
        cameraEuler = camera.localEulerAngles;

        xClamp = currentPlace.xClamp;
        yClamp = currentPlace.yClamp;

        currentSensitivity = mouseSensitivity;
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
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");


        if (smoothClamp)
        {          
            float xLeftLimit = xClamp.x - xClamp.x * smoothRatio;
            //Debug.Log("X left limit: " + xLeftLimit);
            float xRightLimit = xClamp.y - xClamp.y * smoothRatio;
            //Debug.Log("X right limit: " + xRightLimit);

            float yUpLimit = yClamp.x - yClamp.x * smoothRatio;
            //Debug.Log("Y up limit: " + yUpLimit);
            float yDownLimit = yClamp.y - yClamp.y * smoothRatio;
            //Debug.Log("Y down limit: " + yDownLimit);

            //Debug.Log("Current X: " + playerEuler.y);
            //Debug.Log("Current sensitivity x: " + currentSensitivity.x);
            
            //Debug.Log("Current Y: " + cameraEuler.x);
            //Debug.Log("Current sensitivity Y: " + currentSensitivity.y);

            // X-Axis (left - right)
            if (playerEuler.y < 0f)
            {
                if (inputX < 0f)
                {
                    currentSensitivity.x = Mathf.Lerp(mouseSensitivity.x, 0f, Mathf.Clamp((playerEuler.y - xLeftLimit) / (xClamp.x - xLeftLimit), 0f, 1f));
                }
                else
                {
                    currentSensitivity.x = mouseSensitivity.x;
                }
            }
            else
            {
                if (-inputX < 0f)
                {
                    currentSensitivity.x = Mathf.Lerp(mouseSensitivity.x, 0f, Mathf.Clamp((playerEuler.y - xRightLimit) / (xClamp.y - xRightLimit), 0f, 1f));
                }
                else
                {
                    currentSensitivity.x = mouseSensitivity.x;
                }
            }

            // Y-Axis (up - down)
            if (cameraEuler.x < 0f) // Camera is inverted. Negative means up, positive means down
            {
                if (-inputY < 0f)
                {
                    currentSensitivity.y = Mathf.Lerp(mouseSensitivity.y, 0f, Mathf.Clamp((cameraEuler.x - yUpLimit) / (yClamp.x - yUpLimit), 0f, 1f));
                }
                else
                {
                    currentSensitivity.y = mouseSensitivity.y;
                }
            }
            else
            {
                if (inputX < 0f)
                {
                    currentSensitivity.y = Mathf.Lerp(mouseSensitivity.y, 0f, Mathf.Clamp((cameraEuler.x - yDownLimit) / (yClamp.y - yDownLimit), 0f, 1f));
                }
                else
                {
                    currentSensitivity.y = mouseSensitivity.y;
                }
            }
        }

        playerEuler.y += inputX * currentSensitivity.x * Time.deltaTime;
        playerEuler.y = Mathf.Clamp(playerEuler.y, xClamp.x, xClamp.y);

        cameraEuler.x += Input.GetAxis("Mouse Y") * currentSensitivity.y * Time.deltaTime;
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
                            //currentPlace.door = null;

                            currentPlace = place;
                            currentPlaceTrigger = trigger;
                            currentPlaceTrigger.gameObject.SetActive(false);

                            xClamp = currentPlace.xClamp;
                            yClamp = currentPlace.yClamp;

                            MoveToPlace(place);
                            break;
                        }
                    }
                }
                else if(currentPlace.door != null)
                {
                    currentDoor.Toggle(!currentDoor.isOpen);

                    if (!currentDoor.isOpen && currentPlace.animatronic.atLastPhase)
                    {
                        currentPlace.animatronic.ResetJumpscare();
                    }
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
}