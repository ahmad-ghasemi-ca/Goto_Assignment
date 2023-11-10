using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public class Messages
{
    public string GameHandlerUndealtCardsPerSuit { get; } = "Undealt cards count per suit are:";
    public string GameHandlerRemainingCards { get; } = "Remaining cards count are:";
    public string GameHandlerListPlayersSorted { get; } = "List of players sorted by their hand value";
    public string GameHandlerNewGameCreated { get; } = "A new game created with players and decks:";
    public string GameHandlerListCardsplayer { get; } = "List of cards for player";
    public string GameHandlerInvalidParamforGameCreation { get; } = "Invalid parameters for creating a game.";
    public string GameHandlerGameDeleted { get; } = "Game ended!";

    public string GameGetCradsErrorNoPlayer(int playerNumber) => $"Get number of Cards Error: player with this number does not exist: {playerNumber}";
    public string GameDealErrorPlayerDoesNotExist { get; } = "Deal to player Error: The player does not exist in the game";
    public string GameRemoveErrorPlayerDoseNotExist { get; } = "Remove player Error: This player does not exist in the game";
    public string GameRemovePlayerSuccess(int playerNumber) => $"The player with this number were removed: {playerNumber}";
    public string GameCardsDealtSuccess(int numCards, int playerNumber) => $"This numbers of cards were dealt to player: {numCards}, {playerNumber}";
    public string GameShuffleGameDeckSuccess { get; } = "The shoe was shuffled!";
    public string GameShuffleGameDeckFailed { get; } = "Shuffle Error: There is not enough cards to shuffle";
    public string GetPlayerListSortedByTotalValueFailed { get; } = "Player cards list Error: There is no player to get the hand";

    public string Line { get; } = "----------";
}
public enum Suit
{
    Hearts = 1, Spades, Clubs, Diamonds
}

public enum Value
{
    Ace = 1, Two, Three, Four, Five, Six, Seven,
    Eight, Nine, Ten, Jack, Queen, King
}

public class Card
{
    public Suit Suit { get; set; }
    public Value Value { get; set; }
}

public class Deck
{
    private List<Card> cards = new List<Card>();

    private void Initialize()
    {
        cards.Clear();
        for (int i = 1; i < 5; ++i)
        {
            for (int j = 1; j < 14; ++j)
            {
                cards.Add(new Card { Suit = (Suit)i, Value = (Value)j });
            }
        }
    }

    public Deck()
    {
        Initialize();
    }

    public List<Card> GetCards()
    {
        return cards;
    }
}

public class Player
{
    private List<Card> hand = new List<Card>();

    public void AddCards(List<Card> newCards)
    {
        hand.AddRange(newCards);
    }

    public int GetTotalValue()
    {
        return hand.Sum(card => (int)card.Value);
    }

    public List<Card> GetHand()
    {
        return hand;
    }
}

public class Game
{
    private List<Player> players = new List<Player>();
    private List<Card> shoe = new List<Card>();
    private Messages message = new Messages();

    private Deck CreateDeck()
    {
        return new Deck();
    }

    public void AddDeckToShoe()
    {
        Deck generatedDeck = CreateDeck();
        shoe.AddRange(generatedDeck.GetCards());
    }

    public void AddPlayer()
    {
        players.Add(new Player());
    }

    public void RemovePlayer(int playerNumberToRemove)
    {
        int indexToRemove = playerNumberToRemove - 1;
        if (indexToRemove >= 0 && indexToRemove < players.Count)
        {
            players.RemoveAt(indexToRemove);
            Console.WriteLine(message.Line);
            Console.WriteLine($"{message.GameRemovePlayerSuccess(playerNumberToRemove)}");
        }
        else
        {
            Console.WriteLine(message.Line);
            Console.WriteLine(message.GameRemoveErrorPlayerDoseNotExist);
        }
    }

    public void DealCardsToPlayer(int playerNumber, int numCards)
    {
        int playerIndex = playerNumber - 1;
        if (playerIndex >= 0 && playerIndex < players.Count)
        {
            List<Card> dealtCards = new List<Card>();
            int shoeSize = shoe.Count;
            for (int i = shoeSize - 1; i > shoeSize - 1 - numCards && shoe.Any(); i--)
            {
                dealtCards.Add(shoe[i]);
                shoe.RemoveAt(i);
            }
            players[playerIndex].AddCards(dealtCards);
            Console.WriteLine(message.Line);
            Console.WriteLine(message.GameCardsDealtSuccess(numCards,playerNumber));
        }
        else
        {
            Console.WriteLine(message.Line);
            Console.WriteLine(message.GameDealErrorPlayerDoesNotExist);
        }
    }

    public List<Card> GetListOfCardsPlayer(int playerNumber)
    {
        int playerIndex = playerNumber - 1;
        if (playerIndex < players.Count && playerIndex>=0)
        {
            return players[playerIndex].GetHand();
        }
        else
        {
            Console.WriteLine(message.Line);
            Console.WriteLine(message.GameGetCradsErrorNoPlayer(playerNumber));
            return new List<Card>();
        }
    }

