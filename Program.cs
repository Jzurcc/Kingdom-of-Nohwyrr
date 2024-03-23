using System.ComponentModel;
using static System.Threading.Thread;

public class Program {
public static dynamic player;
// public static Warrior Sacrifice = new(25, 550, ConsoleColor.DarkRed, "SACRIFICE"); // Warrior equivalent
// public static Mage Enigma = new(13, 550, ConsoleColor.DarkMagenta, "ENIGMA"); // Mage equivalent
// public static Archer Harvest = new(40, 650, ConsoleColor.DarkGreen, "HARVEST"); // Archer equivalent
public class Archer(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "Archer", bool is_player = false, int HP = 10, double DMGMultiplier = 1, double arrows = 10) : Character(tspeed, tduration, color, name, job, is_player, HP, DMGMultiplier) {
        public string weapon_type = "Bow";
        public double arrows = arrows;
    }
    public class Warrior(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "Warrior", bool is_player = false, int HP = 10, double DMGMultiplier = 1, double stamina = 100) : Character(tspeed, tduration, color, name, job, is_player, HP, DMGMultiplier) {
        public string weapon_type = "Blade";
        public double stamina = stamina;
    }
    public class Mage(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "Mage", bool is_player = false, int HP = 10, double DMGMultiplier = 1, double mana = 100) : Character(tspeed, tduration, color,  name, job, is_player, HP, DMGMultiplier) {
        public string weapon_type = "Staff";
        public double mana = mana;
    }

public class Item(string name = "???", string description = "???", int x = 0, int y= 0) {
        public string name = name, description = description;        
        public int x = x, y = y;
    }

public class Weapon(string name = "???", string description = "???", string wclass = "None", double DMGMultiplier = 1, int x = 0, int y = 0) : Item(name, description, x, y) {
    public double DMGMultiplier = DMGMultiplier;
    public string wclass = wclass;
}

static void Main() {
    Start();
}
static void Start() {
    // int choice = GetChoice(0, 0, "Start Game", "Exit");
    // if (choice == 2)
    //     Environment.Exit(0);
    player = new Character(35, 450, ConsoleColor.White, "Player", "???", true);
    RoomGen = new(7, 30, ConsoleColor.DarkGray, 0, 25, 2, 2, 0.6);
    RoomGen.SetExits(-1, [0,0], [0,0], [1,1], [0,0]);
    RoomGen.InitializeRoom();
    RoomGen.AddItemTile(new Dictionary<Item, int>{
        {new Weapon("Wooden Bow", "A wooden bow and a quiver of arrows.", "Archer"), 1},
        {new Weapon("Wooden Blade", "Durable, but it gets the job done.", "Warrior"), 1},
        {new Weapon("Wooden Staff", "It lacks beauty, but it's usable.", "Mage"), 1}
    });
    RoomGen.SetSpawnPoint(3, 1);
    RoomGen.DisplayRoom();

    if (player.weapon.wclass == "Archer")
        player = new Archer(35, 450, ConsoleColor.White, "Player", "Archer", true);
    else if (player.weapon.wclass == "Warrior")
        player = new Warrior(35, 450, ConsoleColor.White, "Player", "Warrior", true);
    else if (player.weapon.wclass == "Mage")
        player = new Mage(35, 450, ConsoleColor.White, "Player", "Mage", true);

    player.UpdateStats(true);
    Console.Clear();
}

public class Character {
    public DeityEnum Deity;
    // Dialogue variables
    public int tspeed, tduration;
    public ConsoleColor color;
    // Attribute variables
    public string name, DeityName, job;
    public int Skill1Timer, Skill2Timer, Skill3Timer, Skill4Timer, Skill5Timer;
    public int HP, ATK, DEF, INT, SPD, LCK, GLD, EXP, MaxEXP, EXPDrop, LVL, PTS, IntHealth, IntMaxHealth;
    public double Health, MaxHealth, DMG, Armor, FinalDMG, DMGMultiplier;
    public dynamic ChosenDeity;
    // Room variables
    public int X, Y, Stage;
    public int TotalKills, SacrificeKills, EnigmaKills, HarvestKills, EndKills, RoomKills;
    public int spawnX = NextInt(1, 5), spawnY = NextInt(1, 5);
    public Weapon weapon;
    public List<Item> inventory = [];
    public bool is_player;
    public List<string> AttackDescriptions;
    public Character(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "???", bool is_player = false, int HP = 10, double DMGMultiplier = 1) {
        // Dialogue variables
        this.X = spawnX;
        this.Y = spawnY;
        this.is_player = is_player;
        this.tspeed = tspeed;
        this.tduration = tduration;
        this.color = color;
        // Info variables
        this.name = name;
        this.job = job;
        this.Stage = 1;
        this.inventory = [];
        this.DeityName = "None";
        this.Deity = DeityEnum.None;
        this.ChosenDeity = new Deity();
        this.AttackDescriptions = [];
        // Attribute variables
        this.HP = HP;
        this.ATK = 10;
        this.DEF = 10;
        this.INT = 10;
        this.SPD = 10;
        this.LCK = 10;
        this.LVL = 1;
        this.PTS = 0;
        // Stats variables
        this.GLD = 100;
        this.EXP = 0;
        this.MaxEXP = 80 + LVL*20;
        this.Health = 20 + HP*8;
        this.IntHealth = Convert.ToInt32(Health);
        this.MaxHealth = Health;
        this.IntMaxHealth = Convert.ToInt32(MaxHealth);
        this.DMG = ATK;
        this.DMGMultiplier = DMGMultiplier;
        this.FinalDMG = DMG;
        this.Armor = Math.Clamp(Math.Round(DEF*0.02), 0, 0.5);
        this.weapon = new();
        // Skill variables
        this.Skill1Timer = 0;
        this.Skill2Timer = 0;
        this.Skill3Timer = 0;
        this.Skill4Timer = 0;
        this.Skill5Timer = 0;
        switch (job) {
            case "Archer":
                AttackDescriptions = [
                    $"Quick Shot\n     Damage: {player.ATK*0.8} DMG\n     Cost: 1 Arrow\n",
                    $"Multi-Shot\n     Damage: {player.ATL*0.8*0.7} DMG\n     Cost: 3 Arrows\n",
                ];
                break;
            case "Warrior":
                AttackDescriptions = [
                    $"Quick Slash\n     Damage: {player.ATK} DMG\n     Cost: 10 Stamina\n",
                    $"Heavy Cleave\n     Damage: {player.ATK*2.3} DMG\n     Cost: 25 Stamina\n",
                ];
                break;
            case "Mage":
                AttackDescriptions = [
                    $"Fireball\n     Damage: {player.INT*1.2} DMG\n     Cost: 10 Stamina\n",
                    $"Mega Fireball\n     Damage: {player.INT*1.2*2.3} DMG\n     Cost: 25 Stamina\n",
                ];
                break;
        }
            
        
    }

