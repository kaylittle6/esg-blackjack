﻿namespace BlackJackApp
{
  public static class RuleBook
  {
    public enum HandResult
    {
      HandValid,
      HandBusted,
      HandBlackjack
    }

    public static void CheckAndResolveBlackJack(bool dealerFlipped)
    {
      var game = Game.GetGameClient;

      foreach (var player in game.Players.Where(player => player is { IsDealer: false, InHand: true }))
      {
        foreach (var hand in player.Hand.Where(hand => hand.HasBlackJack))
        {
          Console.Clear();
          Display.ShowTable(dealerFlipped);
          Console.WriteLine();
          Console.WriteLine($"{player.Name} has Blackjack! They win ${hand.CurrentBet * 2.5m}");
          player.CurrentMoney += hand.CurrentBet * 2.5m;
          player.InHand = false;
          Thread.Sleep(4000);
        }
      }
    }

    public static void WinBet(Player player, Hand hand)
    {
      player.CurrentMoney += hand.DoubledDown ? hand.CurrentBet * 4 : hand.CurrentBet * 2;
    }

    public static HandResult CheckHand(Hand hand)
    {
      CheckForAceAndReduce(hand);

      return hand.Value switch
      {
        21 => HandResult.HandBlackjack,
        > 21 => HandResult.HandBusted,
        _ => HandResult.HandValid
      };
    }

    public static void CheckForAceAndReduce(Hand hand)
    {
      if (!hand.Cards.Any(c => c is { CardValue: 11 }) || hand.Value < 21) { return; }
        
      do
      {
        hand.Cards.First(c => c is { CardValue: 11 }).ChangeAceValueToOne();
        
      } while (hand.Value > 21 && !hand.Cards.Any(c => c is { CardValue: 11 }));
    }

    public static void ResetPlayer(Player player)
    {
      player.InHand = false;
      player.HasInsurance = false;
      player.Hand.Clear();
    }
  }
}