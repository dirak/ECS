using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class EntitySystem
    {
        public float DeltaToRun
        {
            get; set;
        }

        private float _cumulative_time;

        public EntityManager EntityManager { get; set; }
        public List<Entity> Compatible { get; private set; }

        protected List<Type> CompatibleTypes { get; private set; }

        public EntitySystem(EntityManager manager, params Type[] compatibleTypes)
        {
            _cumulative_time = 0.0f;

            if (compatibleTypes.Any(t => t.IsAssignableFrom(typeof(IComponent))))
            {
                throw new Exception();
                //not a component interface
            }

            CompatibleTypes = new List<Type>();
            CompatibleTypes.AddRange(compatibleTypes);

            EntityManager = manager;
            EventManager eManager = EventManager.getInstance();

            Compatible = GetCompatibleInManager();

            eManager.registerListener(ECSEventTypes.EntityAdded, OnManagerEntityChanged);
            eManager.registerListener(ECSEventTypes.EntityRemoved, OnManagerEntityChanged);

            eManager.registerListener(ECSEventTypes.EntityComponentAdded, OnManagerEntityChanged);
            eManager.registerListener(ECSEventTypes.EntityComponentRemoved, OnManagerEntityChanged);
        }

        private void OnManagerEntityChanged(GameEvent e)
        {
            Compatible = GetCompatibleInManager();
        }

        protected virtual List<Entity> GetCompatibleInManager()
        {
            return EntityManager.Entities.Where(ent => ent.hasComponents(CompatibleTypes)).ToList();
        }

        public abstract void Update(float delta);

        public void Run(float delta)
        {
            _cumulative_time += delta;
            if(_cumulative_time >= DeltaToRun)
            {
                _cumulative_time = 0.0f;
                Update(DeltaToRun + delta);
            }
        }
    }
}
