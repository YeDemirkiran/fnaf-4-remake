using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType { Hinge, Slide }
    [SerializeField] DoorType type;

    [SerializeField] Vector3 openRotation, closedRotation; // For hinge doors
    [SerializeField] Vector3 openPosition, closedPosition; // For slide doors
    [SerializeField] float moveTime = 0.25f;

    public bool isOpen { get; private set; }

    private void Awake()
    {
        Toggle(false);
    }

    public void Toggle(bool state, float time = -1f)
    {
        if (time < 0f)
        {
            time = moveTime;
        }

        if (type == DoorType.Hinge)
        {
            transform.DORotate(state ? openRotation : closedRotation, time, RotateMode.Fast);
        }
        else
        {
            transform.DOMove(state ? openPosition : closedPosition, time);
        }
        isOpen = state;
    }
}