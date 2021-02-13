using System.Collections.Generic;
using UnityEngine;

public abstract class BinaryTreeNode {
    protected abstract BinaryTreeNode RightChild { get; set; }
    protected abstract BinaryTreeNode LeftChild { get; set; }
    protected abstract BinaryTreeNode Parent { get; set; }
    public int Value { get; set; }
    public Vector2 Position { get; set; }

    public void PrintTree(ITreeNode root) {
        PrintTree((BinaryTreeNode)root);
    }

    public ITreeNode SearchNode(Vector2 nodePosition, int value, ITreeNode root) {
        return SearchNode(nodePosition, value, (BinaryTreeNode)root);
    }

    private void PrintTree(BinaryTreeNode node) {
        if (node == null) {
            Debug.Log("null \n");
            return;
        }

        Debug.Log(node.Value);

        PrintTree(node.LeftChild);
        PrintTree(node.RightChild);
    }

    private ITreeNode SearchNode(Vector2 position, int value, BinaryTreeNode node) {
        //searching for node in tree, if nonexistent return null.
        while (true) {
            if (node == null) return null;
            if (node.Position == position) return (ITreeNode)node;
            node = value > node.Value ? node.RightChild : node.LeftChild;
        }
    }
    
    protected BinaryTreeNode FindMax(BinaryTreeNode node) {
        while (true) {
            if (node.RightChild == null) {
                return node;
            }

            node = node.RightChild;
        }
    }

    protected BinaryTreeNode FindMin(BinaryTreeNode node) {
        while (true) {
            if (node.LeftChild == null) {
                return node;
            }

            node = node.LeftChild;
        }
    }
}