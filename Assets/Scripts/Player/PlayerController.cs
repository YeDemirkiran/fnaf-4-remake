using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isOn {  get; private set; }

    // Public settings, will be adjustable from the game settings
    public Vector2 mouseSensitivity;
    public Vector2 yClamp;
    
    [SerializeField] new Transform camera;

    Vector3 playerEuler, cameraEuler;

    [Header("Eyelids")]
    [SerializeField] Animator eyelidsAnimator;

    public static Place currentPlace {  get; private set; }
    PlaceTrigger currentPlaceTrigger;
    Door currentDoor;

    // Start is called before the first frame update
    void Start()
    {
        SetControlState(true);

        playerEuler = transform.localEulerAngles;
        cameraEuler = camera.localEulerAngles;

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

                            if (currentDoor != null) currentDoor.Toggle(false);
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