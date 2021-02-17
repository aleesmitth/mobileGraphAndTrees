using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AVLNode : BinaryTreeNode, ITreeNode {
    private AVLNode parent;
    private AVLNode leftChild;
    private AVLNode rightChild;
    private int balanceFactor;

    ///i make sure that i can cast BinaryTreeNodes to my node type (AVLNode)
    public sealed override BinaryTreeNode LeftChild {
        get => leftChild;
        set => leftChild = (AVLNode)value;
    }

    ///i make sure that i can cast BinaryTreeNodes to my node type (AVLNode)
    public sealed override BinaryTreeNode RightChild {
        get => rightChild;
        set => rightChild = (AVLNode)value;
    }
    
    public sealed override BinaryTreeNode Parent {
        get => parent;
        set => parent = (AVLNode)value;
    }

    public AVLNode() { 
        leftChild = null; 
        rightChild = null;
        balanceFactor = 0;
    }
    
    public void Insert(ref ITreeNode node, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
        var avlNode = (AVLNode) node;
        Insert(ref avlNode, ref value, ref insertedPosition, ref parentPosition, balancedNodesPositions);
    }

    public void DeleteNode(ITreeNode node, Dictionary<string, string> textNodesUpdate, out Vector2 deletedNodePosition,
        Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
        DeleteNode((BinaryTreeNode)node, textNodesUpdate, out deletedNodePosition, balancedNodesPositions);
    }
    
    private void DeleteNode(BinaryTreeNode node, Dictionary<string, string> textNodesUpdate,
        out Vector2 deletedNodePosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
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
            DeleteNodeRecursive(node, maxLeft, textNodesUpdate, out deletedNodePosition, balancedNodesPositions);
        }
        else {
            var minRight = FindMin(node.RightChild);
            DeleteNodeRecursive(node, minRight, textNodesUpdate, out deletedNodePosition, balancedNodesPositions);
        }
        if (node.Value <= -1) return;
        BalanceTreeFromBelow((AVLNode) node, balancedNodesPositions);
    }

    private void BalanceTreeFromBelow(AVLNode node,
        Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
        if (node == null) return;
        if (node.Value == -1) return;
        balancedNodesPositions ??= new Dictionary<string, List<Vector2>>();
        var nodeParent = node.parent;
        UpdateBalanceFactor(node);
        BalanceNode(node, balancedNodesPositions);
        UpdateBalanceFactor(nodeParent);
        BalanceTreeFromBelow(nodeParent, balancedNodesPositions);
        Debug.Log("PASEXD \n");
    }

    private void DeleteNodeRecursive(BinaryTreeNode deletedNode, BinaryTreeNode nodeToDelete, Dictionary<string, string> textNodesUpdate,
        out Vector2 deletedNodePosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null) {
        
        deletedNode.Value = nodeToDelete.Value;
        
        var key = TreeContainer.MakeNodeKey(deletedNode.Position);
        textNodesUpdate.Add(key, deletedNode.Value.ToString(CultureInfo.InvariantCulture));
            
        DeleteNode(nodeToDelete, textNodesUpdate, out deletedNodePosition, balancedNodesPositions);
    }

    private void Insert(ref AVLNode avlNode, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, Dictionary<string,List<Vector2>> balancedNodesPositions = null, int depth = -2, BinaryTreeNode parentNode = null) {
        depth++;
        if (depth > GameManager.MAX_HEIGHT) {
            throw new HeightTreeLimitException("Max tree height reached, cant insert node.");
        }
        if (avlNode == null) {
            // hardcodeo para no tener 2 nodos superpuestos. no se coloca si ya hay uno ahi.
            if (GameManager.instance.IsPositionOccupied(insertedPosition)) {
                throw new PositionOccupiedByNodeException("Position already occupied by another node.");
            }
            avlNode = new AVLNode {Position = insertedPosition, Value = value, Parent = parentNode, Depth = depth, Height = 0};
        }
        else if (value > avlNode.Value) {
            InsertNodeRecursive(ref avlNode.rightChild, ref value, ref insertedPosition,
                ref parentPosition, avlNode, balancedNodesPositions, 1);
        } else {
            InsertNodeRecursive(ref avlNode.leftChild, ref value, ref insertedPosition,
                ref parentPosition, avlNode, balancedNodesPositions, -1);
        }

        if (avlNode.Value <= -1) return;
        UpdateBalanceFactor(avlNode);
        BalanceNode(avlNode, balancedNodesPositions);
    }

    private void UpdateBalanceFactor(AVLNode avlNode) {
        if(avlNode == null) return;
        avlNode.Height = GetHeight(avlNode);
        Debug.Log(avlNode.Height);
        Debug.Log("Height of: " + avlNode.Value + " is " + avlNode.Height);
        avlNode.balanceFactor = GetHeight(avlNode.leftChild) - GetHeight(avlNode.rightChild);
        Debug.Log("Balance factor: " + avlNode.balanceFactor);
    }

    private void BalanceNode(AVLNode avlNode, Dictionary<string,List<Vector2>> balancedNodesPositions) {
        if (avlNode.balanceFactor < 2 && avlNode.balanceFactor > -2) return;
        AVLNode headNode;
        if (avlNode.balanceFactor > 1) {
            if (avlNode.leftChild.rightChild == null || avlNode.leftChild.balanceFactor > 0) {
                headNode = RightRotation(avlNode);
            }
            else {
                headNode = LeftRightRotation(avlNode);
            }
        }
        else {
            if (avlNode.rightChild.leftChild == null || avlNode.rightChild.balanceFactor < 0)
                headNode = LeftRotation(avlNode);
            else {
                headNode = RightLeftRotation(avlNode);
            }
        }
        UpdateBalanceFactor(headNode);
        UpdateBalanceFactor(headNode.rightChild);
        UpdateBalanceFactor(headNode.leftChild);
        UpdateDepth(headNode);
        Debug.Log("Position of " + headNode.Value + " will be " + avlNode.Position);
        UpdatePosition(headNode, avlNode.Position, balancedNodesPositions);
    }

    private void UpdateDepth(AVLNode node) {
        if (node == null) return;
        node.Depth = node.parent.Depth + 1;
        UpdateDepth(node.leftChild);
        UpdateDepth(node.rightChild);
    }

    private void UpdatePosition(AVLNode avlNode, Vector2 position, Dictionary<string,List<Vector2>> balancedNodesPositions) {
        if (avlNode == null) return;
        balancedNodesPositions ??= new Dictionary<string, List<Vector2>>();
        
        var key = TreeContainer.MakeNodeKey(avlNode.Position);
        if (balancedNodesPositions.ContainsKey(key)) {
            balancedNodesPositions[key][0] = position;
            balancedNodesPositions[key][1] = avlNode.parent.Position;
        }
        else {
            balancedNodesPositions.Add(key, new List<Vector2> {position, avlNode.parent.Position});
        }
        //hardcoded to ignore my invisible root, otherwise it'll draw a line towards it
        if (avlNode.parent.Value == -1 && balancedNodesPositions.ContainsKey(key)) balancedNodesPositions[key][1] = position;
        Debug.Log("se supone q el nodo " + avlNode.Value + " en la posicion " + position + " va a tener padre a " + avlNode.parent.Value + " en la posicion " + avlNode.parent.Position + "\n");

        avlNode.Position = position;
        var positionRight = position + new Vector2(GameManager.TREE_X_OFFSET - avlNode.Depth, -GameManager.TREE_Y_OFFSET);
        Debug.Log("position right: " + positionRight + "\n");
        var positionLeft = position + new Vector2(-(GameManager.TREE_X_OFFSET - avlNode.Depth), -GameManager.TREE_Y_OFFSET);
        Debug.Log("position left: " + positionLeft + "\n");
        UpdatePosition(avlNode.rightChild, positionRight, balancedNodesPositions);
        UpdatePosition(avlNode.leftChild, positionLeft, balancedNodesPositions);
    }

    private AVLNode RightLeftRotation(AVLNode originalHeadNode) {
        var newMiddleNode = originalHeadNode.rightChild.leftChild;
        var branch = newMiddleNode.rightChild;
        
        //do rotation
        newMiddleNode.rightChild = originalHeadNode.rightChild;
        newMiddleNode.rightChild.leftChild = branch;
        originalHeadNode.rightChild = newMiddleNode;
        
        //update parent nodes
        newMiddleNode.rightChild.parent = newMiddleNode;
        if (branch != null) branch.parent = newMiddleNode.rightChild;
        newMiddleNode.parent = originalHeadNode;
        
        return LeftRotation(originalHeadNode);
    }

    private AVLNode LeftRightRotation(AVLNode originalHeadNode) {
        var newMiddleNode = originalHeadNode.leftChild.rightChild;
        var branch = newMiddleNode.leftChild;
        
        //do rotation
        newMiddleNode.leftChild = originalHeadNode.leftChild;
        newMiddleNode.leftChild.rightChild = branch;
        originalHeadNode.leftChild = newMiddleNode;
        
        //update parent nodes
        newMiddleNode.leftChild.parent = newMiddleNode;
        if (branch != null) branch.parent = newMiddleNode.leftChild;
        newMiddleNode.parent = originalHeadNode;
        
        return RightRotation(originalHeadNode);
    }

    private AVLNode LeftRotation(AVLNode originalHeadNode) {
        var newHeadNode = originalHeadNode.rightChild;
        var branch = originalHeadNode.rightChild.leftChild;
        var parentNode = originalHeadNode.parent;
        
        //position the head node according to parent of previous head node.
        if (parentNode.rightChild != null && parentNode.rightChild.Position == originalHeadNode.Position) {
            parentNode.rightChild = newHeadNode;
        }
        else {
            parentNode.leftChild = newHeadNode;
        }

        //do rotation
        newHeadNode.leftChild = originalHeadNode;
        originalHeadNode.rightChild = branch;
        
        //update parent nodes
        newHeadNode.parent = parentNode;
        originalHeadNode.parent = newHeadNode;
        if (branch != null) branch.parent = originalHeadNode;
        
        //update depth of head node
        newHeadNode.Depth--;
        
        //return head node
        return newHeadNode;
    }

    private AVLNode RightRotation(AVLNode originalHeadNode) {
        var newHeadNode = originalHeadNode.leftChild;
        var branch = originalHeadNode.leftChild.rightChild;
        var parentNode = originalHeadNode.parent;
        
        //position the head node according to parent of previous head node.
        if (parentNode.rightChild != null && parentNode.rightChild.Position == originalHeadNode.Position) {
            parentNode.rightChild = newHeadNode;
        }
        else {
            parentNode.leftChild = newHeadNode;
        }

        //do rotation
        newHeadNode.rightChild = originalHeadNode;
        originalHeadNode.leftChild = branch;
        
        //update parent nodes
        newHeadNode.parent = parentNode;
        originalHeadNode.parent = newHeadNode;
        if (branch != null) branch.parent = originalHeadNode;
        
        //update depth of head node
        newHeadNode.Depth--;
        
        //return head node
        return newHeadNode;
    }

    private void InsertNodeRecursive(ref AVLNode childNode, ref int value, ref Vector2 insertedPosition,
        ref Vector2 parentPosition, BinaryTreeNode parentNode, Dictionary<string,List<Vector2>> balancedNodesPositions, int childDirection) {
        parentPosition = insertedPosition;
        // divido por n para q no se superpongan los hijos de nodos hermanos.
        insertedPosition += new Vector2(childDirection * (GameManager.TREE_X_OFFSET - parentNode.Depth), -GameManager.TREE_Y_OFFSET);
        Insert(ref childNode, ref value, ref insertedPosition, ref parentPosition, balancedNodesPositions, parentNode.Depth, parentNode);
    }
    
    public ITreeNode ChangeTreeType() {
        return new BSTNode {Value = -1, Position = new Vector2(-GameManager.TREE_X_OFFSET, GameManager.TREE_Y_OFFSET)};
    }
}