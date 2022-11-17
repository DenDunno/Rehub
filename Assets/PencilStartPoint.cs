using UnityEngine;

public class PencilStartPoint : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void ResetPencil() // on end grab 
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;
    }
}