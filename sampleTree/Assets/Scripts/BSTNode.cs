using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class BSTNode : BinaryTreeNode, ITreeNode {
    private BSTNode parent;
    private BSTNode leftChild;
    private BSTNode rightChild;
    //private BSTNode leftChild;
    //public int Value { get; set; }
    //public Vector2 Position { get; set; }

    ///i make sure that i can cast BinaryTreeNodes to my node type (BSTNode)
    public sealed override BinaryTreeNode LeftChild {
        get => leftChild;
        set => leftChild = (BSTNode)value;
    }

    ///i make sure that i can cast BinaryTreeNodes to my node type (BSTNode)
    public sealed override BinaryTreeNode RightChild {
        get => rightChild;
        set => rightChild = (BSTNode)value;
    }
    
    public sealed override BinaryTreeNode Parent {
        get => parent;
        set => parent = (BSTNode)value;
    }

    public BSTNode() { leftChild = null; rightChild = null; }

    public void DeleteNode(ITreeNode node, Dictionary<string, string> textNodesUpdate, out Vector2 deletedNodePosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
        DeleteNode((BinaryTreeNode)node, textNodesUpdate, out deletedNodePosition);
    }

    public void Insert(ref ITreeNode node, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
        var bstNode = (BSTNode) node;
        Insert(ref bstNode, ref value, ref insertedPosition, ref parentPosition);
    }

    private void Insert(ref BSTNode bstNode, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, int depth = -2, BinaryTreeNode parentNode = null) {
        depth++;
        if (depth > GameManager.MAX_HEIGHT) {
            throw new HeightTreeLimitException("Max tree height reached, cant insert node.");
        }
        if (bstNode == null) {
            // hardcodeo para no tener 2 nodos superpuestos. no se coloca si ya hay uno ahi.
            if (GameManager.instance.IsPositionOccupied(insertedPosition)) {
                throw new PositionOccupiedByNodeException("Position already occupied by another node.");
            }
            bstNode = new BSTNode {Position = insertedPosition, Value = value, Parent = parentNode, Depth = depth};
        }
        else if (value > bstNode.Value) {
            InsertNodeRecursive(ref bstNode.rightChild, ref value, ref insertedPosition,
                ref parentPosition, bstNode, 1);
        } else {
            InsertNodeRecursive(ref bstNode.leftChild, ref value, ref insertedPosition,
                ref parentPosition, bstNode, -1);
        }

        bstNode.Height = GetHeight(bstNode);
    }

    private void InsertNodeRecursive(ref BSTNode childNode, ref int value, ref Vector2 insertedPosition,
            ref Vector2 parentPosition, BinaryTreeNode parentNode, int childDirection) {
            parentPosition = insertedPosition;
            // divido por depth para q no se superpongan los hijos de nodos hermanos.
            insertedPosition += new Vector2(childDirection * (GameManager.TREE_X_OFFSET - parentNode.Depth), -GameManager.TREE_Y_OFFSET);
            Insert(ref childNode, ref value, ref insertedPosition, ref parentPosition, parentNode.Depth, parentNode);
    }

    private void DeleteNode(BinaryTreeNode node, Dictionary<string, string> textNodesUpdate, out Vector2 deletedNodePosition) {
        if (node.RightChild == null && node.LeftChild == null) {
            deletedNodePosition = node.Position;
            //deletes node from the tree, leaving parent without corresponding child.
            if (node.Parent.RightChild != null && node.Parent.RightChild.Position == node.Position) node.Parent.RightChild = null;
            else {
                node.Parent.LeftChild = null;
            }
        }
        else if (node.LeftChild != null && node.RightChild == null) {
            var maxLeft = FindMax(node.LeftChild);
            DeleteNodeRecursive(node, maxLeft, textNodesUpdate, out deletedNodePosition);
        }
        else {
            var minRight = FindMin(node.RightChild);
            DeleteNodeRecursive(node, minRight, textNodesUpdate, out deletedNodePosition);
        }
    }

    private void DeleteNodeRecursive(BinaryTreeNode deletedNode, BinaryTreeNode nodeToDelete, Dictionary<string, string> textNodesUpdate,
        out Vector2 deletedNodePosition) {
        
        deletedNode.Value = nodeToDelete.Value;
        
        var key = TreeContainer.MakeNodeKey(deletedNode.Position);
        textNodesUpdate.Add(key, deletedNode.Value.ToString(CultureInfo.InvariantCulture));
            
        DeleteNode(nodeToDelete, textNodesUpdate, out deletedNodePosition);
    }
    
    public ITreeNode ChangeTreeType() {
        return new AVLNode() {Value = -1, Position = new Vector2(-GameManager.TREE_X_OFFSET, GameManager.TREE_Y_OFFSET), Depth = 0};
    }
}