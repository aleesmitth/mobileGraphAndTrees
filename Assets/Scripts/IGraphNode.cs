using System.Collections.Generic;
using UnityEngine;

public interface IGraphNode {
    public LinkedList<Edge> InEdges { get; set; }
    public LinkedList<Edge> OutEdges { get; set; }
    public bool WasVisited { get; set; }
    public float Value { get; set; }
    public Vector2 Position { get; set; }
    IGraphNode Parent { get; set; }
    void AddNode(IGraphNode node, Vector2 position);
}