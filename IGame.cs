using System.Collections.Generic;

public interface IGame
{
    void AddDeckToShoe();
    void AddPlayer();
    void DealCardsToPlayer(int playerNumber, int numCards);
    List<Card> GetListOfCardsPlayer(int playerNumber);
    List<KeyValuePair<int, int>> GetPlayerListSortedByTotalValue();
    List<KeyValuePair<KeyValuePair<Suit, Value>, int>> GetRemainingCardCount();
    Dictionary<Suit, int> GetUndealtCountPerSuit();
    void RemovePlayer(int playerNumberToRemove);
    void ShuffleGameDeck();
}