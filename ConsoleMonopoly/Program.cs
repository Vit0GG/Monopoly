namespace ConsoleMonopoly
{
    public abstract class Cell
    {
        public string Name { get; set; }
        public abstract void LandOn(Player player, Game game);
    }

    public class StartField : Cell
    {
        public override void LandOn(Player player, Game game)
        {
            player.Money += 200;
            Console.WriteLine($"{player.Name} получил 200 за проход стартового поля.");
        }
    }

    public class BonusField : Cell
    {
        public int BonusAmount { get; set; }
        public BonusField(string name, int bonus)
        {
            Name = name;
            BonusAmount = bonus;
        }
        public override void LandOn(Player player, Game game)
        {
            player.Money += BonusAmount;
            Console.WriteLine($"{player.Name} получил бонус: {BonusAmount}.");
        }
    }

    public class Jail : Cell
    {
        public override void LandOn(Player player, Game game)
        {
            Console.WriteLine($"{player.Name} попал в тюрьму. Пропускает следующий ход.");
            player.SkipTurn = true;
        }
    }

    public class PenaltyField : Cell
    {
        public int PenaltyAmount { get; set; }
        public PenaltyField(string name, int penalty)
        {
            Name = name;
            PenaltyAmount = penalty;
        }
        public override void LandOn(Player player, Game game)
        {
            game.ApplyPenalty(player, PenaltyAmount);
        }
    }

    public abstract class LotSquareDecorator : Property
    {
        protected Property property;

        protected LotSquareDecorator(Property property) : base(property.Name, property.Price, property.ColorGroup)
        {
            this.property = property;
            this.Owner = property.Owner;
            this.BaseRent = property.BaseRent;
            this.CountUpGrade = property.CountUpGrade;
        }

        public override int GetRent()
        {
            return property.GetRent();
        }
    }

    public class Monopoly : LotSquareDecorator
    {
        public Monopoly(Property property) : base(property) { }

        public override int GetRent()
        {
            return base.GetRent() * 2;
        }

        public override void LandOn(Player player, Game game)
        {
            property.LandOn(player, game);
        }
    }

    public class FirstUpGrade : LotSquareDecorator
    {
        public FirstUpGrade(Property property) : base(property) { this.CountUpGrade = 1; }

        public override int GetRent()
        {
            return (int)(base.GetRent() * 1.5);
        }
        
        public override void LandOn(Player player, Game game)
        {
            property.LandOn(player, game);
        }
    }

    public class SecondUpGrade : LotSquareDecorator
    {
        public SecondUpGrade(Property property) : base(property) { this.CountUpGrade = 2; }

        public override int GetRent()
        {
            return (int)(base.GetRent() * 2.0);
        }

        public override void LandOn(Player player, Game game)
        {
            property.LandOn(player, game);
        }
    }

    public class ThirdUpGrade : LotSquareDecorator
    {
        public ThirdUpGrade(Property property) : base(property) { this.CountUpGrade = 3; }

        public override int GetRent()
        {
            return (int)(base.GetRent() * 2.5);
        }

        public override void LandOn(Player player, Game game)
        {
            property.LandOn(player, game);
        }
    }

    public class Property : Cell
    {
        public Player Owner { get; set; }
        public int BaseRent { get; set; }
        public int Price { get; set; }
        public int CountUpGrade { get; set; } = 0;
        public int ColorGroup { get; set; }

        public Property(string name, int price, int colorGroup)
        {
            Name = name;
            Price = price;
            BaseRent = price / 2;
            ColorGroup = colorGroup;
        }

        private string GetColorByGroup(int group)
        {
            return group switch
            {
                0 => "Красный",
                1 => "Синий",
                2 => "Зеленый",
                3 => "Желтый",
                _ => "Белый"
            };
        }

        public virtual int GetRent()
        {
            return BaseRent + (CountUpGrade * 50);
        }

        public override void LandOn(Player player, Game game)
        {
            if (Owner == null)
            {
                if (player.Money >= Price)
                {
                    Console.WriteLine($"{player.Name} покупает {Name} за {Price}.");
                    player.Money -= Price;
                    Owner = player;
                    player.Properties.Add(this);
                }
                else
                {
                    Console.WriteLine($"{player.Name} не может купить {Name}. Недостаточно средств.");
                }
            }
            else if (Owner != player)
            {
                int rent = GetRent();
                Console.WriteLine($"{player.Name} должен заплатить {rent} {Owner.Name} за поле {Name}.");
                game.ApplyPenalty(player, rent, Owner);
            }
            else
            {
                var groupProperties = game.Board.Cells.OfType<Property>().Where(p => p.ColorGroup == ColorGroup && p.Owner == player);
                bool ownsFullGroup = groupProperties.Count() == game.Board.Cells.OfType<Property>().Count(p => p.ColorGroup == ColorGroup);
                bool noCountUpGradeInGroup = groupProperties.All(p => p.CountUpGrade == 0);

                if (CountUpGrade < 3 && player.Money >= 100 && ownsFullGroup && noCountUpGradeInGroup)
                {
                    Console.WriteLine($"{player.Name} улучшает {Name}. Филиалов: {CountUpGrade + 1}.");
                    player.Money -= 100;
                    CountUpGrade++;

                    // Обновление декоратора до соотвествующего
                    var index = game.Board.Cells.IndexOf(this);
                    Cell upgraded = this;
                    upgraded = CountUpGrade switch
                    {
                        1 => new FirstUpGrade(this),
                        2 => new SecondUpGrade(this),
                        3 => new ThirdUpGrade(this),
                        _ => upgraded
                    };

                    game.Board.Cells[index] = upgraded;
                }
                else if (!ownsFullGroup || !noCountUpGradeInGroup)
                {
                    Console.WriteLine($"{player.Name} не может улучшить {Name}. Требуется владение всей группой без улучшений.");
                }
                else
                {
                    Console.WriteLine($"{player.Name} не хватает денег для улучшения {Name}.");
                }
            }
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public int Money { get; set; } = 1500;
        public int Position { get; set; } = 0;
        public List<Property> Properties { get; set; } = new();
        public bool SkipTurn { get; set; } = false;

        public bool IsBankrupt => Money <= 0 && Properties.Count == 0;
    }

    public class Board
    {
        public List<Cell> Cells { get; set; } = new();

        public Board()
        {
            var customProperties = new List<(string Name, int Price, int ColorGroup)>
            {
                ("Mercedes-Benz", 300, 0),
                ("KFC", 250, 0),
                ("UFC", 280, 1),
                ("Nike", 270, 1),
                ("Apple", 350, 2),
                ("Microsoft", 330, 2),
                ("Tesla", 400, 3),
                ("Coca-Cola", 310, 3),
                ("Mercedes-Benz2", 300, 0),
                ("KFC2", 250, 0),
                ("UFC2", 280, 1),
                ("Nike2", 270, 1),
                ("Apple2", 350, 2),
                ("Microsoft2", 330, 2),
                ("Tesla2", 400, 3),
                ("Coca-Cola2", 310, 3)
            };

            int index = 0;
            Cells.Add(new StartField { Name = "Старт" });
            foreach (var (name, price, group) in customProperties)
            {
                Cells.Add(new Property(name, price, group));
                index++;
                if (index % 4 == 0) Cells.Add(new PenaltyField("Штрафное поле", 100));
                if (index % 5 == 0) Cells.Add(new BonusField("Бонус", 150));
                if (index % 6 == 0) Cells.Add(new Jail { Name = "Тюрьма" });
            }
            while (Cells.Count < 32)
                Cells.Add(new PenaltyField("Штрафное поле", 100));
        }
    }

    public class Game
    {
        public Board Board { get; set; } = new();
        public List<Player> Players { get; set; } = new();
        public int CurrentPlayerIndex { get; set; } = 0;
        private Random random = new();
        private int CountSteps = 0;

        public void Start()
        {
            while (Players.Count(p => !p.IsBankrupt) > 1)
            {
                var player = Players[CurrentPlayerIndex];
                if (player.IsBankrupt) { NextPlayer(); continue; }
                if (player.SkipTurn) { Console.WriteLine($"{player.Name} пропускает ход."); player.SkipTurn = false; NextPlayer(); continue; }

                var (dice1, dice2, roll) = RollDice();
                Console.WriteLine($"\nВыпало: {dice1} и {dice2} (Сумма: {roll})");
                Console.WriteLine($"Ход игрока: {player.Name}");

                player.Position = (player.Position + roll) % Board.Cells.Count;
                var cell = Board.Cells[player.Position];

                Console.WriteLine($"{player.Name} попал на поле: {cell.Name}");
                cell.LandOn(player, this);

                PrintStatus();
                CountSteps++;
                NextPlayer();
            }

            var winner = Players.FirstOrDefault(p => !p.IsBankrupt);
            Console.WriteLine($"Игра окончена! Победил: {winner?.Name}");
            Console.WriteLine($"Общее количество шагов: {CountSteps}");
        }

        public (int Dice1, int Dice2, int Total) RollDice() => (random.Next(1, 7), random.Next(1, 7), random.Next(1, 7) + random.Next(1, 7));

        public void ApplyPenalty(Player player, int amount, Player to = null)
        {
            if (player.Money >= amount)
            {
                player.Money -= amount;
                if (to != null) to.Money += amount;
            }
            else
            {
                Console.WriteLine($"{player.Name} не может заплатить штраф: {amount}. Недостаточно средств.");

                foreach (var prop in player.Properties.OrderByDescending(p => p.CountUpGrade))
                {
                    while (prop.CountUpGrade > 0 && player.Money < amount)
                    {
                        prop.CountUpGrade--;
                        player.Money += 50;
                        Console.WriteLine($"{player.Name} продает улучшение на {prop.Name} за 50.");
                    }
                }

                foreach (var prop in player.Properties.OrderBy(p => p.Price).ToList())
                {
                    if (player.Money >= amount) break;
                    player.Money += prop.Price / 2;
                    Console.WriteLine($"{player.Name} продает {prop.Name} за {prop.Price / 2}.");
                    prop.Owner = null;
                    player.Properties.Remove(prop);
                }

                if (player.Money >= amount)
                {
                    player.Money -= amount;
                    if (to != null) to.Money += amount;
                }
                else
                {
                    Console.WriteLine($"{player.Name} обанкротился!");
                    player.Money = 0;
                    foreach (var prop in player.Properties)
                        prop.Owner = null;
                    player.Properties.Clear();
                }
            }
        }

        public bool PlayerHasAllColorGroup(Player player, int group)
        {
            return Board.Cells.OfType<Property>().Where(p => p.ColorGroup == group).All(p => p.Owner == player);
        }

        public void PrintStatus()
        {
            foreach (var p in Players)
            {
                var props = string.Join(", ", p.Properties.Select(pr => $"{pr.Name} (филиалов: {pr.CountUpGrade})"));
                Console.WriteLine($"{p.Name} - Баланс: {p.Money}, Собственности: {props}");
            }
        }

        public void NextPlayer()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Players.Add(new Player { Name = "Игрок 1" });
            game.Players.Add(new Player { Name = "Игрок 2" });
            game.Start();
        }
    }
}
