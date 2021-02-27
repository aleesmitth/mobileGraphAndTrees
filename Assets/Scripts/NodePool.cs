using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodePool : MonoBehaviour{
    public abstract GameObject Prefab { get; set; }
    public abstract Transform Parent { get; set; }
    public abstract int GrowthSize { get; set; }
    public abstract Material NewNodeMaterial { get; set; }
    protected abstract Queue<GameObject> Queue { get; set; }

    public GameObject Get() {
        if (Queue.Count == 0) {
            Grow();
        }
        GameObject depooledObject = Queue.Dequeue();
        while(depooledObject == null) {
            if (Queue.Count == 0) {
                Grow();
            }

            depooledObject = Queue.Dequeue();
        }

        depooledObject.SetActive(true);
        depooledObject.transform.SetParent(Parent, false);
        return depooledObject;
    }

    public void DestroyObject(GameObject pooledObject) {
        if (pooledObject == null) return;
        ResetLineRenderer(pooledObject);
        ResetMaterial(pooledObject);
        pooledObject.SetActive(false);
        Queue.Enqueue(pooledObject);
    }

    private void ResetLineRenderer(GameObject pooledObject) {
        var lineRenderer = pooledObject.GetComponent<LineRenderer>();
        var position = pooledObject.transform.position;
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, position);
        lineRenderer.enabled = false;
    }
    private void Grow() {
        if (GrowthSize == 0) GrowthSize ++;
        for (int i = Queue.Count; i < GrowthSize; i++) {
            GameObject pooledObject = Instantiate(Prefab, Parent, false);
            pooledObject.SetActive(false);
            Queue.Enqueue(pooledObject);
        }
    }

    private void ResetMaterial(GameObject pooledObject) {
        pooledObject.GetComponentInChildren<MeshRenderer>().material = NewNodeMaterial;
    }
}
