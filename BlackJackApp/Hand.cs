namespace BlackJackApp
{
  public class Hand
  {
    public List<Card> Cards { get; set; } = [];
    public int Value => Cards.Where(c => !c.IsFaceDown).Sum(c => c.CardValue);
    public bool HasBlackJack => Cards.Sum(cv => cv.CardValue) == 21;
    public bool DoubledDown { get; set; }
    public decimal CurrentBet { get; set; }
  }
}