    public void ShuffleGameDeck()
    {
        if (shoe.Count > 1)
        {
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                int n = shoe.Count;
                while (n > 1)
                {
                    byte[] box = new byte[1];
                    do provider.GetBytes(box);
                    while (!(box[0] < n * (Byte.MaxValue / n)));
                    int k = (box[0] % n);
                    n--;
                    Card value = shoe[k];
                    shoe[k] = shoe[n];
                    shoe[n] = value;
                }
            }
            Console.WriteLine(message.Line);
            Console.WriteLine(message.GameShuffleGameDeckSuccess);
        }
        else
        {
            Console.WriteLine(message.Line);
            Console.WriteLine(message.GameShuffleGameDeckFailed);
        }
    }

    public List<KeyValuePair<int, int>> GetPlayerListSortedByTotalValue()
    {
        List<KeyValuePair<int, int>> playerValues = new List<KeyValuePair<int, int>>();

        if (players.Count > 0)
        {
            for (int i = 0; i < players.Count; ++i)
            {
                playerValues.Add(new KeyValuePair<int, int>(i, players[i].GetTotalValue()));
            }

            playerValues.Sort((a, b) => b.Value.CompareTo(a.Value));

            return playerValues;
        }
        else
        {
            Console.WriteLine(message.Line);
            Console.WriteLine(message.GetPlayerListSortedByTotalValueFailed);
            return new List<KeyValuePair<int, int>>();
        }
    }

    public Dictionary<Suit, int> GetUndealtCountPerSuit()
    {
        Dictionary<Suit, int> undealtCount = new Dictionary<Suit, int>();

        foreach (Card card in shoe)
        {
            if (undealtCount.ContainsKey(card.Suit))
                undealtCount[card.Suit]++;
            else
                undealtCount[card.Suit] = 1; //ToDo: check validity
        }
        return undealtCount;
    }

    public List<KeyValuePair<KeyValuePair<Suit, Value>, int>> GetRemainingCardCount()
    {
        Dictionary<KeyValuePair<Suit, Value>, int> cardCount = new Dictionary<KeyValuePair<Suit, Value>, int>();

        foreach (Card card in shoe)
        {
            KeyValuePair<Suit, Value> cardKey = new KeyValuePair<Suit, Value>(card.Suit, card.Value);
            if (cardCount.ContainsKey(cardKey))
                cardCount[cardKey]++;
            else
                cardCount[cardKey] = 1; //ToDo: check validity
        }

        List<KeyValuePair<KeyValuePair<Suit, Value>, int>> cardList = cardCount.ToList();
        cardList.Sort((a, b) =>
        {
            int suitComparison = a.Key.Key.CompareTo(b.Key.Key);
            return (suitComparison == 0) ? b.Key.Value.CompareTo(a.Key.Value) : suitComparison;
        });

        return cardList;
    }
}

public class GameHandler
{
    private Game game;
    private Messages message;

    public GameHandler()
    {
        game = new Game();
        message = new Messages();
    }

    public Game CreateGame(int playerNum, int decks)
    {
        if (decks <= 0 || playerNum <= 0)
        {
            Console.WriteLine(message.GameHandlerInvalidParamforGameCreation);
            return null;
        }

        for (int i = 0; i < decks; i++)
        {
            game.AddDeckToShoe();
        }

        for (int i = 0; i < playerNum; i++)
        {
            game.AddPlayer();
        }

        Console.WriteLine($"{message.GameHandlerNewGameCreated} {playerNum}, {decks}");

        game.ShuffleGameDeck();

        return game;
    }

    public void DisplayReport(int playerNumber)
    {
        var list = game.GetListOfCardsPlayer(playerNumber);
        Console.WriteLine(message.Line);
        Console.WriteLine($"{message.GameHandlerListCardsplayer} {playerNumber} is:");  //ToDo check 

        foreach (var card in list)
        {
            Console.WriteLine($"Suit: {card.Suit}, Value: {card.Value}");
        }

        var list2 = game.GetPlayerListSortedByTotalValue();
        Console.WriteLine("----------");
        Console.WriteLine(message.GameHandlerListPlayersSorted);

        foreach (var elem in list2)
        {
            Console.WriteLine($"Player {elem.Key + 1}, Value: {elem.Value}");
        }

        var list3 = game.GetRemainingCardCount();
        Console.WriteLine("----------");
        Console.WriteLine(message.GameHandlerRemainingCards);

        foreach (var elem in list3)
        {
            Console.WriteLine($"Suit: {elem.Key.Key}, Value: {elem.Key.Value}, Count: {elem.Value}");
        }

        var list4 = game.GetUndealtCountPerSuit();
        Console.WriteLine("----------");
        Console.WriteLine(message.GameHandlerUndealtCardsPerSuit);

        foreach (var elem in list4)
        {
            Console.WriteLine($"Suit: {elem.Key}, Count: {elem.Value}");
        }

        Console.WriteLine("----end-----");
    }

    public void deleteGame()
    {
        game = null;
        Console.WriteLine();
        Console.WriteLine(message.GameHandlerGameDeleted);
    }
}



class Program
{
    static void Main()
    {
        GameHandler gameHandler = new GameHandler();
        Game game = gameHandler.CreateGame(4, 2); // Creates a game with given number of players and decks and shuffles the shoe.
        game.RemovePlayer(2);

        game.ShuffleGameDeck(); // On-demand shuffle of the shoe.

        game.DealCardsToPlayer(1, 3); // Deals the given player the given number of cards
        game.DealCardsToPlayer(2, 3);
        game.DealCardsToPlayer(3, 3);

        gameHandler.DisplayReport(1); // Displays the status of the game and especially cards in the hand of the given player.
        gameHandler.deleteGame();
    }
}