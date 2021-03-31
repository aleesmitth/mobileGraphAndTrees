using TMPro;
using UnityEngine;

internal class NodeSelecter : MonoBehaviour{

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0))
            EventManager.OnSelectNode(transform.position);
        if (Input.GetMouseButtonDown(1)) {
            EventManager.OnNewEdgeWith(transform.position);
        }
    }
}