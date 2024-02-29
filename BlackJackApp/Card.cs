namespace BlackJackApp
{
  public class Card
  {
    public string Display => $"{CardNumber} of {CardSuit}";
    public string CardNumber { get; set; }
    private string CardSuit { get; set; }
    public int CardValue { get; private set; }
    public bool IsFaceDown { get; set; }

    public Card(string cardNumber, string cardSuit)
    {
      CardNumber = cardNumber;
      CardSuit = cardSuit;
      CardValue = GetCardValue();
    }

    private int GetCardValue()
    {
      if (int.TryParse(CardNumber, out var value) || CardNumber == "Ace")
      {
        return CardNumber == "Ace" ? 11 : value;
      }

      return 10;
    }

    public void ChangeAceValueToOne()
    {
      CardValue = 1;
    }
  }
}