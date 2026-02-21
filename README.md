# Monopoly

## Описание проекта

Консольная реализация настольной игры **Монополия** на языке C# (.NET 9.0).  
Два игрока по очереди бросают кости, перемещаются по полю из 32 клеток, покупают собственность известных брендов, улучшают её и пытаются обанкротить соперника.

### Ключевые механики
- Покупка собственности (Mercedes-Benz, Apple, Tesla, Nike и др.)
- Улучшение собственности до 3 уровней (филиалы)
- Аренда с учётом улучшений и монополии на цветовую группу
- Штрафные и бонусные поля
- Тюрьма (пропуск хода)
- Автоматическая продажа имущества при нехватке средств

---

## Инструкция по сборке

### Требования

| Компонент | Версия |
|-----------|--------|
| .NET SDK  | 9.0+   |

> Скачать .NET SDK можно [здесь](https://dotnet.microsoft.com/download/dotnet/9.0)

### Шаги

```bash
# 1. Клонировать репозиторий
git clone https://github.com/Vit0GG/Monopoly.git
cd Monopoly

# 2. Восстановить зависимости
dotnet restore

# 3. Собрать проект
dotnet build

# 4. Запустить
dotnet run


Инструкция по использованию
После запуска игра работает полностью автоматически — участвуют два игрока с начальным балансом 1500$.

Типы полей
Поле	Описание
Старт	+200$ при попадании
Собственность	Покупка / оплата аренды / улучшение
Штрафное поле	−100$
Бонус	+150$
Тюрьма	Пропуск следующего хода
Цветовые группы
Группа	Бренды
Красный	Mercedes-Benz, KFC
Синий	UFC, Nike
Зелёный	Apple, Microsoft
Жёлтый	Tesla, Coca-Cola
Каждая группа дублируется (Mercedes-Benz2, KFC2 и т.д.)

Правила улучшения
Необходимо владеть всеми полями цветовой группы
Ни одно поле в группе не должно иметь улучшений
Стоимость улучшения — 100$
Максимум — 3 уровня на поле
Банкротство
При нехватке средств игрок автоматически:

Продаёт улучшения (по 50$ за уровень)
Продаёт собственность (за половину цены)
Если средств всё ещё не хватает — банкротится
Игра завершается, когда остаётся один платёжеспособный игрок.

Лицензия
Проект распространяется под лицензией MIT.


MIT License

Copyright (c) 2026 [Vit0GG]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Связь
Канал	Ссылка
GitHub	Vit0GG
Email	st129005@student.spbu.ru
Telegram	@djfem1nist
Дополнительная информация
Структура проекта
text

ConsoleMonopoly/
├── Program.cs                  # Все классы и точка входа
├── Monopoly.csproj             # Файл проекта .NET 9.0
└── README.md                   # Документация
Диаграмма классов


Cell (abstract)
├── StartField
├── BonusField
├── Jail
├── PenaltyField
└── Property
    └── LotSquareDecorator (abstract)
        ├── Monopoly
        ├── FirstUpGrade
        ├── SecondUpGrade
        └── ThirdUpGrade
        
Player
Board
Game
```
