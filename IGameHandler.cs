public interface IGameHandler
{
    Game CreateGame(int playerNum, int decks);
    void deleteGame();
    void DisplayReport(int playerNumber);
}