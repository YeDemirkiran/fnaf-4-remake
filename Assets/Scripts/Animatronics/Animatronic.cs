using UnityEngine;
using UnityEngine.Serialization;

// THE BASE CLASS FOR ANIMATRONIC LOGIC
// MIGHT WORK ON ITS OWN WITHOUT FURTHER MODIFICATION
// CREATE A NEW SCRIPT AND INHERIT FROM THIS CLASS TO MAKE YOUR OWN ANIMATRONIC

public class Animatronic : MonoBehaviour
{
    public new string name;

    [SerializeField] Transform[] paths;

    public Place animatronicPlace { get; set; }

    int currentPlaceIndex;

    public float difficulty = 1f;
    public float probabilityChangePerDifficulty = 2f;

    [SerializeField] Vector2 actionTimeMinMax;
    float actionTimer = 0f, currentActionTime;

    [SerializeField] [FormerlySerializedAs("_probabilities")] Vector2 _movingProbabilities; // x = Probability of going forward, y = Probability of going back
                                                                                            // Probability of staying is calculated with those values' sum
    Vector3 movingProbabilities;

    [SerializeField] Vector2 jumpscareWaitMinMax; // How much time should it wait in the door before jumpscaring
    float jumpscareTimer = -1f, currentJumpscareTime = -1f;

    public bool atLastPhase { get { return currentJumpscareTime != -1f; } }

    // Start is called before the first frame update
    protected void Initialize()
    {
        difficulty = Mathf.Abs(difficulty);

        movingProbabilities = _movingProbabilities;
        movingProbabilities.x = (movingProbabilities.x + difficulty * probabilityChangePerDifficulty) % 100f;
        movingProbabilities.y = (movingProbabilities.y - difficulty * probabilityChangePerDifficulty / 2f) % 100f;
        //probabilities.y = (probabilities.y / Mathf.Sqrt(difficulty) % 100f);
        movingProbabilities.z = 100f - movingProbabilities.x - movingProbabilities.y;

        //Debug.Log("Probabilities");
    }

    // Update is called once per frame
    protected void AnimatronicLoop()
    {
        PlayerController player = PlayerController.Instance;

        bool playerAtPlace = player.currentPlace == animatronicPlace;
        bool playerLightOpen = player.flashlight.lightOpen;
        bool placeDoorOpen = animatronicPlace.door.isOpen;

        bool playerArmedAtDoor = playerAtPlace &&
                        playerLightOpen
                        && placeDoorOpen; // If the player is at this animatronics' door while the door is open and the light is on

        //Debug.Log("Player armed at door: " + (jumpscareTimer > currentJumpscareTime || playerArmedAtDoor));

        if (paths.Length <= currentPlaceIndex + 1) // If the animatronic is at the door. Last transform in the path should be ALWAYS the door.
        {
            //Debug.Log("0");

            if (currentJumpscareTime == -1f)
            {
                //Debug.Log("0.01");

                currentJumpscareTime = Random.Range(jumpscareWaitMinMax.x, jumpscareWaitMinMax.y);
                jumpscareTimer = 0f;
                Debug.Log($"({name}): Jumpscare mode initiated");
            }

            if (jumpscareTimer > currentJumpscareTime)
            {
                if (playerArmedAtDoor || (player.currentPlace == GameManager.Places[0]))
                {
                    Jumpscare();
                    //Debug.Log("2");
                }
                else
                {
                    Debug.Log($"({name}): Player is either at door without flashlight being open, or not at the default place");
                }
            }

            else
            {
                //Debug.Log("1");
                jumpscareTimer += Time.deltaTime;
            }
        }
        else
        {
            if (currentActionTime <= actionTimer)
            {
                if (!playerArmedAtDoor) // Can't move if the player is at the door with flashlight open
                {
                    // Action
                    float action = Random.Range(0f, 1f) * 100f;

                    Debug.Log($"({name}): Probability: " + action);

                    if (action < movingProbabilities.x)
                    {
                        Debug.Log($"({name}): Going forward");

                        currentPlaceIndex += 1;

                    }
                    else if (action < movingProbabilities.x + movingProbabilities.y)
                    {
                        Debug.Log($"({name}): Going backward");
                        currentPlaceIndex -= 1;
                    }
                    else
                    {
                        Debug.Log($"({name}): Staying");
                    }

                    currentPlaceIndex = Mathf.Clamp(currentPlaceIndex, 0, paths.Length);

                    transform.position = paths[currentPlaceIndex].position;

                    //Debug.Log("Time taken: " + actionTimer);

                    // Reset timer
                    currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
                    actionTimer = 0f;
                }
                else
                {
                    ResetJumpscare();
                }
            }
            else
            {
                actionTimer += Time.deltaTime;
            }
        }
    }

    public void Jumpscare()
    {
        Debug.Log($"({name}): Jumpscared");
        this.enabled = false;
        GameManager.Instance.GameFadeOut(2f);
    }

    public void ResetJumpscare()
    {
        actionTimer = 0f;
        jumpscareTimer = -1f;
        currentPlaceIndex = 0;
        transform.position = paths[currentPlaceIndex].position;
        currentJumpscareTime = -1f;
    }
}