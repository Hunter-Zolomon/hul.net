using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace HUL
{
    class DLinkedListEnumerator : IEnumerator<object>
    {
        protected DNode Head = null;
        protected DNode _current = null;
        public object Current => _current.Data;
        internal DNode CurrentNode => _current;

        public DLinkedListEnumerator(DNode headNode) { Head = _current = new DNode(null) { RPointer = headNode }; }

        public void Dispose() { Head = null; _current = null; }

        public bool MoveNext()
        {
            if (_current.RPointer is null) { return false; }
            else { _current = _current.RPointer; return true; }
        }

        public void Reset() { _current = Head; }

        object IEnumerator.Current => Current;
    }

    class SLinkedListEnumerator : IEnumerator<object>
    {
        protected Node Head = null;
        protected Node _current = null;
        public object Current => _current.Data;
        internal Node CurrentNode => _current;
        
        public SLinkedListEnumerator(Node headNode) { Head = _current = new Node(null) { Pointer = headNode}; }

        public void Dispose() { Head = null; _current = null; }

        public bool MoveNext()
        {
            if (_current.Pointer is null) { return false; }
            else { _current = _current.Pointer; return true; }
        }

        public void Reset() { _current = Head; }

        object IEnumerator.Current => Current;
    }

    public class HashTable<T>
    {
        private DLList<Package>[] _table; /* changed DLinkedList to DlinkedList<dynamic> */
        private int _size;

        public struct Package
        {
            public T Data;
            public string Key;
        }

        private static Int64 MultiHashFunction(Int64 key, int size)
        {
            var a = new Random().NextDouble();
            return (int)Math.Floor(size * ((key * a) - Math.Floor(key * a)));
        }

        private static Int64 DivHashFunction(Int64 key, int size) { return Math.Abs(key % size); }

        private static string HexHashFunction(string data)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(data));

            for (int i = 0; i < bytes.Length / 2; i++) { hash.Append(bytes[i].ToString("x2")); }

            return hash.ToString();
        }

        public int TestCounter()
        {
            int counter = 0;
            for (int i = 0; i < _size; i++)
            {
                if (!(_table[i].IsEmpty())) { counter += 1; }
            }
            return counter;
        }

        public void CreateTable(int size)
        {
            T[] InitializeArray<T>(int length) where T : new()
            {
                T[] array = new T[length];
                for (int i = 0; i < length; ++i) { array[i] = new T(); }
                return array;
            }
            _table = InitializeArray<DLList<Package>>(size); /* changed DLinkedList to DlinkedList<dynamic> */
            _size = size;
        }

        public void Insert(T data, string key)
        {
            Int64 index = DivHashFunction(Int64.Parse(HexHashFunction(key).ToUpper(), NumberStyles.HexNumber), _size);
            _table[index].Append(new Package(){ Data = data, Key = key});
        }

        public T FindRecord(string searchKey)
        {
            Int64 index = DivHashFunction(Int64.Parse(HexHashFunction(searchKey).ToUpper(), NumberStyles.HexNumber), _size);
            for (int i = 0; i < _table[index].Size/*Size()*/; i++)
            {
                if (((Package)_table[index][i]).Key == searchKey)
                {
                    Debug.WriteLine("Package Found!");
                    return ((Package)_table[index][i]).Data;
                }
            }

            Debug.WriteLine("Package Not Found!");
            return default(T);
        }

        public void PrintHashTable(int delay = 0)
        {
            for (int i = 0; i < _table.Length - 1; i++)
            {
                foreach (var VARIABLE in _table[i])
                {
                    Thread.Sleep(delay);
                    Console.Write(((Package)VARIABLE).Data + " --- ");
                }
                Console.WriteLine();
            }
        }
    }

    public class Node
    {
        public dynamic Data;
        public Node Pointer;

        public Node(dynamic Data)
        {
            this.Data = Data;
            Pointer = null;
        }
    }

    public class DNode
    {
        public dynamic Data;
        public DNode LPointer;
        public DNode RPointer;

        public DNode(dynamic Data)
        {
            this.Data = Data;
            LPointer = null;
            RPointer = null;
        }
    }

    public class AVLNode
    {
        public dynamic Data;
        public int Balance;
        public AVLNode Parent;
        public AVLNode LPointer;
        public AVLNode RPointer;

        public AVLNode(dynamic Data)
        {
            this.Data = Data;
            Parent = null;
            LPointer = null;
            RPointer = null;
        }
    }

    /* Linked List implementation
        AddBefore
        CopyTo (Identical to ToArray. Implement if needed)
        GetObjectData
        OnDeserialization
        */

    public class SLList<T> : IEnumerable<object>
    {
        private Node _head;
        private Node _tail;
        public int Size { get; internal set; }

        public void Push(T data)
        {
            Node newNode = new Node(data) {Pointer = _head};
            if (_head != null) { _head = newNode; }
            else { _head = newNode; _tail = newNode; }
            Size++;
        }

        public void Append(T data)
        {
            Node newNode = new Node(data);
            if (_head == null) { _head = newNode; _tail = newNode; Size++; }
            else
            {
                //while (last.Pointer != null) { last = last.Pointer; } head changed to tail so no ite needed
                _tail.Pointer = newNode;
                _tail = _tail.Pointer;
                Size++;
            }
        }

        public void InsertAfter(Node prevNode, T data)
        {
            if (prevNode == null) { throw new ArgumentException("The Node value is invalid!", nameof(prevNode)); }
            else if (prevNode == _tail) { Append(data); Size++; }
            else { Node newNode = new Node(data) { Pointer = prevNode.Pointer }; prevNode.Pointer = newNode; Size++; }
        }

        public void Clear()
        {
            Node temp = _head;
            Node deleter = _head;
            while (temp != null) { temp = temp.Pointer; deleter = null; deleter = temp; }
            _head = deleter = temp = _tail = null;
            Size = 0;
        }

        public List<int> Contains(T item) /*changed List<T> to <int> */
        {
            List<int> returnList = new List<int>();
            for (int i = 0; i < Size; i++)
            {
                if (this[i].ToString() == item.ToString()) { returnList.Add(i); }
            }
            //Node temp = _head;
            //while (temp != null)
            //{
            //    if (object.Equals(temp.Data, item)/*_AStack[i] == item*/) {/*temp.Data added instead of temp */ returnList.Add(temp.Data); temp = temp.Pointer; }
            //    else { temp = temp.Pointer; }
            //}
            return returnList;
        }

        public bool IsEmpty() { if (_head == null) { return true; } else { return false; } }

        public int IndexOf(T key)
        {
            int counter = 0;
            foreach (T item in this)
            {
                if (item.ToString() != key.ToString()) { counter++; }
                else { return counter; }
            }

            return -1;
        }

        public void Remove(T key)
        {
            Node temp = _head;
            if (temp != null)
            {
                if (temp.Data == key)
                {
                    _head = temp.Pointer;
                    temp = null;
                    Size--;
                    return;
                }
            }
            Node prev = temp;
            temp = temp.Pointer;
            while (temp != null)
            {
                if (temp.Data == key) { break; }
                prev = temp;
                temp = temp.Pointer;
            }
            if (temp == null) { throw new Exception("Key wasn't found in LL"); }

            prev.Pointer = temp.Pointer;
            temp = null;
            Size--;
        }

        //public IEnumerator<object> GetEnumerator() { return new LinkedListEnumerator(); }

        //IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        //public object this[int index]
        //{
        //    get
        //    {
        //        var enumerator = (LinkedListEnumerator)GetEnumerator();
        //        for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { return enumerator.Current; } }
        //        throw new IndexOutOfRangeException();
        //    }
        //    set
        //    {
        //        var enumerator = (LinkedListEnumerator)GetEnumerator();
        //        for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { enumerator.CurrentNode.Data = value; return; } }
        //        throw new IndexOutOfRangeException();
        //    }
        //}

        public IEnumerator<object> GetEnumerator() { return new SLinkedListEnumerator(_head); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public object this[int index]
        {
            get
            {
                var enumerator = (SLinkedListEnumerator)GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { return enumerator.Current; } }
                throw new IndexOutOfRangeException();
            }
            set
            {
                var enumerator = (SLinkedListEnumerator)GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { enumerator.CurrentNode.Data = value; return; } }
                throw new IndexOutOfRangeException();
            }
        }

        public IReadOnlyCollection<T> ReadOnly()
        {
            var returnedArray = ToArray();
            return new ReadOnlyCollection<T>(returnedArray);
        }

        /* Now a Property instead of a method */
        //public int Size()
        //{
        //    int counter = 0;
        //    Node temp = _head;
        //    while (temp.Pointer != null) { counter++; temp = temp.Pointer; }

        //    return counter;
        //}

        public void RemoveFirst() { if (_head != null) { _head = _head.Pointer; Size--; } }

        public void RemoveLast()
        {
            Node temp = _head;
            Node deleter = _head;
            if (_head != null)
            {
                while (temp.Pointer != null) { deleter = temp; temp = temp.Pointer; }
                temp = null;
                deleter.Pointer = null;
                Size--;
            }
        }

        public T ReturnLast()
        {
            Node temp = _head;
            if (_head != null)
            {
                while (temp.Pointer != null) { temp = temp.Pointer; }
                return temp.Data;
            }
            else { throw new Exception("List is Empty!"); }
        }

        public T[] ToArray()
        {
            T[] returnarray;
            List<T> tempList = new List<T>();
            Node temp = _head;
            while (temp != null) { tempList.Add(temp.Data); temp = temp.Pointer; }
            returnarray = tempList.ToArray();
            return returnarray;
        }

        public void PrintList(int delay = 0)
        {
            Node temp = _head;
            while (temp.Pointer != null)
            {
                Thread.Sleep(delay);
                Console.WriteLine(temp.Data);
                temp = temp.Pointer;
            }
            Console.WriteLine(temp.Data);
        }
    }

    public class DLList<T> : IEnumerable<object>
    {
        private DNode _head;
        private DNode _tail;
        public int Size { get; internal set; }

        public void Push(T data)
        {
            if (IsEmpty()) { DNode newNode = new DNode(data); _head = newNode; _tail = newNode; Size++; }
            else
            {
                DNode newNode = new DNode(data);
                _head.LPointer = newNode;
                newNode.RPointer = _head;
                _head = newNode;
                Size++;
            }
        }

        public void Append(T data)
        {
            if (IsEmpty()) { DNode newNode = new DNode(data); _head = newNode; _tail = newNode; Size++; }
            else
            {
                DNode temp = _tail;
                DNode newNode = new DNode(data);
                //while (temp.RPointer != null) { temp = temp.RPointer; } head changed to tail no iti needed
                temp.RPointer = newNode;
                _tail = newNode;
                newNode.LPointer = temp;
                Size++;
            }
        }

        public void InsertAfter(DNode prevNode, T data)
        {
            if (IsEmpty()) { DNode newNode = new DNode(data); _head = newNode; _tail = newNode; Size++; }
            else if (prevNode == _tail) { Append(data); Size++; }
            else
            {
                DNode newNode = new DNode(data);
                DNode temp1 = _head;
                DNode temp2;
                while (temp1 != prevNode) { temp1 = temp1.RPointer; if (temp1 == null) { Console.WriteLine("Node not found!"); } return; }
                temp2 = temp1.RPointer;
                temp1.RPointer = newNode;
                newNode.LPointer = temp1;
                newNode.RPointer = temp2;
                temp2.LPointer = newNode;
                Size++;
            }
        }

        public void Clear()
        {
            DNode temp = _head;
            while (temp.RPointer != null) { temp = temp.RPointer; temp.LPointer = null; }
            _head = temp = _tail = null;
            Size = 0;
        }

        public List<int> Contains(T item) /*changed List<T> to <int> */
        {
            List<int> returnList = new List<int>();
            for (int i = 0; i < Size; i++)
            {
                if (this[i].ToString() == item.ToString()) { returnList.Add(i); }
            }
            //DNode temp = _head;
            //int counter = 0;
            //if (IsEmpty()) { Console.WriteLine("List is Empty! Search not possible"); return returnList; }
            //while (temp != null)
            //{
            //    if (object.Equals(temp.Data, item)/*_AStack[i] == item*/) { returnList.Add(counter); temp = temp.RPointer; }
            //    else { temp = temp.RPointer; counter++; }
            //}
            return returnList;
        }

        public bool IsEmpty() { if (_head == null) { return true; } else { return false; } }

        public int IndexOf(T key)
        {
            int counter = 0;
            foreach (T item in this)
            {
                if (item.ToString() != key.ToString()) { counter++; }
                else { return counter; }
            }

            return -1;
        }

        public void Remove(T key)
        {
            if (IsEmpty()) { throw new Exception("List Empty! Deletion not possible"); }
            else
            {
                DNode temp = _head;
                while (temp.Data != key) { temp = temp.RPointer; if (temp == null) { Console.WriteLine("Node not found!"); return; } }
                if (temp.LPointer == temp.RPointer) { _head = null; temp = null; Size--; return; }
                else
                {
                    if (temp == _head) { _head = _head.RPointer; _head.LPointer = null; temp = null; Size--; return; }
                    else if (temp.RPointer == null) { temp.LPointer.RPointer = null; temp.LPointer = temp.RPointer = null; temp = null; Size--; return; }
                    else
                    {
                        temp.LPointer.RPointer = temp.RPointer;
                        temp.RPointer.LPointer = temp.LPointer;
                        temp = null;
                        Size--;
                    }
                }
            }
        }

        public IEnumerator<object> GetEnumerator() { return new DLinkedListEnumerator(_head); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public object this[int index]
        {
            get
            {
                var enumerator = (DLinkedListEnumerator)GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { return enumerator.Current; } }
                throw new IndexOutOfRangeException();
            }
            set
            {
                var enumerator = (DLinkedListEnumerator)GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { enumerator.CurrentNode.Data = value; return; } }
                throw new IndexOutOfRangeException();
            }
        }

        public IReadOnlyCollection<T> ReadOnly()
        {
            var returnedArray = ToArray();
            return new ReadOnlyCollection<T>(returnedArray);
        }

        /* Now a property instead of a method */
        //public int Size()
        //{
        //    int counter = 0;
        //    DNode temp = _head;
        //    while (temp != null) { counter++; temp = temp.RPointer; }

        //    return counter;
        //}

        public T ReturnFirst() { return _head.Data; }

        public T ReturnLast() { return _tail.Data; }

        public void RemoveFirst()
        {
            if (IsEmpty()) { throw new Exception("List Empty! Deletion not possible"); }
            else
            {
                DNode temp = _head;
                if (temp.LPointer == temp.RPointer) { _head = null; temp = null; Size--; }
                else { _head = temp.RPointer; _head.LPointer = null; temp = null; Size--; }
            }
        }

        public void RemoveLast()
        {
            if (IsEmpty()) { throw new Exception("List Empty! Deletion not possible"); }
            else
            {
                DNode temp = _tail;
                if (temp.LPointer == temp.RPointer) { _head = null; temp = null; _tail = null; Size--; }
                else
                {
                    //while (temp.RPointer != null) { temp = temp.RPointer; } head changed to tail so no ite needed
                    _tail = temp.LPointer;
                    temp.LPointer.RPointer = null;
                    temp.LPointer = temp.RPointer = null;
                    temp = null;
                    Size--;
                }
            }
        }

        public T[] ToArray()
        {
            T[] returnarray;
            List<T> tempList = new List<T>();
            DNode temp = _head;
            while (temp != null) { tempList.Add(temp.Data); temp = temp.RPointer; }
            returnarray = tempList.ToArray();
            return returnarray;
        }

        public void PrintList(int delay = 0)
        {
            if (IsEmpty()) { Console.WriteLine("List is Empty! Print not possible"); }
            else
            {
                DNode temp = _head;
                Console.Write("NULL <--- ");
                while (temp.RPointer != null) { Thread.Sleep(delay); Console.Write(temp.Data + " <==> "); temp = temp.RPointer; }
                Console.Write(temp.Data + " ---> NULL");
            }
        }

        public void ReversePrintList(int delay = 0)
        {
            if (IsEmpty()) { Console.WriteLine("List is Empty! Print not possible"); }
            else
            {
                DNode temp = _head;
                while (temp.RPointer != null) { temp = temp.RPointer; }
                Console.Write("Reverse: NULL <--- ");
                while (temp.LPointer != null) { Thread.Sleep(delay); Console.Write(temp.Data + " <==> "); temp = temp.LPointer; }
                Console.Write(temp.Data + " ---> NULL");
            }
        }
    }

    /* Stack Implementations
         CopyTo(Identical to ToArray, Implement if needed)
         */

    public class AStack<T>
    {
        private int _top;
        private dynamic[] _AStack;
        private int _arraySize;
        /* _arraySize readonly removed for TrimmedExcess() */

        public AStack(int size)
        {
            _arraySize = size;
            _AStack = new dynamic[_arraySize];
            _top = -1;
        }

        public int Count()
        {
            int Counter = 0;
            foreach (dynamic element in _AStack) { if (element != null) { Counter++; } }
            return Counter;
        }
        
        public void Clear() { for (int i = 0; i < _AStack.Length; i++) { _AStack[i] = null; } }

        public SLList<int> Contains(T item)
        {
            SLList<int> returnList = new SLList<int>();
            for (int i = 0; i < _AStack.Length; i++)
            {
                if (object.Equals(_AStack[i], item)/*_AStack[i] == item*/) { returnList.Append(i); }
            }
            return returnList;
        }

        public bool IsEmpty() { if (_top == -1) { return true; } else { return false; } }

        public dynamic Peek() { return _AStack[_top]; }

        public dynamic[] ToArray(T[] array, int index)
        {
            /* Change _Astack.Length */
            if (array.Length - index >= _AStack.Length)
            {
                for (int i = index; i < (_AStack.Length + index) - 1; i++) { array[i] = _AStack[i - index]; }
                return _AStack;
            }
            else { throw new ArgumentException("Array too small!", nameof(array)); }
        }
        /* Obsolete Method 
        public AStack TrimExcess(AStack stack)
        {
            if (stack.Count() < 0.9 * stack._arraySize)
            {
                AStack trimmedStack = new AStack(stack.Count());
                for (int i = 0; i < stack.Count(); i++) { trimmedStack.Push(stack.Pop()); }
                stack.Clear();
                return trimmedStack;
            }
            else { return stack; }
        } */

        public T this[int index]
        {
            get { return _AStack[index]; throw new IndexOutOfRangeException(); }
        }

        public void TrimExcess()
        {
            int threshhold = (int) (((double) _arraySize) * 0.9);
            if (Count() < threshhold)
            {
                dynamic[] trimmedArray = new dynamic[Count()];
                for (int i = 0; i < Count(); i++) { trimmedArray[i] = _AStack[i]; }
                _AStack = trimmedArray;
                _arraySize = Count();
            }
        }

        public void Push(T data)
        {
            if (_top == _arraySize - 1) { throw new Exception("Stack seems to be Full! No APush Possible"); }
            else { _top++; _AStack[_top] = data; }
        }

        public dynamic Pop()
        {
            dynamic popHolder;
            if (_top == -1) { throw new Exception("Stack seems to be empty! No APop Possible"); }
            else { popHolder = _AStack[_top]; _AStack[_top] = null; _top--; }
            return popHolder;
        }

        public dynamic ReturnElement(int index) { return _AStack[index]; }

        public int Size() { return _arraySize; }

        public void PrintAStack(int delay = 0)
        {
            if (_top == -1) { throw new Exception("Stack seems to be empty! No APrintStack Possible"); }
            else
            {
                var i = _top;
                while (i != -1)
                {
                    /*Fix Exception*/
                    try
                    {
                        Thread.Sleep(delay);
                        Console.WriteLine(_AStack[i]);
                        i--;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("No Present Object to print");
                        throw;
                    }
                }
            }
        }
    }

    public class LLStack<T> : IEnumerable<object>
    {
        private Node _top;
        public int Size { get; internal set; }

        /* Removed for Size property */ 
        //public int Count()
        //{
        //    int Counter = 0;
        //    Node temp = _top;
        //    while (temp != null) { if (temp.Data != null) { Counter++; temp = temp.Pointer; } }
        //    return Counter;
        //}

        public void Clear()
        {
            Node temp = _top;
            Node deleter = _top;
            while (temp != null) { temp = temp.Pointer; deleter = null; deleter = temp; }
            _top = deleter = temp = null;
            Size = 0;
        }

        public SLList<int> Contains(T item) /* Changed from <Node> to <int> */
        {
            SLList<int> returnList = new SLList<int>();
            //Node temp = _top;
            //while (temp != null)
            //{
            //    if (object.Equals(temp.Data, item)/*_AStack[i] == item*/) { returnList.Append(temp); temp = temp.Pointer; }
            //    else { temp = temp.Pointer; }
            //}
            for (int i = 0; i < Size; i++) {if (object.Equals(this[i], item)) { returnList.Append(i); } }
            return returnList;
        }

        public IEnumerator<object> GetEnumerator() { return new SLinkedListEnumerator(_top); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public object this[int index]
        {
            get
            {
                var enumerator = (SLinkedListEnumerator)GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { return enumerator.Current; } }
                throw new IndexOutOfRangeException();
            }
        }

        public bool IsEmpty() { if (_top == null) { return true; } else { return false; } }

        public T Peek() { return _top.Data; }

        public void Push(T data)
        {
            Node newNode = new Node(data);
            if (_top == null) { newNode.Pointer = null; }
            else if (_top != null) { newNode.Pointer = _top; }
            _top = newNode;
            Size++;
        }

        public T Pop()
        {
            if (_top == null) { throw new Exception("Stack seems to be empty! No LLPop Possible");/*Console.WriteLine("Stack seems to be empty!"); return;*/ }
            else {Node temp = _top; _top = _top.Pointer; Size--; return temp.Data; }
        }

        public void PrintLLStack(int delay = 0)
        {
            if (_top == null) { throw new Exception("Stack seems to be empty! No LLPrintStack Possible"); /*Console.WriteLine("Stack seems to be empty!"); return;*/ }
            else
            {
                Node temp = _top;
                while (temp != null) { Thread.Sleep(delay); Console.WriteLine(temp.Data); temp = temp.Pointer; }
            }
        }

        public dynamic[] ToArray(T[] array, int index)
        {
            /* Change _Astack.Length */
            dynamic[] returnStack = new dynamic[array.Length];
            Node temp = _top;
            if (array.Length - index >= /*Count()*/ Size)
            {
                for (int i = index; i < (/*Count()*/ Size + index) - 1; i++) { array[i] = temp.Data; temp = temp.Pointer; }
                for (int j = 0; j < array.Length; j++) { returnStack[j] = array[j]; }
                return returnStack;
            }
            else { throw new ArgumentException("Array too small!", nameof(array)); }
        }
    }

    /* Queue Implementations
        CopyTo (Identical to ToArray implement if needed)
        */

    public class AQueue<T>
    {
        private int _aSize;
        private int _front;
        private int _rear;
        private dynamic[] _AQueue;
        /* _aSize readonly removed for TrimExcess */

        public AQueue(int size)
        {
            _aSize = size;
            _AQueue = new dynamic[_aSize];
            _front = -1;
            _rear = -1;
        }

        public void EnQueue(T data)
        {
            if (_rear == _aSize - 1) { throw new Exception("Queue seems to be full! No AenQueue Possible"); }
            else { _rear++; _AQueue[_rear] = data; }
        }

        public T DeQueue()
        {
            if (_front == _rear) { throw new Exception("Queue seems to be empty! No AdeQueue Possible"); }
            else {dynamic DeQueueHolder = _AQueue[_front]; _front++; if (_front == _rear) { _front = -1; _rear = -1; } return DeQueueHolder; }
        }

        public int Count()
        {
            int counter = 0;
            foreach (dynamic element in _AQueue) { if (element != null) { counter++; } }
            return counter;
        }

        public void Clear() { for (int i = 0; i < _AQueue.Length; i++) { _AQueue[i] = null; } }

        public SLList<int> Contains(T item)
        {
            SLList<int> returnList = new SLList<int>();
            for (int i = 0; i < _AQueue.Length; i++)
            {
                if (object.Equals(_AQueue[i], item)/*_AStack[i] == item*/) { returnList.Append(i); }
            }
            return returnList;
        }

        public bool IsEmpty() { if (_front == -1 && _rear == -1) { return true; } else { return false; } }

        public void GetEnumerator() { }

        public dynamic Peek() { return _AQueue[_front]; }

        public dynamic[] ToArray(T[] array, int index)
        {
            if (array.Length - index >= _AQueue.Length)
            {
                for (int i = index; i < (_AQueue.Length + index) - 1; i++) { array[i] = _AQueue[i - index]; }
                return _AQueue;
            }
            else { throw new ArgumentException("Array too small!", nameof(array)); }
        }

        public void TrimExcess()
        {
            int threshhold = (int)(((double)_aSize) * 0.9);
            if (Count() < threshhold)
            {
                dynamic[] trimmedArray = new dynamic[Count()];
                for (int i = 0; i < Count(); i++) { trimmedArray[i] = _AQueue[i]; }
                _AQueue = trimmedArray;
                _aSize = Count();
            }
        }

        public T this[int index]
        {
            get { return _AQueue[index]; throw new IndexOutOfRangeException(); }
        }

        public dynamic ReturnElement(int index) { return _AQueue[index]; }

        public int Size() { return _aSize; }

        public void PrintAQueue(int delay = 0)
        {
            if (_front == _rear) { throw new Exception("Queue seems to be empty! No PrintAQueue Possible"); }
            else { var i = _front + 1; while (i <= _rear) { Thread.Sleep(delay); Console.WriteLine(_AQueue[i]); i++; } }
        }
    }

    public class LLQueue<T> : IEnumerable<object>
    {
        private Node _front;
        private Node _rear;
        public int Size { get; internal set; }

        public void EnQueue(T data)
        {
            Node newNode = new Node(data);
            if (_rear == null) { _front = newNode; _rear = newNode; Size++; }
            else { _rear.Pointer = newNode; _rear = newNode; Size++; }
        }

        public T DeQueue()
        {
            if (_front == null) { throw new Exception("Queue seems to be empty! No LLdeQueue Possible"); /*Console.WriteLine("Queue seems to be empty!"); return;*/ }
            else
            {
                Node temp = _front;
                if (_front == _rear) { _front = _rear = _front.Pointer; Size--; }
                else { _front = _front.Pointer; Size--; }
                return temp.Data;
            }
        }

        /* Removed for Proeprty Size */
        //public int Count()
        //{
        //    int Counter = 0;
        //    Node temp = _front;
        //    while (temp != null) { if (temp.Data != null) { Counter++; temp = temp.Pointer; } }
        //    return Counter;
        //}

        public void Clear()
        {
            Node temp = _front;
            Node deleter = _front;
            while (temp != null) { temp = temp.Pointer; deleter = null; deleter = temp; }
            _front = _rear = deleter = temp = null;
            Size = 0;
        }

        public SLList<int> Contains(T item) /* <Node> Changed to <int> */
        {
            SLList<int> returnList = new SLList<int>();
            //Node temp = _front;
            //while (temp != null)
            //{
            //    if (object.Equals(temp.Data, item)/*_AStack[i] == item*/) { returnList.Append(temp); temp = temp.Pointer; }
            //    else { temp = temp.Pointer; }
            //}
            for (int i = 0; i < Size; i++) { if (object.Equals(this[i], item)) { returnList.Append(i); } }
            return returnList;
        }

        public IEnumerator<object> GetEnumerator() { return new SLinkedListEnumerator(_front); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public object this[int index]
        {
            get
            {
                var enumerator = (SLinkedListEnumerator)GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++) { if (i == index) { return enumerator.Current; } }
                throw new IndexOutOfRangeException();
            }
        }

        public bool IsEmpty() { if (_front == null && _rear == null) { return true; } else { return false; } }

        public T Peek() { return _front.Data; }

        public void PrintLLQueue(int delay = 0)
        {
            if (_front == null) { throw new Exception("Queue seems to be empty! No LLPrintQueue Possible"); /*Console.WriteLine("Queue seems to be empty!"); return;*/ }
            else
            {
                Node temp = _front;
                while (temp != null) {Thread.Sleep(delay); Console.WriteLine(temp.Data); temp = temp.Pointer; }
            }
        }

        public dynamic[] ToArray(T[] array, int index)
        {
            /* Change _Astack.Length */
            dynamic[] returnStack = new dynamic[array.Length];
            Node temp = _front;
            if (array.Length - index >= Size)
            {
                for (int i = index; i < (Size + index) - 1; i++) { array[i] = temp.Data; temp = temp.Pointer; }
                for (int j = 0; j < array.Length; j++) { returnStack[j] = array[j]; }
                return returnStack;
            }
            else { throw new ArgumentException("Array too small!", nameof(array)); }
        }
    }

    public class BST
    {
        private DNode _root;
        private Switcher status = Switcher.L;

        private enum Switcher { L = 1, R = 0 }

        private Switcher Switch(int integer, Switcher status)
        {
            if (integer == 1) { status = Switcher.L; return status; }
            else if (integer == 0) { status = Switcher.R; return status; }
            else { return status; }
        }

        public DNode Search(int data)
        {
            if (data == _root.Data) { Console.WriteLine("Node Found!"); return _root; }
            else
            {
                DNode checkerNode = _root;
                while ((checkerNode?.Data ?? data) != data)
                {
                    if (data <= checkerNode.Data) { checkerNode = checkerNode.LPointer; }
                    else { checkerNode = checkerNode.RPointer; }
                }
                if (checkerNode != null) { Console.WriteLine("Node Found!"); return checkerNode; }
                else { Console.WriteLine("Node Not Found!"); return new DNode(null); }
            }
        }

        public DNode Search(string data)
        {
            if (data == _root.Data) { Console.WriteLine("Node Found!"); return _root; }
            else
            {
                DNode checkerNode = _root;
                while ((checkerNode?.Data ?? data) != data)
                {
                    if (String.Compare(data, checkerNode.Data) <= 0) { checkerNode = checkerNode.LPointer; }
                    else { checkerNode = checkerNode.RPointer; }
                }
                if (checkerNode != null) { Console.WriteLine("Node Found!"); return checkerNode; }
                else { Console.WriteLine("Node Not Found!"); return new DNode(null); }
            }
        }

        public void Insert(int number)
        {
            DNode newNode = new DNode(number);
            if (_root == null) { _root = newNode; }
            else
            {
                DNode currentNode = _root;
                while (currentNode != null)
                {
                    if (newNode.Data <= currentNode.Data) { if (currentNode.LPointer != null) { currentNode = currentNode.LPointer; } else { currentNode.LPointer = newNode; break; } }
                    else if (newNode.Data > currentNode.Data) { if (currentNode.RPointer != null) { currentNode = currentNode.RPointer; } else { currentNode.RPointer = newNode; break; } }
                }
            }
        }

        public void Insert(string data)
        {
            DNode newNode = new DNode(data);
            if (_root == null) { _root = newNode; }
            else
            {
                DNode currentNode = _root;
                while (currentNode != null)
                {
                    if (String.Compare(newNode.Data, currentNode.Data) <= 0)
                    {
                        if (currentNode.LPointer != null) { currentNode = currentNode.LPointer; } else { currentNode.LPointer = newNode; break; }
                    }
                    else if (String.Compare(newNode.Data, currentNode.Data) > 0)
                    {
                        if (currentNode.RPointer != null) { currentNode = currentNode.RPointer; } else { currentNode.RPointer = newNode; break; }
                    }
                }
            }
        }

        public void Insert<T>(T data)
        {
            DNode newNode = new DNode(data);
            if (_root == null) { _root = newNode; }
            else
            {
                Random newRandom = new Random();
                DNode currentNode = _root;
                while (currentNode != null)
                {
                    if (status == Switcher.L)
                    {
                        if (currentNode.LPointer != null) { currentNode = currentNode.LPointer; status = (Switcher) newRandom.Next(2); }
                        else { currentNode.LPointer = newNode; status = (Switcher) newRandom.Next(2); break; }
                    }
                    else if (status == Switcher.R)
                    {
                        if (currentNode.RPointer != null) { currentNode = currentNode.RPointer; status = (Switcher) newRandom.Next(2); }
                        else { currentNode.RPointer = newNode; status = (Switcher)newRandom.Next(2); break; }
                    }
                }
            }
        }

        public void IEDelete<T>(T key)
        {
            DNode currentNode = _root;
            DNode temp = _root;
            while (currentNode.Data != key || currentNode != null)
            {
                if (key == currentNode.Data)
                {
                    if (currentNode.LPointer == null && currentNode.RPointer == null) { currentNode = null; }
                    else if (/*currentNode._LPointer != null &&*/ currentNode.RPointer == null)
                    {
                        if (temp.LPointer == currentNode) { temp.LPointer = currentNode.LPointer; }
                        else if (temp.RPointer == currentNode) { temp.RPointer = currentNode.LPointer; }
                    }
                    else if (currentNode.LPointer == null /*&& currentNode._RPointer != null*/)
                    {
                        if (temp.LPointer == currentNode) { temp.LPointer = currentNode.RPointer; }
                        else if (temp.RPointer == currentNode) { temp.RPointer = currentNode.RPointer; }
                    }
                    else if (currentNode.LPointer != null && currentNode.RPointer != null)
                    {
                        /*Implement double child deletion*/
                        DNode childHolderNode = currentNode.LPointer;
                        if (temp.LPointer == currentNode) {childHolderNode = currentNode.LPointer; temp.LPointer = currentNode.RPointer; currentNode = null; Insert(childHolderNode); }
                        else if (temp.RPointer == currentNode) { childHolderNode = currentNode.LPointer; temp.RPointer = currentNode.RPointer; currentNode = null; Insert(childHolderNode); }
                        else { throw new Exception("Unknown Error!"); }
                    }
                }
                else if (key <= currentNode.Data) { temp = currentNode; currentNode = currentNode.LPointer; }
                else if (key > currentNode.Data) { temp = currentNode; currentNode = currentNode.RPointer; }
            }
        }

        public DNode Root() { return _root; }

        public void ATraversal(DNode root, int delay = 0)
        {
            if (root.LPointer != null) { ATraversal(root.LPointer, delay); }
            Thread.Sleep(delay);
            Console.WriteLine(root.Data);
            if (root.RPointer != null) { ATraversal(root.RPointer, delay); }
        }

        public void DTraversal(DNode root, int delay = 0)
        {
            if (root.RPointer != null) { DTraversal(root.RPointer, delay); }
            Console.WriteLine(root.Data);
            if (root.LPointer != null) { DTraversal(root.LPointer, delay); }
        }
    }

    public class AVLT
    {
        private AVLNode _root;

        public void Clear() { _root = null; }

        private void InsertBalance(AVLNode node, int balance)
        {
            while (node != null)
            {
                balance = (node.Balance += balance);
                if (balance == 0) { return; }
                else if (balance == 2)
                {
                    if (node.LPointer.Balance == 1) { RotateRight(node); }
                    else { RotateLeftRight(node); }
                    return;
                }
                else if (balance == -2)
                {
                    if (node.RPointer.Balance == -1) { RotateLeft(node); }
                    else { RotateRightLeft(node); }
                    return;
                }

                AVLNode parent = node.Parent;

                if (parent != null) { balance = parent.LPointer == node ? 1 : -1; }

                node = parent;
            }
        }

        private AVLNode RotateLeft(AVLNode node)
        {
            AVLNode right = node.RPointer;
            AVLNode rightLeft = right.LPointer;
            AVLNode parent = node.Parent;

            right.Parent = parent;
            right.LPointer = node;
            node.RPointer = rightLeft;
            node.Parent = right;

            if (rightLeft != null) { rightLeft.Parent = node; }

            if (node == _root) { _root = right; }
                
            else if (parent.RPointer == node) { parent.RPointer = right; }

            else { parent.LPointer = right; }

            right.Balance++;
            node.Balance = -right.Balance;

            return right;
        }

        private AVLNode RotateRight(AVLNode node)
        {
            AVLNode left = node.LPointer;
            AVLNode leftRight = left.RPointer;
            AVLNode parent = node.Parent;

            left.Parent = parent;
            left.RPointer = node;
            node.LPointer = leftRight;
            node.Parent = left;

            if (leftRight != null) { leftRight.Parent = node; }

            if (node == _root) { _root = left; }

            else if (parent.LPointer == node) { parent.LPointer = left; }

            else { parent.RPointer = left; }

            left.Balance--;
            node.Balance = -left.Balance;

            return left;
        }

        private AVLNode RotateLeftRight(AVLNode node)
        {
            AVLNode left = node.LPointer;
            AVLNode leftRight = left.RPointer;
            AVLNode parent = node.Parent;
            AVLNode leftRightRight = leftRight.RPointer;
            AVLNode leftRightLeft = leftRight.LPointer;

            leftRight.Parent = parent;
            node.LPointer = leftRightRight;
            left.RPointer = leftRightLeft;
            leftRight.LPointer = left;
            leftRight.RPointer = node;
            left.Parent = leftRight;
            node.Parent = leftRight;

            if (leftRightRight != null) { leftRightRight.Parent = node; }

            if (leftRightLeft != null) { leftRightLeft.Parent = left; }

            if (node == _root) { _root = leftRight; }

            else if (parent.LPointer == node) { parent.LPointer = leftRight; }

            else { parent.RPointer = leftRight; }

            if (leftRight.Balance == -1) { node.Balance = 0; left.Balance = 1; }

            else if (leftRight.Balance == 0) { node.Balance = 0; left.Balance = 0; }

            else { node.Balance = -1; left.Balance = 0; }

            leftRight.Balance = 0;

            return leftRight;
        }

        private AVLNode RotateRightLeft(AVLNode node)
        {
            AVLNode right = node.RPointer;
            AVLNode rightLeft = right.LPointer;
            AVLNode parent = node.Parent;
            AVLNode rightLeftLeft = rightLeft.LPointer;
            AVLNode rightLeftRight = rightLeft.RPointer;

            rightLeft.Parent = parent;
            node.RPointer = rightLeftLeft;
            right.LPointer = rightLeftRight;
            rightLeft.RPointer = right;
            rightLeft.LPointer = node;
            right.Parent = rightLeft;
            node.Parent = rightLeft;

            if (rightLeftLeft != null) { rightLeftLeft.Parent = node; }

            if (rightLeftRight != null) { rightLeftRight.Parent = right; }

            if (node == _root) { _root = rightLeft; }

            else if (parent.RPointer == node) { parent.RPointer = rightLeft; }

            else { parent.LPointer = rightLeft; }

            if (rightLeft.Balance == 1) { node.Balance = 0; right.Balance = -1; }

            else if (rightLeft.Balance == 0) { node.Balance = 0; right.Balance = 0; }

            else { node.Balance = 1; right.Balance = 0; }

            rightLeft.Balance = 0;

            return rightLeft;
        }

        private void DeleteBalance(AVLNode node, int balance)
        {
            while (node != null)
            {
                balance = (node.Balance += balance);

                if (balance == 2)
                {
                    if (node.LPointer.Balance >= 0) { node = RotateRight(node); if (node.Balance == -1) { return; } }
                    else { node = RotateLeftRight(node); }
                }
                else if (balance == -2)
                {
                    if (node.RPointer.Balance <= 0) { node = RotateLeft(node); if (node.Balance == 1) { return; } }
                    else { node = RotateRightLeft(node); }
                }
                else if (balance != 0) { return; }

                AVLNode parent = node.Parent;

                if (parent != null) { balance = parent.LPointer == node ? -1 : 1; }

                node = parent;
            }
        }

        private static void Replace(AVLNode target, AVLNode source)
        {
            AVLNode left = source.LPointer;
            AVLNode right = source.RPointer;

            target.Balance = source.Balance;
            target.LPointer = left;
            target.RPointer = right;

            if (left != null) { left.Parent = target; }
            if (right != null) { right.Parent = target; }
        }

        public void Insert(int data)
        {
            AVLNode newNode = new AVLNode(data);
            if (_root == null) { _root = newNode; }
            else
            {
                AVLNode currentNode = _root;
                while (currentNode != null)
                {
                    if (newNode.Data <= currentNode.Data)
                    {
                        if (currentNode.LPointer != null) { currentNode = currentNode.LPointer; }
                        else { currentNode.LPointer = newNode; newNode.Parent = currentNode; InsertBalance(currentNode, 1); return; }
                    }
                    else if (newNode.Data > currentNode.Data)
                    {
                        if (currentNode.RPointer != null) { currentNode = currentNode.RPointer; }
                        else { currentNode.RPointer = newNode; newNode.Parent = currentNode; InsertBalance(currentNode, -1); return; }
                    }
                }
            }
        }

        public void Insert(string data)
        {
            AVLNode newNode = new AVLNode(data);
            if (_root == null) { _root = newNode; }
            else
            {
                AVLNode currentNode = _root;
                while (currentNode != null)
                {
                    if (String.Compare(newNode.Data, currentNode.Data) <= 0)
                    {
                        if (currentNode.LPointer != null) { currentNode = currentNode.LPointer; }
                        else { currentNode.LPointer = newNode; newNode.Parent = currentNode; InsertBalance(currentNode, 1); return; }
                    }
                    else if (String.Compare(newNode.Data, currentNode.Data) > 0)
                    {
                        if (currentNode.RPointer != null) { currentNode = currentNode.RPointer; }
                        else { currentNode.RPointer = newNode; newNode.Parent = currentNode; InsertBalance(currentNode, -1); return; }
                    }
                }
            }
        }

        public bool Delete<T>(T key)
        {
            AVLNode currentNode = _root;

            while (currentNode != null)
            {
                if (key <= currentNode.Data) { currentNode = currentNode.LPointer; }
                else if (key > currentNode.Data) { currentNode = currentNode.RPointer; }
                else
                {
                    AVLNode left = currentNode.LPointer;
                    AVLNode right = currentNode.RPointer;

                    if (left == null)
                    {
                        if (right == null)
                        {
                            if (currentNode == _root) { _root = null; }
                            else
                            {
                                AVLNode parent = currentNode.Parent;

                                if (parent.LPointer == currentNode)
                                {
                                    parent.LPointer = null;
                                    DeleteBalance(parent, -1);
                                }
                                else
                                {
                                    parent.RPointer = null;
                                    DeleteBalance(parent, 1);
                                }
                            }
                        }
                        else
                        {
                            Replace(currentNode, right);
                            DeleteBalance(currentNode, 0);
                        }
                    }
                    else if (right == null)
                    {
                        Replace(currentNode, left);
                        DeleteBalance(currentNode, 0);
                    }
                    else
                    {
                        AVLNode successor = right;
                        if (successor.LPointer == null)
                        {
                            AVLNode parent = currentNode.Parent;
                            successor.Parent = parent;
                            successor.LPointer = left;
                            successor.Balance = currentNode.Balance;

                            if (left != null) { left.Parent = successor; }
                            if (currentNode == _root) { _root = successor; }
                            else
                            {
                                if (parent.LPointer == currentNode) { parent.LPointer = successor; }
                                else { parent.RPointer = successor; }
                            }

                            DeleteBalance(successor, 1);
                        }
                        else
                        {
                            while (successor.LPointer != null) { successor = successor.LPointer; }

                            AVLNode parent = currentNode.Parent;
                            AVLNode successorParent = successor.Parent;
                            AVLNode successorRight = successor.RPointer;

                            if (successorParent.LPointer == successor) { successorParent.LPointer = successorRight; }
                            else { successorParent.RPointer = successorRight; }
                            if (successorRight != null) { successorRight.Parent = successorParent; }

                            successor.Parent = parent;
                            successor.LPointer = left;
                            successor.Balance = currentNode.Balance;
                            successor.RPointer = right;
                            right.Parent = successor;

                            if (left != null) { left.Parent = successor; }
                            if (currentNode == _root) { _root = successor; }
                            else
                            {
                                if (parent.LPointer == currentNode) { parent.LPointer = successor; }
                                else { parent.RPointer = successor; }
                            }

                            DeleteBalance(successorParent, -1);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public AVLNode Search(int data)
        {
            if (data == _root.Data) { Console.WriteLine("Node Found!"); return _root; }
            else
            {
                AVLNode checkerNode = _root;
                while ((checkerNode?.Data ?? data) != data)
                {
                    if (data <= checkerNode.Data) { checkerNode = checkerNode.LPointer; }
                    else { checkerNode = checkerNode.RPointer; }
                }
                if (checkerNode != null) { Console.WriteLine("Node Found!"); return checkerNode; }
                else { Console.WriteLine("Node Not Found!"); return new AVLNode(null); }
            }
        }

        public AVLNode Search(string data)
        {
            if (data == _root.Data) { Console.WriteLine("Node Found!"); return _root; }
            else
            {
                AVLNode checkerNode = _root;
                while ((checkerNode?.Data ?? data) != data)
                {
                    if (String.Compare(data, checkerNode.Data) <= 0) { checkerNode = checkerNode.LPointer; }
                    else { checkerNode = checkerNode.RPointer; }
                }
                if (checkerNode != null) { Console.WriteLine("Node Found!"); return checkerNode; }
                else { Console.WriteLine("Node Not Found!"); return new AVLNode(null); }
            }
        }

        public AVLNode Root() { return _root; }

        public void ATraversal(AVLNode root, int delay = 0)
        {
            if (root.LPointer != null) { ATraversal(root.LPointer, delay); }
            Console.WriteLine(root.Data);
            if (root.RPointer != null) { ATraversal(root.RPointer, delay); }
        }

        public void DTraversal(AVLNode root, int delay = 0)
        {
            if (root.RPointer != null) { DTraversal(root.RPointer, delay); }
            Console.WriteLine(root.Data);
            if (root.LPointer != null) { DTraversal(root.LPointer, delay); }
        }
    }
}
/* implement type check "as" instead of multi method */