    // Methods
    public void EvaluateEXP() {
        while (EXP >= MaxEXP) {
            PTS += 2;
            EXP -= MaxEXP;
            LVL++;
            UpdateStats();
        }
    }
    public bool Acquire(dynamic item) {
        Narrate($"Name: {item.name}");
        Narrate($"Description: {item.description}");
        
        if (GetChoice(0, 0, "Acquire", "Ignore") == 1) {
            if (item.GetType() == typeof(Weapon)) {
                if (item.wclass == weapon.wclass || weapon.wclass == "None" || first_room) {
                    if (!(item.wclass == weapon.wclass || weapon.wclass == "None")) {
                        Talk("I already have a weapon.");
                        Talk("Should I drop my weapon to pick this one up? (Irreversible)");
                        if (GetChoice(0, 0, "Yes", "No") == 2)
                            return false;
                    }
                    if (weapon.wclass != "None")
                        inventory.Remove(weapon);
                    weapon = item;
                    inventory.Add(item);
                    return true;
                }
                return false;
            } else if (item.GetType() == typeof(Item))
                inventory.Add(item);
        }
        return false;
    }
    public void ShowInventory() {
        Console.WriteLine("Inventory");
        for (int i = 0; i < inventory.Count; i++)
        {
            Console.WriteLine($"[{i+1}] {inventory[i].name}");
        }
        Console.Write("\n> ");
        Console.ReadKey();
    }
    public void Talk(string str, int tspeed = 15, int tduration = 250, ConsoleColor color = ConsoleColor.White, string name = "Player") {
        Program.Print(str, tspeed, tduration, color, name);
    }


    public void Think(string str) {
        Program.Print($"({str})", Convert.ToInt32(tspeed*0.6), tduration, ConsoleColor.DarkGray);
    }

    public void Narrate(string str, int speed = 3, int duration = 250, ConsoleColor color = ConsoleColor.White) {
        Program.Print($"[{str}]", 3, 450, color);
    }

    public void Move(int dx, int dy) {
        int newX = X + dx;
        int newY = Y + dy;
        if (newX >= 0 && newX < current_xSize && newY >= 0 && newY < current_ySize && Room[newX, newY].Type != TileType.Wall) {
            X = newX;
            Y = newY;
        }
    }

    public Dictionary<int, string> GetStats() {
        Dictionary<int, string> StatsDict = [];
        StatsDict.Add(1, "   --------------------------------------------");
        List<string> strings = [string.Format("   Level: {0, -13} Gold: {1}", LVL, GLD), string.Format("   Health: {0:0}/{1, -8} EXP: {2}/{3}", Health, MaxHealth, EXP, MaxEXP), string.Format("   Points: {0, -13} Deity: {1}", PTS, DeityName), "   --------------------------------------------", string.Format("   HP: {0, -16} DEF: {1}", HP, DEF), string.Format("   ATK: {0, -15} INT: {1}", ATK, INT), string.Format("   SPD: {0, -15} LCK: {1}", SPD, LCK), string.Format("   Kills: {0, -13} Room: {1}", TotalKills, Stage)];

        for (var i = 0; i < strings.Count; i++)
            StatsDict.Add(2+i, strings[i]);

        return StatsDict;
    }

    public void WriteStats() {
        Console.WriteLine($"name: {name}");
        List<string> strings = ["--------------------------------------------", string.Format("Level: {0, -13} Gold: {1}", LVL, GLD), string.Format("Health: {0:0}/{1, -8} EXP: {2}/{3}", Health, MaxHealth, EXP, MaxEXP), string.Format("Points: {0, -13} Deity: {1}", PTS, DeityName), "--------------------------------------------", string.Format("HP: {0, -16} DEF: {1}", HP, DEF), string.Format("ATK: {0, -15} INT: {1}", ATK, INT), string.Format("SPD: {0, -15} LCK: {1}", SPD, LCK),  string.Format("Kills: {0, -13}", TotalKills)];
        for (var i = 0; i < strings.Count; i++)
            Console.WriteLine(strings[i]);
    }

    public void SetDeity(DeityEnum deity) {
        if (deity != DeityEnum.None) {
            Deity = deity;
            DeityName = "THE " + deity.ToString().ToUpper();
            DeityList.Remove(deity);
        }
    }

