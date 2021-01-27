using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeDeleter : MonoBehaviour {
    public TextMeshProUGUI textMeshProUGUI;
    public void DeleteNode() {
        EventManager.OnDeleteNode(transform.position, textMeshProUGUI);
    }
}
