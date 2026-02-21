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
            // Добавим имущество, которое придется продать
            var prop = new Property("House", 100, 1); 
            player.Properties.Add(prop);
            prop.Owner = player;

            // Штраф больше, чем денег (50) + цена продажи дома (50) = 100. Нужно 200.
            game.ApplyPenalty(player, 200);

            Assert.True(player.IsBankrupt);
            Assert.Empty(player.Properties);
            Assert.Null(prop.Owner);
        }
    }
}