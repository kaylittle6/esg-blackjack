namespace BlackJackApp
{
  public class Player(string name)
  {
    public string? Name { get; protected init; } = name;
    public List<Hand> Hand { get; set; } = [new Hand()];
    public decimal CurrentMoney { get; set; }
    public bool IsDealer => Name == "Dealer";
    public bool InHand { get; set; } = true;
    public bool HasInsurance { get; set; }
  }
}