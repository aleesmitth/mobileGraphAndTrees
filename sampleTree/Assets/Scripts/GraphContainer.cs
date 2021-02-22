using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static GameManager;

public class GraphContainer : MonoBehaviour {
    private IGraphNode selectedNode;
    private Dictionary<string, GameObject> nodesDictionaryGO;
    private Dictionary<string, IGraphNode> nodesDictionary;
    [SerializeField]
    public Material defaultNodeMat;
    [SerializeField]
    public Material selectedNodeMat;
    private Camera mainCamera;
    private bool activateInsert;
    private Rect debugNodeRect;
    private bool searchForMin;
    public int arrowWingsSize = 5;
    public int arrowWingSeparation = 5;
    
    [SerializeField] public Color lineRendererStartColor;
    [SerializeField] public Color lineRendererEndColor;
    [SerializeField] public Color lineRendererPathStartColor;
    [SerializeField] public Color lineRendererPathEndColor;
    [SerializeField] public Color gizmoNodeColor = Color.green;

    private void OnEnable() {
        activateInsert = true;
        searchForMin = false;
        mainCamera = Camera.main;
        Debug.Log("modo graph\n");
        CreateDictionaries();
        CreateRootNode();
        EventManager.onSelectNode += SelectNode;
    }

    private void CreateRootNode() {
        var node = new GraphNode {Position = Vector2.zero};
        nodesDictionary.Add(MakeNodeKey(Vector2.zero), node);
        
        //display the node on screen
        var nodeGO = GraphNodePool.instance.Get();
        nodesDictionaryGO.Add(MakeNodeKey(Vector2.zero), nodeGO);
        nodeGO.transform.position = Vector3.zero;
        nodeGO.transform.rotation = Quaternion.identity;

        selectedNode = node;
    }

    private void OnDisable() {
        Debug.Log("end modo graph\n");
        EventManager.onSelectNode -= SelectNode;
        DeleteEverything();
    }
    
    private void Update() {
        if (!Input.GetMouseButtonDown(0)) return;
        if (selectedNode == null) return;
        if (!activateInsert) return;
        var insertPosition = Input.mousePosition;
        insertPosition = mainCamera.ScreenToWorldPoint(new Vector3(insertPosition.x, insertPosition.y, -mainCamera.transform.position.z));
        //chekcs if mouse click wasn't near another node
        foreach (var node in nodesDictionary) {
            MeshRenderer mr =  nodesDictionaryGO[node.Key].GetComponentInChildren<MeshRenderer>();
            var meshRendererBounds = mr.bounds;
            Rect bounds = new Rect(
                node.Value.Position.x - (meshRendererBounds.size.x + GameManager.instance.BALL_MARGIN)/2,
                node.Value.Position.y - (meshRendererBounds.size.y + GameManager.instance.BALL_MARGIN)/2,
                meshRendererBounds.size.x + GameManager.instance.BALL_MARGIN,
                meshRendererBounds.size.y + GameManager.instance.BALL_MARGIN);
            debugNodeRect = bounds;
            
            if (bounds.Contains(insertPosition)) return;
        }
        InsertNode(insertPosition);
    }

    private void OnDrawGizmos() {
        if (nodesDictionary == null || nodesDictionaryGO.Count == 0) return;
        foreach (var nodeGO in nodesDictionaryGO) {
            Gizmos.color = gizmoNodeColor;
            var position = nodeGO.Value.transform.position;
            MeshRenderer mr =  nodeGO.Value.GetComponentInChildren<MeshRenderer>();
            var meshRendererBounds = mr.bounds;
            Gizmos.DrawWireCube(new Vector3(position.x, position.y, 0.01f),
                new Vector3(meshRendererBounds.size.x + GameManager.instance.BALL_MARGIN, meshRendererBounds.size.y + GameManager.instance.BALL_MARGIN, 0.01f));
        }
    }

