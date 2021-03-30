using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDrawer : MonoBehaviour {
    public GameObject iconPrefab;
    public Sprite arrowBodySprite;
    public Sprite arrowHeadSprite;
    public Sprite greenArrowBodySprite;
    public Sprite greenArrowHeadSprite;
    private Dictionary<Vector2, ArrowContainer> arrowContainers;

    private void Awake() {
        arrowContainers = new Dictionary<Vector2, ArrowContainer>();
    }

    public void DrawArrow(Vector2 initialPosition, Vector2 finalPosition) {
        GameObject arrowBodyGO = Instantiate(iconPrefab, gameObject.transform, false);
        Icon arrowBody = arrowBodyGO.GetComponent<Icon>();
        arrowBody.SetArtworkSprite(arrowBodySprite);
        
        Vector2 centerPosition = (initialPosition + finalPosition) / 2f;
        arrowBodyGO.transform.position = centerPosition;
        Vector2 direction = (finalPosition - initialPosition).normalized;
        arrowBodyGO.transform.right = direction;
        Vector3 scale = new Vector3(1,1,1);
        //no entiendo bien por que estas divisiones, mas q nada lo de "math.max" entre los bounds, pero sino no anda bien.
        //si solo divido por el size en x por ejemplo, cuando pongo un nodo nuevo en eje y, anda mal
        //viceversa con nodo en x tomando size en y
        scale.x = Mathf.Abs((initialPosition - finalPosition).magnitude) /
                  Math.Max(
                      arrowBodyGO.GetComponent<SpriteRenderer>().bounds.size.x,
                      arrowBodyGO.GetComponent<SpriteRenderer>().bounds.size.y);
        arrowBodyGO.transform.localScale = scale;

        GameObject arrowHeadGO = Instantiate(iconPrefab, gameObject.transform, false);
        arrowHeadGO.transform.right = direction;
        Icon arrowHead = arrowHeadGO.GetComponent<Icon>();
        arrowHead.transform.position = finalPosition;
        arrowHead.SetArtworkSprite(arrowHeadSprite);
        
        arrowContainers.Add(initialPosition, new ArrowContainer(arrowBody, arrowHead));
    }

    public void MakeArrowGreen(Vector2 parentNodePosition) {
        if (!arrowContainers.ContainsKey(parentNodePosition)) return;
        arrowContainers[parentNodePosition].ChangeBodySprite(greenArrowBodySprite);
        arrowContainers[parentNodePosition].ChangeHeadSprite(greenArrowHeadSprite);
    }
    
    public void RestoreArrowColors() {
        if (arrowContainers.Count == 0) return;
        foreach (var arrowContainer in arrowContainers) {
            arrowContainer.Value.ChangeBodySprite(arrowBodySprite);
            arrowContainer.Value.ChangeHeadSprite(arrowHeadSprite);
        }
    }
}

public class ArrowContainer {
    private Icon arrowBody;
    private Icon arrowHead;

    public ArrowContainer(Icon arrowBody, Icon arrowHead) {
        this.arrowBody = arrowBody;
        this.arrowHead = arrowHead;
    }

    public void ChangeBodySprite(Sprite sprite) {
        arrowBody.SetArtworkSprite(sprite);
    }

    public void ChangeHeadSprite(Sprite sprite) {
        arrowHead.SetArtworkSprite(sprite);
    }
}
