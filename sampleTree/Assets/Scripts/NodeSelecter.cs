using TMPro;
using UnityEngine;

internal class NodeSelecter : MonoBehaviour{

    private void OnMouseDown() {
        EventManager.OnSelectNode(transform.position);
    }
}