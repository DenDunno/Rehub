using UnityEngine;

public class RoundAnimation : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _angularSpeed = 1f;
    [SerializeField] private float _circleRad = 1f;
 
    private float _currentAngle;

    private void Update ()
    {
        _currentAngle += _angularSpeed * Time.deltaTime;
        Vector3 offset = new Vector3 (Mathf.Sin (_currentAngle), 0, Mathf.Cos (_currentAngle)) * _circleRad;
        transform.position = _target.position + offset;
        transform.LookAt(_target);
    }
}
