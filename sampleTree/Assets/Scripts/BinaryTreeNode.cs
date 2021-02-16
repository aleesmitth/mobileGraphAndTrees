using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BinaryTreeNode {
    public abstract BinaryTreeNode RightChild { get; set; }
    public abstract BinaryTreeNode LeftChild { get; set; }
    public abstract BinaryTreeNode Parent { get; set; }
    public virtual int Value { get; set; }
    public virtual int Height { get; set; }
    
    public virtual int Depth { get; set; }
    public Vector2 Position { get; set; }

    public void PrintTree(ITreeNode root) {
        PrintTree((BinaryTreeNode)root);
    }

    public void PrintHeight(ITreeNode root) {
        PrintHeight((BinaryTreeNode) root);
    }
    
    public void PrintDepth(ITreeNode root) {
        PrintDepth((BinaryTreeNode) root);
    }

    public ITreeNode SearchNode(Vector2 nodePosition, int value, ITreeNode root) {
        return SearchNode(nodePosition, value, (BinaryTreeNode)root);
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
    
    private void PrintTree(BinaryTreeNode node) {
        if (node == null) {
            Debug.Log("null \n");
            return;
        }

        Debug.Log(node.Value);

        PrintTree(node.LeftChild);
        PrintTree(node.RightChild);
    }

    protected int GetHeight(BinaryTreeNode node) {
        if (node == null) return -1;
        return Math.Max(GetHeight(node.LeftChild), GetHeight(node.RightChild)) + 1;
    }

    private void PrintHeight(BinaryTreeNode node) {
        if (node == null) return;
        Debug.Log("Height of: " + node.Value + " is " + node.Height + ".");
        PrintHeight(node.LeftChild);
        PrintHeight(node.RightChild);
    }

    private void PrintDepth(BinaryTreeNode node) {
        if (node == null) return;
        Debug.Log("Depth of: " + node.Value + " is " + node.Depth + ".");
        PrintDepth(node.LeftChild);
        PrintDepth(node.RightChild);
    }
}