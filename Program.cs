using static System.Threading.Thread;

public class Program {
public static dynamic player;
// public static Warrior Sacrifice = new(25, 550, ConsoleColor.DarkRed, "SACRIFICE"); // Warrior equivalent
// public static Mage Enigma = new(13, 550, ConsoleColor.DarkMagenta, "ENIGMA"); // Mage equivalent
// public static Archer Harvest = new(40, 650, ConsoleColor.DarkGreen, "HARVEST"); // Archer equivalent
public class Archer(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "Archer", int HP = 10, double DMGMultiplier = 1, double arrows = 10) : Character(tspeed, tduration, color, name, job, HP, DMGMultiplier) {
        public string weapon_type = "Bow";
        public double arrows = arrows;
    }
    public class Warrior(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "Warrior", int HP = 10, double DMGMultiplier = 1, double stamina = 100) : Character(tspeed, tduration, color, name, job, HP, DMGMultiplier) {
        public string weapon_type = "Blade";
        public double stamina = stamina;
    }
    public class Mage(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "Mage", int HP = 10, double DMGMultiplier = 1, double mana = 100) : Character(tspeed, tduration, color,  name, job, HP, DMGMultiplier) {
        public string weapon_type = "Staff";
        public double mana = mana;
    }
public class Weapon(string name = "???", string description = "???", double DMGMultiplier = 1) {
    public string name = name, description = description;
    public double DMGMultiplier = DMGMultiplier;
}

static void Main() {
    Start();
}
static void Start() {
    int choice = GetChoice(0, 0, "Start Game", "Exit");
    if (choice == 2)
        Environment.Exit(0);
    
    player = new Character(35, 450, ConsoleColor.White, "Player");
    player.Narrate("You jolted awake.");
    player.Talk("Where am I...?");
    GetChoice(0, 0, "Look around.");
    player.Narrate("You looked around.");
    player.Narrate("You seem to be in a dim cave with a faint light in front of you.");
    player.Narrate("You see three weapons on the ground.");
    player.Talk("I need to protect myself.");
    bool flag = true;
    while (flag) {
        player.Think("Which should I pick?");
        GetChoice(0, 0, "Blade", "Bow", "Staff");
        if (choice == 1) {
            player.Narrate("A wooden blade. Durable, but it gets the job done.");
            choice = GetChoice(5, 100, "Pick up.", "Choose another weapon.");
            if (choice == 1) {
                flag = false;
                player = new Warrior(15, 550, ConsoleColor.White, "Player");
                player.AcquireWeapon(new Weapon("Wooden Blade", "Durable, but it gets the job done.", 1));
            }
        } else if (choice == 2) {
            player.Narrate("A wooden bow and a quiver of arrows.");
            choice = GetChoice(5, 100, "Pick up.", "Choose another weapon.");
            if (choice == 1) {
                flag = false;
                player = new Archer(15, 550, ConsoleColor.White, "Player");
                player.AcquireWeapon(new Weapon("Wooden Bow", "Could use some fixing, but it works.", 1));
            }
        } else {
            player.Narrate("A wooden staff.");
            choice = GetChoice(5, 100, "Pick up.", "Choose another weapon.");
            if (choice == 1) {
                flag = false;
                player = new Mage(15, 550, ConsoleColor.White, "Player");
                player.AcquireWeapon(new Weapon("Wooden Staff", "It lacks beauty, but usable.", 1));
            }
            
        }
    }
    Console.Write("\nPress any key to continue...");
    Console.ReadKey();
    player.UpdateStats(true);
    Console.Clear();
    RoomGen = new();
    RoomGen.InitializeRoom();
    RoomGen.DisplayRoom();
}

public class Character {
    public DeityEnum Deity;
    public List<dynamic> inventory;
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
    public int spawnX = NextInt(1, 20), spawnY = NextInt(1, 26);
    public Weapon weapon;
    public Dictionary<int, string> inventoryDict;
    public Character(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "???", int HP = 10, double DMGMultiplier = 1) {
        // Dialogue variables
        this.X = spawnX;
        this.Y = spawnY;
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
        this.inventoryDict = [];
    }

    // Methods
    public void AcquireWeapon(Weapon weapon) {
        this.weapon = weapon;
        Narrate($"Acquired 1x {this.weapon.name}");
        Narrate($"Description: {this.weapon.description}");
    }
    public void EvaluateEXP() {
        while (EXP >= MaxEXP) {
            PTS += 2;
            EXP -= MaxEXP;
            LVL++;
            UpdateStats();
        }
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
        if (newX >= 0 && newX < RoomGen.xSize && newY >= 0 && newY < RoomGen.ySize && Room[newX, newY].Type != TileType.Wall) {
            X = newX;
            Y = newY;
        }
    }

