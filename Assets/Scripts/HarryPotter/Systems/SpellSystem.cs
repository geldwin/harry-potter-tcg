using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class SpellSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Subscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Subscribe(Notification.Perform<CastSpellAction>(), OnPerformCastSpell);
        }

        //TODO: OnValidatePlayCard - if the card is a spell and it has a target selector, make sure there are targets available (?)
        
        private void OnPerformPlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;
            var playerSystem = Container.GetSystem<PlayerSystem>();

            if (action.Card.Data.Type == CardType.Spell)
            {
                playerSystem.ChangeZone(action.Card, Zones.None);
                var spellAction = new CastSpellAction(action.Card);
                Container.AddReaction(spellAction);
            }
        }

        private void OnPrepareCastSpell(object sender, object args)
        {
            var action = (CastSpellAction) args;
            
            var conditionAbilities = action.Card.GetAbilities(AbilityType.PlayCondition);

            foreach (var ability in conditionAbilities)
            {
                var reaction = new AbilityAction(ability);
                Container.AddReaction(reaction);
            }
            
            var playEffectAbilities = action.Card.GetAbilities(AbilityType.PlayEffect);

            if (playEffectAbilities.Count == 0)
            {
                Debug.LogWarning($"CastSpellAction - No PlayEffect ability found for card {action.Card.Data.CardName}");
            }
            
            foreach (var ability in playEffectAbilities)
            {
                var reaction = new AbilityAction(ability);
                Container.AddReaction(reaction);
            }
        }

        private void OnPerformCastSpell(object sender, object args)
        {
            var action = (CastSpellAction) args;
            var discardSystem = Container.GetSystem<DiscardSystem>();
            
            discardSystem.DiscardCard(action.Card, action.Card);
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<PlayCardAction>(), OnPerformPlayCard);
            Global.Events.Unsubscribe(Notification.Prepare<CastSpellAction>(), OnPrepareCastSpell);
            Global.Events.Unsubscribe(Notification.Prepare<CastSpellAction>(), OnPerformCastSpell);
        }
    }
}