    private void InsertNode(Vector2 position) {
        print("Insert in: " + position);
        print("Parent in: " + selectedNode.Position);
        var node = new GraphNode();
        nodesDictionary.Add(MakeNodeKey(position), node);
        selectedNode.AddNode(node, position);
        
        //display the node on screen
        var nodeGO = GraphNodePool.instance.Get();
        nodesDictionaryGO.Add(MakeNodeKey(position), nodeGO);
        nodeGO.transform.position = position;
        nodeGO.transform.rotation = Quaternion.identity;
        
        //restores the color of the selected node to normal, and draws branch to parent
        var selectedNodeGO = nodesDictionaryGO[MakeNodeKey(selectedNode.Position)];
        var meshRenderer = selectedNodeGO.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = defaultNodeMat;
        LineRenderer lineRenderer = nodeGO.GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.startColor = lineRendererStartColor;
        lineRenderer.endColor = lineRendererEndColor;
        //draw arrow
        var directionOfInsertNormalized = (selectedNode.Position - position).normalized;
        var directionOfInsert = directionOfInsertNormalized * arrowWingsSize + position;
        
        var normalDirection = new Vector2(-directionOfInsertNormalized.y, directionOfInsertNormalized.x);
        normalDirection *= arrowWingSeparation;

        var rightWingDirection = directionOfInsert + normalDirection;
        var leftWingDirection = directionOfInsert - normalDirection;
        

        //if i want the lines at the borders i should divide the meshrederer size * direction normalized by 2
        var nodeSize = meshRenderer.bounds.size * directionOfInsertNormalized;
        
        lineRenderer.SetPosition(0, selectedNode.Position - nodeSize);
        lineRenderer.SetPosition(1, position + nodeSize/2);
        lineRenderer.SetPosition(2, leftWingDirection);
        lineRenderer.SetPosition(3, position + nodeSize/2);
        lineRenderer.SetPosition(4, rightWingDirection);

        selectedNode = node;
    }

    private void CreateDictionaries() {
        nodesDictionary = new Dictionary<string, IGraphNode>();
        nodesDictionaryGO = new Dictionary<string, GameObject>();
        selectedNode = null;
    }

    private void SelectNode(Vector3 position) {
        if (selectedNode != null) {
            if (searchForMin) {
                var shortestPath = Dijkstra(selectedNode, position);
                if (shortestPath == null) {
                    Debug.Log("Shortest path not found.");
                    return;
                }

                var firstSelected = false;
                foreach (var node in shortestPath) {
                    print(node.Position + " ESTE SALIO DE DIJSKTRA");
                    var nodeGO = nodesDictionaryGO[GameManager.MakeNodeKey(node.Position)];
                    nodeGO.GetComponentInChildren<MeshRenderer>().material = selectedNodeMat;
                    if (!firstSelected) {
                        firstSelected = true;
                        continue;
                    }
                    var lineRenderer = nodeGO.GetComponent<LineRenderer>();
                    lineRenderer.startColor = lineRendererPathStartColor;
                    lineRenderer.endColor = lineRendererPathEndColor;
                }

                return;
            }

            var previousSelectedNodeGO = nodesDictionaryGO[MakeNodeKey(selectedNode.Position)];
            previousSelectedNodeGO.GetComponentInChildren<MeshRenderer>().material = defaultNodeMat;

        }
        selectedNode = nodesDictionary[MakeNodeKey(position)];
        var selectedNodeGO = nodesDictionaryGO[MakeNodeKey(selectedNode.Position)];
        selectedNodeGO.GetComponentInChildren<MeshRenderer>().material = selectedNodeMat;
    }

