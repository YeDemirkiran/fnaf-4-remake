using UnityEngine;
using UnityEngine.Serialization;

// THE BASE CLASS FOR ANIMATRONIC LOGIC
// MIGHT WORK ON ITS OWN WITHOUT FURTHER MODIFICATION
// CREATE A NEW SCRIPT AND INHERIT FROM THIS CLASS TO MAKE YOUR OWN ANIMATRONIC

public class Animatronic : MonoBehaviour
{
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
    float jumpscareTimer = 0f, currentJumpscareTime = -1f;

    public bool atLastPhase { get { return currentJumpscareTime != 1f; } }

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
        if (currentActionTime <= actionTimer)
        {
            if (paths.Length <= currentPlaceIndex + 1) // If at the door. Last place should be ALWAYS the door.
            {
                if (currentJumpscareTime == -1f)
                {
                    currentJumpscareTime = Random.Range(jumpscareWaitMinMax.x, jumpscareWaitMinMax.y);
                    //Debug.Log("Jumpscare mode initiated");
                }

                if (jumpscareTimer < currentJumpscareTime)
                {
                    jumpscareTimer += Time.deltaTime;
                }
                else
                {
                    if (PlayerController.Instance.currentPlace == animatronicPlace)
                    {

                    }
                    // Jumpscare baby
                    Jumpscare();
                }
            }
            else // Not at the door, proceed.
            {
                // Action
                float action = Random.Range(0f, 1f) * 100f;

                //Debug.Log("Prob: " + action);

                if (action < movingProbabilities.x)
                {
                    //Debug.Log("Bonnie going forward");

                    currentPlaceIndex += 1;

                }
                else if (action < movingProbabilities.x + movingProbabilities.y)
                {
                    //Debug.Log("Bonnie going backward");
                    currentPlaceIndex -= 1;             
                }
                else
                {
                    //Debug.Log("Bonnie staying");
                }

                currentPlaceIndex = Mathf.Clamp(currentPlaceIndex, 0, paths.Length);

                transform.position = paths[currentPlaceIndex].position;

                //Debug.Log("Time taken: " + actionTimer);

                // Reset timer
                currentActionTime = Random.Range(actionTimeMinMax.x, actionTimeMinMax.y);
                actionTimer = 0f;
            }
        }
        else
        {
            actionTimer += Time.deltaTime;
        }
    }

    public void Jumpscare()
    {
        //if (onlyIfAtDoor)
        //{
        //    if (PlayerController.Instance.currentPlace != animatronicPlace)
        //    {
        //        return;
        //    }
        //}

        Debug.Log("BOOO! Jumpscared");
        this.enabled = false;
        GameManager.Instance.GameFadeOut(2f);
    }

    public void ResetJumpscare()
    {
        actionTimer = jumpscareTimer = 0f;
        currentPlaceIndex = 0;
        currentJumpscareTime = -1f;
    }
}