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
    protected sealed override BinaryTreeNode LeftChild {
        get => leftChild;
        set => leftChild = (BSTNode)value;
    }

    ///i make sure that i can cast BinaryTreeNodes to my node type (BSTNode)
    protected sealed override BinaryTreeNode RightChild {
        get => rightChild;
        set => rightChild = (BSTNode)value;
    }
    
    protected sealed override BinaryTreeNode Parent {
        get => parent;
        set => parent = (BSTNode)value;
    }

    public BSTNode() { leftChild = null; rightChild = null; }

    public void DeleteNode(ITreeNode node, Dictionary<string, string> textNodesUpdate, out Vector2 deletedNodePosition) {
        DeleteNode((BSTNode) node, textNodesUpdate, out deletedNodePosition);
    }

    public ITreeNode Insert(ITreeNode node, ref int value, ref Vector2 insertedPosition, ref Vector2 parentPosition, int n = -1, BSTNode parent = null) {
        return Insert((BSTNode) node, ref value, ref insertedPosition, ref parentPosition);
    }

    private BSTNode Insert(BSTNode bstNode, ref int value, ref Vector2 insertedPosition, ref Vector2 parentPosition, int n = -1, BSTNode parent = null) {
        n++;
        if (n > GameManager.MAX_HEIGHT) {
            throw new HeightTreeLimitException("Max tree height reached, cant insert node.");
        }
        if (bstNode == null) {
            // hardcodeo para no tener 2 nodos superpuestos. no se coloca si ya hay uno ahi.
            if (GameManager.instance.IsPositionOccupied(insertedPosition)) {
                throw new PositionOccupiedByNodeException("Position already occupied by another node.");
            }
            //Debug.Log("nuevo en n = " + n + " - vale : " + value + "\n");
            bstNode = new BSTNode {Position = insertedPosition, Value = value, Parent = parent};
            //Debug.Log("inserte " + treeNode.Value + "\n");
        }
        else if (value > bstNode.Value) {
            //Debug.Log("pasada r en n = " + n + " - vale : " + this.Value + "\n");
            parentPosition = insertedPosition;
            // divido por n para q no se superpongan los hijos de nodos hermanos.
            insertedPosition += new Vector2(GameManager.TREE_X_OFFSET - n, -GameManager.TREE_Y_OFFSET);
            //Debug.Log("inserta" + value + " en derecha de " + treeNode.Value + " parent position: " + parentPosition.x + "," + parentPosition.y+"\n");
            bstNode.rightChild = Insert(bstNode.rightChild, ref value, ref insertedPosition, ref parentPosition, n, bstNode);

            /*Debug.Log("el hijo derecho de " + root.value + " es " + rightChild + " y su valor es " + rightChild.value + "\n");*/
        } else {
            //Debug.Log("pasada l en n = " + n + " - vale : " + this.Value + "\n");
            parentPosition = insertedPosition;
            // divido por n para q no se superpongan los hijos de nodos hermanos.
            insertedPosition += new Vector2(-(GameManager.TREE_X_OFFSET - n), -GameManager.TREE_Y_OFFSET);
            //Debug.Log("inserta" + value + " en izquierda de " + treeNode.Value + " parent position: " + parentPosition.x + "," + parentPosition.y+ "\n");
            bstNode.leftChild = Insert(bstNode.leftChild, ref value, ref insertedPosition, ref parentPosition, n, bstNode);
            /*Debug.Log("el hijo izquierdo de " + root.value + " es " + leftChild + " y su valor es " + leftChild.value + "\n");*/
        }

        return bstNode;
    }

    private void DeleteNode(BSTNode bstNode, Dictionary<string, string> textNodesUpdate, out Vector2 deletedNodePosition) {
        if (bstNode.rightChild == null && bstNode.leftChild == null) {
            deletedNodePosition = bstNode.Position;
            //deletes node from the tree, leaving parent without corresponding child.
            if (bstNode.parent.rightChild != null && bstNode.parent.rightChild.Position == bstNode.Position) bstNode.parent.rightChild = null;
            else {
                bstNode.parent.leftChild = null;
            }
        }
        else if (bstNode.leftChild != null && bstNode.rightChild == null) {
            //reeplace node values with max value left branch
            //debugs
            //Debug.Log("estoy borrando el-----: " + treeNode.Value + " por izquierda" + "\n");
            //Debug.Log("por el numero: " + treeNode.leftChild.Value+ "\n");
            var key = TreeContainer.MakeNodeKey(bstNode.Position);
            var maxLeft = (BSTNode)FindMax(bstNode.leftChild);
            bstNode.Value = maxLeft.Value;
            textNodesUpdate.Add(key, bstNode.Value.ToString(CultureInfo.InvariantCulture));
            
            DeleteNode(maxLeft, textNodesUpdate, out deletedNodePosition);
        }
        else {
            //reeplace node values with min value right branch
            //debugs
            //Debug.Log("estoy borrando el-----: " + treeNode.Value + " por derecha" + "\n");
            //Debug.Log("por el numero: " + treeNode.rightChild.Value+ "\n");
            var key = TreeContainer.MakeNodeKey(bstNode.Position);
            var minRight = (BSTNode)FindMin(bstNode.rightChild);
            bstNode.Value = minRight.Value;
            textNodesUpdate.Add(key, bstNode.Value.ToString(CultureInfo.InvariantCulture));
            
            DeleteNode(minRight, textNodesUpdate, out deletedNodePosition);
        }
    }
}