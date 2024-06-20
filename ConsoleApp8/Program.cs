using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



namespace SnakeGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            GamePreferences gamePreferences = new GamePreferences();

            Console.CursorVisible = false;
            Console.Title = "Snake";
            Console.SetWindowSize(20, 10);
            Console.SetBufferSize(20, 12);
            Console.ForegroundColor = ConsoleColor.White;

            Snake snake = new Snake();

            Random rnd = new Random();
            Food food = new Food(rnd.Next(0, Console.WindowWidth), rnd.Next(0, Console.WindowHeight));

            Menu menu = new Menu();

            Task task = ActionHandler(snake, gamePreferences, menu);
            MainCycle(gamePreferences, snake, food, menu);
        }
        public class GamePreferences
        {
            public bool isShowing = true;
            public string gameMode = "gameplay";
        }

        public class Snake
        {
            private int lenght = 1;

            public List<BodyPoint> body = new List<BodyPoint>() { new BodyPoint(0,0) , new BodyPoint(0, 0) };

            public string movementDirection = "right";

            public bool CompareSnakeAndFood(Food food)
            {
                int snakeCount = this.body.Count - 1;
                if (this.body[snakeCount].x == food.x && this.body[snakeCount].y == food.y) return true;
                return false;
            }
            public void MoveSnake(int changeX, int changeY,ref Food food)
            {
                int newX = this.body[body.Count - 1].x + changeX;
                int newY = this.body[body.Count - 1].y + changeY;
                this.body.Add(new BodyPoint(newX, newY));
                Random rnd = new Random();
                if (!(this.CompareSnakeAndFood(food)))
                {
                    this.body.RemoveAt(0);
                }
                else if ((this.CompareSnakeAndFood(food)))
                {
                    food = new Food(rnd.Next(0, Console.WindowWidth), rnd.Next(0, Console.WindowHeight));
                }
            }
        }
        public class BodyPoint 
        {
            public int x = 0;
            public int y = 0;
            public BodyPoint(int x, int y)
            {  
                this.x = x; this.y = y;
            }
        }
        public class Food 
        {
            public int x = 0;
            public int y = 0;
            public Food(int x, int y)
            {
                this.x = x; this.y = y;
            }
        }
        public class Menu
        {
            public List<string> menuItems = new List<string>() { "Resume", "Preferences", "Exit" };
            public int selectedItemIndex = 0;
            public bool varForAnimation = true;
            public int verticalMenuPad = 3;
            public int horizontalMenuPad = 6;
            public void SelectNextItem()
            {
                if(selectedItemIndex == menuItems.Count - 1)
                {
                    selectedItemIndex = 0;
                }
                else
                {
                    selectedItemIndex++;
                }
            }
            public void SelectPreviousItem()
            {
                if (selectedItemIndex == 0)
                {
                    selectedItemIndex = menuItems.Count - 1;
                }
                else
                {
                    selectedItemIndex--;
                }
            }

        }


        private static void MainCycle(GamePreferences gamePreferences, Snake snake, Food food, Menu menu)
        {
            while (gamePreferences.isShowing)
            {
                string line = "";
                if (gamePreferences.gameMode == "gameplay")
                {
                    for (int i = 0; i < Console.WindowHeight; i++)
                    {
                        for (int j = 0; j < Console.WindowWidth; j++)
                        {
                            string buffer = ".";

                            if (snake.body[snake.body.Count-1].y < 0 || snake.body[snake.body.Count - 1].x < 0 
                                || snake.body[snake.body.Count - 1].y > Console.WindowHeight || snake.body[snake.body.Count - 1].x > Console.WindowWidth)
                            {
                                gamePreferences.gameMode = "game over";
                            }
                            for (int n = 0; n < snake.body.Count; n++)
                            {
                                if (snake.body[n].x == j && snake.body[n].y == i)
                                {
                                    buffer = "M";
                                    break;
                                }
                            }
                            if ((food.x == j && food.y == i))
                            {
                                buffer = "O";
                            }
                            line = line + buffer;
                        }
                        line = line + "\n";
                    }
                    int cornersCount = snake.body.Count - 1;
                    int newX = 0;
                    int newY = 0;
                    Random rnd = new Random();
                    switch (snake.movementDirection)
                    {
                        case "right":
                            snake.MoveSnake(1, 0, ref food);
                            break;
                        case "left":
                            snake.MoveSnake(-1, 0, ref food);
                            break;
                        case "up":
                            snake.MoveSnake(0, -1, ref food);
                            break;
                        case "down":
                            snake.MoveSnake(0, 1, ref food);
                            break;
                    }
                    if (snake.movementDirection == "right" || snake.movementDirection == "left") Thread.Sleep(100);
                    else Thread.Sleep(200);
                }
                else if(gamePreferences.gameMode == "menu")
                {
                    int itemIndex = 0;
                    for (int i = 0; i < Console.WindowHeight; i++)
                    {
                        for (int j = 0; j < Console.WindowWidth; j++)
                        {
                            string buffer = ".";
                            if(i > menu.verticalMenuPad && j > menu.horizontalMenuPad
                                && menu.menuItems.Count > itemIndex)
                            {
                                if (itemIndex == menu.selectedItemIndex && line != "")
                                {
                                    int index = line.Length - 1;
                                    line = line.Remove(index-1, 1).Insert(index, "*");
                                }
                                buffer = menu.menuItems[itemIndex].PadRight(Console.WindowWidth- menu.horizontalMenuPad - menu.menuItems.Count,'.');
                                j += Console.WindowWidth - menu.horizontalMenuPad;
                                itemIndex++;
                            }

                            line = line + buffer;
                        }
                        line = line + "\n";
                    }
                }
                else if (gamePreferences.gameMode == "game over")
                {
                    for (int i = 0; i < Console.WindowHeight; i++)
                    {
                        for (int j = 0; j < Console.WindowWidth; j++)
                        {
                            string text = "Game over! :(";
                            string buffer = ".";
                            int height = Console.WindowHeight;
                            int width = Console.WindowWidth;
                            int horizontalPad = (int)((width - text.Length) / 2);
                            if (i == (int)(height / 2) && j == horizontalPad)
                            {
                                buffer = text.PadRight(horizontalPad, '.');
                            }

                            line = line + buffer;
                        }
                        line = line + "\n";
                    }

                }
                else
                {
                    Console.WriteLine("Ошибка игровых режимов");
                }
                Console.WriteLine(line);
                Console.SetCursorPosition(0, 0);
                line = "";
            }
        }
        public static async Task ActionHandler(Snake snake, GamePreferences gamePreferences, Menu menu)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    var b = Console.ReadKey().Key;
                    switch (b)
                    {
                        case ConsoleKey.UpArrow:
                            if (gamePreferences.gameMode == "gameplay" && !(snake.movementDirection=="down")) snake.movementDirection = "up";
                            else if(gamePreferences.gameMode == "menu") menu.SelectPreviousItem();
                            break;
                        case ConsoleKey.DownArrow:
                            if (gamePreferences.gameMode == "gameplay" && !(snake.movementDirection == "up")) snake.movementDirection = "down";
                            else if (gamePreferences.gameMode == "menu") menu.SelectNextItem();
                                break;
                        case ConsoleKey.RightArrow:
                            if (gamePreferences.gameMode == "gameplay" && !(snake.movementDirection == "left")) snake.movementDirection = "right";
                            break;
                        case ConsoleKey.LeftArrow:
                            if (gamePreferences.gameMode == "gameplay" && !(snake.movementDirection == "right")) snake.movementDirection = "left";
                            break;
                        case ConsoleKey.Escape:
                            gamePreferences.gameMode = "menu";
                            break;
                        case ConsoleKey.Enter:
                            switch (menu.menuItems[menu.selectedItemIndex])
                            {
                                case "Resume":
                                    gamePreferences.gameMode = "gameplay";
                                    break;
                                case "Preferences":
                                    gamePreferences.gameMode = "preferences";
                                    break;
                                case "Exit":
                                    Environment.Exit(0);
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                }
            });

        }
    }
}
