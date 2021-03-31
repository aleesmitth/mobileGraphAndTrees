using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
        //the idea is to rotate and stretch the original x axis of the sprite.
        
        //create the line of the arrow
        GameObject arrowBodyGO = Instantiate(iconPrefab, gameObject.transform, false);
        Icon arrowBody = arrowBodyGO.GetComponent<Icon>();
        arrowBody.SetArtworkSprite(arrowBodySprite);
        
        //positions the sprite
        Vector2 centerPosition = (initialPosition + finalPosition) / 2f;
        arrowBodyGO.transform.position = centerPosition;
        
        var originalSizeX = arrowBodyGO.GetComponent<SpriteRenderer>().bounds.size.x;

        //rotates the sprite
        Vector2 direction = (finalPosition - initialPosition).normalized;
        arrowBodyGO.transform.right = direction;
        
        
        Vector3 scale = new Vector3(1,1,1);
        
        //scale the x axis. scale = distance you want to cover / original sprite size. 
        scale.x = Vector2.Distance(initialPosition, finalPosition) / originalSizeX;
        arrowBodyGO.transform.localScale = scale;

        //create the arrow head
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

    public void DeleteAllArrows() {
        foreach (var arrowContainer in arrowContainers) {
            arrowContainer.Value.DeleteAllArrows();
        }
        arrowContainers.Clear();
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

    public void DeleteAllArrows() {
        Object.Destroy(arrowBody.gameObject);
        Object.Destroy(arrowHead.gameObject);
    }
}
