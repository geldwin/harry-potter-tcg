using System.Collections;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Views
{
    public class HandView : MonoBehaviour
    {
        // TODO: Should we store these positions via some transform in the hierarchy? What's more maintainable?
        private static readonly Vector3 PreviewPosition = new Vector3(0f, 0f, 40f);
        private static readonly Vector3 PreviewRotation = new Vector3(0f, 180f, 0f);
        
        private GameViewSystem _gameView;
        
        private void Awake()
        {
            Global.Events.Subscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Subscribe(Notification.Prepare<ReturnToHandAction>(), OnPrepareReturnToHand);
            Global.Events.Subscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
            
            _gameView = GetComponent<GameViewSystem>();

            if (_gameView == null)
            {
                Debug.LogError("BoardView could not find GameView");
            }
        }

        private void OnPrepareDrawCards(object sender, object args)
        {
            var action = (DrawCardsAction) args;
            action.PerformPhase.Viewer = DrawCardAnimation;
        }
        
        private void OnPrepareReturnToHand(object sender, object args)
        {
            var action = (ReturnToHandAction) args;
            action.PerformPhase.Viewer  = ReturnToHandAnimation;
        }
        
        private void OnPreparePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) args;

            if (action.Card.Data.Type == CardType.Spell)
            {
                action.PerformPhase.Viewer = SpellPreviewAnimation;
            }
        }
        
        private IEnumerator SpellPreviewAnimation(IContainer container, GameAction action)
        {
            var playCardAction = (PlayCardAction) action;
            var cardView = _gameView.FindCardView(playCardAction.Card);
            
            //_gameView.ChangeZoneView(cardView, Zones.Discard, from: Zones.Hand);
            yield return true; //NOTE: Moves the card out of the Hand Zone

            var previewSequence = GetPreviewSequence(cardView, Zones.Discard, Zones.Hand);
            while (previewSequence.IsPlaying())
            {
                yield return null;
            }
        }


        private IEnumerator DrawCardAnimation(IContainer container, GameAction action)
        {
            yield return true;
            var drawAction = (DrawCardsAction) action;

            foreach (var card in drawAction.DrawnCards)
            {
                var cardView = _gameView.FindCardView(card);

                // TODO: Only displaying previews for single card draws is a hack to prevent the preview animation from playing during the initial 7 card hand draw.
                //       Refactor to something that would allow the player to preview cards for multiple card draws after game begin.
                // TODO: Card Draw Preview flag in settings that could enable/disable this animation per user?
                if (cardView.Card.Owner.Index == _gameView.Match.LocalPlayer.Index && drawAction.DrawnCards.Count == 1)
                {
                    var previewSequence = GetPreviewSequence(cardView, Zones.Hand, Zones.Deck);
                    while (previewSequence.IsPlaying())
                    {
                        yield return null;
                    }
                }
                else
                {
                    var sequence = _gameView.GetMoveToZoneSequence(cardView, Zones.Hand, Zones.Deck);
                    while (sequence.IsPlaying())
                    {
                        yield return null;
                    }
                }
            }
        }

        private IEnumerator ReturnToHandAnimation(IContainer game, GameAction action)
        {
            var returnAction = (ReturnToHandAction) action;

            // TODO: Only Spells?
            if (returnAction.Source.Data.Type == CardType.Spell)
            {
                var sequence = DOTween.Sequence();

                foreach (var returnedCard in returnAction.ReturnedCards)
                {
                    if (returnAction.Source == returnedCard)
                    {
                        // TODO: Different effect for this case?
                        continue;
                    }
                    
                    var particleType = returnAction.Source.GetLessonType();
                    var particleSequence = _gameView.GetParticleSequence(returnAction.Player, returnedCard, particleType);
                    sequence.Append(particleSequence);
                }
                
                while (sequence.IsPlaying())
                {
                    yield return null;
                }
            }

            var cardViews = _gameView.FindCardViews(returnAction.ReturnedCards);
            // TODO: Cards could come from multiple zones, but we need to capture the from zones for each card for the animation.
            var fromZone = returnAction.ReturnedCards.Select(c => c.Zone).Distinct().Single(); 
            yield return true;
            
            foreach (var cardView in cardViews)
            {
                var sequence = _gameView.GetMoveToZoneSequence(cardView, Zones.Hand, fromZone);
                while (sequence.IsPlaying())
                {
                    yield return null;
                }
            }
        }

        private Sequence GetPreviewSequence(CardView target, Zones to, Zones from, float duration = 0.5f)
        {
            var endZoneView = _gameView.FindZoneView(target.Card.Owner, to);

            var previewSequence = DOTween.Sequence()
                .Append(target.Move(PreviewPosition, PreviewRotation, duration));

            _gameView.ChangeZoneView(target, to, from);
            
            if (from != Zones.None)
            {
                var startZoneView = _gameView.FindZoneView(target.Card.Owner, from);
                previewSequence.Join(startZoneView.GetZoneLayoutSequence(duration));
            }

            var finalPos = endZoneView.GetNextPosition();
            var finalRot = endZoneView.GetRotation();
            
            return previewSequence
                    .AppendInterval(duration)
                    .Append(target.Move(finalPos, finalRot, 1.5f * duration));
        }

        private void OnDestroy()
        {
            Global.Events.Unsubscribe(Notification.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
            Global.Events.Unsubscribe(Notification.Prepare<ReturnToHandAction>(), OnPrepareReturnToHand);
            Global.Events.Unsubscribe(Notification.Prepare<PlayCardAction>(), OnPreparePlayCard);
        }
    }
}