    private LinkedList<IGraphNode> Dijkstra(IGraphNode startNode, Vector3 positionEndNode) {
        var key = GameManager.MakeNodeKey(positionEndNode);
        var endNode = nodesDictionary[key];
        //LinkedList<IGraphNode> unvisitedNodes = new LinkedList<IGraphNode>();
        var unvisitedNodes = new BinaryHeap(10);
        bool nodeFound = false;
        foreach (var node in nodesDictionary) {
            node.Value.Value = 999999;
            node.Value.WasVisited = false;
            node.Value.Parent = null;
        }

        startNode.Value = 0;
        unvisitedNodes.InsertElementInHeap(startNode);
        while (unvisitedNodes.SizeOfHeap() > 0) {
            //var currentNode = FindMinNode(unvisitedNodes);
            var currentNode = unvisitedNodes.ExtractHeadOfHeap();
            //unvisitedNodes.Remove(currentNode);
            Debug.Log("Removing: " + currentNode.Position + " from the list\n");
            /*foreach (var node in unvisitedNodes) {
                Debug.Log("Remaining node: " + node.Position);
            }*/
            
            if (currentNode.Position == endNode.Position) {
                Debug.Log("found the end \n");
                nodeFound = true;
                break;
            }
            
            if (currentNode.WasVisited) continue;
            
            currentNode.WasVisited = true;
            
            foreach (var edge in currentNode.OutEdges) {
                float newPossiblyMinValue = edge.Weight + currentNode.Value;
                var edgeNode = edge.node;
                if (edgeNode.Value > newPossiblyMinValue) {
                    edgeNode.Value = newPossiblyMinValue;
                    edgeNode.Parent = currentNode;
                }
                
                //unvisitedNodes.AddFirst(edgeNode);
                unvisitedNodes.InsertElementInHeap(edgeNode);
                Debug.Log("Added: " + edgeNode.Position + " to the list\n");
            }
        }

        if (!nodeFound) return null;
        LinkedList<IGraphNode> minNodePath = new LinkedList<IGraphNode>();
        minNodePath.AddFirst(endNode);
        var bufferNode = endNode;
        while (bufferNode.Parent != null) {
            minNodePath.AddFirst(bufferNode.Parent);
            bufferNode = bufferNode.Parent;
        }

        return minNodePath;
    }

    private IGraphNode FindMinNode(LinkedList<IGraphNode> listOfNodes) {
        float minValueBuffer = 0;
        IGraphNode bufferNode = listOfNodes.First.Value;
        foreach (var node in listOfNodes.Where(node => node.Value <= minValueBuffer)) {
            minValueBuffer = node.Value;
            bufferNode = node;
        }

        return bufferNode;
    }

    private void DeleteEverything() {
        foreach (var node in nodesDictionaryGO) {
            //deletes node from the scene pooling it back.
            GraphNodePool.instance.DestroyObject(node.Value); 
        }
        
        nodesDictionaryGO.Clear();
        nodesDictionary.Clear();
        selectedNode = null;
    }

    public void DeactivateInsertNode() {
        activateInsert = false;
    }

    public void ActivateInsertNode() {
        searchForMin = false;
        activateInsert = true;
        UnselectNodes();
    }

    private void UnselectNodes() {
        foreach (var nodeGO in nodesDictionaryGO) {
            nodeGO.Value.GetComponentInChildren<MeshRenderer>().material = defaultNodeMat;
            var lr = nodeGO.Value.GetComponent<LineRenderer>();
            lr.startColor = lineRendererStartColor;
            lr.endColor = lineRendererEndColor;
        }

        var selectedNodeGO = nodesDictionaryGO[GameManager.MakeNodeKey(selectedNode.Position)];
        selectedNodeGO.GetComponentInChildren<MeshRenderer>().material = selectedNodeMat;
    }

    public void SearchForMinMode() {
        searchForMin = true;
    }

    public void PrintPositions() {
        foreach (var node in nodesDictionary) {
            print(node.Value.Position);
        }
    }

    public void PrintPositionsGO() {
        foreach (var node in nodesDictionaryGO) {
            print(node.Value.transform.position);
        }
    }

    public void PrintOutEdges() {
        foreach (var node in nodesDictionary) {
            print("In node: " + node.Value.Position);
            foreach (var edge in node.Value.OutEdges) {
                print("connects OUTER edge to: " + edge.node.Position);
            }
        }
    }

    public void PrintInEdges() {
        foreach (var node in nodesDictionary) {
            print("In node: " + node.Value.Position);
            foreach (var edge in node.Value.InEdges) {
                print("connects INNER edge to: " + edge.node.Position);
            }
        }
    }

    public void PrintWeights() {
        foreach (var node in nodesDictionary) {
            print("Weight of: " + node.Value.Position + " with value: " + node.Value.Value);
            foreach (var edge in node.Value.InEdges) {
                print("has inner edge of weight: " + edge.Weight);
            }
        }
    }
}