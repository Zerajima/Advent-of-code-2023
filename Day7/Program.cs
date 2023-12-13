namespace Day7;

internal static class Program
{
    private const string TestInputPath = "TestInput.txt";
    private const string RealInputPath = "RealInput.txt";

    private static Dictionary<char, int> cardRank = new Dictionary<char, int>()
    {
        { '2', 2 },
        { '3', 3 },
        { '4', 4 },
        { '5', 5 },
        { '6', 6 },
        { '7', 7 },
        { '8', 8 },
        { '9', 9 },
        { 'T', 10 },
        { 'J', 1 },
        { 'Q', 12 },
        { 'K', 13 },
        { 'A', 14 },
    };

    static void Main(string[] args)
    {
        //Task1(RealInputPath);
        Task2(RealInputPath);
    }

    private static void Task1(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var games = input.Select(x =>
        {
            var parts = x.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return new Game()
            {
                Cards = parts[0],
                Bid = long.Parse(parts[1])
            };
        }).ToList();
        
        games.Sort(Comparison1);

        long gameSum = 0;
        for (int i = 1; i <= games.Count; i++)
        {
            var game = games[i-1];
            checked
            {
                gameSum += game.Bid * i;       
            }
        }

        Console.WriteLine(gameSum);
    }

    private static int Comparison1(Game game1, Game game2)
    {
        var game1Rank = game1.GetRank();
        var game2Rank = game2.GetRank();

        if (game1Rank != game2Rank)
        {
            return game1Rank > game2Rank ? 1 : -1;
        }

        for (int i = 0; i < game1.Cards.Length; i++)
        {
            var card1 = game1.Cards[i];
            var card2 = game2.Cards[i];
            if(card1 == card2) continue;

            return cardRank[card1] > cardRank[card2] ? 1 : -1;
        }

        return 0;
    }

    private static void Task2(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);
        var games = input.Select(x =>
        {
            var parts = x.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return new Game()
            {
                Cards = parts[0],
                Bid = long.Parse(parts[1])
            };
        }).ToList();
        
        games.Sort(Comparison2);

        long gameSum = 0;
        for (int i = 1; i <= games.Count; i++)
        {
            var game = games[i-1];
            checked
            {
                gameSum += game.Bid * i;       
            }
        }

        Console.WriteLine(gameSum);
    }
    
    private static int Comparison2(Game game1, Game game2)
    {
        var game1Rank = game1.GetRank2();
        var game2Rank = game2.GetRank2();

        if (game1Rank != game2Rank)
        {
            return game1Rank > game2Rank ? 1 : -1;
        }

        for (int i = 0; i < game1.Cards.Length; i++)
        {
            var card1 = game1.Cards[i];
            var card2 = game2.Cards[i];
            if(card1 == card2) continue;

            return cardRank[card1] > cardRank[card2] ? 1 : -1;
        }

        return 0;
    }

    public class Game
    {
        public string Cards { get; set; }
        
        public long Bid { get; set; }

        public int GetRank()
        {
            var cardCounts = new Dictionary<char, int>();
            foreach (var card in Cards)
            {
                if (cardCounts.ContainsKey(card))
                {
                    cardCounts[card] += 1;
                }
                else
                {
                    cardCounts.Add(card, 1);
                }
            }

            if (cardCounts.Any(x => x.Value == 5)) return 10;

            if (cardCounts.Any(x => x.Value == 4)) return 9;

            if (cardCounts.Any(x => x.Value == 3) && cardCounts.Any(x => x.Value == 2)) return 8;

            if (cardCounts.Any(x => x.Value == 3)) return 7;

            if (cardCounts.Count(x => x.Value == 2) == 2) return 6;

            if (cardCounts.Count(x => x.Value == 2) == 1) return 5;

            return 4;
        }
        
        public int GetRank2()
        {
            var cardCounts = new Dictionary<char, int>();
            foreach (var card in Cards)
            {
                if (cardCounts.ContainsKey(card))
                {
                    cardCounts[card] += 1;
                }
                else
                {
                    cardCounts.Add(card, 1);
                }
            }

            var jCount = 0;
            if (cardCounts.ContainsKey('J')) jCount = cardCounts['J'];
            var topCard = cardCounts.Count(x => x.Key != 'J') > 0
                ? cardCounts.Where(x => x.Key != 'J').MaxBy(x => x.Value).Value
                : 0;
            var secondTopCard = cardCounts.Count(x => x.Key != 'J') > 1
                ? cardCounts.Where(x => x.Key != 'J').OrderByDescending(x => x.Value).Skip(1).First().Value
                : 0;

            if (topCard + jCount >= 5) return 10;
            
            if (topCard + jCount >= 4) return 9;

            if (topCard + jCount >= 3 && secondTopCard + (topCard - 3) + jCount >= 2) return 8;

            if (topCard + jCount >= 3) return 7;

            if (topCard + jCount >= 2 && secondTopCard + (topCard - 2) + jCount >= 2) return 6;

            if (topCard + jCount >= 2) return 5;
            return 4;
        }

        public override string ToString()
        {
            return $"{Cards} - {GetRank2()}";
        }
    }
}