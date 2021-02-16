using System.Collections.Generic;
using UnityEngine;

public interface ITreeNode {
    /// <summary>
    /// inserts node in tree data structure
    /// </summary>
    /// <param name="node">root of the tree</param>
    /// <param name="value">new node value</param>
    /// <param name="insertedPosition">outs the inserted position</param>
    /// <param name="parentPosition">outs the parent position</param>
    /// <param name="balancedNodesPositions">key with node positions, returns list with first list item new position, second item parent position.</param>
    void Insert(ref ITreeNode node, ref int value, ref Vector2 insertedPosition, ref Vector2 parentPosition,
        Dictionary<string,List<Vector2>> balancedNodesPositions = null);

    int Value { get; }
    Vector2 Position { get; }
    ITreeNode SearchNode(Vector2 nodePosition, int value, ITreeNode root);
    void DeleteNode(ITreeNode node, Dictionary<string,string> textNodesUpdate, out Vector2 deletedNodePosition);
    //---methods for debugging---
    void PrintTree(ITreeNode root);
    void PrintHeight(ITreeNode root);
    void PrintDepth(ITreeNode root);
}