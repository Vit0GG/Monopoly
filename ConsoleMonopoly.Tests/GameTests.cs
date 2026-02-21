using Xunit;
using ConsoleMonopoly;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleMonopoly.Tests
{
    public class GameTests
    {
        [Fact]
        public void StartField_GivesMoney()
        {
            // Arrange
            var game = new Game();
            var player = new Player { Name = "TestPlayer", Money = 0 };
            var startField = new StartField();

            // Act
            startField.LandOn(player, game);

            // Assert
            Assert.Equal(200, player.Money);
        }

        [Fact]
        public void Jail_SkipsTurn()
        {
            var game = new Game();
            var player = new Player { Name = "Prisoner" };
            var jail = new Jail();

            jail.LandOn(player, game);

            Assert.True(player.SkipTurn);
        }

        [Fact]
        public void PenaltyField_TakesMoney()
        {
            var game = new Game();
            var player = new Player { Name = "Loser", Money = 500 };
            var penalty = new PenaltyField("Penalty", 100);

            penalty.LandOn(player, game);

            Assert.Equal(400, player.Money);
        }

        [Fact]
        public void Property_Buy_Success()
        {
            var game = new Game();
            var player = new Player { Name = "Buyer", Money = 1000 };
            var prop = new Property("Street", 200, 1);

            prop.LandOn(player, game);

            Assert.Equal(player, prop.Owner);
            Assert.Equal(800, player.Money);
            Assert.Contains(prop, player.Properties);
        }

        [Fact]
        public void Property_PayRent_Success()
        {
            var game = new Game();
            var owner = new Player { Name = "Owner", Money = 1000 };
            var guest = new Player { Name = "Guest", Money = 1000 };
            var prop = new Property("Street", 200, 1) { Owner = owner }; // Устанавливаем владельца

            prop.LandOn(guest, game);

            // Рента = 200 / 2 = 100
            Assert.Equal(900, guest.Money);
            Assert.Equal(1100, owner.Money);
        }

        [Fact]
        public void Decorator_Upgrade_Logic()
        {
            var prop = new Property("Street", 200, 1);

            // Базовая рента (100)
            Assert.Equal(100, prop.GetRent());

            // 1 уровень
            var level1 = new FirstUpGrade(prop);
            // 100 * 1.5 = 150
            Assert.Equal(150, level1.GetRent());

            // 2 уровень
            var level2 = new SecondUpGrade(prop);
            // 100 * 2.0 = 200
            Assert.Equal(200, level2.GetRent());

            // 3 уровень
            var level3 = new ThirdUpGrade(prop);
            // 100 * 2.5 = 250
            Assert.Equal(250, level3.GetRent());
        }

        [Fact]
        public void Bankruptcy_Logic()
        {
            var game = new Game();
            var player = new Player { Name = "Bankrupt", Money = 50 };
            var prop = new Property("House", 100, 1);
            player.Properties.Add(prop);
            prop.Owner = player;

            game.ApplyPenalty(player, 200);

            Assert.True(player.IsBankrupt);
            Assert.Empty(player.Properties);
            Assert.Null(prop.Owner);
        }
        [Fact]
        public void BonusField_GivesMoney()
        {
            var game = new Game();
            var player = new Player { Name = "Lucky", Money = 100 };
            var bonusField = new BonusField("Jackpot", 500);

            bonusField.LandOn(player, game);

            Assert.Equal(600, player.Money);
        }

        [Fact]
        public void Monopoly_Decorator_DoublesRent()
        {
            var prop = new Property("Street", 200, 1);
            // Базовая рента 100.

            var monopoly = new Monopoly(prop);
            // Монополия удваивает ренту -> 200.

            Assert.Equal(200, monopoly.GetRent());
        }

        [Fact]
        public void Board_Initialization_Check()
        {
            // Этот тест покроет код конструктора Board, который создает 32 клетки
            var board = new Board();

            Assert.Equal(32, board.Cells.Count);
            Assert.IsType<StartField>(board.Cells[0]);
            Assert.Contains(board.Cells, c => c is Jail);
        }

        [Fact]
        public void Cannot_Buy_If_No_Money()
        {
            var game = new Game();
            var player = new Player { Name = "Poor", Money = 10 }; // Мало денег
            var prop = new Property("Expensive", 500, 1);

            prop.LandOn(player, game);

            Assert.Null(prop.Owner);
            Assert.Empty(player.Properties);
            Assert.Equal(10, player.Money); // Деньги не списались
        }

        [Fact]
        public void Upgrade_Logic_With_FullGroup()
        {
            var game = new Game();
            var player = new Player { Name = "Tycoon", Money = 5000 };

            var prop1 = new Property("Prop1", 100, 1);
            var prop2 = new Property("Prop2", 100, 1);

            game.Board.Cells.Clear();
            game.Board.Cells.Add(prop1);
            game.Board.Cells.Add(prop2);

            prop1.Owner = player;
            prop2.Owner = player;
            player.Properties.Add(prop1);
            player.Properties.Add(prop2);


            prop1.LandOn(player, game);

            Assert.Equal(1, prop1.CountUpGrade);
            Assert.IsType<FirstUpGrade>(game.Board.Cells[0]);
        }
                [Fact]
        public void Final_Coverage_Booster()
        {
            var game = new Game();
            game.Players.Add(new Player { Name = "A" });
            game.Players.Add(new Player { Name = "B" });

            var roll = game.RollDice();
            Assert.InRange(roll.Total, 2, 12);

            game.CurrentPlayerIndex = 0;
            game.NextPlayer();
            Assert.Equal(1, game.CurrentPlayerIndex);
            game.NextPlayer();
            Assert.Equal(0, game.CurrentPlayerIndex);

            var propPrint = new Property("PrintProp", 100, 99);
            propPrint.CountUpGrade = 2;
            game.Players[0].Properties.Add(propPrint);
            game.PrintStatus(); 

            var player = game.Players[0];
            var prop = new Property("Solo", 100, 5); 
            
            game.Board.Cells.Clear();
            game.Board.Cells.Add(prop); 
            game.Board.Cells.Add(new Property("Other", 100, 5)); 

            prop.Owner = player;
            player.Properties.Add(prop);
            player.Money = 1000;
            
            prop.LandOn(player, game); 
            
            Assert.Equal(0, prop.CountUpGrade);
        }
    }
    
}