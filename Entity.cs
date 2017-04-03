using System.Collections.Generic;
using System;
using System.Linq;

namespace ECS
{
    public class Entity
    {
        public string Id { get; set; }
        public EntityManager ownerManager { get; set; }

        public List<IComponent> Components { get; set; }

        public Entity(string entityId, EntityManager entityManager)
        {
            Id = entityId;
            ownerManager = entityManager;
            Components = new List<IComponent>();
        }

        public void addComponent(IComponent Component)
        {
            if (HasComponent(Component.GetType()))
            {
                throw new Exception();
                //component already exists
            }

            Components.Add(Component);
            ownerManager.ComponentAdded(this);
        }

        public void removeComponent<TComponent>()
            where TComponent : IComponent
        {
            if (!HasComponent<TComponent>())
            {
                throw new Exception();
                //component doesn't exist
            }

            IComponent remove = GetComponent<TComponent>();
            Components.Remove(remove);
            ownerManager.ComponentRemoved(this);
        }

        public void removeComponents()
        {
            Components = new List<IComponent>();
        }

        public TComponent GetComponent<TComponent>()
            where TComponent : IComponent
        {
            TComponent match = Components.OfType<TComponent>().FirstOrDefault();
            if (match != null) return match;

            throw new Exception();
            //component does not exist
        }

        public bool HasComponent(Type type)
        {
            var match = Components.Any(c => c.GetType() == type);
            if (match) return true;
            else return false;
        }

        public bool hasComponents(IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                if (!HasComponent(t)) return false;
            }
            return true;
        }

        public bool HasComponent<TComponent>()
            where TComponent : IComponent
        {
            var match = Components.Any(comp => comp.GetType() == typeof(TComponent));
            if (match) return true;
            else return false;
        }

        public static Entity operator +(Entity entity, IComponent component)
        {
            if (entity != null && component != null)
            {
                entity.addComponent(component);
                return entity;
            }
            else
            {
                throw new Exception();
                //null argument issue
            }
        }
    }
}