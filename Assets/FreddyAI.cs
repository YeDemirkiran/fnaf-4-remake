/* 

-- FREDDY AI RULES --

Every x seconds, there is a %y chance of a Freddle spawning at the tunnel.
Freddle Path: Tunnel > Under Bed > On Bed
At least 2 Freddles are needed for one of them to leave under the bed
You need 5 Freddles on top of the bed to form a Freddy
Each Freddle on the bed increases the chance of the rest's movement probabilites
You need 10 seconds to distrupt those 5 Freddles before they form Freddy

*/

using System.Collections.Generic;
using UnityEngine;

public class FreddyAI : MonoBehaviour
{
    [SerializeField] int minFreddlesForUnderBed = 2;
    [SerializeField] int freddlesToFormFreddy = 5;
    [SerializeField] float freddleCreateProbability = 20f;
    [SerializeField] float jumpscareTime = 10f, actionTime = 5f;

    [SerializeField] Transform[] path;
    [SerializeField] GameObject freddlePrefab;
    List<FreddleAI> freddleList = new List<FreddleAI>();

    float jumpscareTimer, actionTimer;

    // Update is called once per frame
    void Update()
    {
        // If there are at least 2 Freddles under the bed
        // But this breaks if all the Freddles are created and all of them are on top of the bed
        // So the last one should be able to move if the Freddles count is 5
        FreddleAI.canMove = (FreddleCountOnIndex(1) >= minFreddlesForUnderBed) || (freddleList.Count >= freddlesToFormFreddy);

        if (FreddleCountOnIndex(path.Length - 1) >= freddlesToFormFreddy)
        {
            if (jumpscareTimer >= jumpscareTime)
            {
                Jumpscare();
            }
            else
            {
                jumpscareTimer += Time.deltaTime;
            }
        }
        else if (freddleList.Count < 5)
        {
            if (actionTimer >= actionTime)
            {
                float prob = Random.Range(0f, 100f);

                if (prob < freddleCreateProbability)
                {
                    CreateFreddle();
                }

                actionTimer = 0f;
            }
            else
            {
                actionTimer += Time.deltaTime;
            }
        }
    }

    void CreateFreddle()
    {
        GameObject gameObject = Instantiate(freddlePrefab);
        FreddleAI freddle = gameObject.GetComponent<FreddleAI>();

        freddle.path = path;
        freddle.Initialize();

        freddleList.Add(freddle);

        freddle.transform.parent = transform;
    }

    int FreddleCountOnIndex(int index) // Go check the path and enter the correct value
    {
        int count = 0;

        foreach (FreddleAI freddle in freddleList)
        {
            if (freddle.currentPlaceIndex == index)
            {
                count++;
            }
        }

        return count;
    }

    void Jumpscare()
    {
        Debug.Log("Freddy jumpscare!");

        // Jumpscare logic
        GameManager.Instance.GameFadeOut(1f);

        // Clean
        foreach (FreddleAI freddle in freddleList)
        {
            Destroy(freddle.gameObject);
        }

        freddleList.Clear();

        enabled = false;
    }
}