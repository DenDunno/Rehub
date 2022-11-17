using UnityEngine;

public class PencilInput : MonoBehaviour
{
    [SerializeField] private Whiteboard _whiteboard;

    private void OnTriggerStay(Collider other)
    {
        _whiteboard.TryAddPosition(transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        _whiteboard.Clear();
    }
}