using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeDeleter : MonoBehaviour {
    public TextMeshProUGUI textMeshProUGUI;

    private void OnMouseDown() {
        EventManager.OnDeleteNode(transform.position, textMeshProUGUI);
    }
}