    public Dictionary<int, string> GetInventory() {
        inventoryDict.Clear();
        
        inventoryDict.Add(10, "   --------------------------------------------");
        for (var i = 0; i < 9; i++)
            if (i < inventory.Count)
                inventoryDict.Add(11+i, string.Format("   {0}. {1}", i+1, inventory[i]));    
            else
                inventoryDict.Add(11+i, $"   {1+i}. Empty");
        inventoryDict.Add(20, "   --------------------------------------------");
        return inventoryDict;
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
    // Sacrifice Skills

    // Decreases Health slightly but increases ATK significantly for the attack.
    public void BloodStrike(dynamic enemy) {
            Health -= MaxHealth*0.1 - MaxHealth*0.1*Armor;
            ATK += 5;
            UpdateStats();
            Narrate($"{name} used Blood Strike!");
            DMG = GetDMG((ATK*1.8)+(MaxHealth*0.05), enemy);
            if (CheckEvade(DMG, enemy)) 
                Narrate($"{enemy.name} got hit for {DMG:0} DMG in exchange for {name}'s {MaxHealth*0.1:0} Health!");
            ATK -= 5;
            UpdateStats();
            
    }
    // Deals a percentage of the user's missing Health to the enemy.
    public void LifeDrain(dynamic enemy) {
        DMG = GetDMG(ATK*0.8+(MaxHealth - Health)*0.25, enemy);
        Health += MaxHealth*0.1+DMG*0.45;
        Narrate($"{name} used Life Drain!");
        if (CheckEvade(DMG, enemy)) {
            Narrate($"{enemy.name} got hit for {DMG:0} DMG based on 25% of their missing Health!");
            Narrate($"{name} healed for {DMG*0.45:0} Health!");
        }
    }
    // Reduces Health but increases ATK and SPD for several turns.
    public void SacrificialPower(dynamic enemy) {
        Narrate($"{name} used Sacrifical Power!");
        if (Skill3Timer == 0) {
            Health -= MaxHealth*0.12 - MaxHealth*0.12*Armor;
            ATK += 3;
            SPD += 3;
            Skill3Timer += 2;
            Narrate("Gained +3 ATK and +3 SPD for two turns!");
            UpdateStats();
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }
    // Lowers the enemy's DEF and Armor for several turns.
    public void WeakenResolve(dynamic enemy) {
        Narrate($"{name} used Weaken Resolve!");
        if (Skill4Timer == 0) {
            enemy.DEF -= 6;
            enemy.UpdateStats();
            Skill4Timer = 3;
            Narrate($"{enemy.name} gained -6 DEF for two turns!");
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }
    // Significantly reduces Health but deals a significant percentage of it to the enemy.
    public void UltimateSacrifice(dynamic enemy) {
        Narrate($"{name} used Ultimate Sacrifice!");
        DMG = GetDMG(ATK*1.7+enemy.MaxHealth*0.4, enemy);
        Health -= Health*0.25 - DEF*0.5;
        Health += MaxHealth*0.1+DMG*0.55;
        if (CheckEvade(DMG, enemy)) {
            Narrate($"{enemy.name} got hit for {DMG:0} DMG for 45% of their max Health in exchange for {name}'s  {(Health*0.25 - DEF*0.5):0} Health!");
            Narrate($"{name} healed for {DMG*0.55} Health!");
        }
    }
    // Enigma Skills
    // Tinamad na ako
    public void SoulTrack(dynamic enemy) {
        DMG = INT*2.5;
        Narrate($"{name} used Soul Track!");
        if (CheckEvade(GetDMG(DMG, enemy), enemy))
            Narrate($"{enemy.name} got hit for {DMG:0} DMG ignoring their armor!");
    }
    public void Shadowflame(dynamic enemy) {
        DMG = INT*1.4;
        Narrate($"{name} used Shadowflame!");
        int times = NextInt(1, 3);
        for (var i = 0; i < times; i++) {
            FinalDMG = GetDMG(DMG, enemy);
            if (CheckEvade(FinalDMG, enemy)) {
                Narrate($"{enemy.name} got hit for {FinalDMG:0} DMG!");
            }
        }
        Narrate($"Shadowflame hit {times} times!");
    }
    public void ManaVeil() {
        Narrate($"{name} used Mana Veil!");
        if (Skill3Timer == 0) {
            DEF += 3;
            SPD += 3;
            UpdateStats();
            Narrate($"Gained +3 DEF for two turns!");
            Skill3Timer = 3;
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }
    public void ConjureIllusions(dynamic enemy) {
        Narrate($"{name} used Conjure Illusions!");
        if (Skill4Timer == 0) {
            enemy.ATK -= 3;
            enemy.SPD -= 3;
            Skill4Timer = 3;
            enemy.UpdateStats();
            Narrate($"{enemy.name} gained -3 ATK and -3 SPD for two turns!");
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }
    public void DimensionalRift(dynamic enemy) {
        Narrate($"{name} used Dimensional Rift!");
        string str = "";
        if (Skill5Timer == 0) {
            SPD += 5;
            str = $" and {name} gained +5 SPD for two turns";
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
        DMG = GetDMG(INT*2.5, enemy);
        Skill5Timer = 3;
        
        if (CheckEvade(DMG, enemy)) 
            Narrate($"{enemy.name} got hit for {DMG:0} DMG{str}!");
    }
    // Harvest Skills
    public void ThornedWrath(dynamic enemy) {
         Narrate($"{name} used Thorned Wrath!");
         DMG = GetDMG(ATK*1.5+LCK*1.5, enemy);
        if (CheckEvade(DMG, enemy)) 
            Narrate($"{enemy.name} got hit for {DMG:0} DMG!");

        double chance = RNG.NextDouble();
        if (LCK*0.04 > chance) {
            double HealedAmount = DMG*RNG.NextDouble()*0.04;
            Health += HealedAmount;
            Narrate($"{name} gained {HealedAmount:0} Health!");
        }
    }
    public void LuckyPunch(dynamic enemy) {
        Narrate($"{name} used Lucky Punch!");
        DMG = GetDMG(NextInt(Convert.ToInt32(LCK), Convert.ToInt32(LCK*2.5)), enemy);
        if (CheckEvade(DMG, enemy)) 
            Narrate($"{enemy.name} got hit for {DMG:0} DMG based on luck!");
    }
    public void Growth() {
        Narrate($"{name} used Growth!");
        if (Skill3Timer == 0) {
            double HealedAmount = MaxHealth*0.25; 
            GrowthAmount = HealedAmount;
            MaxHealth += HealedAmount;
            Health += HealedAmount;
            Skill3Timer = 3;
            Narrate($"Gained {HealedAmount} max Health for two turns!");
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }
    public void Wither(dynamic enemy) {
        Narrate($"{name} used Wither!");
        if (Skill4Timer == 0) {
            enemy.SPD -= 3;
            enemy.INT -= 3;
            Skill4Timer = 3;
            Narrate($"{enemy.name} gained -3 SPD and -3 INT!");
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }
    public void RootedRampage(dynamic enemy) {
        int times = NextInt(3, 6);
        Narrate($"{name} used Rooted Rampage!");
        for (var i = 0; i < times; i++) {
            DMG = GetDMG(LCK*1.5, enemy);
            if (CheckEvade(DMG, enemy))
                Narrate($"{enemy.name} got hit for {DMG:0} DMG based on luck!");
        }
        Narrate($"Rooted Rampage hit {times} times!");
    }

    // End Skills
    public void SoulBleed(dynamic enemy) {
        Narrate($"{name} used Soul Bleed!");
        DMG = GetDMG(ATK*1.7, enemy, true);
        if (CheckEvade(DMG, enemy)) {
            Narrate($"{enemy.name} got hit for {DMG:0} DMG ignoring armor!");
            for (int i = 0; i < 3; i++) {
                double Bleed = GetDMG(enemy.MaxHealth*0.09, enemy, true);
                enemy.Health -= Bleed;
                Narrate($"{enemy.name} bled for {Bleed} DMG ignoring armor!");
            }
        }
    }

    public void VoidSlash(dynamic enemy) {
        Narrate($"{name} used Void Slash!");
        int times = NextInt(1, 3);
        for (int i = 0; i < times; i++)
        {
            DMG = GetDMG(ATK*1.2, enemy);
            if (CheckEvade(DMG, enemy)) 
                Narrate($"{enemy.name} got hit for {DMG:0} DMG!");
        }
        Narrate($"Void Slash hit {times} times!");
    }
    
    public void ShroudedMist() {
        Narrate($"{name} used Shrouded Mist!");
        if (Skill3Timer == 0) {
            SPD += 3;
            LCK += 3;
            Skill3Timer = 3;
            Narrate($"Gained +3 SPD and +2 LCK!");
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }

    public void StealStrength(dynamic enemy) {
        Narrate($"{name} used Steal Strength!");
        if (Skill4Timer == 0) {
            enemy.ATK -= 4;
            ATK += 4;
            Narrate($"Gained +4 ATK and the enemy gained -4 ATK for two turns!");
        } else {
            Narrate($"This skill is already in play!");
            Narrate($"Effect will last for two more turns!");
        }
    }

    public void Annhilation(dynamic enemy) {
        Narrate($"{name} used Annhilation!");
        DMG = GetDMG(Math.Clamp(ATK*1.2+ATK*0.2*Math.Max(1, TotalKills), 0, ATK*20), enemy);
        if (CheckEvade(DMG, enemy))
            Narrate($"{enemy.name} got hit for {DMG:0} DMG based on {name}'s kills!");
    }
    
}


public class Enemy(int x, int y, int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???", string job = "???", int HP = 10, double DMGMultiplier = 1) : Character(tspeed, tduration, color, name, job, HP, DMGMultiplier) {
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
        int[] dx = { -1, 0, 0, 1 };
        int[] dy = { -1, 0, 0, 1 };

        // Attempt to move in a random direction
        for (int attempts = 0; attempts < 4; attempts++) {
            int direction = NextInt(4);
            int newX = X, newY = Y;
            if (RNG.NextDouble() > 0.5)
                newX = X + dx[direction];
            else
                newY = Y + dy[direction];

            // Check if the new position is within bounds and not a wall
            if (newX >= 0 && newX < RoomGen.xSize && newY >= 0 && newY < RoomGen.ySize && Room[newX, newY].Type != TileType.Wall)
            {
                X = newX;
                Y = newY;
                break; // Successfully moved
            }
        }
    }
}

public class RoomGenerator {
    public int xSize, ySize;
    public RoomGenerator() {
        // this.xSize = NextInt(25, 33);
        // this.ySize = NextInt(33, 55);
        this.xSize = this.ySize = NextInt(10, 15);
    }
        public void InitializeRoom() {
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
            
            // Initializes enemies and walls that scale with inner room area
            InitializeWalls(xSize);
            InitializeEnemies(3+xSize/2); 
            // InitializeItems(NextInt(6+player.Stage, 15+player.Stage), NextInt(5+player.Stage, 10+player.Stage));
            // Randomizes and teleports player to random spawnpoint.
            player.spawnX = player.spawnY = NextInt(1, 9);
            player.X = player.spawnX;
            player.Y = player.spawnY;
    }

    public void InitializeEnemies(int maxEnemies) {
        enemies.Clear();
        while (enemies.Count < maxEnemies) {
            int x = NextInt(1, xSize-1);
            int y = NextInt(1, ySize-1);
            if (Room[x, y].Type != TileType.Wall) {
                enemies.Add(new Enemy(x, y));
                enemies[^1].Initialize();
            }
        }
    }
    public void InitializeItems(int HealingPotions = 0, int Golds = 0) {
        int CurrentHealingPotions = 0;
        int CurrentGolds = 0;

        while (CurrentHealingPotions != HealingPotions || CurrentGolds != Golds) {
            int x = NextInt(1, xSize - 1);
            int y = NextInt(1, ySize - 1);
            
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

    private void InitializeWalls(int numOfWalls) {
        for (int i = 0; i < numOfWalls; i++) {
            int x = NextInt(1, xSize - 1);
            int y = NextInt(1, ySize - 1);

            // if ((x == xSize / 2 && y >= ySize / 2 - 1 && y <= ySize / 2 + 1) || (y == ySize / 2 && x >= xSize / 2 - 1 && x <= xSize / 2 + 1)) {
            //     continue; 
            // }

            Room[x, y] = new Tile(TileType.Wall);

            if (RNG.NextDouble() >= 0.5) {
                int length = NextInt(1, 4);
                for (int j = 0; j < length; j++) {
                    int nx = x + j < xSize ? x + j : x; // Ensure within bounds 
                    Room[nx, y] = new Tile(TileType.Wall);
                }
            }
            else {
                int length = NextInt(1, 4); 
                for (int l = 0; l < length; l++) {
                    int ny = y + l < ySize ? y + l : y; // Ensure within bounds
                    Room[x, ny] = new Tile(TileType.Wall);
                }
            }
        }
        for (int i = 0; i < 4; i++) {
            int[][] tiles = [[1, ySize/2], [xSize - 1, ySize / 2], [xSize/2, 1], [xSize/2, ySize - 1]];
            int x = tiles[i][0];
            int y = tiles[i][1];
            if (Room[x, y].Type == TileType.Wall)
                Room[x, y] = new Tile(TileType.Empty);
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
            
            // Checks if player's tile is a portal
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

        xSize = ySize = NextInt(10, 15);
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
            switch (enemy.Deity) {
            case DeityEnum.Sacrifice:
                SacrificeSkills(ChosenSkill, enemy);
                break;
            case DeityEnum.Enigma:
                EnigmaSkills(ChosenSkill, enemy);
                break;
            case DeityEnum.Harvest:
                HarvestSkills(ChosenSkill, enemy);
                break;
            case DeityEnum.End:
                EndSkills(ChosenSkill, enemy);
                break;
            }
            if(!IsOver) CheckHealth(enemy);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    public static void SacrificeSkills(double ChosenSkill, Enemy enemy) {
        if (ChosenSkill < 0.3)
            enemy.BloodStrike(player);
        else if (ChosenSkill < 0.6)
            enemy.LifeDrain(player);
        else if (ChosenSkill < 0.75)
            enemy.SacrificialPower(player);
        else if (ChosenSkill < 0.9)
            enemy.WeakenResolve(player);
        else
            enemy.UltimateSacrifice(player);
    }
    public static void EnigmaSkills(double ChosenSkill, Enemy enemy) {
        if (ChosenSkill < 0.3)
            enemy.SoulTrack(player);
        else if (ChosenSkill < 0.6)
            enemy.Shadowflame(player);
        else if (ChosenSkill < 0.75)
            enemy.ManaVeil();
        else if (ChosenSkill < 0.9)
            enemy.ConjureIllusions(player);
        else
            enemy.DimensionalRift(player);
    }
    public static void HarvestSkills(double ChosenSkill, Enemy enemy) {
        if (ChosenSkill < 0.3)
            enemy.ThornedWrath(player);
        else if (ChosenSkill < 0.6)
            enemy.LuckyPunch(player);
        else if (ChosenSkill < 0.75)
            enemy.Growth();
        else if (ChosenSkill < 0.9)
            enemy.Wither(player);
        else
            enemy.RootedRampage(player);
    }
    
    public static void EndSkills(double ChosenSkill, Enemy enemy) {
        if (ChosenSkill < 0.3)
            enemy.SoulBleed(player);
        else if (ChosenSkill < 0.6)
            enemy.VoidSlash(player);
        else if (ChosenSkill < 0.75)
            enemy.ShroudedMist();
        else if (ChosenSkill < 0.9)
            enemy.StealStrength(player);
        else
            enemy.Annhilation(player);
    }


    public static void PlayerTurn(Enemy enemy) {
        bool StayTurn = false;
        do {
            if (!IsOver) {
                StayTurn = false;
                EvaluateTimers(enemy);
            
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

    public static void EvaluateTimers(Enemy enemy) {
        // Evaluates the timers of each skill and their effects. Needs optimization.
        foreach (KeyValuePair<dynamic, dynamic> kvp in new Dictionary<dynamic, dynamic>(){{player, enemy}, {enemy, player}}) {
                if (kvp.Key.Skill3Timer != 0)
                    kvp.Key.Skill3Timer--;
                if (kvp.Key.Skill4Timer != 0)
                    kvp.Key.Skill4Timer--;
                if (kvp.Key.Skill5Timer != 0)
                    kvp.Key.Skill5Timer--;
                switch(kvp.Key.Deity) {
                case DeityEnum.Sacrifice:
                    if (kvp.Key.Skill3Timer == 1) {
                        kvp.Key.ATK -= 3;
                        kvp.Key.SPD -= 3;
                    } else if (kvp.Key.Skill4Timer == 1) {
                        kvp.Value.DEF += 6;
                    }
                    break;
                case DeityEnum.Enigma:
                    if (kvp.Key.Skill3Timer == 1) {    
                        kvp.Key.DEF -= 3;
                        kvp.Key.SPD -= 3;
                    } else if (kvp.Key.Skill4Timer == 1) {
                        kvp.Value.ATK += 3;
                        kvp.Value.SPD += 3;
                    } else if (kvp.Key.Skill5Timer == 1) {
                        kvp.Value.SPD -= 5;
                    }
                    break;
                case DeityEnum.Harvest:
                if (kvp.Key.Skill3Timer == 1) {
                    kvp.Key.MaxHealth -= GrowthAmount;
                    kvp.Key.Health -= GrowthAmount;
                } else if (kvp.Key.Skill4Timer == 1) {
                    kvp.Value.SPD += 3;
                    kvp.Value.INT += 3;
                }
                    break;
                case DeityEnum.End:
                    if (kvp.Key.Skill1Timer == 1) {
                        kvp.Key.SPD -= 3;
                        kvp.Key.LCK -= 3;
                    } else if (kvp.Key.Skill4Timer == 1) {
                        kvp.Value.ATK += 4;
                        kvp.Key.ATK -= 4;
                    }
                    break;
                }
            }
    }

    public static bool Attack(Enemy enemy) {
        int Choice, maxChoices;
        bool ValidChoice;
        bool StayTurn = false;
        do {
            Console.Clear();
            List<string> AttackDescriptions = [];
            player.UpdateStats();
            // Changes the choices depending on the user's deity.
            switch(player.Deity) {
            case DeityEnum.Sacrifice:
                AttackDescriptions = 
                [
                    $"Blood Strike\n     Damage: {(5+player.ATK)*1.8+player.MaxHealth*0.05:0} DMG\n     Cost: {player.MaxHealth*0.1} Health\n", 
                    $"Life Drain\n     Damage: {player.ATK*0.8+(player.MaxHealth - player.Health)*0.25:0} DMG (Scales with Enemy Missing Health)\n     Effect: Heal {player.MaxHealth*0.1+(player.MaxHealth - player.Health)*0.25*0.45:0} Health\n", 
                    "Sacrificial Power\n     Effect: +3 ATK & +3 SPD\n", 
                    "Weaken Resolve\n     Effect: Enemy -6 DEF\n", 
                    $"Ultimate Sacrifice\n     Damage: {player.ATK*0.6+enemy.MaxHealth*0.4:0} DMG\n     Cost: {player.Health*0.25} Health\n     Effect: Heal {(player.ATK*0.6+enemy.MaxHealth*0.4)*0.55:0} Health\n"
                ];
                break;
            case DeityEnum.Enigma:
                AttackDescriptions = 
                [
                    $"Soul Track\n     Damage: {player.INT*1.5:0} INT DMG\n", 
                    $"Shadowflame\n     Damage: {player.INT*0.7:0} INT DMG 1-3 times\n", 
                    $"Mana Veil\n     Effect: +3 DEF & +3 SPD\n", 
                    $"Conjure Illusions\n     Effect: Enemy -3 ATK & -3 SPD\n", 
                    $"Dimensional Rift\n     Damage: {player.INT*2.5:0} DMG\n     Effect: +5 SPD\n"
                ];
                break;
            case DeityEnum.Harvest:
                AttackDescriptions = 
                [
                    $"Thorned Wrath\n     Damage: {player.ATK*1.5+player.LCK*1.5:0} DMG\n     Effect: {player.LCK*0.04*100}% chance to heal {player.ATK*1.5+player.LCK*1.5*0.5:0} Health\n", 
                    $"Lucky Punch\n     Damage: {player.LCK*0.5:0}-{player.LCK*2.5*1.5:0} DMG\n", 
                    $"Growth\n     Effect: Gain {player.MaxHealth*0.25:0} Health for two turns\n", 
                    $"Wither\n     Effect: Enemy -3 SPD & -3 INT\n", 
                    $"Rooted Rampage\n     Damage: {(player.LCK*1.5).ToString("0")} DMG 3-6 times\n"
                ];
                break;
            case DeityEnum.End:
                    AttackDescriptions = 
                [
                    $"Soul Bleed\n     Damage: {player.ATK*1.2:0} DMG & {enemy.MaxHealth*0.06:0} DMG 0-3 times\n", 
                    $"Void Slash\n     Damage: {player.ATK*0.9:0} DMG 1-3 times\n", 
                    $"Shrouded Mist\n     Effect: +3 SPD & +3 LCK\n", 
                    $"Steal Strength\n     Effect: +4 ATK & Enemy -4 ATK\n", 
                    $"Annhilation\n     Damage: {player.ATK*0.5+player.ATK*0.1*(player.TotalKills+1):0} DMG\n"
                ];
                break;
            }
            AttackDescriptions.Add("Cancel");

            // Gets the input from the player using a modified GetChoice();
            maxChoices = AttackDescriptions.Count;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            for (int i = 0; i < AttackDescriptions.Count; i++)
                Console.WriteLine(string.Format("[{0}] {1}", i+1, AttackDescriptions[i].ToString()));
            
            Console.Write("> ");
            ValidChoice = int.TryParse( Console.ReadKey().KeyChar + "", out Choice);

            if (!ValidChoice || Choice < 1 || Choice > maxChoices) {
                Console.Clear();
            }
        } while (!ValidChoice || Choice < 1 || Choice > maxChoices);

        Console.Clear();

        // Activates the selected skill
        switch(player.Deity) {
            case DeityEnum.Sacrifice:
            switch (Choice) {
            case 1:
                player.BloodStrike(enemy);
                break;
            case 2:
                player.LifeDrain(enemy);
                break;
            case 3:
                player.SacrificialPower(enemy);
                break;
            case 4:
                player.WeakenResolve(enemy);
                break;
            case 5:
                player.UltimateSacrifice(enemy);
                break;
            case 6:
                return true;
            }
            break;
            case DeityEnum.Enigma:
            switch (Choice) {
            case 1:
                player.SoulTrack(enemy);
                break;
            case 2:
                player.Shadowflame(enemy);
                break;
            case 3:
                player.ManaVeil();
                break;
            case 4:
                player.ConjureIllusions(enemy);
                break;
            case 5:
                player.DimensionalRift(enemy);
                break;
            case 6:
                StayTurn = true;
                break;
            }
            break;
            case DeityEnum.Harvest:
            switch (Choice) {
            case 1:
                player.ThornedWrath(enemy);
                break;
            case 2:
                player.LuckyPunch(enemy);
                break;
            case 3:
                player.Growth();
                break;
            case 4:
                player.Wither(enemy);
                break;
            case 5:
                player.RootedRampage(enemy);
                break;
            case 6:
                StayTurn = true;
                break;
            }
            break;
            case DeityEnum.End:
            switch (Choice) {
            case 1:
                player.SoulBleed(enemy);
                break;
            case 2:
                player.VoidSlash(enemy);
                break;
            case 3:
                player.ShroudedMist();
                break;
            case 4:
                player.StealStrength(enemy);
                break;
            case 5:
                player.Annhilation(enemy);
                break;
            case 6:
                StayTurn = true;
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
        Dictionary<int, string> items = player.GetInventory();
        for (int i = 0; i < items.Count; i++) {
            try {
                Console.WriteLine($"{i+1}. {items[i]}");
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
            EvaluateTimers(enemy);
            Console.ReadKey();
        } else {
            player.Narrate("You failed to flee.");
        }
    }

    public static void PrintEnemy(Enemy enemy, bool ShowStats = true, bool NeedInput = true) {
        string enemyArt = "";
        switch (enemy.name) {
        case "Mind Walker":
            enemyArt = @"
████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                          ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████
████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                                 ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████
████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                                       ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████
████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                                         ▒ ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓███████████████████
████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                                                              ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓█████████████████████
█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                            ▒▒▒▒▒▒                                ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓██████████████████████
█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                          ▒▒▒▒▓▓▓▓▓▓▒                              ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓███████████████████████
█████▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒                          ▒▒▒▒▓▓▓▓▓▓▓▓▓▒                            ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓██████████████████████
███████▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒                        ▒▒▒▒▒▓▓██▓▓▓▓▓▓▓▓                            ▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓██████████████████████
███████▓██████▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                       ▒▒▒▒▓▓███▓█▓▓▓▓▓▓▓▓                           ▒  ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████
███████████████▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒                         ▒▒▒▓██▓▓▒▒▒▒▓▓▓▓███                              ▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓████████████████████████
████████████████▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒                         ▒▓▓██▓▒▒    ▒▒▓▓▓███▓                              ▒▒▒▒▒▒▒▒▒▓▓▓▓▓██████████████████████████
███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒                         ▒▒▓███▓▒   ▒   ▒▓▓████▒                              ▒▒▒▒▒▒▒▒▓▓▓▓▓██████████████████████▓▓▓▓
██████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒                          ▒▓███▓▒▒  ▒▒▒  ▒▓█████▓                              ▒▒▒▒▒▒▒▓▓▓▓▓██████████████████████▓▓▓▓▓
████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒                         ▒▓█████▓▓▓▒▒ ▒▒▓▓▓██████▓                             ▒▒▒▒▒▓▓▓▓▓▓▓▓▓██████████████████▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒                         ▓██████▓▓▓▒▒▒▒▓▓▓▓▓██████▒                            ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓████████████████▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒                       ▒████████▓▓▓▓▒▒▓▓▓█████████                            ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓████████████████▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒                      ▓█████████▓▓▒▒ ▒▒███████████                            ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓███████████████▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒                        ▓███████████▓▒▒▓███████████▓▒     ▒      ▒                 ▒▓▓▓▓▓▓▓▓▓▓▓██████████████▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒                         ▓█████████▓▓▓▓▓▓▓▓█████████▓▓▒ ▒▒▒ ▒▒  ▒▒                 ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒                        ▒▒▓███████▓▓▓▒▒▓▓▓▓██████████▓▓▒▒▒   ▒▒▓▓▒▒                ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▓▓
█████████████▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒                  ▒▒▒▒▒▒▓▓████████▓▒▒▒▒▒▒▓██████████▓▓▓▒▓▒▒▒▒▒▒▓▓██▒▒               ▒▒▒▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▓▓
█████████████▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒                 ▒▒▒▓▒▒▓▓▓███████▓▓▒▒  ▒▒▓████████▓▓▓▓▓█▓▓▓▓▓▓▓▓▓█▓▓▓▒▒▒          ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▓
█████████████▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒                  ▒▒▒▓▓▓▓▓▓▓▓▓▓▓██▓▓▒▒▒      ▒▒▒▓██▓▓▓▓▓▒▒▓▓█▓█▓▓▓▓▓▓▓▓███▓           ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓████████████▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒             ▒▒▒▒▒▓▓▓██▓▓▓▓▓▓▓█▓▒▒▒            ▒▒▓██▓▓▓▓▓▓▓███████████████▓         ▒▒▒▒▓▓▓▓▓▓▓▓▓▓███████████████▓▓▓▓▓▓▓▓
███████▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒            ▒▒▓▓▓▓█▓████▓▓█▓▓▓█▓▒▒               ▒▒▓▓█▓▓▓▓█▓████████████████▒        ▒▒▒▒▓▓▓▓▓▓▓▓▓████████████████▓▓▓▓▓▓▓▓
███████▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒            ▒▓▓▓▓███████▓█▓▓▓█▓▒                    ▒▓▓▓▓██████████▓████▓▓▒▒▒▒     ▒▒▒▒▓▓▓▓▓▓▓▓▓▓█████████████████▓▓▓▓▓▓▓▓
███████▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒             ▒█████████████▓▓██▒▒                    ▒▒█▓████████████▓▓███▒       ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓█████████████████▓▓▓▓▓▓▓▓
███████▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒             ▓▒▒███████████▓███▓▒                    ▒▓██▓███████████████▓▓▒      ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒              ▓███████████▓███▓▒▒                  ▒▒▓███████████████████▒       ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████▓▓▓▓▓▓▓▓
████████▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒             ▒▓▓██████████████▓▒▒▒▒              ▒▓▓▓████████████████████▓           ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒              ▓███████████▓█████▓▓▒            ▒▓▓███████████████████████▒              ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓▓▓▓▓▓▓
██████████▓▓▓▓▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒              ██████████████████▓▓▓▒▒▒      ▒▒▓▓▓████████████▓█████████▓▓█▒              ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓▓▓▓▓▓
███████████▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒             ▒▓██████████████████████▓▒▒  ▒▓▓███████████████▓  ▒██████████▓▓      ▒▒       ▒▒▒▒▒▒▒▒     ▒▓▓▓▓████▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒             ▓▓████████▓▒█████▓████▓▓▓▓▓▓▓▓▒▓▓▓█████████████     ▓██████████▓▒    ▒▓▒            ▒▒     ▒▓▓▓▓████▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒            ▓▓▓███████▓  █████████▓██▓▒▒▓▒▒▓██▓▓███████████▒      ▒███████████▓  ▒▒█▓▒▒          ▒▒    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒             ▓▓████████▒   ▒████████▓▓▓▓▓▓█▓▓▓▓▓▓███████████▓         ▓█████████▓▓ ▒▓██▓▓          ▒▒   ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒               ▓▓████████      ▓███████▓██▓▒▒▓▒▓██▓▓███████████           ▓█████████▓▓▒████▓          ▒▒  ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒              ▒▓▓▓███████▒      ▓████████▓▓▓█▓█▓▓▓▓▓███████████▒            ██████▒▓▓▒▓▓█████▒         ▒▒ ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒           ▒▒▓██████████▓       ▓████████▓█▓▒▒▓▒▓██▓███████████              ▒████▒▒▓▒██▓████▒         ▒▒  ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒           ▒▓██▓████████        █████████▓▓▓█▓█▓▓▓▓████████████▒            ▒ ▒██▓█▓▓▓▓▓▓████▒        ▒▒▒  ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒           ▓███▓███████        ▓█████████▓██▓▓▓▓██▓████████████▒             ▓████████▓█▓████▒      ▒▒▒▒▒▒▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒         ▒███████████▓▒       █████████████████████████████████▒  ▓▓▒         ▓██████▓▓█████▓     ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒       ▓███████████▒     ▒██████████████▓▓▓▓█████████████████▓▒ ▓█▓           ▓████▓▓▓▓▓██▓     ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▒▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒      ▓█████████▓▒       █████████████▓▓▒▒▒▒▒▓▓█████████▓▓██▓▓██████▒          ▒▓███▓▓▓▓▓██▒      ▒▒▒▒  ▒ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒        ▒▓████████▒        ▓███████████▓█▓▓▒▓▒▒▒▓▓▓█▓█▓▓███████████████▓            ▒███▓▒▓▓▓▓▓      ▒▒▒   ▒ ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒         ▒▓████████▒        ▓██████████████▓▓▓▓▓▓▓▓▓██████████████████████▒             ▓██▓▓▓▓▓▓▓       ▒   ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▒▒▒▒▒ ▒▒▒▒                  ▓███████▓          ████████████████████████████████████████████▓██              ▓█▓▓▓▓███▓          ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▒▒▒▒▒  ▒  ▒                 ▓███████▒          ▓█████████████████████████████████████████████▓█▒              ████████           ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▒▒▒▒▒▒  ▒  ▒                ▒███████▒          ▒████▓████▓▓▓▓▓▓███████████████████████████████▓██              ▓███████           ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▒▒▒▒▒▒                      ▓██████▒           █▓▓█▓██▓▓▓▓▓▓▓▓▓██████████████████████▓█████████▓█▓            ▓▓███████           ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▒▒▒▒▒▒                     ▓███▓▓█▓           ▒▓▓▓▓██▓▓▓▓▓▓████████████████████▓▓▓▓███████████▓███            █████████▓          ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▒▒▒▒▒                    ▓████▓███ ▒▓       ▒▒▓▓███▓▓▓▓▓▓▓████████████████████▓▓▓▓▓█▓▓▓██████  ▒▓█▓▓▒        ██▒▒█████▓       ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▒▒▒▒▒▒ ▒                  ▓█▓███████▓       ▒▒▓▓███▓▓▓▓▓▓▓▓█████████████████████▓▓▓▓▓█▓▓▓▓▓▓███    ▒▒▓█▓      █▓  ▓████▒       ▒▒ ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▒▒▒▒▒▒▒                 ▓▓█████▒▒        ▒▒▓▓████▓▓▓▓▓▓▓▓▓▓████████████████████▓▓▓▓▓█▓▓▓▓▓▓▓▓█▒       ▒     ▒   ▓███▒           ▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▒▒▒▒▒▒                ▒▓▓█▒▓▒▓         ▒▒▓▓█████▓▓▓▓▓▓▓████████████████████▓▓▓█▓▓▓▓██▓▓▓▓▓▓▓██▒               ▒██▓             ▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▒▒▒▒▒▒▒    ▓▒       ▒▓▓█▓            ▒▓▓▓████▓▓██▓▓▓▓▓▓▓█████████████████████▓▓▓▓▓▓█▓▓▓▓▓▓▓▓██▓              ▒█▒              ▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▓▓▓▓▓
▓▓▓▓▒▒▒▒▒▒▒     ▓████▓█▓▓▓▓█▒           ▒▓▓▓█████████▓▓▓▓█▓███████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▒            ▒                ▒ ▒▒▒▒▒▒▒▒▓▒▒▒▒▒▓▓▓▓
▓▓▓▒▒▒▒▒▒▒▒▒      ▒▒▓▓▓▓▓▓▓█▓          ▒▓▓▓████████████████████████████████████▓▓▓▓▓▓██▓▓▓▓▓▓▓███████▓                            ▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▓▓▓▓
▒▒▒▒▒▒▒▒▒▒▒▒         ▒█▓▓██████▓▓▒▒   ▒▓▓▓███████████████████████████████████████▓▓▓▓▓█▓▓▓▓▓▓▓█████████                          ▒▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓
▒▒▒▒▒▒▒▒▒▒▒▒          ▓▓██▓▒▒▒▒▓▒▒   ▓▓▓▓▓█████████████████████████████████████████▓▓▓██▓▓▓▓▓▓██████████▒                        ▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▒▒
▒▒▒▒▒ ▒▒▒▒▒▒▒        ▒▓██▒          ▓▓▓▓▓███████████████████████████████████████████▓▓▓██▓▓▓█████████████▓                     ▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▒▒▒
▒▒▒▒▒   ▒            ███▓          ▓▓▓▓▓████████████████████████████████████████████▓▓▓███▓▓▓██████████████▒                 ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒
▒▒▒▒▒               ▒██▓          ▓▓▓▓▓███████████████████████████████████████████████▓████▓▓▓██████████████▓              ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒
▒▒▒▒  ▒▒            ▓██          ▓▓▓▓▓███████████████████████████████████████████████████████▓▓███████████████▓          ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒               ▒██▒         ▓███▓███████████████████████████████████████████████████████████████████████████▓       ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒               ▓██         ▓██████████████████████████████████████████████████████████████████████████████████▓     ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒              ▓▓█▒        ▒█████████████████████████████████████████████████████████████████████████████████████▓▒    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒   ▒          ███         █████████████████████████████████████████████████████████████████████████████████████████▓    ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒   ▒         ▓██▒        ▓██████████████████▒ ▒▓███████████████████████████████████▒  ▒▓█████████████████████████████▓     ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒   ▒        ▒███        ▒██████████████████▓   ▒▓██████████████████▒▒█████████████▒▒  ▒▓███████████████████████████████▓         ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒   ▒        ▓██▒        ███████████████████▓  ▓███████████████▓████▒  ▓███████████▓▓▒▒▒██████████████████ ▒▓████████████▓▒        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ 
▒▒▒▒   ▒        ██▓        ▒██████████████████████████████████▒ ▒█▒▒███▓    ▓██▓█████████████████████████████▓    ▒▓██████████▓▒      ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ 
▒▒▒            ▓██         ███████████████████████████████████▒      ██▒      ▓ ▓██████████████████████████████      ▒▓█████████▓       ▒▒▒▒▒▒▒    ▒▒ 
▒▒▒            ██▒         ███████████████████████████████████▓       █   ▓       ██████████████████████████████▒       ▒▓▓██████▓▒     ▒▒▒   ▒ ▒     
▒ ▒           ▓██         ▒███████████████████████████████████▓        ▒  ▓        ▒█████████████████████████████▓         ▒▓▓█████▓▒         ▒       
▒           ██▒         ▒█████████████▓ ▓ ▓█▓████████████████                     █████████████████▒█████████████▒          ▒▒▓███▓▒               ▓
▒          ▒██          ▓██▓    ▒█▒▓██▒    ▒▒████████████████▒                    █████████████████▓ █████████████▓            ▒▒▓▓█▓             ▒█
▒   ▒      ▓█▒          ▓██▓      ▒ ▓██     █████████████████▓                    ██████████████████ ▒██▓▒██████████▒              ▒▓▓▒           ██
▒   ▒      ██           ▒███         █▒▒     █████████████████        ▒           ▓████████████████▓  ▒█▓▒ ████▒▓████▓               ▒▓▒         ▓██
▒   ▒     ▒█▒            ▓███        ▒▓     ▒████████████████▒        ▓            █████████████████   ▓▓   ▓▒▒ ▒▒▓█▓▓▓                 ▒       ▒███
▒ ▒   ▒     ▓█              ▓███▒       ▒     ▓███████████████▓▒   ▓    ▓            ▓███████████████▓▒  ▒▒   ▒     ▒▓▓▓▓▒                        ███▒
▒▒▒   ▒     █▒                ▓██▓           ▒▒▓▓████████████▓          ▓     ▒      ▒ ▓█████████████▓    ▒   ▒     ▒▒▓▒▒▓▒                      ███▒ 
▒▒▒   ▒    ▒▓                    ▓▓▓▒            ████████████           ▒               ▓████████████▒▒       ▓    ▒▒▒▒▒▒▒                   ▒▒▒▓██▓▒▒
▒▒▒▒▒ ▒    ▓▒                       ▒▒▒          ███████████▒                           ▒█████████████        ▓    ▒▒▒▒▒▒▒         ▒▒        █████████
▒▒▒▒▒▒▒    ▓                                    ▓███████████                            ▒█████████████▓       ▒   ▒▒▒▒▒            ▓███▓▒▒  ▓█████████
▒▒▒▒▒▒▒   ▒▒                                   ▓▓██████████▓                            ▓▓▓██████████▒█▒   ▒  ▒                      ▓████████████████
█▓▒▒▒▒▒▒▒ ▒                                   ▒  ▓█████████                             ▒ ▒██████████  ▓                       ▒      ▓███████████████
██▓▒▒▒▒▒▒▒▒  ▒▒  ▒                               ▒█████████                                ▒█████████                         ▓▓▒ ▒   ▒▓██████████████
███▓▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                             ▓████████▓                                 ▒████████▒                        █▓▒       ██████████████
██████▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                         ██████████                                  ████████▓                       ▒█▒    ▒▓▓████▓██████████
███████▓▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒                       ▒█████████▓                                  ▓████████▒  ▒▒              ▒  ▒ ▒    ▓██████▓▓██████████
████████▓▒▒▒██▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒               ▒▓██████████▓                                 ▒▓█████████▒                 ▒▒    ▒█▓████████████████████
██████████▓▓▓██▓▒▒▒▒▒▒▒▒▒▒█████▓▓▒▒          ▒▓███████████▓                                  ▒████████▓      ▒     ▒    ▓███▓ ▓▓▓█████████████████████
█████████████████▓▓▒▒▒▒▒▒██████████▓▒        ▒█████████████▒                                 ▓████████▓            ▒  ▓██████▓████████████████████████
███████████████████▓▓█▓████████████████▓▒  ▒▓▓█████████████▒                                  ███▓█████▒            ▒ ▒▓██████████████████████████████
███████████████████████████████████████▓▓▓▓▓▓██████████████▓▒▒▒     ▒           ▒▒▒▒▒▒▒▒▒▒▒▒ ▒▓▓▓▓▓▓███▓      ▒  ▒ ▒▓▓████████████████████████████████
████████████████████████████████████████████████████████████▓▓▒▒▒▒▒▓▓▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▓███▓▓▓▓▓██▓▓▒ ▒▓████████████████████████████████████████
█████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓██████▓▓▓████████████▓▒▓██████████████████████████████████████████
██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████";
            break;
        case "Bloodbound Stalker":
            enemyArt = @"
█████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████
██████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████
████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████
██████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████
████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████
██████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████
█████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████████████
███████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████
█████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████████
████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████
██████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████
█████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒█▒▒▒▒▒▒▒ ▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████
████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒█▒▒         ▓█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████
███████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▒        ▒▒▓█▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████
██████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓▒▒▒▒▒ ▒▒▒▓███▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████
█████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓█▓▓▓▓▓▓▓▓▓▓███▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████
████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████▓▓███████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████
███████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████▓████████████▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▓█▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████
███████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████████████████▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████
██████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒███▓███▓██████████▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▓█▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████
█████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████████▒▓▒▒▒▒▒▒▒▒▒▓█▒▒▒▒▒█▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████
████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▒▒▒▒▒▓████████████████████▓▒▒▒▒▒▒▒▓█▓▒▒▒▓█▒▒▒▒▒▒▒▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████
████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▓▒▒▒▓▒▓█████████████████▓▓▓▒▒▒▒▒▒▒▓█▓▒▓██▓▒▒▒▒▒▓▓▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████
███████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▒▒▒▓▓▒▒▒▓▒▒▓█▒▒█████████████████▓▒▒▒▒▒▒ ▒▓▓▓▓▓▓██▒▓▒▒▒▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████
██████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▒▒▒▒▓▒▒▒█▒▒▒▒█▒▓▓▒▓█████████████████▓▓▒▒▒▒▓▓██▓▓████▓▓▓▒▒▓▓▒▒▓▓█▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████
█████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓▒▒██▓▓▓▓▓▒▓███████████████████▓▓▓▓██▓█▓▓▓▓▓▓▒▓▓▓▓█▓▓█▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████
█████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███▓▒▒▒▓▓▓▓██████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████
████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▓▓▓▒▒▒▒▒▒▒▓████████████████████████▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓███▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████
████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██▓▓█▓▓▓▓▒▓▓█████████████████████▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████
███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▒▓▓▓▒▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████▓▓▓▓▓▓▓▓█▓▓██████████▓▓▒▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████
██████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓██████▓▓▓▓▓▓▓████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████
██████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████▓████████████████████████████████▓▓▓▓▓▓█████████████▒▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████
█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████████████████████▓███▓▓▒▓▓▓▓█████████████████████▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████
█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓█████████████████████████████▓▓█▓▒▒▒▒▓▓▓▓█████████████████████▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████
████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▓█████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████▓▒▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████
████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒████████████████████████▓▓▓▓▓▓▓▓▓▓▓█████████████████████████▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████
████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▓████████████████████████▓▓▓██▓▓▓████████████████████████████▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████
████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████████████████████████████████████████████████████▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒███████████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓████████████████████████████████████████████████████████████▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒██████████████████████████████████████████████▒▓█████████████▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████████████████████████████████████████████▒▒█████████████▒▒▒▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████████████████████████████████████████▓  ▒▓████████████▒▒█▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████████████████████████████████████████▒    ▒▓██████████▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████████████████████████████████████▓▒      ▒█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████████████████████████████████████▓▒█▒        ▒████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████▒▒███████████████████████████████▒  █        ▓▓████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▓██████████▓▒▒▓█████████████████████████████▓    ▒        ▒███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████▓▒ ▓█████████████████████████████▓              ▓█████████████▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████▓▒▒███████████▓▓█████████████████▓              ▓████████████▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████▒ ▓███████████▓███████████████████▓▓▓▒       ▒▒▒█████████████▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████▓▒▓████████████████████▓▓███▓████████▒          ▒████████████▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓█████████████▒▓█████████████▓▓██████▓▓▓████████████▒▒▒       ▒▒███████████▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓████████████▒███████████████████████████████████████▓          ▓█████████▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒██████████▓▒▒████████████████████████████████████████▓▒         ██████████▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓█████████▒▒▒▓██████████████████████████████████████████        ▒▒████████▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▒▓████████▓▒▒ ▒███████████████████████████████████████████▒         ▓███████▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓████████▒▓   ▒███████████████████████████████████████████▓         ████████▒▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓████████▒▒▒   ▓████████████████████████████████████████████▒        ███████▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓████████▓▓▒   ▒██████████████████████████████████████████████      ▒████████▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓███████▓▒▒▒▒   ▒██████████████████████████████████████████████▒     ▓████████▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▒▒▒▒   ▓███████████████████████████████████████████████    ▒▓███████▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▒▓▓▓▓▓   ████████████████████████████████████████████████▒  ▓█████████▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓█▓▒▒   ▒███████████████████████████▓▓███████████████████▒▒███████████▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████▒▒▒▒   ▒██████████████████████████▓▓████████████████████▓▓███████████▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▒▒▒▒▒ ▒██████████████████████████▓██▓▓█████████████████▓▒██▓████████▓▒▒▒▒▒▒▒▒▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓████████▓██▒▒▒▒  ▒▒████████████████████████████▓▓▓█████████████████▓▓██▒▓███████▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓████████▓▓▓▓▓▓▓▓███████▓▒▓█▓▒▒▒  ▒▒███████████████████████████▓▓▓▓█████████████████▓▓██▒▓██████▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓████████████▓▓▓▓█████████▓██▓▒▒▒▒  ▓███████████████████████████▓▓▓██████████████████▓▒▓▒▒██████▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓█████████████▓█████████████▓▒▒▒▒▒  ▒████████████████████████████▓███████████████████▓ ▒▓████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓███████████████▓▓██▓▓▓▓▒▒▒▒▒▒▒▒▒▒ ▓█████████████████████████▓▓██████████████████████▒ ▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓██████████████▓▓▓█▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████████████████████████▓██████████████████████▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓██████████████▓███▓▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████████████████████████████████████████▓ ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓██████████████▓▓███▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒██████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓███████████████▓▓███▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████████████████████████████████████████████▒▒▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓███████████████▓▓█▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓███████████████████████████████████████████████████▓▒   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓███████████████▓▓▓█▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓███████████████████████████████████████████████████▓▒   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓████████████████▓▓█▓▒▓▒▒▒▒▒▒▒▒▒▒▓▓███████████████████████████████████████████████████▓▒    ▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓███████████████████▓▒▒▒▒▒▒▒▒▒▒▒▓█████████████████████████████████████████████████████▓▒   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓▓▓██████████████████▓▒▒▒▒▒▒▒▒▒▒▒▓██████████████████████████████████████████████████████▓▒ ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓▓▓▓█████████████████▓▒▒▒▒▒▒▒▒▒▒▒▓███████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
██████████▓▓▓▓▓▓▓▓▓▓▓██████████████████▓▒▓▒▒▒▒▒▒▒▒▓███████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓██████████████████▓▒▒▒▒▒▒▒▒▓████████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
███████████▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓██▓██▓▓▓▒▒▒▒▒▓█████████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▓▓▓██▓▓▒▒▒▒▒▓██████████████████████████████████████████████████████████▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▒▒▓█▓▓▒▒▒▒▒▓██████████████████████████████████████████████████████████▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▒▒▒▒▒▓▓▓▒▓▒▒▒███████████████████████████████████████████████████████████▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▒▒▒▒▓▓▓▒▒▒▒▓███████████████████████████████████████████████████████████▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▒▒▒▒▒▒▒▓▒▒▒▓████████████████████████████████████████████████████████████▓▓▓▓▒▒▒▒▒▒▒▒▒▓█▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▒▒▓█████████████████████████████████████████████████████████████▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓█████████████████████████████████████████████████████████████▓▓▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▒▒▒▒▒▒▒▓████████████████████████████████████████████████████████████████▓▓▓▓▓▒▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓";
            break;
        case "Graveborn Revenant":
            enemyArt = @"
                                                                                                                                                    
                                                                                                                        ▒                          
                                                                                                                        ▓▒                         
                                                                                                                        ▓▓                         
                                                                                                                        ▒▓▓▒                        
                                                    ▒                                                                 ▓▓▓▒                        
                                                    ▒▒▒                                                               ▓▓▓▓                        
                                                        ▒▒▒▒▒▒▒▒▒▒                                                       ▓▓▓▓                        
                                                        ▒▒▒▒▒ ▒▒▒▒▒                                                    ▓▓▓▓▓                        
                                                                                                                        █▓▓▓▓                        
                                                ▒▒▒▒    ▒▒▒                                                         ▒▓▓▓▓▓                        
                                                        ▒▒▒▒▒▒▒▒▒▒                                                  ▓▓▒▓▓▓                        
                                                        ▒▒▒▒▒▒▒▒▒▒▒                                                 ▒▒▓ ▒▒▓                        
                                                        ▒▒▒▒▒▒▒▒▒▒▒▒▒                                                 ▓▓▓ ▒▒▓                        
                                                        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                               ▒▓▓█▒▒▓▒                        
                                                        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                             ▒▓▓▓▓▒▒▒                         
                                                        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                           ▒▓▓▓▓▓▒▒                         
                                                        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                           ▓▓▓▓█▓▓                          
                                                        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                          ▒▓▓▓▓█▓▒                          
                                                        ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▒                                      ▓█▒██▓▓                           
                                                        ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓                                    ▒▓▓█▓▓▓                           
                                                        ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓█▓                                  ▒▓▓▓▒▓▓▒▓                           
                                                    ▒ ▒▒▒▓▒▒▓▓▓▓▓██▓▓▓▓▓▓▓▓██                                   ▒▒ ▓▒▒▓ ▓                           
                                                    ▒▒▒▓▓▓▓▓▓▓██████████▓███▓  ▒▒▒▒▒▒ ▒▒                        ▓▒▒▓▒▓  ▓                           
                                                    ▒▒▓▓▓▓▓███████████▓██████████████▓▓                      ▓▓▓▒▓▓   ▓                           
                                                        ▒▒███████████████▓█████▓██▓▓▓█████▓                    ▒▓▓▓▒▓▒   █                           
                                                    ▒▓▓█▓▓▓██████████████▓▓█▓██▓█▓▓███████▒          ▒▒▓▓▒▒   ▓▓▓▓▓ ▒   ▓                           
                                                ▒▓███▓▓████████████████▓███▓██▓██████▓▓██▓▒  ▒▒  ▒███████▓▒▓▓▓▓▓  ▓   ▓                           
                                                    ▓█▓▓▓▓███████████████████▓▓▓██▓███▓▓█▓█████▓███▓████████▓▓▓▒▓▓   ▓   ▒                           
                                                ▒▓█▓▓▓▓█████████▓████▓███▓█▓▓▓██████▓██████▓███▓▓▓▓▓█████▓▓█▓▓▓▓  ▒  ▒▒                           
                                                ▒█▓█▓▓▓█████████▓███████████▓▓▓█████▓▓█████████▓██▓████████████▓   ▒ ▓                            
                                                ▒▓▓██▓▓█████████▓█████▓███▓████████████████████████████████████▓   ▒▓▒                            
                                                ▒▒▒▓██▓▓▓██████████████▓▓██▓▓▓▓▓████████████▓█████████████████▓▓▓                                  
                                                ▒▒▓▓██▓▓████▓▓█████████▓▒▓▓▓▓▓▓▓████████████████████████▓█████▓▓▓▒                                 
                                                ▒▒▓▓▓█▓▓▓█▓█████████████▒▓▓▓▓▓▓███████   ▓ ▓ ▒▒▓ ▓█████████████▓▒                                  
                                                ▒▒▓▓█▓▓▓███████▓███████▒▓▓▓▓▓▓██████▒   ▓ ▒▒  ▒▒ ▒▓███▓███▒██▓                                    
                                                    ▓▓▓█▓▓▓█▓▓▓▓██████████▓▓█▓▓▓▓██████    ▓  ▓   ▒  ▓▒ ▒▓█▓▓▓▒                                      
                                                    ▒▓█▓▓▓▓▓▓▓▓▓██████████▓█▓██▓▓█████▓▒  ▓▒ ▓  ▒▒  ▒▒  ▓▓                                          
                                                    ▓▒▓█▓▓█▓▓▓█▓▓██████▓███▓█▓▓▓▓▓▓█▓███▓   ▓     ▒  ▒▓                                              
                                                ▒█▒▒▓█▓▓▓▓▓█▓▓▓██▓██▓█████▓▓▓▓▓██████▒    ▓   ▒    ▓                                              
                                                ▓▓█▒▒▓███▓█▓▓▓▓█████▓████▓▓▓▓█▓▓█████▓      ▒▒▒▓    ▓                                              
                                        ▓▓▓▒  ▒▓████▓▓▓▓▓▓██▓▓▓███████████▒▓██▓▓▓███▓▓              ▓                                              
                                    ▒▓▓▓███▓▓▓▓█████▒▓▓▓▓████████████████▓▓█▓▓▓█████▓              ▒                                              
                                    ▒▓▓████████▓▓███████▒▓▓▓▓██████████████████▓██████▓               ▒                                              
                                ▒██▓▓█████████████████▓▓▓▓█████████████████████████▒    ▒▒                                                        
                            ▒▓▓▓██████████████████████▓▓█▓▓███▓██████████████████████▓   ▒▓▓▓▒      ▒            ▒▓██                              
                            ▓█▓█████████████████████▓    ▓▓███▓▓████████████████████████▓  ▒████▒    ▒▒      ▒▓▓▓█▓▓▓                               
                        ▓▓██████████████████████▓ ▓   ▒▓████▓▓██▓███▓███████████████████▓███████    ▓█▓ ▒▓████▓▓▒▒▒                               
                        ▒█▓█████████████████████▒ ▓  ▒██████▓███████▓█████████████████████████▓▓   ▒▓███████▓▓  ▒ ▓                               
                            ▒█▓███████████████████▓  ▓  ▒███████▓██████▓███████████████████████▓  ▒▓▓█▓█████▒   ▒  ▓                                 
                            ▓▓█████▓██▒▓███▓▒▒▓▓   ▓▒ ▒▓▓███████████████▓██████▓███████████▓▓██▓███████████▓   ▒▒ ▓                                 
                            ▓▒ ▓▓▓   ▓        ▒▒  ▒▓ ▓███████████████████▓█████▓▓██████████████████████▓█ ▓▓▒  ▒▓ ▓                                 
                            ▓▒ ▒▓▓   █         ▒▓▒ ▒▓█████▓█████████████████████▓███████████████████▒▒▒▓▒ ▓ ▒▒  ▒▒▓                                 
                            ▓  ▒▓▓   ▒             ▓███████▓████████████████████▓█▓██████████████████▓▒   ▒▒    ▓▓                                  
                            ▓  ▒▒▓   ▒            ▒▓███████████████████████████████████████████████████▒   ▓▒▒▒▓▓                                   
                            ▓  ▒▒▓▒               █▓██████████████████████████████████▓▓████████████████▓       ▒                                   
                            ▓  ▒▒▓▒              ▒███████████████████████████████████▓▒▓███████▓█████████▓      ▓                                   
                            ▓  ▒▒▒▒              ▓████████████████████████████████████▓▓▓██████████████████▒    ▒                                   
                            ▓▓▒ ▒▒▒▒             ▒██████████████████████████████████████▒▓▓██████████████████▓                                       
                        ▒▓▓▓▓███▓▓▓             ▒██████████████████████████████████████▓▒▓████████████████████▒                                     
                    ▒██▓▒▓██████▓              ▓▓█████████████████████████████████████▒▒▓▓█████████████████████▓▒                                 
                    ▓▓▓▓████████▓█             ▓▓███████████████████████▓██████████████▒▓▓▓██████████████████████                                 
                    ▒▒▒▓▓▓▓████████▒           ▒▓████████████████████████▓██████████████▓▒▓▓███████████████████████▒                               
                    ▓▓▓▓▓█████████▒          ▒▓▓████████████████████████████████████████▒▒▒▓████████████████████████▓                             
                    ▒█████████▓▓▓▓███▓▒      ▓█▓█████████████████████████████████████████▒▒▓▓██████████████████▓██████▓                           
                        ▒▓██▓▓▓▒ ▒▓▓▓████▓▒   ▒▓███████████████████████████████████████████▓▒▒▒█████████████████▓▓████████▒                         
                        ▒██▓▓▒ ▒▒▓▓▓████▓▓▒  █▓▓███████████████████████████████████████▓▓██▓▓▓▓███████████████▓▓██████████▓                        
                        ▒██▓▓▓▒▒▓▓▓█████▓▓▒▒▓▓▓██▓██████████▓▓▓██████████████████████████████▓▓▓▓████████████▓▓▓████████████                        
                        ▓███▓▓▓▓████▓▓▓▓█▓▓▓▓▓████████████████████████████████████████████████▓▓▓███████████▓▓▓████████████▒                        
                        ██████████▓▓▓▓▒▓▓███▓▓████████████████████████████████████████████████▓▓▓██████████▓▓▓████████████▒                         
                        ▓▓██▓▓████▓▓▓▓▓▓███▓▓████████████████████████     ▓▓███████████▒▓██████▓▓██████████▓█████████████▒                          
                        ▒▓▓▓▓▓▓▓████▓▓██████▓███████████████████████▓     ▒▓███████████▓▒██████▓▓▓▓███████████████████▒                             
                        ▒▓▓▓▓▓▓███████████▓▒██████████████▓▓▓███████▓     ▓▓███████████  ▓█████▓▓▓███████████████▓▒                                
                            ▒▓███████████▓▓▒  ▒████████████████████████▓    ▒████████████▒ ▓▓█████▓▓▓██████████▓▒                                    
                            ▒▓██████▓▒▒      ▓████████████████████▓▒      █████████████ ▒▓█████▓▓▓██████████▓                                     
                                ▒▓               ▒▓███████████████▓██▒       ▓▓███████████   ▓▓████▓▓███████████▓                                    
                                ▓▒▒                 █████████████████▒       ▓█████████████▒ ▒▓▓█████████████████                                    
                            ▒▓▓▓███▓▒             ▒▓▓████████████████       ██████████▓▓▒▒   ▓▓█▓█████████████▒                                     
                            ▓▓▒▓██████▓            ▒▓████████████████▓      ▒██████████        ███▓▓███████████▓                                     
                        ▒███▓██████▓▓               ▓██████████▓        ▒███████████▒           ▒███████████▓                                     
                        ▒▓▓▓▓████▓▓▓▒▒              ▒▓█▓████████▓      ▓██▓█████████▓            ▒██████████▓                                     
                            ▒▓▓██████▓▒                 ███████████         ▒▒▓█▓▓█▓▓█▒▓▒            ▒█▓████████▓                                    
                            ▒▒▓▓▓▒▒                   ▓█████████▓             ▒ ▒▒ ▒                ▓█████████▓                                    
                                                        ▒█████████▒                                   ▒█████████▓   ▒▒                              
                                                        ▓█████████▒                                    ▓█████████▒ ▒▒▒                              
                                            ▒       ▒▒██████████▓▒▒▒▒▒▓▓▓▓▒▒▓▓▒▒▒▒▒▒▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓██████████▒▒▒▒▒▒▒▒▒▒▒▒                      
                                        ▒▒▓▓▓▓▓▓▓▓█▓▓██████████████▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓█▓▓▓▓▓▓██▓▓██▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒                
                            ▒▒▒▒▒▒▓▓▓██▓▓▓▓▓██▓██████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▓▓▓█▓▓▓███████████████▓▓████▓▓▓▓▓▓▒               
                    ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓█████████▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▒              
                ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████▓▓█▓▓████▓█▓▓▓▒▒             
                ▒▒▒▒▒▓▓▓▓▓▓▓██▓▓███████████████████████████▓▓▓████████▓▓▓▓▓▓████▓▓▓▓▓▓▓▓▓██▓███████████████████████████▓▓██▓▓▓▓▓▓▓▓▒▒              
                    ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓█▓▓██▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓█▓▓▓▓▓▓▓▓▓▓██████████████████▓▓▓▓▓▓▓▓▓▒                  
                        ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓▓▓▓▓▓▓▓▒▒▒▒                       
                                        ▒ ▒   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                                                         
                                                                                                                                                    
                                                                                                                                                    
                                                                                                                                                    
";
            break;
        case "Twilight Herald":
            enemyArt = @"
                                                                                                                                                    
                                                                    ▒▒▒▒▒▒▒▒▒▒▒▒                                                                    
                                                                ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                                                 
                                                            ▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                                                
                                                        ▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒   ▒▓█▓▒                                                           
                                                    ▒▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒    ▒▓██████▓▒                                                         
                                                ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒     ▒███████▓███▓▒                                                       
                                                ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒    ▒█████████▓██████▓▒                                                     
                                                ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒   ▒████████▓▓█▓███▓████▓                                                    
                                            ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒   ▒█████▓▓▒▓▓▓▓▓▓███▓██████▒                                                  
                                            ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒   ▓████▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓██████▓                                                 
                                            ▒ ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒  ▒█████▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▒▓▓███████▒                                               
                                            ▒ ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒  ▒█████▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▓▓███████▒                                              
                                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒█████▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▓▓▓███████▒                                             
                                            ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒█████▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓█▓▒▒▒▒▓▓████████                                             
                                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒█████▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▓▓████████▓                                            
                                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒█████▓▓▓▓▓████▓▓▓▒▒▒▒▒▓▒▒▒▒▒▓▓▓████████▒                                           
                                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓█████▓▓▓▓▓▓▓▓████▓▒▒▒▒▒▓▓▓██████████████                                           
                                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓███████████▓██▓▓▓▓▓▓▒▒▒▓▓█████▓██████████▓                                          
                                        ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓██████████████▓██▓▓▓▒▒▒▓▓▓▓████████████████                                          
                                        ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓█████▓▒▓▓▓▓▓▓▓▓▓▓▒▓▓▒▒▒▒▓▓████▓████████████▒                                         
                                        ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ ▒▒▓▒▓▓▓▓▓████████████▓                                         
                                        ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▓▓▒▒▒▒▒▒▒▒▒▒▒▒ ▒▒▒  ▒▒▒▒▒▒▓▓▓▓▓██████████                                         
                                        ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ ▒▒▒▒▒▒▒▒▒▓▓██████████                                         
                                        ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓███████████▓                                        
                                    ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓████████████                                        
                                    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▓██▓▓▓▓▓▒▒▒▒▓▓█████████████                                        
                                    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▓▓██████████████▓                                       
                                    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▒▓▓▓████████████████                                       
                                    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓▓▓██████████▓▓▓█████████████████▓                                      
                                    ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓███████████████████                                      
                                ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▒▒▓▓▓▓██▓▓▓▓▓▓████████████████████▓                                     
                                ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓██████████████████████▒                                    
                                ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓████████████████████████                                    
                                ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████▓▓▓▓▓▓▓▓▓██████████████████████████▓                                   
                                ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████████████████▒                                  
                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████████████████████████                                  
                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████████▓▓▓███████▓                                 
                            ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████████▓▓▓▓▓██████                                 
                        ▒▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓███████████████████████████████████████████████████▓▓▓▓▓▓▓▓█████                                
                        ▒████████████▓▓▓▓▓▓▓▓█████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓███▓                               
                        ▓██████████████████████████████████████▓▓█████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒                              
                    ▒█████████████████████████████████████████▓▓██▓███████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒                              
                        █████████████████████████████████████████▓▓▓▓▓▓██▓▓██▓███████████▓▓███████████▓▒▒                                            
                        ▒▓▓▓▓▓▓▓█████████████████████████████████▓▓▓▓▒▓▓▓▓▓▓▓▓██▓▓███████▓▓▓█████████████▓▓▒                                         
                        ▒▒▒▒▒▓▓▓▓▓▓▓████████████████████████████▓▓▓▓▓▓▒▒▓▓▓▓▓▓█▓▓▓▓▓██████▓▓██████████████▓▓▓▓▓▒                                     
            ▒██████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓█████████████████████▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓███▓▓▓▓▓███▓▓▓██▓███████████▓▓▓▓▓▓▓▓▓▓▒▒                              
            ██████████▓▓▒            ▒▒▒▓▓▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓██████▓▓▓█▒██▓██████████▓▓▓▓▓▓▓▓█████▓▓▒                        
            ███████████████▓▒            ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▒▓██████▓▓█▒▓█▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒                     
            ███████████████████▓▒          ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓▒ ▒▒▒▒▓▓▓▓▓█████▓▓█▒▓▓▓▓███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▒                     
            ███████████████████████▓▒         ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒  ▒▒▒▓▓▓▓▓▓█▓██▓█▒▒▓▓▒▓███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                      
            ███████████████████████████▓▒       ▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒  ▒▒▒▓▓███▓███▓▓▒▒▓▒▒▓▓▓█████████▓▓▓▓▓▓▓▓▓▓▓▓▓                       
            ▓█████████████████████████████▓▒      ▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒  ▒▒▒▒▒▒▒▒▒▒▒▒  ▒▒▒▓█▓██▓▓██▓▒ ▒▓▒▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▒                       
            ███████████████████████████████▓▒      ▒▒▒▒ ▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒  ▒▒▓▓▓▓▓▓██▓▒ ▒▒▒▒▓▓▓▓▓███████████████▓▓                         
            █████████████████████████████████▓▒      ▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▒▒▒  ▒▒▒▒▒▒▒▒  ▒▒▓▓▒▒▓▓▓▓▒  ▒▒ ▒▒▓▓▓▓▓▓▓█▓█████████▓▒                         
            ▓███████████▓▓▓▓███████████████████▓▒      ▒▒▒▒▒▒▓█▒▒██▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ ▒▒▒▒▒▓▓▓▓▒  ▒▒ ▒▒▒▓▓▒▓▓▓▓▓▓▓████████▓                         
            ▓██████████▓▓▓▓▓▓████████████████████▓▒      ▒▒▒▒▒▓█▓███▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒  ▒▒▒▓▓▓▓▒▒ ▒▒▒ ▒▒▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓████▓                        
            ▓██████████▓▓▓▓▓▓▓▓▓█████████████████████▒     ▒▒▓▓▓▓▓███▓▓█▓▒ ▒▒▒▒▒▒▒▒▒▒▒  ▒▒▓▓▓▓▓▓ ▒▒▒ ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▓                       
            ▓██████████▓▓▓▓▓▓▓▓▓▓▓▓██▓▓█████████████████▒     ▒▒▓▓▓▓▒▓▓▓▓▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒  ▒▓▓▓▓▓▓ ▒▒▒ ▒▒▒▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓███▓▒                      
            ▒▓▓███████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████▒   ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▒ ▒▓▓▓▓▓▒▒▒▒▒ ▒▒▓▓▒▒▒▒▓▒▒▒▒▒▓▓▒▓███▓▓▒                     
        ▒▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████▓▒ ▒ ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▒▒▓▓▓▓▓▒▒▒▒▒ ▒▒▓▓▒▒▒▒▓▓▓▒▒▒▓▒▒▒▓██▓▓▓▒                    
        ▒▓▓▓▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████▒ ▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▓▓▓▓▒▒▒▒▒▒▒▓▓▒▓█▓▓███▒▓▒▒▒▓▒▓███▓▓▓                    
        ▒▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓█████████████████████▓▒ ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▓▒▓▓▓▓▓▓▓▒▒▓▓▓ ▒▒▒▒▒▓▓▓███▓▓███▓▓▒▒▒▒▒▒▓██▓▓▓▓                   
        ▒▓▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓███████████████████████▓▒▒▒▒▒▒▓▒▒▒▒▒▒▓▒▓▓▓▓▒▓▓▓▓▓▓▓▒▓▓▓ ▒▓▓▓▒▓▓███▓▓▓███▓▓▒▒▒▒▒▒▓██▓▓▓▓▒                  
    ▒▓▓▓▓▓████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓█████████████████████████▓▒▒▒▒▒▓▒▒▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒ ▓▓▓▓▓▓████▓██████▓▓▓▓▓▓▓▓██▓▓▓▓▓▒                 
    ▒▓▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓███████▓▓█████████████████▓▒▒▒▓▒▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▒▓▓▓▓▓▓████████▓█▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▒                
    ▒▓▓▓▓▓████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓██████▓▓▓███████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓█▓▓▓▓▓▓▓▓ ▒▓▓▓▓▓████████▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▓                
    ▒▓▓▓▓▓████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓██████▓▓█████████████████████▓▓▓▓▓▓█▓▓█▓▓▓██▓███▓▓▓██▓▒▓▓▓▓▓▓███████▓▓▓▓▓▓▓█▓█▓▓▓███▓▓▓█▓▓▓               
▓▓▓▓▓▓▓████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓██████████████████████████████▓▓▓▓▓▓█▓▓██▓███████████▓▒▓▓▓▓▓▓█████▓▓▓▓▓▓▓████████████▓▓█▓▓▓▒              
▓▓▓▓▓▓██████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓██████████████████████████████▓███▓▓████████████████▓▓▓▓▓▓▓▓██████▓██▓██████████████▓▓██▓▓▓▒             
▓▓▓▓▓▓███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓████████████████████████████████████████████████████▓█▓▓▓▓▓███████▓█████████████████▓▓▓█▓▓▓▓▒            
▓▓▓▓▓▓███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓███████████████████████████████████████████████████████▓▓▓▓██████████████████████████▓▓██▓▓▓▓▒           
▓▓▓▓▓▓████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▓███████████████████████████████████████████████████████▓▓▓▓██████████████████████████▓▓██▓▓▓▓▓▒          
▓▓▓▓██████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████████▓▓▓▓████████████████████████████▓██▓▓▓▓▓▓▒         
▓▓████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████████████████████▓▓▓▓████████████████████████████▓██▓▓▓▓▓▓▓▒        
▓█████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████████████▓▓▓█████████████████████████████▓██▓▓▓▓█▓▓▓▒       
██████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████████████▓▓▓▓████████████████████████████████▓▓▓████▓▓▒      
██████████████████████▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████████████████████████████████▓▓▓▓█████████████████████████████████▓▓█████▓▓      
██████████████████████▓▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████████████████▓▓▓███████████████████████████████████▓█████▓▓▓     
████████████████████████▓▓▓▓▓▓█████████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓    
█████████████████████████▓▓▓█████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████▓▓▓▓   
██████████████████████████▓███████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████████▓▓▓▓▒  
█████████████████████████████████████████████████████████████████████████████████████████▓█▓█▓▓▓▓▓▓▓███████████████████████████████████████████▓▓▓▓▓▒ 
████████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓██████████████████████████████████████████████▓▓▓▓▒
█████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████████████▓▓▓
██████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓██████▓▓▓▓█████████████████████████████████████████████▓
███████████████████████████████████████████████████████████████████████████████████▓██▓▓▓▓▓▓▓▓████████████████████████████████████████████████████████
████████████████████████████████████████████████████████████████████████████████████▓▓██████████▓█████████████████████████████████████████████████████";
            break;
        case "Dream Specter":
            enemyArt = @"
                                                                                                                                                    
                                                                        ▒                                                                           
                                                                        ▒▓▓▒                                                                        
                                                                ▒▒ ▒  ▒▓▓▓▓▒                                                                      
                                                                ▒▒▒  ▒███████▒                                                                     
                                                                ▒▒▒  ▓█████████▒                                                                    
                                                                ▒▒ ▓▓▓█▓▓▓█████                                                                    
                                                                ▒ ▒▒▓▓▒█▓██▒█████▓                                                                   
                                                            ▒  ▒██▒▒  ▓████████▒                                                                  
                                                                ▒███▓▒▒▓▓█████████                                                                  
                                                                ▒█████▒▒▓▓█████████▓                                                                 
                                                            ▓██████▓▒▒▓██████████▒                                                                
                                                            ▒▒█████████▓███████████▓                                                                
                                                            ▒████▒▓████████████████▓▒                                                               
                                                            ▓████▓██████████▓██████▓██▓▒                                                            
                                                        ▒▒▒██████████████████████▓█████▒                                                          
                                                        ▒▒▒▓███▓▓███████████████████████▓                                                         
                                                    ▒ ▒▒▒▓▒▒▓▒▓▓▓████████████████████████▓                                                        
                        ▒▒                            ▒▓ ▒▓▒▓▒▓▒▒▓▓████████▓█████████████████▓                                                       
                ▒     ▒     ▒                     ▓▒▓▒▒▓▒▓▒▓▒▒▓█████████▓▓█████████████████▓                                                      
                ▒ ▒▒ ▒▒▒    ▒                    ▒▓▒█▒▒▓▒█▒▓ ▓▓█████████▓▓██████████████████▒                                                     
                ▒▒▒▒▒▓▓▓▒   ▒                    ▒▓▒▓▓▒▓▓█▒▓ ▓▒▓▓██▓████▓▓███████████████████                                                     
                ▒▒▒▒ ▓█▓▒                        ▓█▒▓█▒▓▓▓▒▒▒▓▓▒▒▓▓▓▓▓██▓██▓▓██▓█████████████▓                                                    
                    ▒▒  ▓█▓▓                        ▓█▒▓█▓▓▓▓▒▒▒█▓▒▒▓▓▓▓▓▓▓▓██▒▓██▓███████████▓██▓                                                   
                    ▒▒▒▒▒▒▒                        ▒▒█▓▒█▓▓▓▓▒▒▒█▓▒▓▓▒▓▓▓▓▓▓██▒▓██▓███████████▓███▒                                                  
                    ▒ ▒                           ▒▒▒▓█▓▓▓▓▓▓▒▒▒█▓▒▓▓▒▓▓▓▓████▒▓██▓██████████▓▓████▒                                                 
                                                ▒▒▒▓██▓▓▓▓▓▓▒▒█▓▒▓█▓▓▓██████▒▓██▓▓█████████▓█████▓                                                 
                    ▒                            ▒▒ ▓███▓▓▓▓▓▒▒█▓▒▓▓▓▓▓██▓███▒▓██▓▓████████▓▓██████▓                                                
                    ▒▒▓▒▒▒▒▒▒                   ▒▓▒▒▓███▓▓▓▓▓▒▒█▓▒▓▓▓▓▓██▓███▒▓██▓▓████████▓▓▓▓▓▓▓▓▓▒                                               
                    ▒▒▓▓▓▒                  ▒▒▒▓█▓▓███▓▓▓▓▓▒▒█▒▓▒▓▓▓▓▓█▓███▓▓██▓▓████████▓██▓▓▓▓▓▓▓▒                                              
                    ▓████▒              ▒▒▒ ▓▓▓▓██▓▓██▓▓▓▓▓▒▒█▒▓▓▓▓▓█▓█▓▓██▓▓██▓▓█████████▓▓▓█▓████▒                                              
                    ▒███▓▒▒▒▒▒▒  ▒  ▒▒▒▒▓▓█▓▓▓█████▓██▓▓▓▓▓▒▒█▒▓▓▓▓▓█▓█▓▓██▓▓██▓▓████████▓▓▓▓▓█▓▓▓▓▓                                              
                    ▒▓███▓▓▓████▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▒▒█▒▒▓▓▓▓██▓▓▓██▓████▒████████▓▓▓▓▒▒▒▓▓█▓▒                                             
                    ▒▓████▓▓██████████████████████████▓▓▓▓█▒▒█▒▒▓▓▓███▓▓███▓████▒▓███████▓▓▒▒▒▓▓▓▓▓▓▓                                             
                        ▓█████▓█████████████████████████▓▓▓▓█▒▓█▒▒▓▓▓██▓█▓███▓▓███▒▓███████▓▓▒▒▒▒▒▓▓▓▓▓▒                                            
                        ▓█▓▓██▓▓█████████████████████████▓▓█▒▓█▒▒██▓██▒█▓███▓▓███▓▓███████▓▒▒▒▒▓▓█████▓                                            
                        ▒█▒▒▓▓▓▓▓▓███████████████████████▓▓█▒▓█▒▒██▓████▓██▓▒▓███▓▓███████▒▒▒▒▒▒▓██████▓                                           
                        ▓▒▒▒▓▓▓▓▓███████████████████████▓▓█▒▓█▒▒██▓████▓██▓▒▓███▓▓██████▓▓▓▒▒▒▒▓▓██▓███                                           
                    ▒▒▒ ▒▒▒▒▒▓▓▒▓███████████████████████▓██▒▓█▒▓██▓████▓██▓▒▓███▓▓███████▓▒▒▒▓▒▓▓██████                                           
                    ▒▒▒  ▒▒▒ ▓▓▓▓▓█████████████████████████▒▓█▒▓██▓████▓██▓▒▓████▓███████▓▓▓▓▒▒▓▓██████                                           
                    ▒▒▒▒▒▒▒▒  ▓▓▓▓█████████████████████████▓▓█▒▓██▓▓███▓██▓▒▓████▓███████▓▓█▒  ▓███████                                           
                        ▒▒▒▒▒▒▒  ▒▓▓▓▓████████████████████████▓▓█▒▓███▓███▓██▓▒▓████▓█████████▒▓▒ ▒▓██████▒                                          
                        ▒▒▒▒▒▒   ▒▓▓▓▓████████████████████████▓▓█▒▓███▓███▓██▓▒▓████▓████████▓▒██▒▓███████▓                                          
                        ▒▒▒▒▒▒▒▒▒▓▓▓▓▓███████████████████████▓▓▓▒████▓███▓██▓▒▓█████████████▒▓██▒▓████████                                          
                        ▒▓▒▒▒▒▓▓▓▓█▓█▓██████████████████████▓▓▓▒████▓███▓███▒▓████████████████▓▓█████████▓                                         
                        ▒▒▒▒▓▒▒▓▓▓▓▓██▓██████████████████████▓▓▒████▓██████▓▒▓███████████████▓▓███████████▒▒▒                                      
                        ▒▒▓▒▓▓▒▒▓▓▓▓██▓▓█████████████████████▓▒▓████▓██████▓▒▓████████████████████████████▓▒▒▒                                     
                        ▒▒▓▒▓▓▓▒▓▓▓▓██▓▓████████████████████▓▒▓████▓██████▓▒▓████████████████████████████▓▒▒▒▒                                    
                            ▒▓▒▒▓▓▓▒▒▓▓█████████████████████████▒████████████▓▓▓████████████████████████████▓▒▒▒                                     
                            ▒▓▒▒▓▒▒▒▓▓████████████████████████▓▒████████████▓▓▓█████████████████████████████▒▒▒                                     
                            ▒ ▒▓▒▓▓▓▒▒███████▓█████████████████▓▓█████▓█████▓▓▓▓█████████████████████████████▓▒▒▒▒▒                                  
                            ▒▒ ▒▒▒▓▓▓▒██████▓▓▓████████████████▓▓█████▓█████▓▓▓▓█████████████████████████████▓▒▒▒▒▒                                  
                            ▒▒▒  ▒▒▓▓▓██████▓█████████████████▒████████████▓▓▓▓██████████████████████████████▓▒▒▒▒                                  
                            ▒▒▒    ▒▒▓███████████████████████▒▓████████▓██▓▓▓▓███████████████████████████████▓▒▒▒  ▒                               
                            ▒▒       ▓██████████████████████▒▓██████▓█▓██▓▓▒▓▓███████████████████████████████▓▓▒  ▒▒                              
                                ▒▒▒▒▒▒██████████████████████▓▓█████▓▓█▓██▓▓▒▓▓█████████████████████████████████▓▒                                 
                                    ▒▒▒▒▒▒█████████████████████▓▓██████▓█▓██▓▓▒▓███████████████████████████████████▓▒      ▒▒▒                       
                                    ▒▒▒▒▒▒▓█████████████▓▓███▓▓▓██████▓████▓▓▒▓████████████████████████████████████▓▒▒   ▒▒▒▒▒                      
                                    ▒▒▒ ▒▒▒▓████████████▓▓███▓▓████████▓███▓▓▒▓█████████████████████████████████████▓▒   ▒ ▒▒▒                      
                                    ▒▓▒▒ ▒▒▒▓██████████▓▓▓███▓▓████████▓███▓▓▓▓███████████████████████████████████████▓▒▒▒▒▒                        
                                    ▓▓▒▒ ▒▒▒▓██████▓▓▓▓▓▓███▓▓▓▓██████▓███▓▓▒▓█████████████████████████████████████████▓▒   ▒▒                     
                                    ▒▓▓▒▒▒  ▒▓█████▓▓▓▓▓▓███▓▓▓███████▓████▓▓▓███████████████████████████████████████████▓▒▓▒▒▒                    
                                    ▒▒▒▒▒   ▒▓██▓▓█▓▓▓▓▓███▓▓▓███████▓████▓▓▓█████████████████████████████████████████████▓▓▓▓▒                   
                                        ▒▒▒▒   ▒▓▓▓▓██▓▓▓▓███▓▓▓████████████▓▓▓▓██████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒             
                                        ▒▒▒   ▒▓▓▓███▓▓▓███▓▓█████████████▓▓▓▓██████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▒             
                                                ▓▓▓███▓▓▓███▓▓█████████████▓▓▓▓████████████████▓███████████████████████████████████▓▒              
                                                ▓▓███▓▓▓███▓▓██████████████▓▓▓████████████████ █████████████████████▓▓▓▓████████▓▒                
                                                ▒▓███▓▓▓███▓▓██████████████▓▓▓████████████████ ▓████████████████████▒    ▓▓▓▒                     
                                                    ▓▓██▓▓▓███▓▓██████████████▓▓▓████████████████▒ ████████████████████▒                             
                                                ▒▓▓██▓▓▓████████████████████▓▓████████████████▒ ▒▒▓▒▒▓█████████████▓                              
                                                ▒▓▓▓▓▓▓▓████████████████████▓█▓███████████████▓       ▒███████████▒                               
                                                ▒▓▓▓▓▓▓█████████████████████▓█▓███████████████▓        ▒▓▓▒▓▓██▓▒                                 
                                                ▒▓▓▓▓▓▓█████████████████████▓█▓███████████████▓                                                   
                                                ▒▓▓▓▓▓▓█████████████████████▓█▓████████████████▒                                                  
                                                ▒▒▓▓▓▓▓█████████████████████▓██▓███████████████▓                                                  
                                                ▒▒▓▓▓▓▓▓██████████████████▓▓▓██▓██████████████▓                                                   
                                                        ▒▓▓▓█████▓▓████████▓▓▓▓▓█▓▓████████████▒                                                     
                                                        ▒▒▓▓▓▓    ▒     ▒▒▓▓▓▓▓▓▓▓████████████▒                                                     
                                                                        ▓▓▓▓▓▓▓▓████████████▒                                                     
                                                                        ▒▓▓▓▓▓▓▓███████████▓▒                                                     
                                                                            ▒████████████████▓▒                                                      
                                                                            ██████████████▓▓▒                                                       
                                                                            █████████████▓▒                                                         ";
            break;
        case "Painforged Emissary":
            enemyArt = @"
                                                                                                                                                    
                                                                                                                                                    
                                                                    ▒▒▒▒▒▒▒▒▒                                                                        
                                                                ▒▒▓▓▒▓▓▓▒▒▒▒▒                                                                     
                                                                ▓▓▓▓▓██████▓▓▓▓                                                                    
                                                                ▓▓██████████▓▓▓▓                                                                   
                                                                ▓████████████▓▓▓▓                                                                   
                                                                ▓████████████████▒                                                                  
                                                                ▓███████████████▓▒                                                                  
                                                                ▓████████████████▓▒                                                                 
                                                                ▓▓████████████████▓▒                                                                
                                                                ▓█████████████████▓▓                                                                
                                                                ▒▓██████████████████▓▒                                                               
                                                            ▒▓████████████████████▓▓▓                                                              
                                                        ▒▒▓▓███████████████████████▓▓▓▒                                                            
                                                        ▒▓▓▓▓▓█████████████████████████▓▓▓▓▒                                                         
                                                        ██▓▓▓▓▓█████████████████████████▓███▒                                                        
                                                        ████████████████████████████████████▓                                                        
                                                    ▒█████████████████▓▓█████████████████▓▒                                                       
                                                    ▒████████▓█▓▓▓███████▓▓▓▓▓▓██████████▓▓▓                                                      
                                                    ▒▓▓█████████▓▓▓▓▓▓███▓▓▓▓▓▓▓▓██████████▒▒▓                                                      
                                                    ▓▓▓▓██████████▓▓▓▓▓████▓▓▓▓▓▓▓▓▓████████▓▓▒                                                      
                                                ▒▓▒▓██████████▓▓▓▓▓████████████▓█████████▓▒▒                                                      
                                                ▓▓ ██████████████████████████████████████▓                                                        
                                                ▓█████████████████████████████████████████                                                        
                                                █▓████████████████████████████████████████▒                                                       
                                                ▒█▓▓███████████████████████████████████████▓                                                       
                                                ▓▓██████████████████████████████████████████                                                       
                                                ▓███████████████████████████████████████████▓                                                      
                                                ▒████████████████████████████████████████████▒                                                     
                                                ▒███████████▓▓█████████████████████████████████▒                                                    
                                                ███████▓█▓▒█▓███████████████████████████████████▒                                                   
                                                ▓██████▓▒█▒▒▓█████████████████████████████████████                                                   
                                            ▓██████▒▒▓▒█▒██████████████████████████████████████▒                                                  
                                            ▒████▓▓▓ ▓▓▒ ▓█████████████████████████████████▓█████                                                  
                                            ▒████▒ ▓█▒▓▓  ▓█████████████████████████████████▓▒████▓                                                 
                                            ███▓   ▓█▓▓▓  █████████████████████████████████▓█  ▓███▓                                                
                                            ▓██▒   ▓█▒▓▓▒ ▒███████████████████████████████▒▒▒█  ▒▒▓██▒                                               
                                        ▓██  ▒ ▓▓ ▒▓▒▒ ████████████████████████████████▒ ▒█     ▒██▒                                              
                                        ▓██   ▒█▒   █  ▒██████████████████████████████▓█▓  ▓▓     ▒██                                              
                                        ███▒   ██   ▒▓  ▓█████████████████████████████████   █ ▒    ▓██▒                                            
                                        ▓███▒ ▒▓ ▓▒  ▓▒  ████████████████████████████████▓█▓ ▒█▓▓    ▓▓██▒                                           
                                    █████▒ ▒▒ ▓█  ▓▓▒▒███████████████████████████████▓▒▒█▓▒▓▓▒    ▓▒███▓                                          
                                    ██████   ▒▒▓██▓▒▒▓████████████████████████████████▓▒▒▓▓ ▓▓     ▒▒████                                          
                                    ███▓▒█▒  ▒▓  ▓█▒▒▒▓█████████████████████████████████▓▒ ▓▒▓▒  ▒▒   █████                                         
                                    ██▓ █▓   ▒   ▓▓ ▒▒▓█████████████████████████████████▓▓▒█▒▒▒▒      ▒▓████                                        
                                    ▒██▓▒▓   ▒    ▓▓▒  ▓█████████████████████████████████▓█▓▓█▓▓     ▒   █▓██▓                                       
                                    ▓█▓▓▒   ▒    ▓█▓  ██████████████████████████████████▓▓█████     ▒   ▒███▒                                       
                                    █▓▓▒    ▒▒ ▒▓█▒▒▒███████████████████████████████████▓█████    ▒▒    ███▓                                       
                                    ▒    ▒▒    ▓▓▓▒▓██████████████████████████████████▓▒▓██▒█▓▒▒▒▒▒▒ ▒▓▓█▓                                        
                                                ▒▓▓▓███████████████████████████████████▒▓█▒▓▒▓▒   ▒▒▒   ▓▒▒                                        
                                                ▒▓▓████████████████████████████████████▓▓█ ▓▒  ▒    ▒▒                                             
                                                ▓██████████████████████████████████████▒▓▒▓▒                                                      
                                                ▒███████████████████████████████████████▒▒▓                                                        
                                                ▓███████████████████████████████████████▒▓▒                                                        
                                                ▒█████████████████████████████████████████▒                                                         
                                                ▒██████████████████████████████████████████▓                                                         
                                            ▒████████████████████████████████████████████                                                         
                                            ▒█████████████████████████████████████████████▓                                                        
                                            ▓██████████████████████████████████████████████▒                                                       
                                            ▒███████████████████████████████████████████████▓                                                       
                                            █████████████████████████████████████████████████▒                                                      
                                            ███████████████████████████████████████████████████                                                      
                                        ▓████████████████████████████████████████████████████                                                     
                                        ▒██████████████████████████████████████████████████████▓                                                   
                                        █████████████████████████████████████████████████████████                                                  
                                        ███████████████████████████████████████████████████████████▒                                                
                                        █████████████████████████████████████████████████████████████▒                                               
                                    ▓███████████████████████████████████████████████████████████████████▓▒▒▒                                        
                                ▒▒████████████████████████████████████████████████████████████████████████████▓▓▒▒▒                                 
                ▒▒▒▒▒▒▒▓▓▓▓█████████████████████████████████████████████████████████████████████████████████████████▓▓▒▒▒                          
        ▒▒▒▓▓▓███████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▒▒                     
▒▓█████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▒▒           
▓████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▒       
████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▒▒▒████████████████▓███████████████▓▒    
████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▒▒ ▓██▒▓▓▒▓▓▓▓▒▓▓▓▓▒▒▓▓▓▓▓▓▒▒▓▓███████▓  
████████████████████████████████████████████████████████████████████████████████████████████████████████████▒▓█▓▓████████▓██▓█▓████▓██▓█▓████████████▒
██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
█████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████▒";
            break;
        case "Bleeding Orchardgeist":
            enemyArt = @"
                                                                                                                                                    
                                                                                                                                                    
                                                                    ▒▒ ▒▒▒                                                                           
                                                                ▒███▓▓█▓▒▒▒▒▓▓▒▒                                                                    
                                                            ▓████▓▓▒▓▓▒▒▒▒▒▒▓▓▒                                                                   
                                                            ▓████▓▓▒▓█▓▓▒▒▒▓▓▓▓▓▓                                                                  
                                                            ▒████▓▓▓█▓████▓▒▓▓▓▓▓▓▓                                                                 
                                                            ▓███████████████▓▓██▓▓▓▓▒                                                               
                                                            ▒███████▓▒▒▒▒▒▒▓▓████████▓▒                                                              
                                                        ▒████████▓▒▒▒▒▒▒▒▓██▓██████▓                                                              
                                                            ████████▓▒▒▒▒▒▒▒▒████▓▓▓██▓▒                                                             
                                                        ▒▓███████████▓▓▓████▓█▓▓███▓▓                                                             
                                                        ▒▓███████████▓▒▒▓▓▓▓▓█▓██████▓                                                            
                                                        ▓████████▓▓▓▓▓▒▒▒▓▓▓██████████▓▒▒                                                         
                                                        ▒██████████▓▓▓██▓█▓▓██████▓▓▓████▓▓▒                                                       
                                                    ▒▒▓███████████████▓▓▓████████▓▓▓▓▓▓▓█▒                                                        
                                                    ▒▓███████████████████▓████▓████▓▒▒▒▓▓▓▒▒▒                                                      
                                                    ▒███▓█████████████▓▓▓███▓▓████▓▒▓████▒▒▒▒▒▒▒▒▒                                                
                                                    ▒▓█████▓████████████████▓▓█▓▓█▓█████▓▓▓▓▒▒▒▒▒▒▒▒▓▓                                             
                                                ▒▓█████████████████████████▓▓█▒▒▓▓███▓▓▓▓▓▓▒▓▒▒▒▒▒▒▓▓▓                                             
                                            ▒▓▓▓▓▓████████████████▓▓▓█████▓▓▓██▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒                                            
                                            ▓▓▓▓▓▓▓▓▓███████████████▓▓▓██▓▓▓▓▓█▓█▓▓▓▓▓▓▓██▓▓█▓▓▓▓█▓▓██                                            
                                            █████▓▓█████████████████▓▓▓▓█▓▓▒▒██▓▓██▓▓▓▓▓▓▓▓▓█▓████████▒                                           
                                            ██████████████████████▓▓▓▒▒▒▓▒▒▒▒▒▓█▓▓▓█▓▓▓▓▓▓▓███████████▓                                           
                                            ▒█████████████████████████▓▒▒▒▒   ▒▒▓▓▓██▓▓▓▓▓▓▓███████████▓                                           
                                            ▒███████████████████████▓▓▒▒▒▒▒▒▒▓███▓███▓▓▓▓▓▓█████████████                                           
                                            ████████████████████████▓▓▒▒▒▒▒▒▓█████████▓▓▓██████████████▒                                          
                                            ▒█████████████████████████▓▓▒▓▓▒▓▓▓█████████▓██████████████▓▓▒                                         
                                            ▒█████████████████████████▓▓▓▓▓▓▓██████████████████████████▓▓▓▒                                        
                                            █████████████████████████████▓▓███████████████████████████▓▓▓▓█                                        
                                            ████████████████████████████████████████████████████▓██████▓▓▓▓██                                       
                                            ▒███████████████████████████████████████████████████▒ ▓███████▓▓██▒                                      
                                            ▓███████████▓███████████████████████████████████████   ▓███████████▒                                     
                                            ███████████▒ ▒█████████████████████████████████████▓    ▓██████████▓▒                                    
                                        ▓██████████▓   █████████████████████████████████████      ▓████████▓▓█▒                                   
                                        ▒██████████▓     ███████████████████████████████████▓       ▓██████▓▓▓▓█▒                                  
                                        ▒██████████▓      ▓██████████████████████████████████         █████▓▓▓▓▓▓█ ▒                                
                                        ███████████▒      ▒██████████████████████████████████▒        ▒███▓▓▓▓▓▓▓███                                
                                    ▓ ███████████▒       ▒██████████████████████████████████▓         ███▓█▓▓▓▓▓███                                
                                    ▒█████████████        ▓███████████████████████████████████▒        ▓████▓▓▓▓▓███                                
                                    ▒████████████         █████████████████████████████████████       ▒▓███▓▓▓▓▓▓███                                
                                    ███████████▓▒▒      ▓██████████████████████████████████████        ▓████▓▓▓▓▓██                                
                                    ▒███████████▓       ▓████████████████████████████████████████         ▓███▓▓▓▓██                                
                                    ▓█████████▒         █████████████████████████████████████████▒         ▒██▓▓▓▓▓█                                
                                    ████████▓          ▓█████████████████████████████████████████▒           ██▓▓▓▓█▒                               
                                    ███████▒           ██████████████████████████████████████████▓            ██▓▓▓█▒                               
                                    ▒██████▒           ▒███████████████████████████████████████████            ▓█▓▓▓█▓                               
                                    ██████▒            █████████████████████████████████████████████           ▒▓▓▓███                               
                                ▒█████▓            ▒█████████████████████████████████████████████▓         ▒▓▒▓▓▓██                               
                                ▓█████             ▓██████████████████████████████████████████████        ▓▓▓▓▓▓▓▓▓                               
                                ▓█████▒            ███████████▓▓▓▓█████████████████████████████████      ▒▓▓▓▓▓▓▓▓▓                               
                                ▓██████▒          ▓████████▓▓▓▓▓▓▓▓████████████████████████████████▓     ▒▓▓██▓▓▓▓▓                               
                                ▓███████          ██████████▓▓▓▓▓▓▓▓████████████████████████████████▓    ▒▓▓  ▓▓▓█▓▒                              
                                ▓███████         ▓█████████▓▓▓▓▓▓▓▓▓█████████████████████████████████▒   ▒▓▒  ▓▓███▓                              
                                ████▒▒██         ███████████▓▓▓▓▓▓▓███████████████████████████████████▒   ▒   ▓▓██▓                               
                                    ███  ██▓       ██████████████▓▓█▓▓████████████████████████████████████    ▒▓████▓                                
                                    ▒███▒▒▓▓      ▓████████████████▓███████████████████████████████████████  ▒███▓▒                                  
                                    ▒▓███▓       ██████████████▓▓▓▓▓▓▓▓███████████████████████████████████▓                                         
                                        ▒▓▓      ▓███████████████▓▓▓▓▓▓▓████████████████████████████████████▒                                        
                                                ████████████████▓▓▓▓▓▓▓█████████████████████████████████████▓                                       
                                                ▓█████████████████▓▓▓▓▓▓██████████████████████████████████████▒                                      
                                            ▒████████████████████▓▓▓▓███████████████████████████████████████                                      
                                            ▓███████████████████▓▓▓▓▓████▓▓██████████████████████████████████                                     
                                            ▒██████████████████▓▓▓▓▓▓▓███▓▓▒▓██████████████████████████████████                                    
                                            ████████████████████▓▓▓▓▓▓███▓▒▒▒███████████████████████████████████                                   
                                            ▓███████████████████▓▓▓▓▓▓████▒   ████████████████████████████████████                                  
                                            ████████████████████████▓█████▒   ████████████████████████████████████▓                                 
                                            ▓██████████████████████████████▓   █████████████████████████████████████▓                                
                                            ███████████████████████████████▓   ▓█████████████████████████████████████▒                               
                                        ▒███████████████████████████████     ██████████████████████████████████████                               
                                        ███████████████████████████████▓     ██████████████████████████████████████▓                              
                                        ███████████████████████████████▓    ▒█████████████████████▓▓████████████████                              
                                        ▒██████████▓█████████████████████     ████████████▒   ▒▓██    ▒▓██████▒▒▒████▓                             
                                        ▓█████████▒ ▒█▓  ▒▓██████████████    ▓████████████       ▓       ███▓     ▓███                             
                                        ▓█████████▒  ▓     ██████████████▓   ████████████▓               ▓██       ▓██                             ";
            break;
        case "Weeping Rosecutter":
            enemyArt = @"
                                                                                                                                                    
                                                                ▒   ▓▓▓▓▓▒   ▒                                                                      
                                                            ▒▒ ▒▒ ▓█████████▓ ▒  ▒                                                                  
                                                            ▒▒▒▒▒▒█████████████▓▒▒▒                                                                  
                                                        ▒ ▓▒▒▒▒█████▓▓▓██████▓▒▒▒                                                                 
                                                            ▒▓▒▓▒▓████▓▓▓▓███████▓▒▓▒                                                                
                                                        ▒ ▓▓▓▒▒▓████▓▓▓▓▓███████▓▓▒                                                                
                                                        ▒▒▓▓▓▒▒██████▓▓█████████▓█▓▒                                                               
                                                        ▒▓▓▓▓ ▓██████▓██████████▓█▓▒▒                                                              
                                                        ▒▒▓▓▓▓ ███████▓▓█████████▓██▒▒                                                              
                                                        ▒▓▓▓▓▒▒██████▓▒▓▓▓███████▓██▓▒▒                                                             
                                                        ▒▓▓▓▓▒▓██████▓▒▒▓████████▓██▓▒▒▒                                                            
                                                        ▒▒▓▓▓▓ ▓████▓▓▓███▓▓███████▓█▓▓▒▒▒                                                           
                                                        ▒▒▓▓▓▓ ████████████████████▓█▓▓▒▒▒▒                                                          
                                                    ▒▒▓▓▓▓▒▒███████▓████████████▓██▓▒▒▒▒                                                          
                                                    ▒▒▓▓▓▓▒▓████████▓███████████▓██▓▓▒▒▒▒                                                         
                                                    ▒ ▒▒▒▓▓▓▓▒▓████████████████████▓██▓▓▒▒▒▒▒                                                        
                                                    ▒▒▓▓▓▓▒▒█████████████████████▓███▓▓▒▒ ▒▒                                                       
                                                ▒ ▒▒▓▓▓▓▓▒▒█████████████████████▓▓██▓▓▒▒▒▒▒                                                       
                                                ▒▒ ▒▒▓▓▓▓▓▒▓█████████████████████▓▓██▓▓▒▒▒ ▒▒▒                                                     
                                                ▒▒▒ ▒▓▓▓▓▓▓▓▓██████████████████████▓███▓▓▒▒▒▒▒▒▒                                                    
                                                ▒▒ ▒▒▓▓▓▓▓▓▓▓██████████████████████▓███▓▓▒▓▓ ▒▒▒▒                                                   
                                                ▒▒▓ ▒▓▓▓▓▓▓▓▓███████████████████████▓▓██▓▓▒▓▓▒ ▒▒▒▒                                                  
                                            ▒  ▒▓▒▒▒▓▓▓▓▓▓▓▓███████████████████████▓▓██▓▓▒▓▓▓▒▒▒▒▒▒▒                                                
                                            ▒▒▒▓▓▒▒▒▓▓▓▓▓▓▓▓███████████▓▓███████████▓▓▓█▓▓▒▓▓▓▒▓▓▓▒▒▒                                               
                                            ▒▒▒▒▓▓ ▒▓▓▓▓▓▓▓▓▓█████████▓▓▓▓▓▓█████████▓▓██▓▓▒▓▓▓▒▒▓▓▓▒▒                                               
                                            ▒▒ ▒▓▒▒▒▓▓▓▓▓▓▒▓██████████▓▓▓▓▓▓▓████████▓▓█▓▓▓▓▒▓▓▒▒▓▓▓▓▒                                               
                                            ▒▒ ▒▓▒▒▒▓▒▓▓▓▓▒▓██████████▓▓▓▒▒▒▓▓███████▓▓█▓▓▓▓▒▓▓▓▒▓▓▓▓▒                                               
                                        ▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▒▓██████████▓▓▓▒▒▒▒▒▓▓██████▓▓██▓▓▓▒▓▓▒▒▓▓▒▒▒                                              
                                        ▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓███████████▓▓▓▒▒▒▒▒▒▓██████▓▓▓█▓▓▓▒▓▓▒▒▓▓▓▓▒                                              
                                        ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓███████████▓▓▓▒▒▒▒▒▒▓▓█████▓▓▓▓▓▓▓▓▒▓▒▒▓▓▒▒▒                                              
                                        ▒▒ ▒▒▒▒▓▒▓▓▓▓▓▓▓█████████▓▓▓▒▒▒▒▒▒▒▒▒▓█████▓▓▓▓▓▓▓▓▒▓▓▒▒▓▒▒▒                                              
                                        ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓████████▓▓▓▒▓▒▒▒▒▒▒▒▒▒▒▓█████▓▓▓▓▓▓▓▓▒▓▓▒▓▒▒▒                                              
                                        ▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓██████▓▓▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓█████▓▓▓▓▓▓▓▓▒▒▓▒▓▒▒▒                                              
                                        ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▒███████▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓█████▓▓▓▓▓▓▓▒▒▒▓▒▓▒▒▒                                              
                                        ▒▒▒▒▒ ▒▒▓▓▓▓▓▓▓▓█████▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓██████▓▓▓▓▓▓▒▒▒▒▒▓▒▒▒                                              
                                        ▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓█████▓▓▓▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓████▓▓▓▓▓▓▓▒▒▒▓▒▒▒▒                                              
                                        ▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓████▓▓▓▓▓▓▓▓▓▒▓▓▒▓▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▓▓▒▒▒▓▒▒▒▒▒                                             
                                        ▒▒▒▒▓ ▒▒▓▓▓▓▓▓▓▒▓███▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓██▓▒▓▓▓▓▓▒▒▒▒▒▒▓▒▒                                             
                                        ▒▒▒▒▒ ▒▒▓▓▓▓▓▓▓▒▓██▓▓▓▓▓▒▓▓▒▒▒▒▒▒▒▓▓▒▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▒▒▒▒▒▒▓▒▒                                             
                                        ▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▒▓██▓▓▓▓▓▒▓▒▒▓▓▒▒▒▒▓▓▒▓▓▒▓▓▓▓▓▓▓█▓▒▒▓▓▓▒▒▒▒▒▒▓▒▒                                             
                                        ▒▒▒▒▒▒▒▓▓▓▓▓▓▓▒▒▓█▓▓▓▓▓▓▒▒▒▒▓▒▒▒▓▒▓▓▒▓▓▒▓▓▓▓▓▓▓▓▓▒▒▓▒▓▓▒▒▒▒▒▒▓▒▒                                            
                                    ▒ ▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▓█▓▓▓▓▓▓▓▒▒▒▓▒▒▓▓▒▓▓▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓▒▒▓▒▒▓▒▒                                            
                                    ▒ ▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▓█▓▓▓▓▓▓▓▒▒▓▓▒▒▓▓▒▓▓▒▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▒▒▒▒▒▓▒▒                                            
                                    ▒ ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▒▒▓▓▓▒▓▓▓▒▓▓▒▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▓▒▒                                            
                                    ▒ ▒▓▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▒▓▓▓▒▓▓▓▒▓▓▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▒▒▓▒                                            
                                    ▒▒▒▓▒▒▒▒▒▓▒▓▒▓▒▒▒▒▒▒▒▒▒▓▓▒▓▒▓▓▓▒▓▓▓▒▓▓▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▒▒▒▒▒▓▒▒                                           
                                    ▒▒▒▓▒▒▒▒▒▓▒▒▒▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▒▓▓▓▒▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▓▒▒                                           
                                    ▒▒▒▒▓▓▒▒▒▒▓▓▒▓▓▒▒▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▒▒▒▒▓▒▒                                           
                                    ▒▒▒▓▓▓▒▓▒▒▓▓▓▓▓▒▒▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▓▒▒                                           
                                    ▒▒▒▓▓▓▒▓▒▒▓▒▓▓▓▒▒▓▓▒▒▒▒▒▒▒▒▒▓█▓▓▓▓█▓▓▓▓▓▓▓▓█▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▒▒                                           
                                    ▒▒▓▓▓▒▒▒▒▓▓▓▓▓▒▓▓▓▒▒▒▒▒▒▒▒▒▓█▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▓▓▒                                           
                                    ▒▒▒▒▓▓▒▒▒▒▓▓▓▓▓▒▓█▓▒ ▒▓▓▓▓▓▓▓▓███▓█▓▓▓█▓▓▓▓▓▓▓████▓▓█▓▓▓▓▓▓▓▓▓▒▒▒▓▓▒▒                                          
                                    ▒▒▒▒▓▓▒▓▒▒▒▓▓▓▓▒▓██████████▓▓▓▓████▓▓▓█▓█▓▓▓██████████▓▓▓▓▓▓▓▓▒▒▒▓▓▒▒                                          
                                    ▒▒▒▒▒▓▓▒▓▒▒▒▓▓▓▓▒███████████████████▓▓▓██▓▓▓███████████▓▓▓▓▓▓▓▓▒▒▒▓▒▒▒                                          
                                    ▒▒▒▒▒▓▓▒▓▒▒▓▓▓▓▓▒███████████████████▓███▓▓█████████████▓▓▓▓▓▓▓▓▒▒▒▓▒▒▒                                          
                                    ▒▒▒▒▒▓▓▒▓▒▒▓▓▓▓▒▒████████████████████▓▓████████████████▓▓▓▓▓▓▓▓▓▒▒▓▒▒▒                                          
                                    ▒▒▒▒▒▓▓▒▓▒▒▓▓▓▓▒▓█▓██▓██▓▓████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒                                          
                                    ▒▒▒▒▒▒▓▓▒▓▒▒▓▓▓▒▒▓████▓█▓▓▓███████████████████████████▓█▓▓▓▓▓▓▓▓▓▓▒▒▓▒▒                                          
                                    ▒▒▒▒▒▒▓▓▒▒▒▓▓▓▓▒▒█▓██▓▓█▓▓▓███████████████████████████▓█▓▓▓▓▓▓▓▓▓▒▒▒▓▒▒                                          
                                    ▒▒▒▒▒▒▓▓▒▒▒▓▓▓▓▒▒█▓██▓▓█▓▓▓███████████████████████████▓█▓▓▓▓▓▓▓▓▓▒▓▒▒▒▒                                          
                                    ▒▒▒▒▒▒▒▓▒▒▓▒▒▓▒▒▓█▓██▓▓█▓▓▓████████▓█▓▓▓████████▓█▓███▓█▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒                                          
                                    ▒▒▒▒▒▒▒▓▒▒▓▒▒▓▓▒▓█▓██▓▓█▓▓▓▓▓█▓▓█▓█▓▓▓▓▓████████▓▓▓▓█▓▓█▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒                                          
                                    ▒▒▒▒▒▒▒▓▒▒▓▒▓▓▓▒▓█▓██▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓████▓██▓▓▓▓█▓▓█▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒                                          
                                    ▒▒▒▒▒▒▒▓▒▒▓▒▓▓▒▒██▓██▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓█▓██▓▓█▓▓▓▓█▓▓█▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒                                          
                                    ▒▒▒▒▒▒▒▓▒▒▓▒▓▓▒▒██▓██▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓█▓██▓▓█▓▓▓▓█▓▓█▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▓▒▒▓▓▓▓▒▓▓█▓██▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓█▓▓█▓▓█▓▓▓▓█▓▓█▓▓▓▓▓▓▓▒▓▓▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▓▒▒▓▓▓▒ ▓██▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓█▓▓█▓▓▓▓█▓▓█▓▓▓▓▓▓▓▒▒▓▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▓▒▒▓▓▒▒▒▓██▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓█▓▓█▓▓▓▓█▓▓▓▓▓▓▒▓▓▓▒▒▓▒▒▒▒                                           
                                ▒▒▒▒▒▒▒▒▓▓▒▓▓▒▒▒▓██▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓█▓▓█▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▒▓▒▒▒▒                                           
                                ▒▒▒▒▒▒▒▒▓▓▒▓▓▒▒▒▓██▓██▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓█▓▓█▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▒▒▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▓▓▒▓▓▒▒▓▓██▓██▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓█▓▓█▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▒▒▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▓▓▒▓▓▒ ▓▓██▓██▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▒▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▒▒▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▒▓▓▒▓▓▒▒▓▓██▓██▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▒▒▒▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▓▓████▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▒▒▓▓▓▒ ▒▓▓▓█▓██▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                                          
                                ▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▓▓██▓▓▓▓▓▓▒▓▓▓▓▓█▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒                                         
                                ▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▓▓██▓▓▓▓▓▓▒▓▓▓▓▓█▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒                                         
                                ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▒▓▓▓▓▓█▓▓▓█▓▓▓▓█▒▓▓▓▓▓▓▓█▓▓▓▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                                         
                                ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▒▓▓▓▓▓█▓▓▓█▓▓▓▓█▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                                         
                                ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▒▓▓▓▓▓█▓▓▓█▓▓▓▓█▓▓▓▓▓█▓▓█▓▓▓▓▓▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                                         
                                ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▒▒▓▓▓▓▓█▓▓▓█▓▓▓▓█▓▓▓▓▓█▓▓█▓█▓▓▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                                        
                                ▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▒▒▓▓▓▓▓█▓▓▓█▓▓▓▓█▓▓▓▓▓▓▓▓█▓█▓▓▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                                        
                                ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓███▓▓▓▓▒▒▓▓▓▓▓█▓▓▓██▓▓▓█▓▓▒▓▓█▓▓█▓█▓▓▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                       
                                ▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▒▒▓▓▓▓▓█▓▓▓██▓▓▓█▓▓▓▓▓▓▓▓█▓█▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▒▒▒▒▒▒▒▒                                       
                            ▒ ▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓████▓▓▓▓▒▒▓▓▓▓▓█▓▓▓███▓▓█▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▒▒▒▒▒▒▒▒▒                                      
                                ▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓█▓███▓▓▓▓▒▒▓▓▓▓▓█▓▓▓███▓▓█▓█▓▓▒▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▒▒▒▒▒▒▒▒▒▒                                     
                            ▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓█████▓▓▓▓▒▒▓▓▓▓▓█▓▓▓███▓▓█▓▓▓▒▒▓▓▓███▓▓▓▓▒▓▓▓▓▓▓▓▓▒▒▓▒▒▒▒▒▒▒▒▒                                      
                            ▒▒▒▒▒▒▒▒▒▒▓▒▒▓▒▓▓▓▓█████▓▓▓▓▒▓▓▓▓▓▓█▓▓▓███▓▓██▓▓▓▒▓▓▓█▓█▓▓▓▓▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒                                      
                            ▒▒▒▒▒▒▒▒▓▒▓▒▒▒▓▓▓▓▓█████▓▓▓▓▒▓▓▓▓▓▓█▓▓▓██▓▓▓▓█▓▓▓▒▓▓▓█▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                                       
                            ▒▒▒▒▒▒▒▒▒▒▓▒▒▒▓▓▓▓▓▓████▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓██▓▓▓█▓▓▓▒▒▓▓█▓█▓▓▓▓▓▓▓▓▒▓▒▓▓▓▒▒▒▒▒▒▒▒▒▒                                       
                            ▒▒▒▒ ▒▒▒▒▒▓▒▒▓▓▓▓▓▓▓███▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓█▓▓▓▓▒▓▓▓▓█▓▓▓▓▓▓▓▒▒▓▓▒▒▓▒▒▒▒▒▒▒▒                                         
                            ▒▒▒▒▒▒▒▒▒▒▒▓▒▒▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▓█▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                       
                            ▒▒▒   ▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓█▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓█▓▓█▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                      
                                ▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                      
                                ▒▒▒ ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒  ▒                                  
                    ▒▒ ▒         ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒  ▒▒▒▒         ▒                          
                ▒▒▒▒▒▒▒▒▒▒ ▒ ▒▒▒▒▒▒▒   ▒▒▒▒▒▒▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒    ▒           ▒      ▒                  
                ▒▒▒▒▒▒▒▒      ▒▒        ▒▒▒▒▒▓▒▓▓▒▓▓▒▒▒▒▒▒▒▒▓▓▓▒▒▓▒▒▓▒▒▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒               ▒▒▒▒▒▒▒▒▒▒                
                            ▒▒▒        ▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒  ▒▒▒▒▒▒▒▒▒                ▒▒▒▒▒▒▒▒▒                
                                ▒▒▒     ▒▒▒    ▒▒▒▒  ▒  ▒▒ ▒▒▒▒▓▓▒▒     ▒▒▒▒▒▒▒     ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                 
                                                        ▒ ▒▒  ▒▒▒▒                                     ▒ ▒  ▒▒       ▒▒▒▒▒▒▒▒                        ";
            break;
        case "Fiery Treant":
            enemyArt = @"
                                                                                            ▒       ▒  ▒   ▒ ▒  ▒                                    
                                                                                            ▒     ▒   ▒  ▒▒                                        
                                                            ▒                                ▓█▓  ▒▓    ▒▒  ▒ ▒                                     
                                                            ▒        ▒▒▒                    ██▒▒▓█▒  ▒   ▓▓  ▒                                     
                                                            ▓        ▒  ▓▒▓▒                █▓   ██▓    ▓▓▒▒▒                                      
                                                            ▓        ▒   ▒█▓▒            ▒██▒▒  ███▓▒▓██▓▓▒                                       
                                                            ▒▓ ▒      ▒  ▒▓▓▓▓▓▒▒       ▓▒ █ █▒▓█▓████▓▓█▓                                        
                                                                ▓▓▓▓     ▒▒ ▓█ ▒  ▒▓▒    ▒▒  ▒▒▒▓██▓▓████▓██▒                                        
                                                                ▒▓███▓▒█  ▓█▓█▒ ▒▒▓▓     ▒ ▓▓ ▒▓▒██▓▒▒▒▓███▓▓                                        
                                                                ▓▒ ▒▒███▓▓███▓▓██▓      ▒▓▓▓█████▒▒▒▒▓▓██▓▒▓▒                                      
                                                        ▒▒  ▒▒▒▓█▓▓▓▓ ▓████▓█▓  ▒           ████▓▓▓▓▒▓███████▓                                    
                                                            ▓   ▒▓████▒▒▓███▒▒▓  ▒          ▒▒▓███▓▒▓▒▒████▓▓▒                                      
                                                            ▓   ▓████████▓▓▒   ▒         ▒▒  ▒█▓█▓▓▓████▓▓▒                                        
                                                            ▓█▓▒▒  ▒▓███▓▓▒  ▒  ▒▒▓▓▓▒  ▒   ██▓████████▓▓▓▓▒                                      
                                                            ▒█▒      ▒▒▓██▓█▒ ▓▓▒▓█████▓▒█▒ ▓▓████▓▓▒       ▓                                      
                                                        ▒▓▓▓▒▒        ▒▓██▓▓▓▓████▓▓▓████▓▓▓██▓▓▒                                                 
                                                        ▒▓▒    ▒       ▒▓████████▓▓▓▓▓███▓▓██▓▓             ▒                                     
                                                        ▒▒▒      ▓▒ ▒ ▒▓▒████████▓▓▒▒▓████████▓▒▒ ▓▓▒▒      ▒     ▒                               
                                                        ▒▓ ▒      ▒▓▓▓▓▓▓▓███████▓▒ ▒▓▓████▓▒▒▒▓▓█▓▒        ▒▓▓▓▒▓▓▓▒                             
                                                        ▓▒ ▒    ▒▓   ▒▓█████████▓▒   ▒▓█████████▓▒▒         ▒▓▓▓▓▓▓▓                              
                                                        ▒▒▒▒ ▓▒ ▒▒     ▒▓██████▓▒▒  ▒▒▒▓██████▓▒▒           ▓▓▓▓▓▓▓ ▒▓▓▒                          
                                                        ▒▓▓    ▒        ▒▒▒▓███▓▒▒▒▒▒▒▓███▓▓▓▒▒    ▒▓  ▒    ▓▓▒▒▓▒▓██▓▒                           
                                                        ▒▓█▓▓▒ ▒         ▓▒▒▓████▓▒▒▓▓█████▒ ▒▒    ▒▒       ▓▓▓▓▓▒▓▓▓▒                            
                                                            ▒▒█▓  ▒        ▒▓█▓███████▓████████▓▓▓▓▒▒▓▓▓     ▒▒▒▒▓▓▓▓▓▓▓                             
                                                    ▒▒▒▒▒▒▒▒▓▒ ▒▒▒▓▒▒▒▓█▓████▓█████████████▓██▓█▓█▓▓█▓▓▒▒ ▒▒▒▒▓▓▒▒▒▓▓                             
                                                    ▒▓▓▓▓▒▓▒     ▒▓▓▓█▓▓▓█████▓█████████████▓▓▓▓██████▓▓  ▒    ▓▓▒▒▒▓▓▓▓▓                           
                                                    ▒▒▒▒▒▒     ▓▓▓██████████▓██████████████▓▓████████▒ ▓▓ ▒ ▒▒▒   ▓▓▓▓▓▒                          
                                                ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓████████████▓█████████████▓███████████▓▓▒▒       ▒▒▒▒▓▒                           
                                                ▒▒▓▓▓▒▒▒▒▓▓▒ ▒▓▒▒▓████████████▓█████████████▓▓███████████▒          ▒▒▒▓                            
                                                ▒ ▒▒▒▒▓▓▒▓▓▒▒▓█▓▓███████████▓▓███████████████████████▒██▓▓▒     ▒▒▒▒▒▒▒▒                           
                                                ▒▒▒▓▓▓▓▓▓▓▓▒▒▓████▓▓██████████▓███████████▓████▓███▓█████▓▓▓▒▒ ▒▓▓▒▒▒▒▓▒                           
                                        ▒▓▓▒    ▒▓▓▓▓▓▓▓▓▓▓▓▓█▓██▓▓▓██████▓███▓██████▓████▓███████████████▓▓▓▒▒▓▒▒▓▒▒▒▓ ▒▓▒                        
                                        ▒▓▓▓▓▓▓▒▓▒▒▒▓▓▓▓▓▓▓█▓▓▓▓████▓▓██████████▓██████▓▓████▓██████████▓▓████▓▒ ▓▓▓▓▒▒▓▒▓▓▓▓▒                       
                                    ▒▓▓▓▒▓▓▓▓▓▓▓▒▓▓▓▓█▓█▓▓▓▓▓██████████████████████████████▓████▓█████████████▓█▓▒▓█▓▓▒▒▓▓                        
                                    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓███████████████████████████████▓▓▓▓▓▓██▓▓▓▓█████████▓████▓▓▓▓                        
                                    ▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████████████▓██▓▓▓▓███▓▓▓▓▒▒▒▓█████████▓█▓█▓▓▓                        
                                    ▓▓▓▒▒▓▓▒▒▓▓▓▓▓██▓▓▓▓███████████████████████████████████▓▓▓▓██████████▓▓████████▓███▓▓▓▓    ▒▒                  
                                    ▒▓▓▒▒▒▒▒▓▓▓▓▓█████▓███▓██████████████████████████████▓▓▓▓██████████████▓▓▓▓▓▓▓▓███▓▓█▓▓▓▓▓▓▓▒                  
                                    ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓████▓▓███▓████████▓▓████████████████▓▓▓▓▓▓█████████████▓▓▓▓▓▓▓▓▓▓▓████▓▓█▓▓▓▓▓▒▒                 
                                    ▒▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓███████████████████████▓▓▓▓█▓████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓█▓██████▓▒▒▒              
                                ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓█████▓▓▓▓████▓█████████▓███████████▓███████████▓▓▓▓▓█▓▓▓▓▓▓▓█▓███▓▓▓▓▓████▓▓▓▒              
                                ▒▒▒▒▓▓▓▓██████▓▓▓██████████▓▓▓████████████████▓▓████▓█▓██████████████▓▓██▓▓██▓▓▓▓▓▓█▓█████▓▓▓████▒                 
                                ▒▓▓▓▓▓▓██████████████████████▓████████████████████████████▓█████████████▓▓▓█▓██████▓███████████████▓▒               
                                ▓▓▓▓▓█▓███████████████▓█▓██▓▓▓▓████████▓▓▓▓▓████▓▓███████▓████████████▓▓▓█▓██████████████████████████▓▒             
                                ▓▓▓▓▓██████▓████▓▓███████▓▓▓▓▓▓▓▓▓█████▓██▓██████████████████▓▓█████▓▓▓▓██▓████████████████████████▓▓▓▓▓▒           
                                ▒▓▓▓▓██████▓█████▓▓▓▓▓▓▓▓▓▓▓      ▒▒▒▓▓████▓████████████████▓███████▓▓▓████████████████████████████▓     ▒          
                                ▒▓▓▓▓▓████████▓▓█▓▓▓▓▓▓▒▒▒          ▒▒▒▓▓█▓▓████████▓█▓████████████████████████████████████▓▓█████▓     ▒         
                                ▒▒▒▒▓▓▓▓██████▓▓█▓▓▓▓▓▒▒▒▒          ▒▒▓▒▒▓█▓▓██▓▓▓█████████████████████████████████████████▒    ▓███▓▒             
                                ▓▒▒▒▓▓▓████████████▓▓▒▓▓▒         ▓▒▒  ▒▓▒▒▒▓▓███████████████████▓▓██████████████████████████▒   ▒▒▒▓▓▓▒           
                                ▒▓▓▓▓███████████████▓▓▒▒          ▒▒▒▒▒▓▓▓▓▓████████████▓▓███████▓▓██████████████████████████▓▒▒  ▒   ▓▓▒▒▒▒       
                                ▓▓████████▓█████████▓▒▓▓▒▒▒▒ ▒▒▒▓▓▓▓▓██▓▓█████████████████████████████████████████████████████████▓▒  ▒▓   ▒▒▒    
                                ▒▓▓▓▓█▓▓▓▓████████████▓▓▓█▓▓▓▓▓▓▓▓▓▓████████████████████████████████████████████████████████████████▒ ▒▒         ▒   
                                ▓▓███▓▓▓▓▓▓▓▓▓▓▓█▓███████▓██▓███████████████████████████████████████████████████████▓██████████████▓     ▒           
                            ▒▓▓▓███▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████████████████████████████████▓▓▓█████████████▒                
                            ▒▓▓▓███▓█████▓▓▓▓▓▓▓▓████████████████████████▒▓▓███████████████████████████████████████▓▓▓▓███████████████               
                            ▒▓▓████████▓▓▓▓▓█▓█▓████████████████████████▓▒▓█████████████████▓▓███████████████████▓▓▓▓▒▓████████████████▒             
                        ▒▓▓▓█████████▓██████████████████████████████████████████████████▓▒▓▓██████████████████▓▓▒   ▓█████████████████▒            
                        ▓▓▓▓▓▓▓█████████████████████████████████████████████████████████▓▒▒▓▓█████▓████████████▓▓  ▒▓▓▓█▓██████████████▓            
                        ▒▓▓▓▓▓▓█████████████████████▓▓▓█████████████████████████████████▓▓▓▓█████████████████████▓▓▓▓▓▓██████████████████            
                    ▓▓▓▓▓███████████████████████▓▒ ▒▓███████████████████████████████████▓██████████████████████████▓▓███████████ ▒▓██▓            
                    ▒▓▓▓▓█████████▓▓▓▓▓▓█████████▓▒  ▓▓██████████████████████████████████████████████████████████████▓███████████▒   ▓██▓          
                    ▒▓▓▓▓▓▓█▓▓██▓▓▓▓▓▓▓▓▓███▓▓███▓▓▒▒▓███████████████████▓███████▓▓▓█████████████████████████████████████▓▓███████     ▓▓▓▓▒       
                    ▓█▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▓▓███████████████████▓███████▓▓▓▓█████████████████████████████████████▓▓██████▓        ▒▓      
                        ▓▓▓▓▓███▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓██████████████████████████▓▓▓██████████████▓████████████████████████████████████████▓               
                        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████▓▓█████████████████████▓▓▓▓▓████████▓█████▒              
                        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓██████████████████████████████████▓▒▒▓████████████████████▓▓  ▓▓▓█████▓█████▓               
                        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓▓█▓▓███████████████████████████▓▓▓▓████████████████████▓▓▒▒▒▓▓▓▓▓▓▓██████                
                            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████▓▓▓▓████████████████████████████▓▓▓███████████████████▓▓▓▓▓▒▓▓▓▓███████▓                 
                            ▓▓█▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓███████▓▓▓▓▓▓▓███████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▒                 
                            ▒▒▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓█████▓▓▓▓██████▓█████████████████████████████▓▓▓▓▓▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████▓                 
                                ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▒▒▒▒▒  ▒                    ▒▒▓▓▓▓▓▓▓▓▓████▓▓▓▓▓▓▓▓▓▓▓                 
                                ███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒ ▒▒▒                                      ▒▓▓▓▓▒▒▒▒▒                          
                                ▒████████████▓▓▓▓▓▓▓▓▓▓▓▓▒▓▒▒▒ ▒▒▒                                                ▒                                ";
            break;
        case "Voidborn Wraith":
            enemyArt = @"
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▒▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓████████████████████████████████████████████▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓█████████████████████████████████████████████████▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓███████████████████████▓██████████████████████████▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓██▓▓▓▓▓▓▓▓█▓▓▓▓▓█████████████████████████████████████▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓████████████████████████████████▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓████████████████████████████▓▓█▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓████████████████████████▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓██████████████████████▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▓▓█▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▓█████████████████▓▓▓▓██▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▓███▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▓████████████████████▓▓▓▓██▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▓▓▒▒▒▒▒▒▒▒▓▓██▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▓▓▒▒▒▒▒▒▒▒▓████████████████████▓▓▓▓▓█▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▒▒▒▒▒▒▒▓▓▓█▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██▓▒▒▒▒▓▓████████████▓▓███████▓▓▓▓█▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓███▓▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓███▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓██████████████▓▓███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓███▓▓▓▓▓▓▓▓▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓██████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓█████▓▓▓▓▓▒▒▓▓▓▓▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓██▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓████▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓██████▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓███████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒███▓▓▓▒▒▒▒▒▒▓██▓▓▓▓▓▓▓███████▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓█████████▓▓▓▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒████▓▓▒▒▒▒███████▓▓██████████▒▒▓▓▓▓▓▓▓▓▓▓▓▓████████████████▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓█▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█████▓▓▓▓▓███████████████████▓▓▓▓▓▓▓▓▓▓▓▓██████████▓▓▓▓▓▓█▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▓█████▓▓▓▓████████████████████▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▒▒▒▒█▓▒▒▒▒▒▓██▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓████████▓▓██▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████▓▓▓█████████████████████▓▓▓▓▓▓▓▓▓▓▓█▓▓▒▒▒▒▓▓▓▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓███████▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████▓███████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▓▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▒▒▒▒▒▒▓▓▓▓████▓▓▓▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒██████████▓▓█████▓▓▓▓█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓█▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████▓▓███▓▓▓▓▓██████████▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒███████████████▓██████████████▓▒▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▒▒▒▒▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████████████████████████▓▒▒▒▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓██████████████████████████████▓▒▒▒▒▒▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓██████████████████████████████▓▒▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▒▒▒▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓███████████████████████████████▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓███████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▓▓▓▓▓▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓██████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓
▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▒▒▓▒▒▒▒▒▒▒▓▒▒▒▒▒▓▓▒▓████████████████████████████████▓▓▒   ▒  ▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▓▒▒▒▒▒▒▓▓▓▓▓███████████████████████████████████▓    ▒▓▒▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓
▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓▓▒▓▓▓▒▒▓▓████████████████████████████████████▓▓▒▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓
▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▓▓███████████████████████████████████████▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓
▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▓▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓███████████████████████████████████████▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓█▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▒▒▒▒███▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓████████████████████████████████████▓▓▓▓▓▓▓▓▓██▓██▓▓██████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▓▓▓▒▒▒▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓█████████████████████████████████▓▓▓▓▓▓▓▓███▓▓█████████▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▒▓▓▓▓▓███████████████████████████████▓▓▓▓▓▓████████████▓▓▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▒▒▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▓▓▓▓█▓▓▓▓▓▓█████████████████████████████▓▓▓███▓███████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓▓▓██▓▒▒▒▒▒▒▓▓▓▓███▓▓▓▓█▓▓▓▓▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████████▓▓▓▓███████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████▓▓▓▓▓█████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████▓█▓▓▒▒▓▓▓▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓██████████▓██████▓▓▓▓▓▓▓████▓▓▓█▓▓▓▓▓▓███▓▓▓▒▓▓▓▓███▓▓▓▓███████████████████████████████████▓▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓██▓▓▓▓▓▓▓▓▓▒▒▓███████████████████▓▓▓▓▓▓█▓▓▓▓▓██████▓▓▓▓▒ ▒▓▓▓███████████████████████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███▓▓▒▒▒▒▒▓▓▓▓▒▓██████████████████████████▓▓▓▓██▓▓▓▓▓▓▒▒▒▒   ▓▓▓▓▓████████████████████████████████▓▒▓█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██▓▓▒▒▒▒▒▒▒▒▒▒▓████████████████████████████████▓▓▒▒▓██▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▓██▓▒▒▒▒▒▒▒▒▒▒▒▓▓██████████████████████████████▓▒▓▒▒▒▓▓▓▓▓▓▓███▓▓████████████████████████████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▓▓▓█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓███████████▓███████████████▓▓▓▓▓▒▓███▓▓▓▓▓▓█▓█████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓██████▓▓▓▓▓▓█████████████▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓███▓████████████████████████████████████████████▓▓▓▓▓██▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█████▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▓█████████▓▓▓█████████████████████████████████████████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████▓▓███▓██████████▓▓▓▓▓▓███▓█▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██▓███████▓▓▓▓█████▓▓▓▓▓▓▓██████████▓▓▓▓▓▓▓█████████████████████████████████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██▓██████▓▓▓██████▓▓▓████████████████████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████▓▓▓▓▓▓▓▓▓▓████▓▓█████████████████████████████████████████████████████▓██████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██▓▓▓▓▓▓▓▓██▓▓▓▓▓▓████████████████████████████████████████████████████▒▒▒ ▒▓████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒
▒▒▒▓▓▓▓▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓█▓▓▓▓███████████████████████████████████████████████████████▓▒      ▓██████████████████▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒
▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓█▓██▓███████████████████████████████████████████████████████████         ▓▓████████████████▓▒▓██▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒
▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓████▓██▓▓██████████████████▓▒█████████████████████████████████████████▒       ▒▒▒ ▒█████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒
▒▒▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒██████████████████████████▓   ▒████████████████████████████████████████▒        ▒▒▓▓██████████████████████▒▒▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒
▒▒▒▓▓██▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒██████████████████████████▒  ▒▒▒████████████████████████████████████████▒         ▓████████████████████████▓▓▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒
▒▒▒▓▓█▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓█████████████████████████▒    █▓▒██████████████████████████████████████████▒        ▒▓███████████▓████████████████▓▓▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▓▓▓▓█▓▒▒▒▒▒▒▒▒▒▒▒▒▓ ▒███████████████████████▒    ▓██████████████████████████████████████████████▓       ▒██████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓  ▒   ▒▓████████████████████▓   ▒▓███████████████████████████████████████████████▓       ▒███████████████████████▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▒  ▒ ▒  ▒▓▓██████████████▓▒▒     ▒█████████████████████████████████████████████████     ▒▓████████████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒    ▓     ▒██████████▓▒        ▒▓█████████████████████████████████████████▓████████▒    ▓████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒  ▒▒  ▒▒     ▒▓▓██▓▓▓▒▒    ▒    ▓████████████████████████████████████████████████████▓   ▓██▓▓▒▒████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒ ▒     ▒      ▒▓▓▓▓           ▒▓▓█████████████████████████████████████████████████████▓▓▓▓▓▒   ▒▓▓▓█████████████████▓▓█▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒       ▒      ▒▒▒▒▒          ████████████████████████████▓▓███████▓███████████████████▒▒▒     ▒▒  ▓████████████████▒ ▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒               ▒  ▒          ▓██████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████▒            ▒███████████████▓   ▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒              ▒▒ ▒▒         █████████████████████████████████▓████████████████████████▒      ▒     ▓▓▓█████████▓▓▓█▒   ▒▒▒▒▒▒▒▒▒▒▒▒
▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒               ▒▒▓▒       ▒▓██████████████████████████████████████████████████████████▒     ▒▒     ▒▒▒▓▓▓██████▓▓▓▓▓     ▒▒▒▒▒▒▒▒▒▒
▒▒▓▓▓▓▓▓▓▓▒▒▒▒▒ ▒▒▒▒                ▓▓▓▒  ▒▒ ▓████████████████████████████████████████████████████████████▒      ▒    ▒   ▒▒▒▓████▓▓▓▓█▒▒ ▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒     ▓▒          ▒▒▓▓▓▓▓▓██████████████████████████████████████████████████████████████▒▒            ▒▒▒▒▒▒▓███▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▓▓▓▓█▓▒▒▒▒▒▒▒▒    ▓▓            ▓▓█████████████████████████████████████████████████████████████████████▓          ▒▒▒▒▒▒▒▒▓███▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▓▓▓▓█▓▓▒▒▒▒▒▒▒              ▒▓████████████████████████████████████████████████████████████████████████▓▒        ▒▒▒▒▒▒▒▒▒▒▓███▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▒▒▒▒▒▒▒           ▒████████████████████████████████████████████████████████████████████████████▓▒     ▒▒▒▒▒▒▒▒▒▒▒▒▒██▓▓▓███▓▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒        ▒▓██████████████████████████████████████████████████████████████████████████████▓▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██▓▓████▓▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▓▓▓█▓▓▓▓▒▒▒▒▒▒    ▒▓████████▓▒▓████████████████████████████████████████████████████████████████████▓▓█▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓███▓▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▒▒▒▒▒▓▓████████▓▒ ▓█████████████████████████████████████████████████████████████████████▒▒▒▒▒▒▒▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓██▓▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒███▓▓▓▓██████▓▓▒   ▓██████████████████████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓█▓▓▓██▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓▓██▓▓▓▒▒▒      ▓████████████████▓▓██▓▓▓█████████████████████████████████████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓███▓▓▓▓█▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒ ▒▒     ▒▓██████████████▓▓▓█▓▓▓▓███████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓████▓▓▓█▓▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒   ▒▒▒      ▒▓██████████████▓▓██▓▓█████████████████▓█████▓███████████▓▓▓█████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▒▒▓█▓▓▓██▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒          ▒▓█████████████████████████████████████▓▓████▓▓████████▓███▓▓▓█████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓██▓▓█▓▒▒▒▒▒▒▒▒▒▒
▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒       ▒▓▓██████████████████████████████████████▓▓████▓▓██████████▓▓▓▓▓▓▓███████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓█████▓▓▓▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ ▒▒▒▒▒▒▒     ▒▒▓██████████████████▓████████████████████▓█▓███▓▓███████████▓▓▓▓▓▓▓▓█████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓████▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▓▓▓▒▒▒▒    ▒▒ ▓██████████████████████████████████████████▓███▓▓█████████████▓▓▓▓▓▓▓█████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓██▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒
▒   ▒▒▒▒▒▒▒▒▒▒▒▒▓███▓▒▒▒▒   ▒▒▒▓███████████████████████████████████████████████▓▓█████████████████▓▓▓██████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒   ▒▒▒▒▒▒▒▒▒▒▒▓███▒▒▒▒▒  ▒▒▒▓███████████████████████████████▓████████████████▓▓██████████████████▓▓▓██████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▒▒    ▒▒▒▒▒▒▒▒▓▓█▓▒▒▒▒▒ ▒▓▓████████████████████████████████████████████████████████████████████████▓▓▓█████████████▓▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒ ▒▒▒▒▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████████████████████████████████████████████████████████████████████▓▓▓▓▓████▓▓▓▓▓█▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████████▓▓▓▓▓█████████████████████████████████████▓▓▓███████████████████████▓▓▓▓████▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒    ▒▒▒▒    ▒▒▒  ▒ ▒▒▒▓▓██████████████████████████████████████████████████████▓▓▓████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒    ▒              ▒▓▓███████████████████████████████████████████████████████████▓███████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
    ▒▒            ▒▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████████████████████████████████▓███▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
                ▒▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████████████████████████████████████████▓▓▓█████▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒
    ▒        ▒▓▓▓▓▓▓▓▓▓██████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
            ▒▓▓▓▓▓▓▓▓▓▓▓██████████▓▓███████████████████████████████████████▓███▓▓█▓▓▓▓▓▓▓▓███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒           ▒▒▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▓▓▓▓▓███████████████████████████▓██▓█▓▓█▓▓██▓▓▓▓▓██▓█▓▓▓▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒ ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▓▓▒▒     ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████▓▓████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒    ▒▒▒▒▒▒▒▒▒▒▒▒
▓▓▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████▓▓▓▓███▓▓▓▓▓▓▓▓▓▓▒▒▒▒▓▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒   ▒   ▒▒▒▒▒▒▒▒▒
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒   ▒▒▒  ▒▒▒▒▒▒
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒    ▒▒▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒ ▒▒▒  ▒▒▒▒▒▒
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▓████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒ ▒▒ ▒ ▒    ▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒  ▒   ▒    ▒▒  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒  ▒▒▒▒▒▓███████████████▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓             ▒  ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒  ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒
▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████▓█▓▓▓▓▓▓▓▓▓▓▒▓▓▒▓▓▒ ▒           ▒    ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒    ▒▓▓▒▒▒▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒   ▒▒▒▒▒▒▒▒";
            break;
        case "Oblivion Scourge":
            enemyArt = @"
                                                                                                                                                    
                                            ▒                                                         ▒                                             
                        ▒▒                 ▒▓▒                       ▓                             ▓▓                                              
                        ▓▓                  ▒▓▓▒                     ▓    ▒▒                   ▒▒▓▓▓                                               
                        ▒█▒                    ▓▓▓▒▒▒                █▒   ▒▓                ▒▓▓██▓▒                                                
                        ▓█▓                      ▒▒▓▓██████▒▒▒      ▓█▒  ▓▒            ▒▓▓███▓▒                         ▓▒                        
                        ▒███▒ ▒                         ▒▓█████▒▒   ▒▓█▓▓█▓▒▒    ▒▓▓██████▓                           ▒▓                          
                            ▓████▒                           ▒▓███▓▒  ▒▓█▓████▓▒ ▓▓████▓▒▒                           ▒▒█▒                           
                            ▓███▓▒  ▒             ▒▒▓▓▓▒    ▒▓████▓▒▒▓█▓████▓▓█████▒      ▒▒▒▒                ▒▓▒▓███▒                            
                                ▓███████▓▒      ▒        ▒▓█▒ ▒▓▓██████▓▓█▓█████████▓     ▒▓▓▒▒             ▒████████▓▒                              
                                ▒▓▓█████████▓▓▓▓█▒         ▒█▓▒▒▓▓██████████████████▒   ▒█▒▒        ▒▒▒▒▓▓▓▓███████▓▒                                
                                    ▓██████████████▓▒▓▒    ▒██▓▓▓███████▓██████████▓  ▒▒█▓▓█▓▓▒▓▓▓▓████████████▓▓▒                                   
                                    ▒▓▓▒▒▓█████████████▓▓▓▓████▓▓▓██▓██▓██▓▓████████▓██████████████████████▓▓▒                                       
                                            ▒▓▓██████████████████████▓█▓▓███████████▓████████████████████▓                                           
                                            ████████████████████████▓█████████████████████████▓▒  ▒██▓                                            
                                            ▒█████████████████████████▓▓▓▓████████████████████▒     ▒▓                                             
                                            ▒█████████████████████████████████████████████████▒                                                    
                                            ▒▒▒    ▓████████████████████████████████████████████▒                                                   
                                                    ▓██████████████████████████████████████▓▒▒  ▒▒▒▒       ▒▓█▓▓▓██▓                                 
                                                ▒▓▓▓▓██▓▓▓███████████▓▓████████████████▓██▒             ▓▓       ▓█▓                               
                                                    ▒▒▒██▓████████████▓███████████████▓▓▒▓▓▓▓         ▒█▓          ██                              
                                ▒▓██████▓▓▒           ▓▓▓▓▓██████████▓█▓▓█▓█████████████▓▓▓▓▓▓▓▓    ▒  ██            ██                             
                            ▓██▓▓▒  ▒▒▓▓█▓█      ▒▒▒▒▓▓▓███████████▓▓████████████████████████▓▓▓▓██████▓           ▓█▒                            
                            ▓█▓         ▒▓███▓▒▓▓▓▓▓██████▓████████▓█▓█████████████████████████████▓█████▒▒ ▒█▒     ▒█▓                            
                            ▓█▒      ▒▒  ▓▓▓███▓██████▓█████████████▓█▓███████████████████████████████████████▓█▓▓█▒ ▓██▒                           
                            ▒██▒     ▒▓▓▓▒█████████████████████▓████████████████████████████████████████████████████▓▓▓██▒                           
                            ▒██▒▒▓▓▓▒▓███▓█▓▓▓█████▓▓▓▓▓█▓█████████▓████▓█▓▓█████████████████████████████████████████████▒                           
                            ▒██▓▒▓██▓▓▓▓▓▓▓▓▓████████████████████▓██████▓▓▓▓█████████████████████████████████████████████▒                           
                            ▒▓██▓▓▓▓▓▓██████▓▓████▓█▓▓▓█▓▓▓█▓▓▓▓███▓████▓▓███████████████████████████████████████████████▒                           
                        ▓▒▒▓█████████▓▓▓▓█████████████████████▓▓█████▓▓█████████████████████████████████████████████████▓                          
                        ▒▒▒▓▓█████▓▓▓██████████████▓▓▓████▓▓█████████▓▓▓█▓▓███████████████████████████████████████████████▓▓▒                       
                    ▒▓▓███▓▓▓▓▓████████▓▓▓█▓██████████▓▓██████████▓███████████████████████████████████████████████████████▓                       
                    ▒▓█▓▓▓▓▓███▓▓█████▓▓███▓███████████▓█████████▓▓█████████████████████████████████████████████████████████▒                      
                    ▒▒▓▒▒▓▓▓█████▓▓▓████████████████████▓███████████████▓█████████████████████████████████████████████████████▓▒                     
                    ▒▒▓███▓▓█▓▓▓████████████████████████▓████████████████▓██████████████████████████████████████████████████████▒                    
                    ▓▓▓█▓▒▓██████▓▓██▓█████████████████▓███▓██████████████▓█████████████████████████████████████████████████████▒                    
                ▓▒▓▓▓▓▓█████████████▓███████████████▓███▓▓██████████████████████████████████████████████████████████████████▓                     
                ▒▓▓████████████████████████████████████▓▓▓▓▓████████████████████████████████████████████████████████████████▓                      
                ▒▒▓▓▓▓██████████████████████▓███████████████████████████▓████████████████████████████████████████████████████▒                      
                ▒▓███▓▓██████████████████████████▓▓████████████████████████████████████████████████████████████████████████▓▒                       
                ▓▓▓██▓▓███████████████████████████████████████████████████████████████████████████████████████████████████                         
                ▒▓▓███▓██████████████████████████████████████████████▓███████████████████████████████████████████████████                         
                    ▓███▓████████████████████████████████████████████▓███████████████████████████████████████████████████▒                        
                    ▒▓▓▓▓▓████████████████████████████████████████████████████████████████████████████████████████████████▒                        
                    ▒▓▓▓▓▓▓███████████████████████████████████████████████████████████████████▓▓▓▓▒▒▒█████████████████████▒                        
                    ▓▓▓███▓▓████████████▓▒▓████████████████████████████████████████████████▓▓▒▒▓▓▒▒▒▒▓▓██████████████████▓                        
                    ▒▓█████▓▓██████████▒▒▒ ▒██████████████████████████████████████████████▒▒▒▒▒▓▒▒▒▒▒▒▒▒▓██████████████████▓▒   ▒▒                
                    ▒▒▓▒▓█████▓▓████████▓▒▒▒▒▒▒▒▓███████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒████████████████████▓▒▓▓█                
                ▒▒ ▒▒▓▓▓▓████▓▓▓████████▓▒▒▒▒▒▒▒▒▓██████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒████████████████████████▓▒              
                ▒▒▓▓▓▓▓▓▓██▓▓▓████████▓▓▒▒▒▒▒▒▓▒▓▓████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒█▓█▓▓███████████████████████▓▒               
        ▒▒     ▒▓▓▓▓▓█▓▓▓▓▓██▓▓▓▓▓█▓▓█▓▒▒▒▒▓▒▒▓▓▓▓██████████████████████████████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████████████████████▓▒▒               
            ▒▓▓▓▓▓▓▓▓▓▓█▓██████▓████▓▓▓▓▓▓▓▓▒▒▒▒▓▓▓▓▓██████████████████████████████████▒▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▓████████████████████████▓███▓▒              
            ▒▒▓▓▓▓▓▓▓█▓██████████▓▓██▓▓▓▒▒▒▒▒▒▒▒▓▓▓██████████████████████████████████▓▒▓▓▒▒▒▒▒█▓▒▒▒▒▒▓█████████████████████████████▒              
                ▒▓▓▓▓▓▓██▓██████████▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓█▓████████▓█████████████████████████▓▓▓▓▒▓██▒▒▒▒▒▒▓▓█████████████████████████▓▒               
                ▒▓▓▓▓▓██▓████████▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓█▓████████▓███████████████████████████▒▓█▓██▒▒▒▒▓▒▒▓████████████████████████                 
            ▒ ▒ ▒▓▓▓▓██▓▓███▓███▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓████████▓███████████████████████████▓█▓▓██▓▓▒▒▒▒▒▒▓█████████████████████▓▒                 
            ▒▓▓▒▓▓▓█▓█▓▓▓██▓▓█▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓█▓▓██████▓█████████████████████████████▓█▓██▓▓▒▒▓▒▓▓▓████████████████████▒                  
            ▒▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▓██████████████████████████████▓▓▓██▓▓▒▒▓▓▓▓▓██████████████████▒                   
            ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓████▓█████████████████████████████▓██████▓▓▓▓▒▓▓▓▓██████████████████▓                   
                ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓████████████████████████████████▓▓██▓▓▓▓▓▓▓▓████████████████████▒                  
                ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓████████████████████████████████▓██▓▓▓▓▓▓▓▓████████████████████▓                  
                ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓████████████████████████████████▓█▓▓▓██████████████████████████▓▓                 
            ▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓██▓█████████████████████████████▓▓▓██████████████████████████▓▓▓▓▒                
            ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓██▓█████████████████▓▓█████▓▓▓▓▓▓█▓▓█▓▓█████████████████████▓▓▓▓▓              ▓▓
            ▒▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓██▓████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████▓▓▓▓▓▓▓▓▒            ▓▓
                ▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████▓▓▓▓▓▓▓▓▓▓▒          ▒█▓
                ▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓█████████████████▓▓▓▓▓▓▓▓▓▓▓▓▒     ▒▓▓▓███
                ▒▒▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓███████
                ▒▒▒▒▒▒▒▒▒▒▒█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████
                ▓▒▒▒▒▒▒▒▒▒█▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓▒▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓██▓▓▓▓▓▓███▓▓▓▓██▓▓█▓▓▓▓▓▓▓▓▓▓███████████████████
                ▒▓▒▓▓▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█▒▓▓▓▓▓▓▓▒▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓██▓▓▓███████████████
                    ▒▓▓▓▒▒    ▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓█▓▓▓▓▓▓▒▓▓▓▓▓█▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓██▓█▓▓▓▓▓▓▓█▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▓███▓▓▓█▓█████████████▓██████
                            ▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▓▓▓▓▓█▓▓▓▓▓▓▓▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓█████▓▓▓█████▓▓▓▓███▓▓▓▓▓▓▓▓▓████▓███▓████████████████████
                            ▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓████▓█████████████▓█▓█▓██▓███▓██████████████▓█████▓▓███▓▓▓█▓███▓█████████████████████████████
                                ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████▓▓█████████████████████████████████████████";
            break;
        case "Ancient Desolator":
            enemyArt = @"
                                                                                                                                                    
                                                                                                                                                    
                                                                                                                                                    
                                                        ▒▒▒▒                            ▒▒▒                                                         
                                                    ▒▒▒                                     ▒▒▒▒                                                    
                                                ▒▓▓▒                                            ▒▓▓▒                                                
                                            ▒▓▒                                                 ▒▓▓▒                                              
                                            ▒▓▒                                                     ▒▓▓▒                                            
                                        ▒▒▓▓                                                         ▒▓▓▒▒    ▒                                    
                                        ▒▒▓▓▓                                                           ▒▓▓▓▒▒  ▒▒                                   
                                ▒   ▓▓▓▓▒                                                             ▒▓▓▓▓▓▒▒▒                                   
                                ▒▒▒▓▓█▓▓                                                               ▓▓██▓▓▒▒                                   
                                    ▓▒▓▓▓▓▓                                                              ▒▒▓▓██▓▓▒▒                                  
                                    ▒▓█▓▓▓▒▒                                                           ▒▒▒▓▓▓█▓▒ ▒▒▒                                
                                    ▒▓▓▓▓▓▓▒▒▒▒▒ ▒                                              ▒▒▒▒▒▒▒▒▓▓▓▓█▓▓▒ ▒▒▒                                
                                    ▓▓█▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒                                      ▒▒▒▒▒▓▓▓▓▓▓▓▓██▓▓▒   ▒▒                                
                                    ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒                       ▒▒▒▒▒▓▓▓▓▓▓███▓███▓▓▓▓▒                                       
                                            ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒             ▒▒▒▒▒ ▒▒▓▓██▓▓▓▓▓▒▒▒▒▒▒▒  ▒▒▒▒                                      
                                                        ▒▓▓██▓▒▒▒▒▒▒▒▒          ▒▒▒▒▒▒▒▓██▓▓▒▒   ▒▒▒ ▒       ▒▒▒▒  ▒ ▒                               
                                                        ▒▓▓▓▓▒▒▒▒▒▒▒▒       ▒▒▒▒▒▒▒▓█▓▒       ▒     ▒     ▒▒▒▒▒                                   
                                                        ▒▒█▓█▓▓▒▒▒▒          ▒▒▒▓▓███▓▒▒     ▒▒         ▒▒▒▒▒▒▒▒▒                                 
                                                    ▒▓▒▒▒▒▓▓█▓███▓▓▒          ▒▒▒▓█████▓▓▓▒▒▒▓▓▒▒▒          ▒▒  ▒▒▒▒                                
                                                    ▒▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒         ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▒  ▒▒          ▒▒▒▒▒▒▒▒                                
                                                        ▒▒▓▓███▓█▓▒▒         ▒▓█▓██▓▒▒▒▒▒▒   ▒            ▒▒▒▒▒▒▒                                 
                                                        ▒▒▒▓▓▓▓▓█████▒       ▒  ▓█████▓▓▓▒▒▒                  ▒▒▒▒▒                                  
                                                    ▒▒▒▒▓▒▒▒▓▓▓███▓▓▓▒ ▒      ▒▓▓███▓▒ ▒                     ▒▒▒▒▒                                  
                                                    ▒▒▒▓▓▓▓▒▓▓▓▓█████▓▓█▓▒▒▒▒▓▓▓▓███▓▒▒            ▒▒          ▒▒▒                                   
                                                    ▒▒▒▒▒▒  ▒   ▓▓▓████▓▓▒  ▒▓▓█████▒▒▒▒          ▒▒▒▒▒▒    ▒ ▒▒▒▒▒▒                                 
                                            ▒▒▒▒▒▒▒▒▒       ▒▒▓▓████▓▓▓▒▒▓▓▓██▓▓▓▓▒▒▒▒         ▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                
                                            ▒▒▒▒  ▒▓▒▒▒       ▒▒▒▓▓▓██▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▒▓▒▒     ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                 
                                ▒            ▒▒▒▒▒ ▒▓▓▒▒▒   ▒▒▒▒▓▓▓▓▓███▓▓▓▓▓▓███▓▒▒▒▓██▓▓▒▒▒▒▒▒▒▒  ▒▒▒▒▒▒▒▒ ▒▓▒▒▒▒         ▒                        
                        ▒▒         ▒      ▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▒▒▓▓▓▒▒▓██▓██▓▓▓▓█▓██▓▒▒▓▓▓██▓▒▒  ▒▒     ▒▒▒▒▒▓▒▒▒▒▓▒▒▒     ▒▒▒▒                         
                        ▒▒       ▒ ▒▒▒  ▒▒▒▒▒▒▒▓▓▒▓▒▒▓▓▓▒▓▓██▓▓▒▒▓▓▓▓██▓▓▓▓█▓█▓▓▒▓▓████▒      ▒    ▒▒▓▒▒▒▒▓▓▓▓▒▓▒    ▒▒▒▒▒                         
                        ▒▒▒        ▒▒▒▒▒▒▒▒▒   ▒▒▒▒▒▒▒▒   ▒▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▒      ▒   ▒▒▓▓▓▓▒▒▒▓▓▓▒▓▓  ▒▒▒▒▒▒▒                         
                        ▒▒▒▒        ▒▒▒▒▒▒▒▒   ▒▒▒▒▒▒▒    ▒▓██▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▒▓▓████▓▒▒   ▒▒▒   ▒▒▓▓▓▓▓▓▓▓▓▒▒▒▒▓▓▒▒▒▒                           
                        ▒▒▒▒▒       ▒▒▒▒▒▒▒▒ ▒▒▓▓▓▒▒▒     ▒▓███▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓█▓███▓▒▒ ▒▒▒▒   ▒▒▒▒▓▓▓▓▓▓▓▓▒▒▓▓▓▓▒▒▒                           
                        ▒▒▒▒▒▒ ▒      ▒▒▒▒▓▓▒▓▒▓▓▒ ▒     ▒▒█████████▓▓█▓▓▓▓███▓▓▓▓▓█▓█████▓▓▓▓▓▒   ▒    ▒▓▓▓▓▓▓▒▒▒▒▓▒▒▒▒▒                          
                        ▒▒▒▓▒▒▒▒▒▒   ▒▒▒▒▓▓▓▓▓▓▓    ▒  ▒▓████████▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓█████████▒▒      ▒  ▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒                         
                        ▒▒▓▓  ▒  ▒▒▓▓▒▓▓▓▓▓▓█▓▓▒    ▒▒▓▓██████▓█▓█▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓█████████▓       ▒▒▒  ▒▒▓▓▒▒ ▓▒▒▒▒▒▒▒▒▒▒▒                        
                ▒▒▒      ▒▒▒▒▒▒▒▓▒  ▒▓▓▓▓▓▓▓█▓█▓▒▓▒   ▒▒▓▓██████▓███▓▓▓▓▓█▓▓▓▓▓▓▓▓█████████▒      ▒▒▒ ▒▒▒▓▓▒▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▒▒                       
                ▒▒▒▒▒ ▒▒▒▓▒▒▒▒▓▓▓▒▒▓▓▒▓▒▓▓▓███▓▒▓▒▒▒  ▒▒▒▓███████████▓▓▓▓▓▓█▓▓▓▓▓█████████▓     ▒▒▒▒▒▒▒▒▒▒ ▒▒     ▒▓▓▒▒▒▒▒▒▒                        
                ▒▓▒▒▒▓▓▓▓▓▓▓▒▓▓▓▓▓▒▓▒▒▒▒█▓▓███▓▒▒▒        ▓██████████▓▓▓▓▓██▓▓▓▓██▓▓████▓▓▒      ▒▓▓▓▓▒  ▒▒▒▒▒      ▒▒▒▒▒▒▒▒▒▒▒                     
                ▓▓▓▒▓▓▒▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓█████▒▓▒▒ ▒     ▒███████▓▓█▓▓▓▓▓██▓▓▓▓▓█▓█▓▓▓▓▓     ▒▒▓▓▓▓▓▒▒▒▒            ▒▓▓▒▒▒▒▒                       
                ▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████▓▒▒▒▒      ▒▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓████▓▓▓▓▒   ▒▒▓▓▓▓▓██▓▒▒▒▒▒          ▒▒▓▓▒▒▒▒                        
                ▒   ▒▓▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓██████▓▓▒ ▒       ▒▒▒▒▒▒▒▓▓▓███▓▓▓▓▓▓▓▓█▓▓▒▒  ▒▒▒▒▒▒▓▓██▓▓▓▒▒▒▓▒▒▒ ▒    ▒▒▒▓▓▒▒▓▓                         
                        ▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▒ ▒     ▒▒▒▒▒▒▒▒▒▓▓▓▓███▓██▓▓▓▓▒ ▒▒▒▒▒▒▒▓▓████▓▓▓▓▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▓▒                         
                    ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▒▒▒▒▒ ▒▒▒▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓█████▓▓▓▓▓▓▒▒▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒                          
                    ▒▓███▓▓▓██▓▓▓▓██▓██████▓▒▒▒██▓▓▒▒▒▒▒▒▓▓▒▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓█████▓▓██▓▓▓▒▒▓▓▓▒▒▒▒▒▓▓▒▓▓▓▓▓▒    ▒▒                    
                        ▓▒█▓▓▓██▓▓▓█▓████████▓█▓▓▓▓█▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▒▓▒▒▒▒▓▒▒▓▓▓█▓▓██████▓███▓▓▓▓▓▓▓▓▒▒▒▓▓▓▒▒▓▓▓▓▓▒    ▒                     
                        ▒▓█▓▓▓▓▓▓▓▓▓███████████▓▓▒▓██▓▓▒▒▒▒▒▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓███████▓██▓▓▓▓▓▓▓▓▓▒▓▓▓▓▒▓▒▓▓▓▓▓▒▒▒▒                       
                        ▒█▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓██▓▓▒▒▒▒▓██▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓█▓▓▓▓████████████▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▒▒▒▒▒▒▒                    
                ▒▒▒      ▒▓▓█████▓▓███████████████▓███▓▓▒▓▒▒▒▓████▓████▓▓▓▓▓█▓▓█▓▓██▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▒                    
            ▒▓▒▓▓▓▓▓▒    ▓▓▓████▓▓████████████████████▓▓▓▓▒▒▒████████████▓███▓█▓▓▓█▓██████▓████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▒▒                    
            ▒▒▒▒▓▓▓▓▓▓▒▒▒▓▓▓███▓▓▓▓██▓▓████████████████▓▓▓▒▓▒▒▒▓████████████████▓█▓▓▓▓████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓                        
        ▒▓▓▒▒▒▓▓▓▓▓█▓██▓▓▓▓▓▓▓▓▓▓▓█▓▓██████████████████▓▓▓▒▒▒▒███████████████████████████████▓▓███▓▓▓▓▓█▓▓▓▓▓▓▓▓▓██▓▓██▓▓▓                         
        ▒▒▓▓▓▓▓▓▓▓▓▓▓█▓██▓▓▓▓▓▒▓▓▓▓▓█▓▓▓█████████████████▓▓▓▓▒▒▒▓██████████████████████████▓▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▒                         
        ▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████▓▓▓▓▓▒▓▒████████████▓▓▓▓██▓█▓██▓█▓▓█████▓▓█▓▓▓▓▓▓▓▓▓▓▓█████████▓▒▒                         
        ▒▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓█▓▓▓██████████████████▓▓▓▓▒▓▓▓▓▓███████████████████▓▓██████████▓▓█▓▓▓▓▓▓▓▓▓█▓█████▓███▓▓▒▒                        
            ▒▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████▓▓▓█▓▓▓▓▓████████████████████████████████▓▓█▓▓███▓▓█████████▓▓██▓▓▓▓▓▒                      
            ▒▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓█████████████████▓█▓█▓▓█▓▓████████████████████████████████▓▓▓▓▓▓████████████▓▓▓██▓▓███▓▓▒▒                   
                ▒▓ ▒▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████▓█▓██▓█▓▓███████████████████████████████▓▓▓▓▓▓▓███████████▓▓▓███████▓▓▓▒▒                   
                    ▒███████▓▓▓▓██▓▓▓▓▓██████████████████████▓█▓█████████████████████████████▓▓▓▓▓▓▓▓█████████████▓█████████▓▒                     
                    ▒▒▓██▓▓████▓▓▓██▓▓▓██████████████████████▓█▓██▓▓████████████████▓▓██████▓▓▓▓▓▓▓▓▓██████████████████████▓▒                      
                        ▒▓▓▓█████▓▓█▓█▓▓████████████████████████████▓█████▓███████▓▓▓▓███████▓▓▓▓▓▓▓██████████████████████▓▒                        
                        ▒▓▓██████▓▓███▓███████████████████████████▓▓██▓███████▓▓▓████████▓▓▓▓▓▓▓▓▓████████████████████▓▓▒                         
                            ▒▓▓▓███████████████████████████████████▓▓████████▓▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████▓▓▓                          
                                ▒██▓▓▓███████████████████████████▓▓▓▓███████▓▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓███████████████▓▓▓▒▒                         
                                ▓    ▓████████████████████████▓▓▓▓▓██████▓▓▓▓▓███▓▓████▓▓▓▓▓▓▓▓██████████████████████▓▓▓▓▒                        
                                        ▒█████████████████████▓▓▓▓▓▓▓███▓▓▓▓▓▓▓▓▓▓▓█████▓▓▓▓▓▓▓▓▓████████████▓▓▒▒▒                                  
                                        ███████████████████▓▓▓▓▓▓▓▓▓▓▓█▓▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓███████▓▓▒▒                                        
                                    ▒▓▓▓██████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓█████▓                                            
                                            ▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▒▒▒▒                                             
                                                                                                                                                    
                                                                                                                                                    
                                                                                                                                                    ";
            break;
        default:
            enemyArt = @"
▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▒▒▒▓▒▒▒▒▒▒▒                                                             ▒                         ▒▒▒▒▒▒▒▓▒▒▒▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▓▒▓▒▒▒▒▒▓▒▒▒▒▒▒                                                               ▒                         ▒▒▒▒▒▒▒▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▓▒▒▒▓▒▒▒▒▒▒▒▒                                       ▒▒▒▓▓▓▓▓▒▒▒▒     ▒▒▓      ▒▒                         ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                    ▒▒▓█▓▒                ▒▓▓▓      ▒                       ▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                                  ▒▓▓█▓                     ▒▓▓▒▓                            ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▒▒▒▒▒▒▒                                    ▒██▒▒                      ▓██▓                            ▒▒▒▒▒▒▒▒▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒                                 ▒  ▓█▒          ▓             ▒▓█▓▒▒▒                         ▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒                                  ▓▓██▒         ▒█▓             ▓██▓                           ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒                               ▓██▒         ▒███            ▒▒██▒                          ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒                             ▒██▒         ▓███▓            ▓██                          ▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒  ▒                           ▒▓▓███▒   ▒   ▓ ████▓▒▓          ▓██                          ▒▒▒▒▒▒▒▒▒▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒ ▒▒                                ███▒  ▓  ▒▒██████▓█  ▓  ▒▒ ▒▓██▓▒                          ▒▒▒▒▒▒▒▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                     ███▓▓▓ ▒▓▒███████▓▓▒█▒ ██████▒                             ▒▒▒▒▒▒▒▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▒▒▒▓▒▒▒▒▒▒▒▒▒▒                                        ▓█████▓█▓████████▓▓██▓███▓▒      ▒ ▒                       ▒▒▒▒▒▒▒▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒                                 ▓         ▒█████████████████▓██▓▓▒▒▒▒▒▒▒▓▓▓▓▒                       ▒▒▒▒▒▒▒▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒                                  ▓▓▒    ▓▒▒▒▓██▓███████████████▓█████▓█▓▓▒ ▒▒                         ▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒                          ▒▒        ▓▓█▓▓▓▓██████████████████████▓▓▓▓▓▒▒                               ▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒                           █▓▒     ▒▓█████████████████████████████▓▓▓▓▓▒▒▒▒▓▒                          ▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒▒                           ▒██▓▓▒▓▓▓█▓████████████████████████████▓▓▓█████▓▓▓▒▒▓                        ▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▒▓▓▓▒▓▒▒▒▒▒▒▒                       ▒▒   ▓████████████████████████████████████████████████▓▒▓▒▒▒    ▒                ▒▒▒  ▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▒▓▓▒▒▓▒▒▒▒▒▒▒                    ▒██▓██████████████████████████████████████████████████████▓▓▓▓▓▓▓▓██▓▒                    ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                   ▒▓  ▒███████████████████████████████████████████████████████▓████▓▒▒▓█▓                   ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒                      ▒ ▒▓█████████████████████████████████████████████████████████▓  ▒  ▒                 ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒                           ▓███████████████████████████████████████████████████████▒                       ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒                    ▒     ▒████████████████████████████████████████████████████████▒▒                    ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒                   ▓█▓▓█████████████████████████████████████████████████████████▓█▓▓▒▒               ▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒                     ▓███████████████████████████████████████████████████████████████▓▓▒▒           ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒                 ▒▒▓████████████████████████████████████████████████████████████████▒▓▒           ▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▒▓▓▓▓▒▒▒▒▒▒▒▒▒▒             ▒▓▒    ▒▓███████████▓████████████████████████████████████████████████████▓▓▓▒     ▒▒   ▒ ▒▒▒▒▒▒▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒             ▒████████████████▓  ▓████████████████████████████████████▓▓███████████████▓▓▓▒  ▒▒   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒               ▒▓█████████████▓   ████████████████████████████████████▓  ▓▒ ▓██████████████▓▓▓▓ ▒ ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒                ▒████████████▓▒▒▒ ▓███████████████████████████████████▓█▒ ▒  ▒  ▒▓████████████▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒       ▒▒▒  ▒▒▓▓███████████▓▒▒    ▓███████████████████████████████████▓██       ▒▓███████████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒           ▓▓▓█████████████▒▒▒▒   ▓████████████████████████████████████▓▓█      ▒▓██████████████▓███▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒            ▒██████████████████▒   ▒██████████████████████████████████████▓▓▒     ▓█▓▓██████████████████▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒        ▒███████████████▓▒▒     ▒███████████████████████████████████████▓▓    ▒▓   ▒████████████████████▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒   ▒▒▓▓██████████▓▓▒ ▒        ▓███████████████████████████████████████▓▓▓   ▒    ▒█▓▓▒▒▓█████████████▓▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒ ▒▓▓▓██████████▓▓  ▒           ▒█████████████████████████████████████████▓▓▒       ▓▓    ▓▒▒▓█████████████▓▓▒▒▒▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒ ▒▓████████▓▒                ▒▒▓███████████████████████████████████████████▓▒       ▓   ▒   ▒▒▒▒▒▓████████████▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▒▒▒████████▒                ▒██▓█████████████████████████████████████████████▓▒       ▒     ▒▒▒▒▒▒▒▒▓███████████▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓█▓████████▓▒            ▒▓████▓▓█████████████████████████████████████████████▓              ▒▒▒▒▒▒▒▒▒▓██████████▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▓█████▓███▓▓▒       ▒▒▓███████████████████████████████████████████████████████▒             ▒▒▒▒▒▒▒▒▒▒▒▒▓█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓█████▒▓███▓▒▒ ▒▓▓████████████████████████████████████████████████████████████▓▒      ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓████████▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▓▒▒▒▒▒▒▒▓████▓  ▓██▓▓▒▒▒▓██████████████████████████████████████████████████████████████▓▒        ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒███████▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓█████▒ ▒▓███▓▒▒▓█████████▓▓▓███████████████████████████████████████████████████▓▒  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓████▓▒  ▓█████████████▓▒▒▓█████████████████████████████████████████████████████▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓█████▓▓█████████████▓▒▒▒▓██████████████████████████████████████████████████████▓▓▓▒▒▒▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████████▓▒▒▒▒▒▓███████████████████████████████████████████████████████▓▓▓▒▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▓▒▒▓██▓███████████▓▓▓▒▒▒▒▓█████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒███████████████▓▓▒ ▒▒▒▒▒▓████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓████████████▓▓▓▒▒▒▒▒▒▒▒▒▒█████████████████████████████████████████████████████████████████▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓█████▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▒▓▒▒▒▓▓███████████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████████████████████████████████████████████████████████████▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▒▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████████████████████████████████████████████████████████████▓▓▓▓▒▒▒▒▒▒▒▒▓▒▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████████████████████████████████████████████████████████████▓▓▓▓█▓▒▒▒▒▒▒▒▒▓███████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓█████▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓████▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓█████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█▓▓██████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓███▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓████▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓█▓███████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓██▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓███▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓█▓██████████████████████████████████████████████████████████████████████████████████▓██▒▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓███▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓██████████████████████████████████████████████████████████████████████████████████▓▓██▓█▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓██▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓███████▓▓▓█████████████████████████████████████████████████████████████████████████████████▓▓▒▒▒▓▒▒▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓██▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓██▓▓▓████████████████████████████████████████████████████████████████████████████████████▓▓▒▒▓▒▒▒▓▓▓▓▓▓▓▓▓▓
▓▓▓▓█▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓█████▓▓▓█████████████████████████████████████████████████████████████████████████████████▓█▓▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓
▓▓▓█▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█████████▓▓▓██████████████████████████████████████████████████████████████████████████████████████▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓▓
▓▓▓█▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓████████████▓████████████████████████████████████████████████████████████████████████▓███████████████▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓████████████▓▓▓█████████████████████████████████████████████████████████████████████▓▒▓▓▒▒▓▓▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████████████████████████████████████████████████████████████████████████▓▒▓▓▒▒▒▒▒▒▓█████████████▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓██████▓▓████████████████████████████████████████████████████████████████████████▒▒▒▒▒▒▒▒▒▒▒▓██████████████▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓████████████████████████████████████▓███████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▒▓▓██████████████▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▒▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒██████████████████████████████████████▓███████████████████████████████████████████▓▓▒▒▒▒▒▒▒▒▒▒▒▓▓███████████████▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓█████████████████████████████████████████████████████████████████████████████████████▓▒▒▒▒▒▒▒▒▒▒▓▓▓█████████████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓████▓▓▓█████████████████████████████████████████████████████████████████████████████████▓▒▒▒▒▒▓▓▒▒▓▓▓▓▓███████████████
▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▒▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▒▒▒▓██████████████████████████████████████████████████████████████████████████████████▓▒▒▓▓▒▓▓▒▓▓▓▒▒▓██████████████
▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█████████████████████████████████████████████████████▓███████████████████████████████▓▒▓▓▒▒▓▓▓▓▓▒▒▓▓▓████████████
▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████▓██████████████████████████████████████████▓██████████████████████████████▓▓▓▓▓▒▓▓▓▓▓▒▒▓▓▓▓▓██████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████▓▒▒███████████████████████████████████████████▓██████████████████████████████▓▓▓▓▒▓▓▓▓▓▓▒▓▓▓▓▓▓▓████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▓███████████▓▒▒▒▓███████████████████████████████████████████▓▓▓████████████████████████████▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒▒▒▒▒▒▓███████████▓▒▒▒▒▒▓████████████████████████████████████████████▓▓▓▓▓███▓▓██▓▓████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒▒▒▒▓▓████████▓▓▒▓▒▒▒▒▒▓█████████████████████████████████████████████▓▓▓▓▒▒▓█▓▒▓▓▒▒▓▓▓▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓██████████▒▒▒▒▒▒▒▒▒▓███████████████████████████████████████████████▓▓▓▒▒▒▓▓▒▒▓▒▒▓▒▒▓█▓███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▒▒▓██████████▓▒▒▒▒▒▒▒▒▒▓███▓▓▓▓██████████████████████████████████████████▓▓▒▒▒▒▒▒▒▒▓▒▓▓▓▓▓▓▓▓███████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓██▓███████████▒▒▒▒▒▒▒▒▒▒▓███▓▓▒▓███████████████████████████████████████████▓▓▓▒▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓█▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓███████████████▓▒▒▒▒▒▒▒▒▒▓▓██▓▒▒▒▓████████████████████████████████████████████▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████▓▓▒▒▒▒▒▒▓▓▓▓▓▓▒▒▒▓███████████████████████████████████████████▓▓▓▓▓▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓▓███████████████████████████▓▒▒▓▓▓▓▒▓▓▒▒▒▓██████████████████████████████▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████▓▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓▓▓█████████████████████████████▓▓▓▓▒▒▒▒▒▒▒▓▓██████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▓▓▓▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓▓██████▓█████████████▓████████████▓▓▒▒▒▒▒▒▓▓███████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████▓▓▓▓▓▓▓▓▓
▓▓▓▓▓▓██████▓▓▓▓██████████▓▓█▓▓██████████▓▒▒▒▒▒▓▓█████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████▓▓▓▓▓▓▓▓
▓▓▓▓▓████▓▓▓▓▓▓▓██████████▓▒▓▓▓▓▓▓▓▒▓▓▓███▓▓▒▒▒▓▓█████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████▓▓▓▓▓▓
▓▓▓▓▓████▓▓▓▓▓▓█████▓▓████▓▓▓▓▒▒▓▓▓▒▒▒▒▒▒▓▓█▓▓▓▓▓██████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████▓▓▓▓▓
▓▓▓▓███▓▓▓▓▓▓▓▓████▓▓▓▓███▓▓▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒▓▓▓▓▓▓███████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████▓▓▓▓
▓▓▓███▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▓▓█████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓▓
▓▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▓▓█████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓▓
▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓██████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓
▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓▓▓
██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████
█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓████████████████████████████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓█████████████████████████████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████████████████▓████████████████████████████████▓▓▓▓▓▓▓▓▓█████████████████████████████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████████████████████████████████▓██████████████████████████████▓▓▓▓▓▓▓▓▓█████████████████████████████████
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓███████████████████████████████
█▓▓▓██▓██▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓█████████████████████████████
████████████████████▓█▓▓▓▓▓▓▓▓▓▓██▓████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓██████████████████████████
████████████████████████▓▓▓█▓███████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████████████
████████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████████████
████████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓███████████████
█████████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓█████████████
█████████████████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████████
██████████████████████████████████████████████████████████████████████████████████████████████████████████▓████▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓███████████
█████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓█████▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓███████████████████
███████████████████████████████████████████████████████████████████████████████████████████████████████▓▓▓▓▓████████████████▓▓▓▓▓▓▓███████████████████";
            break;
        }
        if (enemy.IsBoss)
            VaryPrint(enemyArt, 200);
        else
            Console.WriteLine(enemyArt);
        Console.WriteLine();
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
                        WriteTile(". ", ConsoleColor.DarkGray);
                        break;
                    case TileType.Wall:
                        WriteTile("# ", ConsoleColor.DarkGray);
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
                    }
                }

                // Updates interface
                Interface.Clear();
                player.UpdateStats();
                UpdateInterface(new(){{0, $"   Controls: Movement (WASD), Inventory (1-9), Quit (Q), Spend Points (P)"}, {21, Menu}});
                UpdateInterface(player.GetInventory());
                UpdateInterface(player.GetStats());
                
                // Prints interface
                foreach (KeyValuePair<int, string> kvp in Interface)
                    if (i == kvp.Key && j == ySize-1) Console.Write(kvp.Value);

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

public static void DisplayTitle() {
    string Title = @" ▄████████    ▄█    █▄     ▄██████▄   ▄██████▄     ▄████████    ▄████████         ▄████████      ████████▄     ▄████████  ▄█      ███     ▄██   ▄   
███    ███   ███    ███   ███    ███ ███    ███   ███    ███   ███    ███        ███    ███      ███   ▀███   ███    ███ ███  ▀█████████▄ ███   ██▄ 
███    █▀    ███    ███   ███    ███ ███    ███   ███    █▀    ███    █▀         ███    ███      ███    ███   ███    █▀  ███▌    ▀███▀▀██ ███▄▄▄███ 
███         ▄███▄▄▄▄███▄▄ ███    ███ ███    ███   ███         ▄███▄▄▄            ███    ███      ███    ███  ▄███▄▄▄     ███▌     ███   ▀ ▀▀▀▀▀▀███ 
███        ▀▀███▀▀▀▀███▀  ███    ███ ███    ███ ▀███████████ ▀▀███▀▀▀          ▀███████████      ███    ███ ▀▀███▀▀▀     ███▌     ███     ▄██   ███ 
███    █▄    ███    ███   ███    ███ ███    ███          ███   ███    █▄         ███    ███      ███    ███   ███    █▄  ███      ███     ███   ███ 
███    ███   ███    ███   ███    ███ ███    ███    ▄█    ███   ███    ███        ███    ███      ███   ▄███   ███    ███ ███      ███     ███   ███ 
████████▀    ███    █▀     ▀██████▀   ▀██████▀   ▄████████▀    ██████████        ███    █▀       ████████▀    ██████████ █▀      ▄████▀    ▀█████▀ ";
    VaryPrint(Title, 148);
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

public static void DisplayArt() {
    Console.ForegroundColor = ConsoleColor.Red;
    VaryPrint(@"                                                                                                                                                                                               
                                                                            ▒         ▒                               ▒▒ ▒                                                                              
                                                                             ▓▓▓  ▒   ▒      ▒ ▒               ▒   ▒ ▒▓▓▓                                                                               
                                                                               ▓▓▒▓▒   ▒▒▓   ▓▓▒      ▒▓   ▒▒▒▒   ▓▒▒▓▒                                                                                 
                                                                                ▓▓▒     ▓▒▒▒▓▓▒        ▓▓▓▒▒▓▒     ▓▓▒                                                                                  
                                                                                ▓▓▒    ▒▓▓▓▒             ▒▓▓▓▓     ▓▓                                                                                   
                                                                                 ▓▓▓▒▒▓▓▓                   ▒▓▓▒▒▓▓▓▒                                                                                   
                                                                                   ▒▓▓▓▓                     ▓▓▓▓▒    ▒                                                                                 
                                                                               ▒▒▒▒ ▒▓▓▓  ▓▒              ▓  ▓▓▓▓  ▒▒▒▒                                                                                 
                                                                                  ▒▓ ▒▓▓▒ ▓▒  ▓▓▓▓▓▓▓▓▓▒ ▒▓ ▒▓▓▓ ▒▒                                                                                     
                                                      ▒                            ▒▓  ▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▓▒ ▒▓                     ▒  ▒▒▒▒▓▓▓▓▓▒▓▓▓                                                 
                                                 ▒▒▓▓▓▓▓▓▓▒▒                        ▓▓▓▓█▓▓█▓▓▓▒▒▒▒▒▒▒▒▓▓██▓██▓▓▓▒                   ▒▒▒▒▒▓▓▓▓▒▒▓█▓▓▓▓▓▓▒▒▒                                             
                                               ▒▒▓▓█▓▒▒▒▒▒▒▒▒                          ▒▓█▓█▓▓▓▒▒▒▒▒▒▒▓▓▓███▓▒                     ▒▒▒█▓▓▓▓▓▓▓▓▓██▓▒▓▓█▓▓▓▓▒                                            
                                              ▒▓▓▓█▓▒▓▓▒▒▒▒▒▓▓                           ▓▓█▓▓▓▓▒▒▒▒▒▒▓▓▓██▓▒                     ▒▒▓▓▓██▓▒▒▒▒▒▒▒▒▓▓▓██▓▓█▓▓▒▒                                          
                                              ▓▓██▓▓▒▒▒▓▓▒▒▓▓▓▓                           ▒███▓▓▒▒▒▒▒▒▓███▓                     ▒▓▓▒▓▓█▓▒▒▒▒▒▒▒▒▒▒▒▓▓█████▓█▓▒                                          
                                             ▒▓██▓▓▓▒▒▒▒▓▓▓▓▓▓▓                            ▓█▓██▓▓▓▒▓██▓▓█▓                      ▓▓▓██▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓███████▓▒                                          
                                             ▒███▓▓▓▒▒▒▒▓▓▓▓▓▓▓▒                           ▓███▓█▓▓▓██▓██▓▒                       ▓▓▓▓▓▒▒▒▒▒▓▒▒▒▒▓▓▓▓█████▓▓▓▒                                          
                                             ▓████▓▓▒▒▒▓▓▓▓▓▓▓▓▓                           ▒▓██████▓█████▓                         ▒▓▓▓▒▒▒▓▓▒▒▒▒▒▓▓██▓██▓▓▓▓▒                                           
                                             ▓████▓▓▒▒▓▓▓▓█▓▓▓▓▓                            ▓███████████▓                            ▒▒▒▒▓███▒▒▒▓▓█████▓▒▒▒▒                                            
                                             ▓██████▓▓▓▓▓▓█▓▓▓▓▓▒                         ▒▓▓████▓▓▓▓▓███▓▒                          ▒▒▒▓█▓▓▓▒▒▒▓▓▓▓███▓  ▒▒                                            
                                            ▒▓██████▓▓▓▓▓██▓▓▓█▓▓▒                     ▒▒▓███████▓▓▓███████▓▓▒                       ▒▒▓███▓▓▓▓▓▓██████▓                                                
                                            ▒▓██████▓▓▓▓████▓██▓█▓▒                 ▒▒▓▓▓▓▓████████████████████▓▒                    ▒▒▒████▓▓▓▓███████▓                                                
                                            ▓▓██████▓██████▓██▓▓▓▓▓▒          ▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓█████████████▓██▓▓▓▓▓▓▓▒▒▒             ▒▒▒▓█████▓▓▓███████▓▒                                               
                                           ▓▓▓███████████████▓▓▓▓▓▓▓▓▒▒    ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒        ▒▒▒▒▒▓██████▓▓███████▓▒                                               
                                         ▒▒▓████████████████▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓██▓█▓▒▒▓▓▓▓▓▒▒▒▒▒▒▒▒▒▓▓▓▓▓▓     ▒▒▒▒▒▓▓███████████████▓▓                                               
                                        ▓▓▓▓████████████▓▓▓▓▓▓▓▒▒▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▒▓█▓▓▓▓▓▓██▓▓▒▒▓▓▓▓▓▓▓▓██▓▒▓▓▒▒▒▒▒▓▓▓▓▓▓▓▓██▓▒ ▒▒▒▒▒▒▓▒▓███▓███████████▓▓▒                                              
                                     ▒▓▓▓▓▓████████████████▓██▓▓▓▓▓▓▓▒▓███▓▓██▓▓▒▒▓▓▓▓███▓▓▒▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▓▓▓▓▓▓▓▓▓████▓▒▒▒▒▒▒▒▒▓▓█▓▓▓▓▓██████████▓▓▓▒                                            
                                   ▒▓███████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▓▓▓████▓▓▓▒▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓▓█▓▓▓▓▓▓▓██████████▓▒▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓███████████▓▓▒                                          
                                 ▒▓▓▓██████████████████████▓███▓▓▓▓▓▓▓████████████████▓▓▓▓▒▒▒▒▒▓▓▓▓▓▓▓▒▒▒▒▒▓▓▓███▓▓▓▓▓▓███████▒▒▒▒▒▒▒▒▒▒▓▓▓▒▓▓▓▓▓▓▓▓█████████▓▓▓                                        
                                ▒▓▓████████████████████▓▓▓██▓▓▓▓▓▓▓▓▓▓█████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████████████▓▒▒▒▒▒▒▒▒▒▓▓█▓█▓▓▓▓▓▓▓████████████▓▒                                      
                                ▒████████████████▓▓██▓▓██▓▓▓█████████▓███████▓███████████▓▓▓▓▓▓▓████▓▓▓▓▓▓▓▓▓█████████████████▓▓▓▓▓▓▓▓▓▒▒▓█▓▓▓▒▒▒▒▒▒▓▓▓███████████                                      
                                ▒▓██████████▓█▓▓▓▓▓█▓█▓▓█████████████▓███████▓████████████████████████████████████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓▓▓█████████▒                                     
                                ▓█████████████▓▓▓▓▓███▓▓▓████▓███████▓█████████████████████████▓▓▓▒▒▓███████████████████▓▓████▓▓▓▓▓▓▓▓███▓▓▓▓▒▒▒▒▒▒▒▒▒▒▓▓█████████                                      
                                ▓█████████████████▓████▓███▓███████████████████████████████▓▓▓▓▓▓▓▓▓▓▓██▓▓▓██████████▓▒▓██████▓▓▓▓█▓▓██▓▓▒▓▓▓▓▒▒▒▒▒▒▒▓▓▓▓▓████████▒                                     
                               ▓█▓████████████████▓▓█████████████████▓██████▓▓███████████████▓▓███████▓▓██████████████▓▓▓▓████▓▓▓▓████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████                                      
                               ▒▓█████████████████▓██████████████████▓████▓▓▓▓▓█████████████████▓▓▓▓▓█████████████████▓▓▓▓████▓▓██████▓▓▓▓▓██▓▓▓▓▓▓▓▓▓█▓▓█████████▓                                     
                               ▒█████████████████████████████████████▓████▓▓▓▓███▓ ███████████████████████████████▒████▓▓▓████████████▓▓▓████████▓▓▓██████████████▒                                     
                               ▓▓████████████████████████████████████▓███████████   ████████████▓▓▓▒▓▓███████████  ▒████████████████████▓█████████████████████████▒                                     
                               ▓████████████████████████████████████▓███████████     ███████████████████████████    ▓███████████▓█████████████████████████████████▓                                     
                              ▓▓████████████████████████████████████▓███▓██████      ▒█████████████████████████▒     ███████████▓▓█████████████████████████████████▓                                    
                              ▓████████████████████████████████████████████████     ▒▓███████████▓▓▓▓██████████▓▒    ████████████▓█████████████████████████████████▓▒                                   
                             ▓▓███████████████████████████████████▓▓████▓██████    ▓█████████████████████████████▒   ▓████▓▓▓▓██████████████████████████████████████▓▒                                  
                            ▓▓▓▓▓███████████████████████████████████████▓██████    ▓██████████████████████▓██████▒   ████▓▓▓█▓████▓▓▓███████████████████████████████▓▓                                  
                           ▓▓██▓████████████████████████████████████████▓██████▒   ██████████████████████████████▓   ████▓▓▓▓█████▓▓▓▓█▓▓███████████████████████▓▓▓▓▓▓▓                                 
                           ▓█▓▓██▓██████▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████▓▓█████████████   ▒████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████▒  ██████▓██████▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓█▓▓▓▓▓▓▓▓                                
                         ▒▓▓▓▓▓▓▓▓██▓▓██▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓████▓▓▓▓▓████████████  ▒██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓██████▓▓▓████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                                
                        ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███████████▒  ▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▒                              
                         ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██████████  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒                               
                                               ▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                                         
                                                 ▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓ ▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                                         
                                                       ▒▒▒▒▒▒▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒             ▒▒▒▒▒▒▒▒▒▒▒▒▓▓▓▓▒▒▒▒▒▓▓▓▒▒▓▓▓▓▓▓▓▓▓▓▒▓▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒                                         ", 200);
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



public enum TileType { Empty, Wall, Portal, HealingPotion, ManaPotion, Gold }
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
public static RoomGenerator RoomGen = new();    public static List<Enemy> enemies = [];
public static bool RoomClear = false;
public static Random RNG = new();
public static string Menu = "";
public static char input = ' ';

public static Dictionary<int, string> Interface = [];

// Combat global variables
public static bool IsOver = false;
public static int Turn = 0;

public class Deity(int tspeed = 35, int tduration = 450, ConsoleColor color = ConsoleColor.White, string name = "???") {
    public int tspeed = tspeed, tduration = tduration;
    public ConsoleColor color = color;
    public string name = name;
    public void Talk(string str) {
        Program.Print(str, tspeed, tduration, color, name);
    }
}
}