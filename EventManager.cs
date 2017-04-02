using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public enum ECSEventTypes
    {
        EntityAdded,
        EntityRemoved,
        EntityComponentAdded,
        EntityComponentRemoved,
        ComponentAdded
    };

    public delegate void GameEventHandler(GameEvent e);

    public interface GameEventArgs { }

    public class EntityEventArgs : GameEventArgs
    {
        private EntityManager _manager;
        private Entity _entity;

        public EntityManager Manager
        {
            get
            {
                return _manager;
            }
            private set { }
        }

        public Entity Entity
        {
            get
            {
                return _entity;
            }
            private set { }
        }

        public EntityEventArgs(EntityManager manager, Entity entity)
        {
            _manager = manager;
            _entity = entity;
        }
    }

    public class GameEvent
    {
        private object _sender;
        private object _eventType;
        private GameEventArgs _args;

        public GameEvent(object eventType, object sender, GameEventArgs args)
        {
            _sender = sender;
            _eventType = eventType;
            _args = args;
        }

        public object Sender
        {
            get
            {
                return _sender;
            }
        }

        public object EventType
        {
            get
            {
                return _eventType;
            }

        }
        public GameEventArgs Args
        {
            get
            {
                return _args;
            }
        }
    }

    public class EventManager
    {
        private Dictionary<object, GameEventHandler> _listeners;
        private List<GameEvent> _newEvents;
        private List<GameEvent> _currentEvents;
        private bool isProcessing;

        private static EventManager instance = new EventManager();

        private EventManager()
        {
            _listeners = new Dictionary<object, GameEventHandler>();
            _newEvents = new List<GameEvent>();
            _currentEvents = new List<GameEvent>();
            isProcessing = false;
        }

        public static EventManager getInstance()
        {
            return instance;
        }

        public void QueueEvent(GameEvent e)
        {
            _newEvents.Add(e);
        }

        public void registerListener(object eventType, GameEventHandler callback)
        {
            if(_listeners.ContainsKey(eventType))
            {
                _listeners[eventType] += callback;
            }
            else
            {
                _listeners[eventType] = callback;
            }
        }

        public void removeListener(object eventType, GameEventHandler callback)
        {
            if (_listeners.ContainsKey(eventType))
            {
                _listeners[eventType] -= callback;
            }
        }

        public int ProcessEvents(float dt)
        {
            if (isProcessing)
            {
                return 0;
            }

            isProcessing = true;

            int processedEvents = 0;

            while (_newEvents.Count > 0)
            {
                _currentEvents.AddRange(_newEvents);
                _newEvents.Clear();

                foreach(GameEvent e in _currentEvents)
                {
                    ProcessEvent(e);
                    ++processedEvents;
                }

                _currentEvents.Clear();
            }

            isProcessing = false;

            return processedEvents;
        }

        public void ProcessEvent(GameEvent e)
        {
            if (_listeners.ContainsKey(e.EventType))
            {
                _listeners[e.EventType](e);
            }
        }
    }
}
