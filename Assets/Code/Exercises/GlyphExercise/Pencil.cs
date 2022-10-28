using UnityEngine;

public class Pencil : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    private Whiteboard _whiteboard;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _whiteboard) == false)
        {
            Debug.LogError($"No Whiteboard component on {other.gameObject}");
        }
        else
        {
            _rigidbody.freezeRotation = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        _whiteboard.TryAddPosition(transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        _whiteboard.Clear();
        _rigidbody.freezeRotation = false;
    }
}