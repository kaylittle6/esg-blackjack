namespace BlackJackApp
{
  public static class EntryPoint
  {
    public static void Main(string[] args)
    {
      var game = Game.GetGameClient;

      game.StartNewGame();
      game.CommenceRound();

      Console.Clear();
      Console.WriteLine("Thanks for playing!");
      Console.WriteLine();
      Console.WriteLine("We'll see ya next time!");
      Thread.Sleep(5000);
    }
  }
}