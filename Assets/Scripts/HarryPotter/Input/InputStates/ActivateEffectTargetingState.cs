using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class ActivateEffectTargetingState : BaseTargetingState
    {
        public override void Enter()
        {
            TargetSelector = InputSystem.EffectSelectors[InputSystem.EffectsIndex];
            base.Enter();
        }
        
        protected override void HandleTargetsAcquired()
        {
            ApplyTargetsToSelector();

            if (InputSystem.EffectsIndex > InputSystem.EffectSelectors.Count - 1)
            {
                InputSystem.EffectsIndex++;
                InputSystem.StateMachine.ChangeState<ActivateEffectTargetingState>();
                
            }
            else
            {
                var action = new ActivateCardAction(InputSystem.ActiveCard.Card);
                Debug.Log("*** PLAYER ACTIVATES CARD EFFECT ***");
                InputSystem.Game.Perform(action);
            
                InputSystem.StateMachine.ChangeState<ResetState>();
            }
        }
    }
}