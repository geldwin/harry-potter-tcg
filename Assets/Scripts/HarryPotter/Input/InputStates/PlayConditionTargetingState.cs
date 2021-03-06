using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class PlayConditionTargetingState : CancelableTargetingState
    {
        public override void Enter()
        {
            TargetSelector = InputSystem.ConditionSelectors[InputSystem.ConditionsIndex];
            base.Enter();
        }

        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputSystem.ConditionsIndex > InputSystem.ConditionSelectors.Count - 1)
            {
                InputSystem.ConditionsIndex++;
                InputSystem.StateMachine.ChangeState<PlayConditionTargetingState>();
            }
            else if (InputSystem.EffectSelectors.Count > 0)
            {
                InputSystem.StateMachine.ChangeState<PlayEffectTargetingState>();
            }
            else
            {
                var action = new PlayCardAction(InputSystem.ActiveCard.Card);
                Debug.Log("*** PLAYER ACTION ***");
                InputSystem.Game.Perform(action);
            
                InputSystem.StateMachine.ChangeState<ResetState>();
            }
        }
        
    }
}