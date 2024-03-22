// using static System.Threading.Thread;
// using System.Media;
// using System.Diagnostics;
// using System.Numerics;
// public class Program1 {
//     static void Main() {
//         Print("You awoke in a dim cave.");
//         Print("(Who am I...)");
//         bool flag = true;
//         string name = "";
//         while (flag) {
//             Print("???: My name is...");
//             Console.Write("> ");
//             name = Console.ReadLine() ?? "";
//             if (name.Length == 0)
//                 name = "???";
            
//             Print($"{name}: My name is {name}.");
//             if (GetChoice("I'm sure.", "No...") == 1)
//                 flag = false;
//         }
        
//         Print($"{name}: Where is this place...");
//         Print("[3 weapons laid on the ground.]");
//         Print($"{name}: What do I pick?");

//         dynamic player;
//         int chosen_job = GetChoice("Bow", "Blade", "Staff");
//         if (chosen_job == 1)
//             player = new Archer(name);
//         else if (chosen_job == 2)
//             player = new Warrior(name);
//         else
//             player = new Mage(name);

//         player.AcquireWeapon(new Dictionary<string, double>{{"du", 1}, {"ui", 1.5}}, "Wooden " + player.weapon_type, player.job);
//         Print("A goblin appeared!");
//         Console.Clear();
//     }

//     public class Weapon(Dictionary<string, double> moveset, string name = "", string job = "") {
//         public string name = name, job = job;
//         public Dictionary<string, double> moveset = moveset;
//     }

//     // I used primary constructors for readability, a new feature released just last year, so that I don't have to call base() anymore
//     public class Character(string name, string job = "None", double health = 1, double damage = 1) {
//         public Weapon weapon = new(new Dictionary<string, double>{{"u", 1},{"ui", 1.5}});
//         public string name = name, job = job;
//         public double health = health, damage = damage;
//         public ConsoleColor color = ConsoleColor.White;
//         public int x = 0, y = 0;

//         public void Talk(string str) {
//             Print(str, 15, 500, color, name);
//         }

//         public void AcquireWeapon(Dictionary<string, double> dictionary, string name, string job) {
//             Print($"[Acquired 1x {name}!]");
//             Sleep(300);
//             this.weapon = new Weapon(dictionary, name, job);
//         }

//         public void Attack() {
//             string input = "";
//             List<string> moveList = new(weapon.moveset.Keys);
//             Print($"[{moveList[0]}] Light attack");
//             Print($"[{moveList[1]} Heavy attack");
//             Console.Write("> ");

//             while (!moveList.Contains(input) && input.Length <= 5) {
//                 Stopwatch sw = new();
//                 if (sw.ElapsedMilliseconds > 500)
//                     Console.Write("Fail!");
//                 Console.Write($"{input}\n\n> ");
//                 sw.Start();
//                 ConsoleKeyInfo KP = Console.ReadKey();
//                 sw.Stop();
//                 Console.Write($"Time: {sw.ElapsedMilliseconds}");
//                 Console.Clear();
//                 input += KP.KeyChar;
//             }
            
//             if (input == moveList[0])
//                 Print("Light attack!");
//             else if (input == moveList[1])
//                 Print("Heavy Attack!");
//         }
//     }
//     public class Archer(string name, string job = "Archer", double health = 1, double damage = 1, double arrows = 10) : Character(name, job, health, damage) {
//         public string weapon_type = "Bow";
//         public double arrows = arrows;
//     }
//     public class Warrior(string name, string job = "Warrior", double health = 1, double damage = 1, double stamina = 100) : Character(name, job, health, damage) {
//         public string weapon_type = "Blade";
//         public double stamina = stamina;
//     }
//     public class Mage(string name, string job = "Mage", double health = 1, double damage = 1, double mana = 100) : Character(name, job, health, damage) {
//         public string weapon_type = "Staff";
//         public double mana = mana;
//     }

//     public static void Print(string str, int speed = 10, int duration = 400, ConsoleColor color = ConsoleColor.White, string Name = "") {
//         Console.ForegroundColor = color;
//         if (!string.IsNullOrEmpty(Name))
//             str = str.Insert(0, Name + ": ");

//         foreach (char c in str) {
//             Console.Write(c);
//             Sleep(speed); 
//         }
//         Sleep(duration);
//         Console.ForegroundColor = ConsoleColor.White;
//     }
//     public static int GetChoice(params string[] Choices) {
//         int maxChoices = Choices.Length;
//         Console.ForegroundColor = ConsoleColor.White;
//         Console.WriteLine();
//         for (int i = 0; i < Choices.Length; i++) {
//             Print(string.Format("[{0}] {1}", i+1, Choices[i].ToString()), 5, 0);
//         }

//         Console.Write("> ");
//         bool ValidChoice = int.TryParse( Console.ReadKey().KeyChar + "", out int Choice);

//         if (!ValidChoice || Choice < 1 || Choice > maxChoices) {
//             Console.ForegroundColor = ConsoleColor.Red;
//             Console.WriteLine("\nInvalid Input. Please try again.");
//             Sleep(800);
//             Console.Clear();
//             Choice = GetChoice(Choices);
//         }
//         Console.Clear();
//         return Choice;
//     }
// }