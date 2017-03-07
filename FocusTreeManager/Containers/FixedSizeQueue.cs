using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FocusTreeManager.Containers
{
    /// <summary>
    /// Thread safe fixed size LIFO. Found here : 
    /// http://stackoverflow.com/questions/5852863/fixed-size-queue-which-automatically-dequeues-old-values-upon-new-enques
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedSizeQueue<T> : IReadOnlyCollection<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private int _count;

        public int Limit { get; }

        public FixedSizeQueue(int limit)
        {
            Limit = limit;
        }

        public void Enqueue(T obj)
        {
            _queue.Enqueue(obj);
            Interlocked.Increment(ref _count);
            // Calculate the number of items to be removed by this thread in a thread safe manner
            int currentCount;
            int finalCount;
            do
            {
                currentCount = _count;
                finalCount = Math.Min(currentCount, Limit);
            } while (currentCount !=
                Interlocked.CompareExchange(ref _count, finalCount, currentCount));
            T overflow;
            while (currentCount > finalCount && _queue.TryDequeue(out overflow))
            {
                currentCount--;
            }
        }

        public int Count => _count;

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}