    public void Die() {
        player.WriteStats();
        string DeathMSG = "";
        string[] DeathMSGs = [
        "Sacrifice: TO DIE IS TO FEED THE GODS’ HUNGER!",
        "SACRIFICE: ANOTHER SOUL FEEDS THE BATTLEGROUND!",
        "SACRIFICE: BLOOD FOR THE BLOOD GOD!",
        "SACRIFICE: YOUR SCREAMS ECHO IN THE VOID!",
        "SACRIFICE: FALL IN BATTLE, RISE IN GLORY!",
        "SACRIFICE: AN HONORABLE DEATH, WARRIOR!",
        "Sacrifice: YOUR VALOR SHINES EVEN IN DEFEAT!",
        "Sacrifice: YES! MORE BLOOD FOR THE BLOOD GOD!",
        "Sacrifice: GLORIOUS! ANOTHER LIFE SPENT IN THE FRAY!",

        "Enigma: I will hold unto the mystery of your end.",
        "Enigma: Even in death, you leave questions unanswered.",
        "Enigma: A puzzle completed, a life concluded.",
        "Enigma: Your story ends, but the secrets linger.",
        "Enigma: An epilogue written in the stars.",
        "Enigma: In death, some find answers.",
        "Enigma: Death, the last enigma you shall unravel.",
        "Enigma: The final piece of your puzzle placed.",
        "Enigma: Your legacy, a riddle for ages to come.",
        
        "Harvest: The cycle continues, as you return to the earth.",
        "Harvest: From dust to dust, your essence nourishes the soil.",
        "Harvest: A seed falls, but a forest awaits.",
        "Harvest: In death, you fulfill life's final promise.",
        "Harvest: An end, but also a beginning in the eternal cycle.",
        "Harvest: A life well lived, a rest well earned.",
        "Harvest: May your spirit find peace in the eternal garden.",
        "Harvest: Like leaves in autumn, you too have fallen.",
        "Harvest: Harvested at season's end, you return to the earth.",
        
        "End: Cease.",
        "End: It is the end.",
        "End: Your death is meaningless.",
        "End: The final whisper fades.",
        "End: An end, predictable and absolute.",
        "End: You are naught before me.",
        "End: Become none.",
        "End: Return to nothing.",
        "End: Silence now."];
        if (player.Deity == DeityEnum.Sacrifice)
            DeathMSG = DeathMSGs[NextInt(8)];
        else if (player.Deity == DeityEnum.Enigma)
            DeathMSG = DeathMSGs[NextInt(9, 18)];
        else if (player.Deity == DeityEnum.Harvest)
            DeathMSG = DeathMSGs[NextInt(18, 28)];
        else 
            DeathMSG = DeathMSGs[NextInt(28, 37)];

        Console.WriteLine("\n\n");
        Sleep(300);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(@"
▄██   ▄    ▄██████▄  ███    █▄          ▄█    █▄       ▄████████  ▄█    █▄     ▄████████      ████████▄   ▄█     ▄████████ ████████▄  
███   ██▄ ███    ███ ███    ███        ███    ███     ███    ███ ███    ███   ███    ███      ███   ▀███ ███    ███    ███ ███   ▀███ 
███▄▄▄███ ███    ███ ███    ███        ███    ███     ███    ███ ███    ███   ███    █▀       ███    ███ ███▌   ███    █▀  ███    ███ 
▀▀▀▀▀▀███ ███    ███ ███    ███       ▄███▄▄▄▄███▄▄   ███    ███ ███    ███  ▄███▄▄▄          ███    ███ ███▌  ▄███▄▄▄     ███    ███ 
▄██   ███ ███    ███ ███    ███      ▀▀███▀▀▀▀███▀  ▀███████████ ███    ███ ▀▀███▀▀▀          ███    ███ ███▌ ▀▀███▀▀▀     ███    ███ 
███   ███ ███    ███ ███    ███        ███    ███     ███    ███ ███    ███   ███    █▄       ███    ███ ███    ███    █▄  ███    ███ 
███   ███ ███    ███ ███    ███        ███    ███     ███    ███ ███    ███   ███    ███      ███   ▄███ ███    ███    ███ ███   ▄███ 
 ▀█████▀   ▀██████▀  ████████▀         ███    █▀      ███    █▀   ▀██████▀    ██████████      ████████▀  █▀     ██████████ ████████▀  
                                                                                                                                      ");
        Console.ForegroundColor = ConsoleColor.White;
        Print(DeathMSG);
        Console.ReadKey();
        Print("\nDo you wish to play again?");
        int input = GetChoice(0, 0, "Yes", "No");
        if (input == 1)
            Start();
        else
            Environment.Exit(0);

    }

    public void UpdateStats(bool updateHealth = false, bool updatePTS = true) {
        MaxEXP = 80 + LVL*20;
        MaxHealth = 20 + HP*8;
        Armor = Math.Clamp(Math.Round(DEF*0.02), 0, 0.5);
        if (updateHealth)
            Health = MaxHealth;
    }

    public bool ChooseDeity(DeityEnum chosen) {
    int choice = GetChoice(5, 100, "Enter the door.", "Go back.");
    if (choice == 1) {
        SetDeity(chosen);
        return false;
    } else
        return true;
    }


    // Combat methods
    // Returns a random number from the range of the attack
    public double GetDMG(dynamic DMG, dynamic enemy, bool PierceArmor = false, double MinMultiplier = 0.5, double MaxMultiplier = 1.5, bool CheckIfEnemy = true) {
        int MinDMG = Convert.ToInt32(DMG*MinMultiplier);
        int MaxDMG = Convert.ToInt32(DMG*MaxMultiplier);
        FinalDMG = NextInt(MinDMG, MaxDMG);
        if (!PierceArmor)
            FinalDMG -= FinalDMG*enemy.Armor;
        
        if (enemy is not Enemy && CheckIfEnemy)
            FinalDMG *= 0.7;
        return FinalDMG;
    }

    // Checks if the DMG is evaded or not
    public bool CheckEvade(double DMG, dynamic enemy) {
        double chance = RNG.NextDouble();
        if (1.0-LCK*0.01 > chance) {
            enemy.Health = Math.Clamp(enemy.Health - DMG + DMG*Armor, 0, enemy.MaxHealth);
            return true;
        } else {
            Narrate($"{enemy.name} evaded the attack!");
            return false;
        }
    }

    // Archer

    public void LightAttack(dynamic enemy, string skill_name) {
        if (job == "Archer") 
            DMG = GetDMG(ATK*0.8, enemy);
         else if (job == "Warrior") 
            DMG = GetDMG(ATK, enemy);
         else if (job == "Mage") 
            GetDMG(INT, enemy);

        Narrate($"{name} used {skill_name}!");
        if (CheckEvade(DMG, enemy))
            Narrate($"{enemy.name} got hit for {DMG:0} DMG!");
    }
    
    public void HeavyAttack(dynamic enemy, string skill_name) {
        for (int i = 0; i < 3; i++) {
            if (job == "Archer")
                DMG = GetDMG(ATK*0.8, enemy);
            else if (job == "Warrior")
                DMG = GetDMG(ATK, enemy);
            else if (job == "Mage")
                GetDMG(INT, enemy);

            Narrate($"{name} used {skill_name}!");
            if (CheckEvade(DMG, enemy))
                Narrate($"{enemy.name} got hit for {DMG:0} DMG!");
        }
    }
}


public class Enemy(int x, int y, int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "???", bool is_player = false, int HP = 10, double DMGMultiplier = 1) : Character(tspeed, tduration, color, name, job, is_player, HP, DMGMultiplier) {
    public bool IsDefeated = false;
    public bool IsBoss = false;
    public void Initialize() {
        X = x;
        Y = y;
        Deity = DeityList[NextInt(DeityList.Count-1)];
        DeityName = "THE " + Deity.ToString().ToUpper();
        HP = 6;
        ATK = 6;
        DEF = 6;
        INT = 6;
        SPD = 6;
        LCK = 6;
        LVL = player.Stage;
        PTS = 10+(LVL*2);
        GetDeityStats();
        DistributePTS();
        MaxEXP = 80 + LVL*20;
        EXPDrop = NextInt(MaxEXP/4, MaxEXP);
        MaxHealth = 20 + HP*4;
        Health = MaxHealth;
        Armor = Math.Clamp(Math.Round(DEF*0.02), 0, 0.5);
        TotalKills = NextInt(0, LVL*3);

        string[][] Names = {
            ["Bloodbound Stalker", "Graveborn Revenant", "Painforged Emissary"], 
            ["Mind Walker", "Twilight Herald", "Dream Specter"], 
            ["Bleeding Orchardgeist", "Weeping Rosecutter", "Fiery Treant"], 
            ["Voidborn Wraith", "Oblivion Scourge", "Ancient Desolator"]
            };
        string[] DeityNames = Enum.GetNames(typeof(DeityEnum));

        
        // Checks the deity of the enemy and assigns a name for it based on their deity.
        for (int i = 0; i < DeityNames.Length; i++)
            if (Deity.ToString() == DeityNames[i])
                name = Names[i][NextInt(Names.Length-1)];
    }
    public int SpendPTS(int Attempts = 0, double Chance = 0.5, bool SpendAll = false) {
        int Amount = 0;
        if (!SpendAll) {
            for (int i = 0; i < Attempts; i++)
                if(Chance > RNG.NextDouble() && PTS > 0) {
                    Amount++;
                    PTS--;
                }
            return Amount;
        } else if (PTS > 0) {
            PTS -= Attempts;
            return Attempts;
        }
        return 0;
    }
    public void GetDeityStats() {
        switch (Deity) {
        case DeityEnum.Sacrifice:
            HP += 5;
            DEF += 3;
            GLD -= 50;
            ATK -= 3;
            SPD -= 3;
            break;
        case DeityEnum.Enigma:
            INT += 5;
            PTS += 3;
            HP -= 3;
            ATK -= 5;
            break;
        case DeityEnum.Harvest:
            LCK += 6;
            PTS += 2;
            DEF -= 3;
            SPD -= 5;
            break;
        case DeityEnum.End:
            ATK += 5;
            SPD += 3;
            HP -= 3;
            DEF -= 5;
            break;
        }
        UpdateStats(true);
    }

