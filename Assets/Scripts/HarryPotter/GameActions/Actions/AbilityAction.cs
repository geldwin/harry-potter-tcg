using HarryPotter.Data.Cards.CardAttributes.Abilities;

namespace HarryPotter.GameActions.Actions
{
    public class AbilityAction : GameAction
    {
        public Ability Ability { get; }

        public AbilityAction(Ability ability)
        {
            Ability = ability;
        }

        public override string ToString() => string.Empty;
    }
}