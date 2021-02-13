using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePool : MonoBehaviour {
    public GameObject prefab;
    public Transform parent;
    public int growthSize;
    public static NodePool instance;
    [SerializeField]
    public Material newNodeMaterial;
    private readonly Queue<GameObject> queue = new Queue<GameObject>();

    private void Awake() {
        this.MakeSingleton();
    }
    
    private void MakeSingleton() {
        if (instance == null) {
            instance = this;
        }
        else if(instance!=this)
            Destroy(gameObject);
    }

    public GameObject Get() {
        if (queue.Count == 0) {
            Grow();
        }
        GameObject depooledObject = queue.Dequeue();
        while(depooledObject == null) {
            if (queue.Count == 0) {
                Grow();
            }

            depooledObject = queue.Dequeue();
        }

        depooledObject.SetActive(true);
        depooledObject.transform.SetParent(parent, false);
        return depooledObject;
    }

    public void DestroyObject(GameObject pooledObject) {
        ResetLineRenderer(pooledObject);
        ResetMaterial(pooledObject);
        pooledObject.SetActive(false);
        queue.Enqueue(pooledObject);
    }

    private void Grow() {
        if (growthSize == 0) growthSize ++;
        for (int i = queue.Count; i < growthSize; i++) {
            GameObject pooledObject = Instantiate(prefab, parent, false);
            pooledObject.SetActive(false);
            queue.Enqueue(pooledObject);
        }
    }

    private void ResetLineRenderer(GameObject pooledObject) {
        var lineRenderer = pooledObject.GetComponent<LineRenderer>();
        var position = pooledObject.transform.position;
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, position);
        lineRenderer.enabled = false;
    }

    private void ResetMaterial(GameObject pooledObject) {
        pooledObject.GetComponentInChildren<MeshRenderer>().material = newNodeMaterial;
    }
}
