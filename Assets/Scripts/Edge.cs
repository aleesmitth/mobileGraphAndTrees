using System.Collections.Generic;

public class Edge {
    public IGraphNode node;
    public float Weight {
        get;
        set;
    }
    public Edge(IGraphNode node, float weight) {
        this.node = node;
        this.Weight = weight;
        node.InEdges.AddFirst(this);
    }
}