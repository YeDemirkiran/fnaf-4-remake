using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public bool isOn {  get; private set; }

    [SerializeField] Animator animator;

    // Private
    Vector3 playerEuler, cameraEuler;
    bool holdingDoor = false;
    Vector2 currentSensitivity;

    // Public variables, will be adjustable from the game settings
    [Header("Controller")]
    public Vector2 mouseSensitivity;

    public Vector2 xClamp {  get; set; }
    public Vector2 yClamp { get; set; }

    public bool smoothClamp = true;
    [Range(0f, 1f)] public float smoothRatio = 0.10f; // 10 percent

    [SerializeField] new Transform camera;
    public Flashlight flashlight; 

    [Header("Eyelids")]
    [SerializeField] Animator eyelidsAnimator;

    public Place currentPlace {  get; private set; }
    public Door currentDoor { get { return currentPlace.door; } }
    PlaceTrigger currentPlaceTrigger;

    

    [Header("Animation")]

    [SerializeField] float animationLerpSpeed = 5f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetControlState(true);
        animator.enabled = false;

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

    public void SetControlState(bool state, bool setAnimator = true)
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

                            currentPlaceTrigger = trigger;
                            currentPlaceTrigger.gameObject.SetActive(false);

                            StopAllCoroutines();

                            MoveToPlace(place);
                            break;
                        }
                    }
                }
                else if(currentPlace.door != null)
                {
                    if (!holdingDoor)
                    {
                        holdingDoor = true;
                        currentDoor.Toggle(false);

                        flashlight.TurnLight(false);
                        flashlight.controlEnabled = false;
                    }                   
                }
            }  
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (holdingDoor)
            {
                holdingDoor = false;
                currentDoor.Toggle(true);
                flashlight.controlEnabled = true;
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

        // Let me explain what's going on here
        // I couldn't find another way to get the rotation information from the first frame of an animation
        // To make a smooth lerp into the animation, we first play the animation which updates the Camera euler angles
        // Then we stop the animation, lerp to that angle information we got and then start the animation again when the lerp is done

        animator.enabled = true; 

        string tag = currentPlace.Name + " to " + place.Name;
        animator.Play(tag, 0, 0f);

        // Wait for a frame and let the animator update the transforms
        yield return null;

        // Store the target rotations
        Quaternion bodyTarget = transform.localRotation;
        Quaternion cameraTarget = camera.localRotation;

        // Reset animators
        animator.enabled = false;
        transform.localEulerAngles = playerEuler;
        camera.localEulerAngles = cameraEuler;

        // Store initials for lerping
        Quaternion bodyInitial = transform.localRotation;
        Quaternion cameraInitial = camera.localRotation;

        float lerp = 0f;

        while (lerp < 1f) 
        {
            transform.localRotation = Quaternion.Slerp(bodyInitial, bodyTarget, lerp);
            camera.localRotation = Quaternion.Slerp(cameraInitial, cameraTarget, lerp);
            lerp += Time.deltaTime * animationLerpSpeed;
            yield return null;
        }

        flashlight.TurnLight(false);

        animator.enabled = true;
        animator.Play(tag, 0, 0f);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            // Animation will be here, replace this stuff
            //UIManager.Instance.playerActionPanel.SetActive(true);
            //UIManager.Instance.playerActionText.text = "Moving to " + place.Name + ", animation playing"; 
            yield return null;
        }
      
        animator.enabled = false;

        currentPlace = place;

        xClamp = currentPlace.xClamp;
        yClamp = currentPlace.yClamp;

        Debug.Log("player before: " + playerEuler);
        Debug.Log("camera before: " + cameraEuler);
        transform.position = place.Transform.position;
        playerEuler = transform.localEulerAngles;
        cameraEuler = camera.localEulerAngles;
        Debug.Log("player after: " + playerEuler);
        Debug.Log("camera after: " + cameraEuler);

        SetControlState(true);

        UIManager.Instance.playerActionPanel.SetActive(false);
        UIManager.Instance.playerActionText.text = "";
    }
}