using UnityEngine;

public class ChicaAI : Animatronic
{
    // Start is called before the first frame update
    void Start()
    {
        base.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        base.AnimatronicLoop();
    }
}