    public void UpdateAsBoss() {
        IsBoss = true;
        int AddedLVL = NextInt(3, 6);
        LVL += AddedLVL;
        EXPDrop += AddedLVL*100;
        PTS += AddedLVL*2;
        DistributePTS();
        UpdateStats(true);
        string[][] upgradedNames = new string[][]
        {
            ["Crimson Abyss Watcher", "Tombwraith Sovereign", "Agony's Last Herald"],
            ["Mindshatter Enchanter", "Eclipse's Final Envoy", "Nightmare Incarnate"],
            ["Thornheart Forest Monarch", "Mourning Bloom Sovereign", "Inferno's Rootbound Titan"],
            ["Nethervoid Lich King", "Eternal Damnation's Harbinger", "Time's Final Arbiter"]
        };
        string[] DeityNames = Enum.GetNames(typeof(DeityEnum));
        for (int i = 0; i < DeityNames.Length; i++)
            if (Deity.ToString() == DeityNames[i])
                name = upgradedNames[i][NextInt(upgradedNames.Length-1)];


    }

    public void DistributePTS() {
        while (PTS > 0) {
            double chance = RNG.NextDouble();
            if (chance < 0.17)
                HP++;
            else if (chance < 0.34)
                ATK++;
            else if (chance < 0.51)
                DEF++;
            else if (chance < 0.68)
                INT++;
            else if (chance < 0.84)
                SPD++;
            else
                LCK++;
            PTS--;
        }
        UpdateStats(true);
    }
    public void Defeat() {
        double HealAmount = player.MaxHealth*0.2;
        if (player.Health + HealAmount >= player.MaxHealth)
            HealAmount = player.MaxHealth - player.Health;

        player.Health += HealAmount;
        
        player.Narrate($"Regenerated {HealAmount:0} Health!");
        Sleep(800);
        player.RoomKills++;
        player.TotalKills++;
        player.EXP += EXPDrop;
        player.EvaluateEXP();
        IsDefeated = true;
        player.Skill3Timer = player.Skill4Timer = Skill3Timer = Skill4Timer = Skill5Timer = player.Skill5Timer = 0;
    }

    // Method to randomly move the enemy
    public void Move() {
        int[] dx = [-1, 0, 0, 1];
        int[] dy = [-1, 0, 0, 1];

        // Attempt to move in a random direction
        for (int attempts = 0; attempts < 4; attempts++) {
            int direction = NextInt(4);
            int newX = X, newY = Y;
            if (RNG.NextDouble() > 0.5)
                newX = X + dx[direction];
            else
                newY = Y + dy[direction];

            // Check if the new position is within bounds and not a wall
            if (newX >= 0 && newX < current_xSize && newY >= 0 && newY < current_ySize && Room[newX, newY].Type != TileType.Wall)
            {
                X = newX;
                Y = newY;
                break; // Successfully moved
            }
        }
    }
}

public class RoomGenerator {
    public int xSize, ySize, walls, enemyCount, minWallLength, maxWallLength, id, exitDirection, exitNum;
    public double horizontalPercentage;
    public ConsoleColor room_color;
    public int[][] exits, exitPositions, outer_tiles;
    public List<Item> itemsOnGround = [];
    public RoomGenerator(int x = 0, int y = 0, ConsoleColor room_color = ConsoleColor.DarkGray, int enemyCount = -1, int walls = -1, int minWallLength = 1, int maxWallLength = 4, double horizontalPercentage = 0.5) {
        this.room_color = room_color;
        this.walls = walls;
        this.enemyCount = enemyCount;
        this.minWallLength = minWallLength;
        this.maxWallLength = maxWallLength;
        this.horizontalPercentage = horizontalPercentage;
        outer_tiles = [];
        id = GetRoomId();
        exits = [[0,0], [0,0], [0,0], [0,0]];
        exitPositions = [[0,0], [0,0], [0,0], [0,0]];
        exitNum = NextInt(1, 4);
        if (x == 0 && y == 0)
            xSize = ySize = NextInt(15, 20);
        else {
            xSize = x;
            ySize = y;
        }
    }
    public int GetRoomId() {
        current_id++;
        return current_id;
    }
        public void InitializeRoom() {
            if (!rooms.Contains(this))
                rooms.Add(this);
            current_xSize = xSize;
            current_ySize = ySize;
            outer_tiles = [[xSize/2, 0], [0, ySize/2], [xSize/2, ySize - 1], [xSize - 1, ySize / 2]];
            current_exitDirection = exitDirection;
            RoomClear = false;
            Room = new Tile[xSize, ySize];
            // Set all tiles as empty tiles
            for (int x = 0; x < xSize; x++) {
                for (int y = 0; y < ySize; y++) {
                    Room[x, y] = new Tile(TileType.Empty);
                }
            }
            
            // Set boundaries as walls
            for (int x = 0; x < xSize; x++) {
                Room[x, 0] = new Tile(TileType.Wall);
                Room[x, ySize - 1] = new Tile(TileType.Wall);
            }

            for (int y = 0; y < ySize; y++) {
                Room[0, y] = new Tile(TileType.Wall);
                Room[xSize - 1, y] = new Tile(TileType.Wall);
            }

            if (walls == -1)
                InitializeWalls(xSize*ySize/10);
            else
                InitializeWalls(walls);
            
            if (enemyCount == -1)
                InitializeEnemies(NextInt(0, xSize*ySize/25)); 
            else
                InitializeEnemies(enemyCount); 

            // InitializeItems(NextInt(6+player.Stage, 15+player.Stage), NextInt(5+player.Stage, 10+player.Stage));
            
            // Randomizes and teleports player to random spawnpoint.
            // SetSpawnPoint(NextInt(1, xSize-2), NextInt(1, ySize-2));
    }
    public void SetSpawnPoint(int x, int y) {
        player.X = player.spawnX = x;
        player.Y = player.spawnY = y;
    }

