public class Driver
{
    public Driver(int player)
    {
        this.Player = player;
        this.Name = player == 0 ? "AI" : $"Player {NumberUtils.NumberToWords(player)}";
    }

    public string Name { get; }

    public int Player { get; }
}
