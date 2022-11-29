using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ErrorPanel : MonoBehaviour
{
    [ChildGameObjectsOnly] [SerializeField] private TMP_Text _errorMessage;
    
    public void Show(string errorMessage)
    {
        _errorMessage.text = errorMessage;
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}