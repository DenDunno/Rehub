using TMPro;
using UnityEngine;

public class DoctorUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _mass;
    [SerializeField] private TMP_InputField _height;
    [SerializeField] private TMP_InputField _fullName;
    [SerializeField] private TMP_InputField _age;
    [SerializeField] private TMP_Dropdown _sex;
    [SerializeField] private ErrorPanel _errorPanel;
    
    public void TryAcceptInput()
    {
        if (ValidateInput())
        {
            Debug.Log("OK");
            float mass = float.Parse(_mass.text);
            float height = float.Parse(_height.text);
            float age = int.Parse(_age.text);
            string fullName = _fullName.text;
            Sex sex = (Sex)_sex.value;
        }
    }

    private bool ValidateInput()
    {
        return ValidateFloat(_mass.text, "Invalid mass") &&
               ValidateFloat(_height.text, "Invalid height") &&
               ValidateInt(_age.text, "Invalid age");
    }

    private bool ValidateInt(string input, string errorMessage)
    {
        bool isValid = int.TryParse(input, out int value) && value > 0;
        
        if (isValid == false)
        {
            _errorPanel.Show(errorMessage);
        }

        return isValid;
    }
    
    private bool ValidateFloat(string input, string errorMessage)
    {
        bool isValid = float.TryParse(input, out float value) && value > 0;

        if (isValid == false)
        {
            _errorPanel.Show(errorMessage);
        }

        return isValid;
    }
}