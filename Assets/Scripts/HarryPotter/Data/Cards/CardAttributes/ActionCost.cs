using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class ActionCost : CardAttribute
    {
        [Range(0, 2)]
        public int Amount;

        private int DefaultAmount { get; set; }

        public override void InitAttribute()
        {
            DefaultAmount = Amount;
        }

        public override void ResetAttribute()
        {
            Amount = DefaultAmount;
        }
    }
}