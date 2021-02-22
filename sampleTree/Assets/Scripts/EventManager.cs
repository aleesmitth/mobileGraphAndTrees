using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventManager {
    public static event Action<Vector3, TextMeshProUGUI> onDeleteNode;
    public static event Action<Vector3> onSelectNode;

    public static void OnDeleteNode(Vector3 position, TextMeshProUGUI textMeshProUGUI) {
        onDeleteNode?.Invoke(position, textMeshProUGUI);
    }
    
    public static void OnSelectNode(Vector3 position) {
        onSelectNode?.Invoke(position);
    }
}