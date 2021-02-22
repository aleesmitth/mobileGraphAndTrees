using System.Collections.Generic;
using UnityEngine;

internal class GraphNode : IGraphNode {
    public LinkedList<Edge> InEdges { get; set; }
    public LinkedList<Edge> OutEdges { get; set; }
    public bool WasVisited { get; set; }
    public float Value { get; set; }
    public Vector2 Position { get; set; }
    public IGraphNode Parent { get; set; }

    public GraphNode() {
        InEdges = new LinkedList<Edge>();
        OutEdges = new LinkedList<Edge>();
    }

    public void AddNode(IGraphNode node, Vector2 position) {
        node.Position = position;
        var edge = new Edge(node, Mathf.Abs(position.magnitude - this.Position.magnitude));
        OutEdges.AddFirst(edge);
    }
}