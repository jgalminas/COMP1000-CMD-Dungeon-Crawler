using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Crawler
{
    /**
     * The main class of the Dungeon Crawler Application
     * 
     * You may add to your project other classes which are referenced.
     * Complete the templated methods and fill in your code where it says "Your code here".
     * Do not rename methods or variables which already exist or change the method parameters.
     * You can do some checks if your project still aligns with the spec by running the tests in UnitTest1
     * 
     * For Questions do contact us!
     */

    public class CMDCrawler
    {
        // Player actions
        public enum PlayerActions { NOTHING, NORTH, EAST, SOUTH, WEST, PICKUP, ATTACK, QUIT };
        private PlayerActions action = PlayerActions.NOTHING;

        // Tracks if the game is running.
        private bool active = true;

        // Indicates if "play" command was used.
        private bool started = false;

        public bool finished = false;

        // Indicates if the game is running in advanced mode.
        private bool advanced = false;

        private bool mapInitialised = false;

        // Original map.
        private char[][] originalMap = new char[0][];

        // Map which represents the game state.
        private char[][] map = new char[0][];

        private string currentMapName;

        // List of monsters
        private List<Monster> monsters;
        private List<Coins> coins;

        // Player
        private Player player;


        /// <summary>
        /// Inner abstract class of CMDCrawler which holds common behaviour of entities 
        /// </summary>
        private abstract class Entity
        {
            public int[] Position { get; set; }
            public CMDCrawler Game { get; set; }
            public bool IsAlive { get; set; }
            public int Health { get; set; }
            public int Coins { get; set; }

            /// <summary>
            /// Checks if coins and the entity share the same location, if so they are picked up.
            /// </summary>
            /// <param name="coins"></param>
            public void PickupCoins(List<Coins> coins)
            {
                foreach (Coins coin in coins)
                {
                    if (coin.Position[0].Equals(Position[0]) && coin.Position[1].Equals(Position[1]))
                    {
                        Coins += coin.Remove();
                        break;
                    }
                }
            }

            public abstract void TakeDamage();
            public abstract int[] Move();
        }

        /// <summary>
        /// Inner class of CMDCrawler which represents Coins
        /// </summary>
        private class Coins
        {
            public int[] Position { get; set; }
            public CMDCrawler Game { get; set; }
            public bool OnFloor { get; set; }
            public int Amount { get; set; }

            public Coins(int amount, int[] position, CMDCrawler game)
            {
                Game = game;
                Amount = amount;
                Position = position;
                OnFloor = true;
            }

            public int Remove()
            {
                OnFloor = false;
                Game.coins.Remove(this);
                return Amount;
            }
        }


        /// <summary>
        /// Inner class of CMDCrawler which inhertis from Entity and represents the player
        /// </summary>
        private class Player : Entity
        {

            public Player(CMDCrawler game, int y, int x)
            {
                Health = 5;
                Game = game;
                Position = new int[] { y, x };
                Coins = 0;
                IsAlive = true;
            }

            public override void TakeDamage()
            {
                if (Health > 1)
                {
                    Health -= 1;
                }
                else
                {
                    Health -= 1;
                    Game.started = false;
                    Game.finished = true;

                    IsAlive = false;
                }
            }


            /// <summary>
            /// Checks if there are any monsters in adjacent positions, if so the player does damage.
            /// </summary>
            /// <param name="monsters"></param>
            public void Attack(List<Monster> monsters)
            {
                foreach (Monster monster in monsters)

                    if (Game.advanced)
                    {
                        if (monster.Position[0].Equals(Position[0] + 1) && monster.Position[1].Equals(Position[1])
                         || monster.Position[0].Equals(Position[0] - 1) && monster.Position[1].Equals(Position[1])
                         || monster.Position[0].Equals(Position[0]) && monster.Position[1].Equals(Position[1] + 1)
                         || monster.Position[0].Equals(Position[0]) && monster.Position[1].Equals(Position[1] - 1))
                        {
                            monster.TakeDamage();
                            break;
                        }
                    }
            }

            /// <summary>
            /// Determines the moving direction based on input and moves to it if there are no collisions.
            /// </summary>
            public override int[] Move()
            {
                int[] previousPos = new int[]{ Position[0], Position[1] };
                int[] movingPos = new int[] { Position[0], Position[1] };

                // Determining the direction the player should be moving in based on the player action.
                switch (Game.GetPlayerAction())
                {
                    case 1:
                        movingPos[0] = Position[0] - 1;
                        break;
                    case 2:
                        movingPos[1] = Position[1] + 1;
                        break;
                    case 3:
                        movingPos[0] = Position[0] + 1;
                        break;
                    case 4:
                        movingPos[1] = Position[1] - 1;
                        break;
                    case 5:
                        if (Game.advanced)
                        {
                            PickupCoins(Game.coins);
                        }
                        break;
                    case 6:
                        Attack(Game.monsters);
                        break;
                    case 7:
                        Game.active = false;
                        break;
                    default:
                        break;
                }

                // Checking the char of the moving direction
                switch(Game.map[movingPos[0]][movingPos[1]])
                {
                    case '-':
                        Position = movingPos;
                        break;
                    case 'C':
                        Position = movingPos;
                        if (!Game.advanced)
                        {
                            PickupCoins(Game.coins);
                        }
                        break;
                    case 'X':
                        Game.started = false;
                        Game.finished = true;

                        Position = movingPos;
                        break;
                    default:
                        break;
                }

                return previousPos;
            }
        }

        /// <summary>
        /// Inner class of CMDCrawler which inherits from Entity and represents Monsters
        /// </summary>
        private class Monster : Entity
        {

            public Monster(CMDCrawler game, int y, int x)
            {
                Game = game;
                Health = new Random().Next(1, 4);
                Position = new int[] { y, x };
                IsAlive = true;
                Coins = new Random().Next(5, 11);
            }

            public override void TakeDamage()
            {
                if (Health > 0)
                {
                    Health -= 1;
                }
                else
                {
                    IsAlive = false;
                    Game.monsters.Remove(this);
                    Game.coins.Add(new Coins(Coins, Position, Game));
                }
            }

            /// <summary>
            /// Checks if the player is in adjacent positions, if so the player takes damage
            /// </summary>
            public void Attack()
            {
                if (Game.player.Position[0].Equals(Position[0] + 1) && Game.player.Position[1].Equals(Position[1])
                     || Game.player.Position[0].Equals(Position[0] - 1) && Game.player.Position[1].Equals(Position[1])
                     || Game.player.Position[0].Equals(Position[0]) && Game.player.Position[1].Equals(Position[1] + 1)
                     || Game.player.Position[0].Equals(Position[0]) && Game.player.Position[1].Equals(Position[1] - 1))
                {
                    Game.player.TakeDamage();
                }
            }


            /// <summary>
            /// Moves the monster in a random direction
            /// </summary>
            /// <returns> int[] Previous Position </returns>
            public override int[] Move()
            {
                int[] movingPos = new int[2];
                int[] previousPos = new int[2] { Position[0], Position[1] };

                int randomNum = new Random().Next(0, 2);

                if (IsAlive)
                {
                    // Determining the direction to move in
                    if (randomNum == 0)
                    {
                        movingPos[0] = Position[0] + new Random().Next(-1, 2);
                        movingPos[1] = Position[1];
                    }
                    else
                    {
                        movingPos[1] = Position[1] + new Random().Next(-1, 2);
                        movingPos[0] = Position[0];
                    }

                    // Checking the char of the moving position
                    switch (Game.map[movingPos[0]][movingPos[1]])
                    {
                        case '-':
                            Position = movingPos;
                            break;
                        case 'C':
                            Position = movingPos;
                            PickupCoins(Game.coins);
                            Health += 2;
                            break;
                        default:
                            break;
                    }
                }

                return previousPos;
            }
        }

        /// <summary>
        /// Reads user input and returns it
        /// </summary>
        /// <returns> string User input</returns>
        private string ReadUserInput()
        {
            string inputRead;

            if (started is false)
            {
                inputRead = Console.ReadLine();
            } else
            {
                inputRead = Console.ReadKey().KeyChar.ToString();
            }

            return inputRead;
        }

        /// <summary>
        /// Processes user input based on the state of the game
        /// </summary>
        /// <param name="input"></param>
        public void ProcessUserInput(string input)
        { 
            input = input.ToUpper();

            // If the game is not started this menu is used
            if (started is false && finished is false)
            {
                switch (input)
                {
                    case "LOAD SIMPLE.MAP":
                        currentMapName = input.Split("LOAD")[1].Trim().ToLower();
                        currentMapName = char.ToUpper(currentMapName[0]) + currentMapName[1..];
                        mapInitialised = InitializeMap(currentMapName);
                        break;
                    case "LOAD ADVANCED.MAP":
                        currentMapName = input.Split("LOAD")[1].Trim().ToLower();
                        currentMapName = char.ToUpper(currentMapName[0]) + currentMapName[1..];
                        mapInitialised = InitializeMap(currentMapName);
                        break;
                    case "ADVANCED":
                        Console.WriteLine("Playing in advanced mode!" + Environment.NewLine);
                        advanced = true;
                        break;
                    case "PLAY":
                        if (!mapInitialised)
                        {
                            Console.WriteLine("You must first load a map by typing \"load <map name>\".");
                        } else
                        {
                            started = true;
                        }
                        break;
                    default:
                        Console.WriteLine($"\"{input}\" command does not exist. Try again. {Environment.NewLine}");
                        break;
                }
              
            } 
            // if the game is finished these options are given to the player
            else if (finished is true)
            {
                switch (input)
                {
                    case "REPLAY":
                        InitializeMap(currentMapName);
                        finished = false;
                        break;
                    case "QUIT":
                        active = false;
                        break;
                    default:
                        Console.WriteLine($"\"{input}\" command does not exist. Try again. {Environment.NewLine}");
                        break;
                }
            }
            // this block runs when the game is active and started
            else
            {
                switch (input)
                {
                    case "W":
                        action = PlayerActions.NORTH;
                        break;
                    case "A":
                        action = PlayerActions.WEST;
                        break;
                    case "S":
                        action = PlayerActions.SOUTH;
                        break;
                    case "D":
                        action = PlayerActions.EAST;
                        break;
                    case "P":
                        action = PlayerActions.PICKUP;
                        break;
                    case " ":
                        action = PlayerActions.ATTACK;
                        break;
                    case "Q":
                        action = PlayerActions.QUIT;
                        break;
                    default:
                        action = PlayerActions.NOTHING;
                        break;
                }
            }
        }

        /// <summary>
        /// Main game loop, this handles all the logic of the game
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool Update(bool active)
        {
            bool working;

            if (started)
            {
                // moving player
                int[] previousPos = player.Move();
                map[previousPos[0]][previousPos[1]] = '-';

                // rendering coins on the map
                foreach (Coins coin in coins)
                {
                    map[coin.Position[0]][coin.Position[1]] = 'C';
                }

                // rendering player on the map
                map[player.Position[0]][player.Position[1]] = '@';

                // moving monsters
                foreach (Monster monster in monsters)
                {
                    if (advanced)
                    {
                        int[] previousPosM = monster.Move();
                        map[previousPosM[0]][previousPosM[1]] = '-';
                        monster.Attack();
                    }
                    map[monster.Position[0]][monster.Position[1]] = 'M';
                }

                working = true;
            } else
            {
                working = false;
            }

            return working;
        }

        /// <summary>
        /// Prints the game map with the current state of the game
        /// </summary>
        /// <returns></returns>
        public bool PrintMap()
        {

            for (int y = 0; y < map.Length; y++)
            {
                Console.WriteLine(map[y]);
            }

            return true;
        }

        /// <summary>
        /// Prints the additional information to the console
        /// </summary>
        /// <returns></returns>
        public bool PrintExtraInfo()
        {
            if (started)
            {
                Console.WriteLine(Environment.NewLine + $"Player Health: {player.Health}");
                Console.WriteLine($"Collected Coins: {player.Coins}" + Environment.NewLine);

            } else if (finished && !player.IsAlive)
            {
                Console.WriteLine(Environment.NewLine + "You died!");
                Console.WriteLine("Enter \"Replay\" to replay the map or \"Quit\" to close the game." + Environment.NewLine);

            } else if (finished)
            {
                Console.WriteLine(Environment.NewLine + "Congratulations, you finished the game!");
                Console.WriteLine("Enter \"Replay\" to replay the map or \"Quit\" to close the game." + Environment.NewLine);

            }

            return true;
        }

        /// <summary>
        /// Loads the map from a file and initialises the game
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public bool InitializeMap(string mapName)
        {
            bool initSuccess;

            string path = Path.Combine(Environment.CurrentDirectory, "maps", mapName);

            string[] lines = new string[0];

            try
            {
                //only reads lines into array where they are not empty
                lines = File.ReadAllLines(path).Where(line => line != "").ToArray();
                initSuccess = true;
            } catch (FileNotFoundException)
            {
                Console.WriteLine("Specified map was not found.");
                initSuccess = false;
            }

            // Convert lines into a jagged char array
            map = new char[lines.Length][];
            originalMap = new char[lines.Length][];

            for (int y = 0; y < lines.Length; y++)
            {
                originalMap[y] = lines[y].ToCharArray();
                map[y] = lines[y].ToCharArray();
            }

             monsters = new List<Monster>();
             coins = new List<Coins>();

            // Initialise player, monsters and coins
            for (int y = 0; y < map.Length; y++)
            {
                for(int x = 0; x < map[y].Length; x++) { 

                    if (map[y][x] == 'S')
                    {
                        map[y][x] = '-';
                        player = new Player(this, y, x);
                    } else if (map[y][x] == 'M')
                    {
                        map[y][x] = '-';
                        monsters.Add(new Monster(this, y, x));
                    } else if (map[y][x] == 'C')
                    {
                        map[y][x] = '-';
                        coins.Add(new Coins(new Random().Next(3, 11), new int[] { y, x }, this));
                    }

                }
            }

            return initSuccess;
        }

        /// <summary>
        /// Returns the currently loaded map without any state changes
        /// </summary>
        /// <returns> char[][] originalMap </returns>
        public char[][] GetOriginalMap()
        {
            return originalMap;
        }

        /// <summary>
        /// Returns the currently loaded map with the current state of the game
        /// </summary>
        /// <returns> char[][] map </returns>
        public char[][] GetCurrentMapState()
        {
            if (started)
            {
                return map;
            } else
            {
                return originalMap;
            }
        }


        /// <summary>
        /// Returns player position
        /// </summary>
        /// <returns> int[] player position </returns>
        public int[] GetPlayerPosition()
        {
            if (player is null)
            {
                return new int[] { 0, 0 };
            }
            else
            {
                return player.Position;
            }
        }

        /// <summary>
        /// Returns the current player action converted from enum to int
        /// </summary>
        /// <returns> int action </returns>
        public int GetPlayerAction()
        {
            return (int)action;
        }

        /// <summary>
        /// Returns the state of the game
        /// </summary>
        /// <returns> bool </returns>
        public bool GameIsRunning()
        {
            if (active == true && originalMap.Length > 0 && started == true)
            {
                return true;
            } else
            {
                return false;
            }
        }

        /**
         * Main method and Entry point to the program
         * ####
         * Do not change! 
        */
        static void Main(string[] args)
        {
            CMDCrawler crawler = new CMDCrawler();

            string input = string.Empty;
            Console.WriteLine("Welcome to the Commandline Dungeon!" +Environment.NewLine+ 
                "May your Quest be filled with riches!"+Environment.NewLine);
            
            // Loops through the input and determines when the game should quit
            while (crawler.active && crawler.action != PlayerActions.QUIT)
            {
                Console.WriteLine("Your Command: ");
                input = crawler.ReadUserInput();
                Console.WriteLine(Environment.NewLine);

                crawler.ProcessUserInput(input);
            
                crawler.Update(crawler.active);
                crawler.PrintMap();
                crawler.PrintExtraInfo();
            }

            Console.WriteLine("See you again" +Environment.NewLine+ 
                "In the CMD Dungeon! ");
        }
    }
}
