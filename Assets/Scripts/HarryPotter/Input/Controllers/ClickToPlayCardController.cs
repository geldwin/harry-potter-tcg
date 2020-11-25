using HarryPotter.Input.InputStates;
using HarryPotter.StateManagement;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Input.Controllers
{
    public class ClickToPlayCardController : MonoBehaviour
    {
        public IContainer Game { get; set; }
        
        public GameViewSystem GameView { get; set; }
        private Container InputStateContainer { get; set; }
        public StateMachine StateMachine { get; private set; }

        public CardView ActiveCard { get; set; }

        //TODO: Is there a better way to detect active state so that we don't have to manually handle this?
        public bool IsCardPreview { get; set; }

        private void Awake()
        {
            GameView = GetComponent<GameViewSystem>(); 
            Game = GameView.Container;
            
            InputStateContainer = new Container(); //TODO: Code smell - Container with null Match, add match system to hold match data for the game container instead.
            StateMachine = InputStateContainer.AddSystem<StateMachine>();
            
            InputStateContainer.AddSystem<WaitingForInputState>().Controller = this;
            InputStateContainer.AddSystem<PreviewState>().Controller = this;
            InputStateContainer.AddSystem<ConfirmOrCancelState>().Controller = this;
            InputStateContainer.AddSystem<CancellingState>().Controller = this;
            InputStateContainer.AddSystem<ConfirmState>().Controller = this;
            InputStateContainer.AddSystem<ResetState>().Controller = this;
            InputStateContainer.AddSystem<TargetingState>().Controller = this;

            IsCardPreview = false;
            StateMachine.ChangeState<WaitingForInputState>();
        }

        private void OnEnable()
        {
            Global.Events.Subscribe(Clickable.CLICKED_NOTIFICATION, OnClickNotification);
        }

        private void OnClickNotification(object sender, object args)
        {
            var handler = StateMachine.CurrentState as IClickableHandler;
            
            handler?.OnClickNotification(sender, args);
        }
        
        private void OnDisable()
        {
            Global.Events.Unsubscribe(Clickable.CLICKED_NOTIFICATION, OnClickNotification);
        }
    }
}