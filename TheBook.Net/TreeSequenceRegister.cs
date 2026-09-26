using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;

namespace TheBook.Net
{
    public class Node<T>
    {
        public Int64 Id = 0;
        public Int64 parentId = 0;
        public Int64 treeSeqHeadId = 0;
        public Int64 treeSeqTailId = 0;
        public Int64 treeSeqNextId = 0;
        public Int64 treeSeqPrevId = 0;
        public T Data;
        public Node<T> Next;
        public Node<T> Prev;


        public Node(T data)
        {
            Data = data;
            Next = null;
            Prev = null;
        }
    }
    public class DoublyLinkedList<T>
    {
        private Node<T> Head;
        private Node<T> Tail;
        private int Size;

        public DoublyLinkedList()
        {
            Head = null;
            Tail = null;
            Size = 0;
        }
        // ... operations will go here ...
        public void InsertEnd(T data)
        {
            Node<T> newNode = new Node<T>(data);
            if (Head == null)
            {
                Head = newNode;
                Tail = newNode;
            }
            else
            {
                Tail.Next = newNode;
                newNode.Prev = Tail;
                Tail = newNode;
            }
            Size++;
        }
        public void InsertBeginning(T data)
        {
            Node<T> newNode = new Node<T>(data);
            if (Head == null)
            {
                Head = newNode;
                Tail = newNode;
            }
            else
            {
                newNode.Next = Head;
                Head.Prev = newNode;
                Head = newNode;
            }
            Size++;
        }
        public void TraverseForward()
        {
            Node<T> current = Head;
            while (current != null)
            {
                Console.Write(current.Data + " <-> ");
                current = current.Next;
            }
            Console.WriteLine("NULL");
        }
        public void TraverseReverse()
        {
            Node<T> current = Tail;
            while (current != null)
            {
                Console.Write(current.Data + " <-> ");
                current = current.Prev;
            }
            Console.WriteLine("NULL");
        }


    }
    public class TreeSequenceRegister
    {

        
    }
}
