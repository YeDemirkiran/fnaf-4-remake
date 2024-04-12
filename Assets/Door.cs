using DG.Tweening;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Vector3 openRotation, closedRotation;
    [SerializeField] float openTime = 0.25f;

    public void Open()
    {
        transform.DORotate(openRotation, openTime, RotateMode.Fast);
    }

    public void Close()
    {
        transform.DORotate(closedRotation, openTime, RotateMode.Fast);
    }
}