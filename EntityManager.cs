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

        public List<Entity> Entities
        {
            get { return _activeEntities; }
            set { _activeEntities = value; }
        }

        public EntityManager()
        {
            _activeEntities = new List<Entity>();
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
                throw new Exception();
            }

            Entity newEntity = new Entity(entityId, this);

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

        public void destroyEntity(ref Entity entity)
        {
            if (!_activeEntities.Contains(entity))
            {
                throw new Exception();
                //throw exception for no entity
            }
            _activeEntities.Remove(entity);
            EventManager eManager = EventManager.getInstance();
            GameEvent _event = new GameEvent(ECSEventTypes.EntityRemoved, this, new EntityEventArgs(null, entity));
            eManager.QueueEvent(_event);
            entity = null; //clear the ref
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
