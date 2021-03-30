using TMPro;
using UnityEngine;

internal class NodeSelecter : MonoBehaviour{

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0))
            EventManager.OnSelectNode(transform.position);
        if (Input.GetMouseButtonDown(1)) {
            print("hello");
            EventManager.OnNewEdgeWith(transform.position);
        }
    }
}