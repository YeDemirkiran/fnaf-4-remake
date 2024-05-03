using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreddleAI : MonoBehaviour
{
    public Transform[] path { get; set; }
    public int currentPlaceIndex { get; private set; }

    static bool _canMove;
    public static bool canMove // To be used by Freddy AI
    { 
        get 
        { 
            return _canMove; 
        } 

        set 
        { 
            _canMove = value;
        } 
    }

    public bool flashed { get; set; } = false;

    [SerializeField] float flashlightTimeToRevert = 2f;

    [SerializeField] float actionTime = 5f;
    float actionTimer, flashTimer;

    // Update is called once per frame
    void Update()
    {
        if (flashed) // Player currently flashing
        {
            if (flashTimer >= flashlightTimeToRevert)
            {
                currentPlaceIndex--;
                currentPlaceIndex = Mathf.Clamp(currentPlaceIndex, 0, path.Length - 1);
                flashed = false;

                MoveBackward();
            }
            else
            {
                flashTimer += Time.deltaTime;
            }
        }
        else
        {
            if (currentPlaceIndex == 0 || canMove) // Can move out of the first place, but can't move further if canMove is false
            {
                if (actionTimer >= actionTime)
                {
                    float action = Random.Range(0f, 100f);

                    if (action < 20f) // Move forward
                    {
                        currentPlaceIndex++;
                        currentPlaceIndex = Mathf.Clamp(currentPlaceIndex, 0, path.Length - 1);

                        MoveForward();
                    }

                    actionTimer = 0f;
                }
                else
                {
                    actionTimer += Time.deltaTime;
                }
            }
            else
            {
                actionTimer = 0f;
            }
        }
        
    }

    public void Initialize() // Call when instantiating
    {
        currentPlaceIndex = 0;
        transform.position = path[0].position;
        actionTimer = 0f;
        flashTimer = 0f;
        canMove = true;
        flashed = false;
    }

    void MoveForward()
    {
        // Animation yet to be implemented
        // For now, just change position

        transform.position = path[currentPlaceIndex].position;
    }

    void MoveBackward()
    {
        // Animation yet to be implemented
        // For now, just change position

        transform.position = path[currentPlaceIndex].position;
    }
}