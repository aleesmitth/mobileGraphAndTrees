using System;
using System.Collections.Generic;
using UnityEngine;

public class AVLNode : BinaryTreeNode, ITreeNode {
    private AVLNode parent;
    private AVLNode leftChild;
    private AVLNode rightChild;
    
    ///i make sure that i can cast BinaryTreeNodes to my node type (AVLNode)
    protected sealed override BinaryTreeNode LeftChild {
        get => leftChild;
        set => leftChild = (AVLNode)value;
    }

    ///i make sure that i can cast BinaryTreeNodes to my node type (AVLNode)
    protected sealed override BinaryTreeNode RightChild {
        get => rightChild;
        set => rightChild = (AVLNode)value;
    }
    
    protected sealed override BinaryTreeNode Parent {
        get => parent;
        set => parent = (AVLNode)value;
    }
    
    public int Value { get; set; }
    
    public Vector2 Position { get; set; }
    
    private Vector2 BalanceFactor { get; set; }

    public AVLNode() { leftChild = null; rightChild = null; }
    
    public ITreeNode Insert(ITreeNode node, ref int value, ref Vector2 insertedPosition, ref Vector2 parentPosition, int n = -1, BSTNode parent = null) {
        return Insert((AVLNode) node, ref value, ref insertedPosition, ref parentPosition);
    }

    public void DeleteNode(ITreeNode node, Dictionary<string, string> textNodesUpdate, out Vector2 deletedNodePosition) {
        throw new NotImplementedException();
    }

    private ITreeNode Insert(AVLNode avlNode, ref int value, ref Vector2 insertedPosition, ref Vector2 parentPosition, int n = -1, BSTNode parent = null) {
        //implement
        return null;
    }

    private void PrintTree(AVLNode avlNode) {
        //implement
    }

    private AVLNode SearchNode(Vector2 nodeposition, int value, AVLNode avlNode) {
        //implement
        return null;
    }
}