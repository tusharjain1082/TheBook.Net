using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryJournal.Net
{
    // tree's node
    public class myTreeDomNode
    {
        public List<myTreeDomNode> children = new List<myTreeDomNode>();
        public myTreeDomNode? parent;
        public myNode? self;
        public Int64 previousID = 0;
        //public bool isPurged = false;
    }

    public class myTreeDom
    {
        // entire tree structure
        public List<myTreeDomNode> tree = new List<myTreeDomNode>();

        // this method deletes the node by id
        public bool DeleteNodeRecursive(Int64 id)
        { 
            myTreeDomNode? node = findNodeRecursive(id);
            if (node != null)
            {
                myTreeDomNode? parent = node.parent;
                if (parent == null)
                    return tree.Remove(node); // root node, remove from root of tree
                else
                    return parent.children.Remove(node); // a child node in some parent node, remove from parent.
            }
            // node not found, return error
            return false;
        }

        // this method finds the node by id
        public myTreeDomNode? findNodeRecursive(Int64 id)
        {
            Queue<myTreeDomNode> queue = new Queue<myTreeDomNode>();

            // first enqueue all root nodes
            foreach (myTreeDomNode rootNode in tree)
            {
                if (rootNode.self.chapter.Id == id)
                    return rootNode;

                queue.Enqueue(rootNode);
            }

            while (queue.Count > 0)
            {
                myTreeDomNode currentNode = queue.Dequeue();

                if (currentNode.self.chapter.Id == id)
                    return currentNode;

                foreach (myTreeDomNode childNode in currentNode.children)
                {
                    if (childNode.self.chapter.Id == id)
                        return childNode;

                    queue.Enqueue(childNode);
                }

            }
            return null;
        }
    }
}
