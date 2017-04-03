using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class EntityManager
    {

        private List<Entity> _activeEntities;
        private Stack<Entity> _inactiveEntities;
        private Dictionary<Type, Stack<IComponent>> _inactiveComponents;

        public List<Entity> Entities
        {
            get { return _activeEntities; }
            set { _activeEntities = value; }
        }

        public int ActiveEntities
        {
            get { return _activeEntities.Count; }
        }
        public int InactiveEntities
        {
            get { return _inactiveEntities.Count; }
        }

        public EntityManager()
        {
            _activeEntities = new List<Entity>();
            _inactiveEntities = new Stack<Entity>();
            _inactiveComponents = new Dictionary<Type, Stack<IComponent>>();

            for(var i = 0; i < 1000; i++)
            {
                _inactiveEntities.Push(new Entity("_cached_" + i, this));
            }
        }

        public Entity MakeEntity(string entityId)
        {
            if (Entities.Any(ent => ent.Id == entityId))
            {
                //duplicate
                throw new Exception("Duplicate entity id: "+entityId);
            }

            if (string.IsNullOrEmpty(entityId))
            {
                //invalid Id
                throw new Exception("Invalid entity id: "+entityId);
            }
            Entity newEntity;
            if (_inactiveEntities.Count > 0)
            {
                newEntity = _inactiveEntities.Pop();
                newEntity.removeComponents();
                newEntity.Id = entityId;
            }
            else
            {
                newEntity = new Entity(entityId, this);
            }
            Entities.Add(newEntity);
            EventManager eManager = EventManager.getInstance();
            GameEvent _event = new GameEvent(ECSEventTypes.EntityAdded, this, new EntityEventArgs(null, newEntity));
            eManager.QueueEvent(_event);
            return newEntity;
        }

        public Entity getEntity(string entityId)
        {
            var match = Entities.FirstOrDefault(ent => ent.Id == entityId);
            if (match != null) return match;

            throw new Exception();
            //throw exception for no entity
        }

        public void destroyEntity(string entityId)
        {
            Entity entity = getEntity(entityId);
            _activeEntities.Remove(entity);
            EventManager eManager = EventManager.getInstance();
            GameEvent _event = new GameEvent(ECSEventTypes.EntityRemoved, this, new EntityEventArgs(null, entity));
            eManager.QueueEvent(_event);
            _inactiveEntities.Push(entity);
        }

        internal void ComponentAdded(Entity entity)
        {
            EventManager eManager = EventManager.getInstance();
            GameEvent _event = new GameEvent(ECSEventTypes.EntityComponentAdded, this, new EntityEventArgs(null, entity));
            eManager.QueueEvent(_event);
        }

        internal void ComponentRemoved(Entity entity)
        {
            EventManager eManager = EventManager.getInstance();
            GameEvent _event = new GameEvent(ECSEventTypes.EntityComponentRemoved, this, new EntityEventArgs(null, entity));
            eManager.QueueEvent(_event);
        }


    }
}