    public void SetExits(int direction = -1, params int[][] exits) {
        if (direction == -1 && exits.Length == 4)
            this.exits = exits;

        else if (direction != -1 && (exits.Length == 1 || exits.Length > 4)) {
            List<int[]> Exits = [];
            int[] empty = [0, 0];
            bool is_empty = true;
            for (var i = 0; i < 4; i++)
            {
                if (!this.exits[i].Equals(empty)) {
                    is_empty = false;
                    break;
                }
            }
            List<int> exitDirections = [];

            while (exitNum > 0) {
                int rand_direction = NextInt(4);
                
                if (rand_direction != exitDirection) {
                    exitDirections.Add(rand_direction);
                    exitNum--;
                }
            }

            for (var i = 0; i < 4; i++)
            {
                if (direction == i && !is_empty) {
                    Exits.Add([exits[0][0], exits[0][1]]);
                } else {
                    int is_exit = (exitDirection == direction || exitDirections.Contains(direction)) ? 1 : 0;
                    Exits.Add([is_exit, NewRoomId(is_exit)]);
                }
            }
            this.exits = [.. Exits];
        } else if (exits.Length == 0) {

        }
       
    }
    public int NewRoomId(int is_exit) {
        if (is_exit == 1) {
            room_id++;
            room_ids.Add(room_id);
            return room_id;
        }
        return 0;
    }
    public void InitializeEnemies(int maxEnemies) {
        enemies.Clear();
        while (enemies.Count < maxEnemies) {
            int x = NextInt(1, xSize-2);
            int y = NextInt(1, ySize-2);
            if (Room[x, y].Type != TileType.Wall) {
                enemies.Add(new Enemy(x, y));
                enemies[^1].Initialize();
            }
        }
    }
    public void AddItemTile(Dictionary<Item, int> items) {
        foreach (KeyValuePair<Item, int> KP in items) {
            Item item = KP.Key;
            int current_amount = 0;
            int max_amount = KP.Value;
            int nx, ny;
            while (current_amount < max_amount) {
                if (item.x == 0 && item.y == 0) {
                    nx = NextInt(1, xSize - 2);
                    ny = NextInt(1, ySize - 2);
                } else {
                    nx = item.x;
                    ny = item.y;
                }

                if (Room[nx, ny].Type == TileType.Empty) {
                    item.x = nx;
                    item.y = ny;
                    Room[item.x, item.y] = new Tile(TileType.Item);
                    current_amount++;
                    itemsOnGround.Add(item);
                }
            }
        }
    }
    


    public void InitializeItems(int HealingPotions = 0, int Golds = 0) {
        int CurrentHealingPotions = 0;
        int CurrentGolds = 0;

        while (CurrentHealingPotions < HealingPotions || CurrentGolds < Golds) {
            int x = NextInt(1, xSize - 2);
            int y = NextInt(1, ySize - 2);
            
            if (Room[x, y].Type == TileType.Empty && CurrentHealingPotions != HealingPotions) {
                Room[x, y] = new Tile(TileType.HealingPotion);
                CurrentHealingPotions++;
            }
            else if (Room[x, y].Type == TileType.Empty && CurrentGolds != Golds) {
                Room[x, y] = new Tile(TileType.Gold);
                CurrentGolds++;
            }
        }
    }

    public void InitializeWalls(int numOfWalls) {
        for (int i = 0; i < numOfWalls; i++) {
            int x = NextInt(1, xSize - 2);
            int y = NextInt(1, ySize - 2);

            // if ((x == xSize / 2 && y >= ySize / 2 - 1 && y <= ySize / 2 + 1) || (y == ySize / 2 && x >= xSize / 2 - 1 && x <= xSize / 2 + 1)) {
            //     continue; 
            // }

            Room[x, y] = new Tile(TileType.Wall);

            if (RNG.NextDouble() >= horizontalPercentage) {
                int length = NextInt(minWallLength, maxWallLength);
                for (int j = 0; j < length; j++) {
                    int nx = x + j < xSize ? x + j : x; // Ensure within bounds 
                    Room[nx, y] = new Tile(TileType.Wall);
                }
            }
            else {
                int length = NextInt(minWallLength, maxWallLength); 
                for (int l = 0; l < length; l++) {
                    int ny = y + l < ySize ? y + l : y; // Ensure within bounds
                    Room[x, ny] = new Tile(TileType.Wall);
                }
            }
        }
        // Set the the outermost tiles beside the border as empty
        double border_chance = 0.1;
        for (int x = 1; x < xSize-1; x++) {
            if (RNG.NextDouble() >= border_chance)
                Room[x, 1] = new Tile(TileType.Empty);
            if (RNG.NextDouble() >= border_chance)
            Room[x, ySize - 2] = new Tile(TileType.Empty);
        }

        for (int y = 1; y < ySize-1; y++) {
            if (RNG.NextDouble() >= border_chance)
                Room[1, y] = new Tile(TileType.Empty);
            if (RNG.NextDouble() >= border_chance)
                Room[xSize - 2, y] = new Tile(TileType.Empty);
        }
        
        InitializeExits();
    }
    public void InitializeExits() {
        for (int i = 0; i < 4; i++) {
            int[][] outer_tiles = [[current_xSize/2, 0], [0, current_ySize/2], [current_xSize/2, current_ySize - 1], [current_xSize - 1, current_ySize / 2]];
            int ox = outer_tiles[i][0];
            int oy = outer_tiles[i][1];
            if (exits[i][0] == 1) {
                Room[ox, oy] = new Tile(TileType.Exit);
                exitPositions[i][0] = ox;
                exitPositions[i][1] = oy;
            }

            int[][] inner_tiles = [[current_xSize/2, 1], [1, current_ySize/2], [current_xSize/2, current_ySize - 2], [current_xSize - 2, current_ySize / 2]];
            int ix = inner_tiles[i][0];
            int iy = inner_tiles[i][1];
            Room[ix, iy] = new Tile(TileType.Empty);
        }
    }

    // Main method for displaying Rooms
    public void DisplayRoom() {
        bool flag = true;
        while (flag) {
            Console.Clear();
            // Removes defeated enemies
            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i].IsDefeated)
                    enemies.RemoveAt(i);
            }

            PrintRoom();
            flag = ProcessInput(); // Asks for input and returns false if input is q

            // 80% chance for enemies to move randomly.
            if (RNG.NextDouble() < 0.8) {
                foreach (Enemy enemy in enemies)
                    if (!enemy.IsDefeated)
                        enemy.Move();
            }
            
