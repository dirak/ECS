using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public interface GameState
    {
        void Exit();
        void Enter();

        void onPause();
        void Resume();

        void HandleEvents();
        void Update(float delta);
        void Draw();

    }

    public class GameStateStack
    {
        private Stack<GameState> _gamestate_stack;
        private GameState _stack_cursor;

        protected void AdjustState()
        {
            if(_stack_cursor != null &&_stack_cursor != _gamestate_stack.Peek())
            {
                /* If the top is not the current cursor
                * we need to do some adjustments
                */
                _stack_cursor.Exit();
                _stack_cursor = _gamestate_stack.Peek();
                _stack_cursor.Enter();
            } else if(_stack_cursor == null && _gamestate_stack.Peek() != null)
            {
                /* Refactor this*/
                _stack_cursor = _gamestate_stack.Peek();
                _stack_cursor.Enter();
            }
        }

        public GameStateStack()
        {
            _gamestate_stack = new Stack<GameState>();
            _stack_cursor = null;
        }

        public void Cleanup()
        {

        }

        public void PopState()
        {
            /*This should work because we keep a reference
            * of the top state in cursor until we run
            * adjustments.
            */
            _gamestate_stack.Pop();
            AdjustState();
        }

        public void PushState(GameState push_state)
        {
            _gamestate_stack.Push(push_state);
            AdjustState();
        }

        public void HandleEvents()
        {
            /* No idea what to do here w/ current setup of event manager.
            * The idea is to have 'Pause' take events differently than
            * the actual game, same w/ menus.
            */
            _stack_cursor.HandleEvents();
        }

        public void Update(float delta)
        {
            _stack_cursor.Update(delta);
        }

        public void Draw()
        {
            _stack_cursor.Draw();
        }
    }
}
