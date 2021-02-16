using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;

namespace HarryPotter.Input.InputStates
{
    public class PlayEffectTargetingState : BaseTargetingState
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