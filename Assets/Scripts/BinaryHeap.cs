internal class BinaryHeap {
    IGraphNode[] arr;
    int  sizeOfTree;

    public BinaryHeap(int size) {
        arr = new IGraphNode[size + 1];
        sizeOfTree = 0;
    }
    
    public int SizeOfHeap(){
        return sizeOfTree;
    }
    
    public void InsertElementInHeap(IGraphNode graphNode) {
        if (sizeOfTree + 1 >= arr.Length) ExtendHeap();
        arr[sizeOfTree + 1] = graphNode;
        sizeOfTree++;
        HeapifyBottomToTop(sizeOfTree);
    }

    private void ExtendHeap() {
        IGraphNode[] bufferArr = new IGraphNode[arr.Length*2];
        for (int i = 0; i < arr.Length; i++) {
            bufferArr[i] = arr[i];
        }

        arr = bufferArr;
    }

    private void HeapifyBottomToTop(int index) {
        if (index <= 1) return;
          
        int parent = index / 2;
        // If Current value is smaller than its parent, then swap
        if (arr[index].Value < arr[parent].Value) {
            IGraphNode tmp = arr[index];
            arr[index] = arr[parent];
            arr[parent] = tmp;
        }
        HeapifyBottomToTop(parent);
    }
    
    public IGraphNode ExtractHeadOfHeap() {
        if(sizeOfTree == 0) return null;
        
        IGraphNode extractedValue = arr[1];
        arr[1] = arr[sizeOfTree];
        sizeOfTree--;
        HeapifyTopToBottom(1);
        return extractedValue;
    }
  
    private void HeapifyTopToBottom(int nodeIndex) {
        int leftChildIndex  = nodeIndex * 2;
        int rightChildIndex = (nodeIndex * 2) + 1;
        //has no children
        if (sizeOfTree < leftChildIndex) return;
        //has left child only
        if (sizeOfTree == leftChildIndex) {
            if (!(arr[nodeIndex].Value > arr[leftChildIndex].Value)) return;
            IGraphNode tmp = arr[nodeIndex];
            arr[nodeIndex] = arr[leftChildIndex];
            arr[leftChildIndex] = tmp;
            return;
        }
        int smallestChild = 0;
        //has both children
        smallestChild = arr[leftChildIndex].Value < arr[rightChildIndex].Value ? leftChildIndex : rightChildIndex;
        
        if (arr[nodeIndex].Value > arr[smallestChild].Value) {
            IGraphNode tmp = arr[nodeIndex];
            arr[nodeIndex] = arr[smallestChild];
            arr[smallestChild] = tmp;
        }
        HeapifyTopToBottom(smallestChild);
    }
}