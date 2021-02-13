using System.Collections.Generic;
using UnityEngine;

public interface ITreeNode {
    ITreeNode Insert(ITreeNode node, ref int value, ref Vector2 insertedPosition, ref Vector2 parentPosition, int n = -1,
        BSTNode parent = null);

    int Value { get; }
    Vector2 Position { get; }
    void PrintTree(ITreeNode root);
    ITreeNode SearchNode(Vector2 nodePosition, int value, ITreeNode root);
    void DeleteNode(ITreeNode node, Dictionary<string,string> textNodesUpdate, out Vector2 deletedNodePosition);
}