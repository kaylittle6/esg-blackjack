namespace BlackJackApp
{
  public class Game
  {
    private static Game? _gameClient;

    public List<Player> Players { get; } = new();
    public Dealer Dealer { get; } = new("Dealer");
    public int MinimumBet { get; private set; }
    
    public static Game GetGameClient => _gameClient ??= new Game();

    public void StartNewGame()
    {
      int p;

       Console.Clear();
       Console.WriteLine("How many players will play? (Maximum 4 players)");
       Console.WriteLine();

       while (!int.TryParse(Console.ReadLine(), out p) || p < 1 || p > 4)
       {
         Console.Clear();
         Console.WriteLine("Please select a valid response");
         Thread.Sleep(3000);
         Console.Clear();
         Console.WriteLine("How many players will play? (Maximum 4 players)");
         Console.WriteLine();
       }
     
       for (var i = 0; i < p; i++)
       {
         int money;
         
         Console.Clear();
         Console.WriteLine("What is this Player's name?");
         Console.WriteLine();
     
         var name = Console.ReadLine();
          
         Console.Clear();
         Console.WriteLine("Please select a starting amount of money (Numbers Only)");
         Console.WriteLine();

         while (!int.TryParse(Console.ReadLine(), out money))
         {
           Console.Clear();
           Console.WriteLine("Please enter a valid amount");
           Console.WriteLine("Please select a starting amount of money (Numbers Only)");
           Console.WriteLine();
         }
     
         Players.Add(new Player(name!) { CurrentMoney = money});
       }
     
       Players.Add(Dealer);
     
       Console.Clear();
       Console.WriteLine("How many decks would you like to play with?");
       Console.WriteLine();
     
       int deckCount;
       
       while (!int.TryParse(Console.ReadLine(), out deckCount))
       {
         Console.Clear();
         Console.WriteLine("Please enter a valid number of decks");
         Console.WriteLine("How many decks would you like to play with?");
         Console.WriteLine();
       }
       
       Dealer.DeckCount = deckCount;
       Dealer.GetDeck(Dealer.DeckCount);
     
       Console.Clear();
       Console.WriteLine("What is the minimum bet for this table?");
       Console.WriteLine();
     
       int minimumBet;
       
       while (!int.TryParse(Console.ReadLine(), out minimumBet))
       {
         Console.Clear();
         Console.WriteLine("Please enter a valid minimum bet");
         Console.WriteLine("What is the minimum bet for this table?");
         Console.WriteLine();
       }
       
       MinimumBet = minimumBet;
     
       Display.ShowTable(false);
     }

    public void CommenceRound()
    {
      do
      {
        Dealer.CollectAntes();
        Dealer.DealStartingCards(Players);

        Console.Clear();

        Display.ShowTable(false);
        
        // Check if Dealer has Blackjack
        if (Dealer.Hand[0].Cards.Any(c => c is { CardValue: 11, IsFaceDown: false }))
        {
          Console.Clear();

          foreach (var player in Players)
          {
            Display.ShowTable(player, true);
            Dealer.OfferInsurance(player);
          }

          if (Dealer.Hand[0].Cards[0].CardValue == 10)
          {
            Dealer.Hand[0].Cards[0].IsFaceDown = false;
            Console.Clear();
            Display.ShowTable(true);
            Console.WriteLine();
            Console.WriteLine("The Dealer has Blackjack, let's look at the Players...");
            Thread.Sleep(4000);

            foreach (var player in Players.Where(p => !p.IsDealer))
            {
              foreach (var hand in player.Hand)
              {
                switch (hand.HasBlackJack)
                {
                  case false when player.HasInsurance:
                    Console.WriteLine();
                    Console.WriteLine($"{player.Name} has Insurance! Smart. You win {hand.CurrentBet / 2:C2} dollars");
                    player.CurrentMoney += hand.CurrentBet / 2;
                    Thread.Sleep(4000);
                    continue;
                  case false when !player.HasInsurance:
                    Console.WriteLine();
                    Console.WriteLine($"Sorry, {player.Name}. You don't have Insurance. You lose.");
                    Thread.Sleep(4000);
                    break;
                  case true:
                    RuleBook.CheckAndResolveBlackJack(true);
                    break;
                }
              }
              
              RuleBook.ResetPlayer(player);
            }
          }
        }

        RuleBook.CheckAndResolveBlackJack(false);
        
        foreach (var player in Players)
        {
          Dealer.AskForPlayerOptions(player);
        }

        if (!Players.Any(p => p is { InHand: true, IsDealer: false }))
        {
          Console.Clear();
          Console.WriteLine("Well shit, nobody is left, guess we'll do it again");
          Dealer.RemoveBrokeAssPlayers(Players);
          Dealer.FinishUpRound(Players);
          Thread.Sleep(4000);
          continue;
        }

        Dealer.Hand[0].Cards[0].IsFaceDown = false;
        
        if (Dealer.Hand[0].Value < 17)
        {
          Dealer.DealerAction();
        }
        else
        {
          Console.Clear();
          Display.ShowDealersActions(Dealer);
          Console.WriteLine();
          Console.WriteLine($"Dealer stands with {Dealer.Hand[0].Value}");
          Thread.Sleep(4000);
        }

        var dealerResult = RuleBook.CheckHand(Dealer.Hand[0]);
        
        if (dealerResult == RuleBook.HandResult.HandBusted)
        {
          foreach (var player in Players.Where(player => player is { InHand: true, IsDealer: false }))
          {
            foreach (var hand in player.Hand)
            {
              var winnings = hand.DoubledDown ? hand.CurrentBet * 2.5m : hand.CurrentBet * 2;

              Console.Clear();
              Display.ShowTable(player, true);
              Console.WriteLine($"The Dealer busted, {player.Name}.");
              Console.WriteLine();
              Console.WriteLine($"You win {winnings:C2} dollars this hand.");
              RuleBook.WinBet(player, hand);
              Thread.Sleep(4000);
            }
            
            RuleBook.ResetPlayer(player);
          }
        }

        foreach (var player in Players.Where(player => player is { InHand: true, IsDealer: false }))
        {
          foreach (var hand in player.Hand)
          {
            var winnings = hand.DoubledDown ? hand.CurrentBet * 2 : hand.CurrentBet;
            
            Console.Clear();
            Display.ShowTable(player, true);
            Console.WriteLine($"Dealer has {Dealer.Hand[0].Value}, you have {hand.Value}, {player.Name}.");
            Console.WriteLine();
          
            if (hand.Value > Dealer.Hand[0].Value)
            {
              Console.WriteLine($"You win {winnings:C2} dollars this hand!");
              RuleBook.WinBet(player, hand);
              Thread.Sleep(4000);
            }
            else if (hand.Value < Dealer.Hand[0].Value)
            {
              Console.WriteLine($"You lose. {hand.CurrentBet:C2} total in fact.");
              Thread.Sleep(4000);
            }
            else
            {
              Console.WriteLine($"Push. You get your money back, all {hand.CurrentBet:C2}.");
              player.CurrentMoney += hand.CurrentBet;
              Thread.Sleep(4000);
            }
          }

          player.InHand = false;
        }

        foreach (var p in Players.Where(p => !p.IsDealer)) { RuleBook.ResetPlayer(p);}
        
        Dealer.RemoveBrokeAssPlayers(Players);
        Dealer.FinishUpRound(Players);
        
      } while (Players.Any(player => !player.IsDealer));
    }
  }
}