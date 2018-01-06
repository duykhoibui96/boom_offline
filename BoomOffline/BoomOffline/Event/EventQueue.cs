using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoomOffline.Event
{
    class EventQueue
    {
        private static EventQueue instance;

        public static EventQueue Instance
        {
            get
            {
                if (instance == null)
                   instance = new EventQueue();
                return instance;
            }
        }

        public Queue<GameEvent> queue;

        private EventQueue()
        {
            queue = new Queue<GameEvent>();
        }

        public GameEvent CheckCurrentEvent()
        {
            return queue.Count > 0 ? queue.Peek() : null;
        }

        public void Next()
        {
            queue.Dequeue();
        }

        public void AddEvent(GameEvent ev)
        {
            queue.Enqueue(ev);
        }

    }
}
