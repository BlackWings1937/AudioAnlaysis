#region 程序集 mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// D:\Program Files\Unity\Editor\Data\MonoBleedingEdge\lib\mono\4.6-api\mscorlib.dll
#endregion

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Collections.Concurrent
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy("System.Collections.Concurrent.IProducerConsumerCollectionDebugView<T>")]
    public class ConcurrentQueue<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
    {
        public ConcurrentQueue() { }
        public ConcurrentQueue(IEnumerable<T> collection) { }

        public int Count { get; }
        public bool IsEmpty { get; }

        public void CopyTo(T[] array, int index);
        public void Enqueue(T item);
        public IEnumerator<T> GetEnumerator();
        public T[] ToArray();
        public bool TryDequeue(out T result);
        public bool TryPeek(out T result);
    }
}