            // Checks if player encounters enemy
            foreach (Enemy enemy in enemies)
                if (player.X == enemy.X && player.Y == enemy.Y) 
                    if (player.RoomKills == 4) {
                        enemy.UpdateAsBoss();
                        Console.Clear();
                        player.Narrate("Boss encountered!", 25, 1400, ConsoleColor.Red);
                        Encounter(player, enemy);
                    } else
                        Encounter(player, enemy);
                        
            // player tile interaction
            switch (Room[player.X, player.Y].Type) {
            case TileType.Portal:
                UsePortal();
                break;
            case TileType.HealingPotion:
                double HealAmount = NextInt(Convert.ToInt32(player.IntMaxHealth/5*0.6), Convert.ToInt32(player.IntMaxHealth/5*1.3)); // Determine the healing amount
                 if (player.Health + HealAmount >= player.MaxHealth)
                    HealAmount = player.MaxHealth - player.Health;

                player.Health += HealAmount;
                Console.WriteLine();
                player.Narrate($"Restored {HealAmount:0} Health.", 5, 50);
                Room[player.X, player.Y] = new Tile(TileType.Empty);
                break;
            case TileType.Gold:
                int GoldAmount = NextInt(5, Convert.ToInt32(player.LCK*5/2)-15);
                player.GLD += GoldAmount;
                Console.WriteLine();
                player.Narrate($"Gained {GoldAmount} gold.", 5, 50);
                Room[player.X, player.Y] = new Tile(TileType.Empty);
                break;
            case TileType.Item:
                for (int i = 0; i < itemsOnGround.Count; i++)
                {
                    dynamic item = itemsOnGround[i];
                    if (item.x == player.X && item.y == player.Y) {
                        bool is_acquired = player.Acquire(item);
                        if (is_acquired)
                            Room[player.X, player.Y] = new Tile(TileType.Empty);
                    }
                }
                break;
            case TileType.Exit:
                if (first_room && player.weapon.wclass == "None") {
                    player.Think("I need to protect myself.");
                    player.Think("There's some weapons around here...");
                } else {
                    // int[][] current_outer_tiles = [[current_xSize/2, current_ySize - 1], [current_xSize - 1, current_ySize/2], [current_xSize/2, 0], [0, current_ySize / 2]];
                    int exitDirection = 0;
                    int exitId = 0;
                    RoomGenerator? nextRoom = null;
                    for (int j = 0; j < 4; j++) {
                        Console.WriteLine($"{outer_tiles[j][0]}, {outer_tiles[j][1]}");
                        if (player.X == outer_tiles[j][0] && player.Y == outer_tiles[j][1]) {
                            exitDirection = j;
                            exitId = exits[exitDirection][1];
                            for (int i = 0; i < rooms.Count; i++)
                                if (exitId == rooms[i].id)
                                    nextRoom = rooms[i];
                        }
                    }
                    int oppDirection = (exitDirection + 2) % 4;
                    if (nextRoom == null) {
                        nextRoom = new();
                        nextRoom.SetExits(oppDirection, [1, id]);
                    }
                    nextRoom.InitializeRoom();
                    // nextRoom.SetSpawnPoint(nextRoom.exitPositions[oppDirection][0], nextRoom.exitPositions[oppDirection][1]);
                    nextRoom.SetSpawnPoint(nextRoom.outer_tiles[oppDirection][0], nextRoom.outer_tiles[oppDirection][1]);
                    player.X = player.spawnX;
                    player.Y = player.spawnY;
                    nextRoom.DisplayRoom();
                }
                break;
            }
        }
    }

    public void UsePortal() {
        player.RoomKills = 0;
        Console.Clear();
        player.Narrate("[You entered the portal.]");
        for (var i = 0; i < 4; i++) {
                Console.Write(". ");
                Sleep(300);
        }

        xSize = ySize = NextInt(20, 30);
        player.Stage++;
        enemies.Clear();
        InitializeRoom();
        DisplayRoom();
    }
    
    public static void Encounter(dynamic player, dynamic enemy) {
        Console.Clear();
        player.Narrate($"A wild {enemy.name} appeared!", 5, 600);
        Console.Clear();
        PrintEnemy(enemy);
        IsOver = false;
        Turn = 0;
        List<int> Timers = new();
        do {
            // Checks timers of skills
            Timers.Clear();
            Timers = [player.Skill3Timer, player.Skill4Timer, player.Skill5Timer, enemy.Skill3Timer, enemy.Skill4Timer, enemy.Skill5Timer];
            for (int i = 0; i < Timers.Count; i++) {
                if (Timers[i] > 0)
                    Timers[i]--;
            }

            Turn++;
            if (player.SPD >= enemy.SPD) {
                Print($"Turn {Turn}: The player goes first this round!");
                Sleep(450);
                PlayerTurn(enemy);
                EnemyTurn(enemy);

                Console.Clear();
            } else {
                Print($"Turn {Turn}: The enemy goes first this round!");
                Sleep(450);
                EnemyTurn(enemy);
                PlayerTurn(enemy);
            }
            if(!IsOver) CheckHealth(enemy);
        } while (!IsOver);

        // Checks if player has killed 5 enemies in the room and spawns a portal if so
        if (player.TotalKills % 5 == 0 && !RoomClear && player.TotalKills != 0) {
            Room[player.spawnX, player.spawnY] = new Tile(TileType.Portal);
            RoomClear = true;
        }
    }

    public static void CheckHealth(Enemy enemy) {
        if (enemy.Health <= 0) {
            enemy.Defeat();
            IsOver = true;
            if (player.TotalKills % 5 == 0 && !RoomClear)
                player.Narrate("A portal has appeared in the room!", 15, 1000);
        } else if (player.Health <= 0) {
            player.Die();
        } 
    }

    public static void Divider() {
        Console.WriteLine("--------------------------------------------");
    }

    public static void EnemyTurn(Enemy enemy) {
        if(!IsOver) CheckHealth(enemy);
        if (!IsOver) {
            Divider();
            enemy.WriteStats();
            Console.WriteLine();
            Divider();
            // Chooses a random skill based on the enemy's deity. The percentage of which skill to use is specific for each deity.
            double ChosenSkill = RNG.NextDouble();
            switch (enemy.job) {
            case "Archer":
                ArcherSkills(ChosenSkill, enemy);
                break;
            case "Warrior":
                WarriorSkills(ChosenSkill, enemy);
                break;
            case "Mage":
                MageSkills(ChosenSkill, enemy);
                break;
            }
            if(!IsOver) CheckHealth(enemy);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    public static void ArcherSkills(double ChosenSkill, Enemy enemy) {
        if (ChosenSkill < 0.7)
            enemy.LightAttack(player, "Quick Shot");
        else
            enemy.HeavyAttack(player, "Multi-Shot");
    }
    public static void WarriorSkills(double ChosenSkill, Enemy enemy) {
        if (ChosenSkill < 0.7)
            enemy.LightAttack(player, "Quick Slash");
        else
            enemy.HeavyAttack(player, "Heavy Cleave");
    }
    public static void MageSkills(double ChosenSkill, Enemy enemy) {
        if (ChosenSkill < 0.7)
            enemy.LightAttack(player, "Fireball");
        else
            enemy.HeavyAttack(player, "Mega Fireball");
    }

    public static void PlayerTurn(Enemy enemy) {
        bool StayTurn = false;
        do {
            if (!IsOver) {
                StayTurn = false;
            
                Divider();
                player.WriteStats();
                Console.WriteLine();
                Divider();
                switch (GetChoice(0, 0, "Attack", "Inventory", "Show Enemy", "Attempt Flee")) {
                case 1:
                    Console.Clear();
                    StayTurn = Attack(enemy);
                    break;
                case 2:
                    Console.Clear();
                    BattleInventory();
                    StayTurn = true;
                    break;
                case 3:
                    PrintEnemy(enemy);
                    StayTurn = true;
                    break;
                case 4:
                    AttemptFlee(enemy);
                    break;
                }
                Console.Clear();
            }
        } while(StayTurn);
        if(!IsOver) CheckHealth(enemy);
    }
    public static bool Attack(Enemy enemy) {
        int Choice, maxChoices;
        bool ValidChoice;
        bool StayTurn = false;
        do {
            Console.Clear();
            player.UpdateStats();
            List<string> AttackDescriptions;
            AttackDescriptions = player.AttackDescriptions;
            AttackDescriptions.Add("Cancel");

            // Gets the input from the player using a modified GetChoice();
            maxChoices = AttackDescriptions.Count;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            for (int i = 0; i < AttackDescriptions.Count; i++)
                Console.WriteLine(string.Format("[{0}] {1}", i+1, AttackDescriptions[i].ToString()));
            
            Console.Write("> ");
            ValidChoice = int.TryParse( Console.ReadKey().KeyChar + "", out Choice);

            if (!ValidChoice || Choice < 1 || Choice > maxChoices)
                Console.Clear();
                
        } while (!ValidChoice || Choice < 1 || Choice > maxChoices);

        Console.Clear();

        // Activates the selected skill
        switch(player.job) {
            case "Archer":
            switch (Choice) {
            case 1:
                player.LightAttack(enemy, "Quick Shot");
                break;
            case 2:
                player.HeavyAttack(enemy, "Multi-Shot");
                break;
            }
            break;
            case "Warrior":
            switch (Choice) {
            case 1:
                player.LightAttack(enemy, "Quick Slash");
                break;
            case 2:
                player.HeavyAttack(enemy, "Heavy Cleave");
                break;
            }
            break;
            case "Mage":
            switch (Choice) {
            case 1:
                player.LightAttack(enemy, "Fireball");
                break;
            case 2:
                player.HeavyAttack(enemy, "Mega Fireball");
                break;
            }
            break;
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
        return StayTurn;
    }
    public static void BattleInventory() {
        for (int i = 0; i < 9; i++) {
            try {
                Console.WriteLine($"{i+1}. {player.inventory[i]}");
            } catch {
                Console.WriteLine($"{i+1}. Empty");
            }
        }
        Console.ReadKey();
    }

    public static void AttemptFlee(Enemy enemy) {
        Console.Clear();
        player.Narrate("You attempted to flee.");
        if (0.4+player.LCK*0.01 > RNG.NextDouble()) {
            player.Skill3Timer = player.Skill4Timer = enemy.Skill3Timer = enemy.Skill4Timer = enemy.Skill5Timer = player.Skill5Timer = 1;
            IsOver = true;
            player.Narrate("You successfully fled from battle!");
            Console.ReadKey();
        } else {
            player.Narrate("You failed to flee.");
        }
    }

    public static void PrintEnemy(Enemy enemy, bool ShowStats = true, bool NeedInput = true) {
        if (ShowStats) 
            enemy.WriteStats();
        if (NeedInput) {
            Console.Write("\nPress any key to continue... ");
            Console.ReadKey();
            Console.Clear();
        }
        Console.WriteLine("\n");
    }

    public void PrintRoom() {
        Console.Write($"player: ({player.X}, {player.Y}), ids: {room_id}, exitNum: {exitNum}\nroom ids: ");
        for (var i = 0; i < rooms.Count; i++)
        {
            Console.Write($"{rooms[i].id},");
        }
        Console.WriteLine();
        for (int i = 0; i < exits.Length; i++)
        {
            Console.Write($"({exits[i][0]}, {exits[i][1]}), ");
        }
        Console.WriteLine("\n\n");
        
        for (int i = 0; i < xSize; i++) {
            for (int j = 0; j < ySize; j++) {
                // Writes the player and each of the enemies
                if (player.X == i && player.Y == j)
                    WriteTile("Y ", ConsoleColor.Cyan);
                else if (enemies.Any(enemy => enemy.X == i && enemy.Y == j))
                    foreach (Enemy enemy in enemies) {
                        WriteEnemies(enemy, i, j);
                    }
                else {
                    // Writes the tiles according to type.
                    switch (Room[i, j].Type) {
                    case TileType.Empty:
                        WriteTile(". ", room_color);
                        break;
                    case TileType.Wall:
                        WriteTile("# ", room_color);
                        break;
                    case TileType.Portal:
                        WriteTile("O ", ConsoleColor.Blue);
                        break;
                    case TileType.HealingPotion:
                        WriteTile("o ", ConsoleColor.Red);
                        break;
                    case TileType.Gold:
                        WriteTile("o ", ConsoleColor.Yellow);
                        break;
                    case TileType.Item:
                        WriteTile("? ", ConsoleColor.Yellow);
                        break;
                    case TileType.Exit:
                        WriteTile(". ", room_color);
                        break;
                    }
                }

                // Updates interface
                Interface.Clear();
                player.UpdateStats();
                // UpdateInterface(new(){{0, $"   Controls: Movement (WASD), Inventory (1-9), Quit (Q), Spend Points (P)"}, {21, Menu}});
                // UpdateInterface(player.GetInventory());
                // UpdateInterface(player.GetStats());
                
                // // Prints interface
                // foreach (KeyValuePair<int, string> kvp in Interface)
                //     if (i == kvp.Key && j == ySize-1) Console.Write(kvp.Value);

                bool flag = true;
                while (Menu != "" && flag && player.PTS != 0) {
                    Console.Clear();
                    Console.Write("Select the Stat you want to level up:\n[1] HP \n[2] DEF \n[3] ATK \n[4] INT \n[5] SPD \n[6] LCK \n[E] Exit Menu\n> ");
                    Menu = "";
                    input = char.ToLower(Console.ReadKey().KeyChar);
                    switch (input) {
                    case '1':
                        player.HP++;
                        player.PTS--;
                        Print("+1 HP");
                        Print($"HP: {player.HP}");
                        Sleep(450);
                        break;
                    case '2':
                        player.DEF++;
                        player.PTS--;
                        Print("+1 DEF");
                        Print($"DEF: {player.DEF}");
                        Sleep(450);
                        break;
                    case '3':
                        player.ATK++;
                        player.PTS--;
                        Print("+1 ATK");
                        Print($"ATK: {player.ATK}");
                        Sleep(450);
                        break;
                    case '4':
                        player.INT++;
                        player.PTS--;
                        Print("+1 INT");
                        Print($"INT: {player.INT}");
                        Sleep(450);
                        break;
                    case '5':
                        player.SPD++;
                        player.PTS--;
                        Print("+1 SPD");
                        Print($"SPD: {player.SPD}");
                        Sleep(450);
                        break;
                    case '6':
                        player.LCK++;
                        player.PTS--;
                        Print("+1 LCK");
                        Print($"LCK: {player.LCK}");
                        Sleep(450);
                        break;
                    case 'e':
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Try again.");
                        break;
                    }
                    Console.Clear();
                }
            }   
            Console.WriteLine();
        }

    }

    public static void WriteEnemies(Enemy enemy, int i, int j) {
        if (enemy.X == i && enemy.Y == j && enemy.Deity == DeityEnum.Sacrifice)
            WriteTile("! ", ConsoleColor.DarkRed);
        else if (enemy.X == i && enemy.Y == j && enemy.Deity == DeityEnum.Enigma)
            WriteTile("! ", ConsoleColor.DarkMagenta);
        else if (enemy.X == i && enemy.Y == j && enemy.Deity == DeityEnum.Harvest)
            WriteTile("! ", ConsoleColor.DarkGreen);
        else if (enemy.X == i && enemy.Y == j && enemy.Deity == DeityEnum.End)
            WriteTile("! ", ConsoleColor.White);
    }
    public static void WriteTile(string str, ConsoleColor color) {
        Console.ForegroundColor = color;
        Console.Write(str);
        Console.ForegroundColor = ConsoleColor.White;
    }
     public static bool ProcessInput() {
        Console.Write("> ");
        input = char.ToLower(Console.ReadKey().KeyChar);
        Console.WriteLine();
        switch (input) {
        case 'w':
            Menu = "";
            player.Move(-1, 0);
            break;
        case 'a':
            Menu = "";
            player.Move(0, -1);
            break;
        case 's':
            Menu = "";
            player.Move(1, 0);
            break;
        case 'd':
            Menu = "";
            player.Move(0, 1);
            break;
        case 'q':
            Menu = "";
            return false;
        case 'r':
            Menu = "";
            player.X += 2;
            player.Y += 2;
            break;
        case 'p':
            Menu = $"   Select the Stat you want to level up: [1] HP, [2] DEF, [3] ATK, [4] INT, [5] SPD, [6] LCK";
            break;
        case 'e':
            player.ShowInventory();
            break;
        default:
            Menu = "";
            break;
        }
        return true;
    }
    public static void UpdateInterface(Dictionary<int, string> Dict) {
    foreach (KeyValuePair<int, string> kvp in Dict)
        try {Interface.Add(kvp.Key, kvp.Value);} catch {}
    }
}

public static void VaryPrint(string str, int wrap, ConsoleColor color1 = ConsoleColor.DarkRed, ConsoleColor color2 = ConsoleColor.Red) {
    for (int i = 0; i < str.Length; i++) {
        double chance = RNG.NextDouble();
        if (chance < 0.4)
            Console.ForegroundColor = color1;
        else
            Console.ForegroundColor = color2;
        
        Console.Write(str[i]);
        if (i+1 % wrap == 0)
            Console.Write("\n");
    }
    Console.ForegroundColor = ConsoleColor.White;
}

public static int GetChoice(int speed = 0, int duration = 100, params string[] Choices) {
    int maxChoices = Choices.Length;
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine();
    for (int i = 0; i < Choices.Length; i++) {
        Print(string.Format("[{0}] {1}", i+1, Choices[i].ToString()), speed, duration);
    }

    Console.Write("> ");
    bool ValidChoice = int.TryParse( Console.ReadKey().KeyChar + "", out int Choice);

    if (!ValidChoice || Choice < 1 || Choice > maxChoices) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nInvalid Input. Please try again.");
        Sleep(800);
        Console.Clear();
        Choice = GetChoice(speed, duration, Choices);
    }
    Console.Clear();
    return Choice;
}

public static int NextInt(params dynamic[] n) {
    Random RNG = new();
    if (n.Length == 1)
        return RNG.Next(n[0]);
    else
        return RNG.Next(n[0], n[1]);
} 

public static void Print(string str, int speed = 1, int duration = 5, ConsoleColor color = ConsoleColor.White, string name = "") {
    Console.ForegroundColor = color;

    if (!string.IsNullOrEmpty(name))
        str = str.Insert(0, name + ": ");

    foreach (char c in str) {
        Console.Write(c);
        Sleep(speed); 
    }
    Console.WriteLine();
    Sleep(duration);
    
    Console.ForegroundColor = ConsoleColor.White;
}



public enum TileType { Empty, Wall, Portal, HealingPotion, ManaPotion, Gold, Item, Exit }
public class Tile(TileType type) {
    public TileType Type { get; set; } = type;
}

public enum DeityEnum { Sacrifice, Enigma, Harvest, End, None }
public enum MenuEnum {Level, None, Inventory}
public static MenuEnum menu = new();
public static List<DeityEnum> DeityList = Enum.GetValues(typeof(DeityEnum)).Cast<DeityEnum>().ToList();

// Room global variables
public static double GrowthAmount = 0;
public static Tile[,] Room = new Tile[0, 0];
public static RoomGenerator RoomGen = new();    
public static List<Enemy> enemies = [];
public static bool RoomClear = false;
public static Random RNG = new();
public static string Menu = "";
public static char input = ' ';
public static int current_xSize, current_ySize;
public static Dictionary<int, string> Interface = [];
public static bool first_room = true;

// Combat global variables
public static bool IsOver = false;
public static int Turn = 0, room_id = 1, current_id = -1, current_exitDirection;
public static List<int> room_ids = [];
public static List<RoomGenerator> rooms = [];

public class Deity(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???") {
    public int tspeed = tspeed, tduration = tduration;
    public ConsoleColor color = color;
    public string name = name;
    public void Talk(string str) {
        Program.Print(str, tspeed, tduration, color, name);
    }
}
}