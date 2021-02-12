using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour{
    [SerializeField] public TextMeshProUGUI[] inputNumberListeners;
    public void UpdateInputNumber(int number) {
        foreach (var box in inputNumberListeners) {
            box.text = number.ToString();
        }
    }
}