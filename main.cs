// Description:
// This is a game engine for a text based monster fight game
//
// List of design patterns used
// [Credit anything borrowed is listed next to each design in parentheses]
// 1. Singleton     for saving      (based on class example)
// 2. Prototype     for characters  (based on class example, Borderlands game, a classmate)
// 3. Adapter       for weapons     (based on class example, Borderlands game)
// 4. Factory       for monsters    (based on class example, Borderlands game)
// 4. Interpreter   for fight hits  (based on class example)
//
// Game description:
// 1. Game start is logged (initial save)
// 2. User picks from available playable characters
// 3. User picks from available weapons
// 4. Game selections are logged (settings save)
// 5. Monsters are created
// 6. User starts a fight with monsters
// 7. Weapon power is multiplied by hit type (hard or soft)
// 8. User can call magic (such as to undo damage), if available
// 9. Game results are logged (fight result save)
//
// Author:
// Stan Turovsky

using System;
using System.Collections.Generic;

namespace GameEngine
{
    // Singleton design pattern start
    class Log
    {
        private static Log _instance;
        public static object SyncRoot = new object();

        public static Log GetInstance()
        {
            lock (SyncRoot)
            {
                if (_instance == null)
                    _instance = new Log();
            }

            return _instance;
        }

        public void WriteLog(string msg)
        {
            Console.WriteLine("Writing out to the game log... " + msg);
        }
    }
    // Prototype design pattern
    public interface ICharacter
    {
        ICharacter Clone();
        string GetDetails();

    }
    public class Character
    {
        public int HealthRechargePerMinute { get; set; }
        public string Name { get; set; }
        public string Race { get; set; }
        public string PreferredArmor { get; set; }
    }
    class Berserker : Character, ICharacter
    {
        public ICharacter Clone()
        {
            return (ICharacter)MemberwiseClone();
        }

        public string GetDetails()
        {
            return $"{Name} - {Race} - {PreferredArmor}";
        }
    }
    class Siren : Character, ICharacter
    {
        public ICharacter Clone()
        {
            return (ICharacter)MemberwiseClone();
        }

        public string GetDetails()
        {
            return $"{Name} - {Race} - {PreferredArmor} - {HealthRechargePerMinute} Health Per Min";
        }
    }

    //Adapter design pattern
    public interface IWeapons
    {
        List<string> GetWeaponList();
    }

    public class GamingSystem
    {
        public string[][] GetWeapons()
        {
            string[][] weapons = new string[2][];
            weapons[0] = new[] { "Shotgun", "Berkserker", "New" };
            weapons[1] = new[] { "Sword", "Siren", "Used" };
            return weapons;
        }
    }
    public class WeaponAdapter : GamingSystem, IWeapons
    {
        public List<string> GetWeaponList()
        {
            List<string> weaponList = new List<string>();
            foreach (string[] weapon in GetWeapons())
            {
                weaponList.Add(weapon[0]);
                weaponList.Add("\t for ");
                weaponList.Add(weapon[1]);
                weaponList.Add("\t");
                weaponList.Add(weapon[2]);
                weaponList.Add(" condition \n");

            }
            return weaponList;
        }
    }
    public class WeaponListingSystem
    {
        private IWeapons weaponSource;
        public WeaponListingSystem(IWeapons weaponSource)
        {
            this.weaponSource = weaponSource;
        }
        public void ShowWeaponList()
        {
            Console.WriteLine("==> Weapon Choices <==");
            foreach (string weapon in weaponSource.GetWeaponList())
            {
                Console.Write(weapon);
            }
            Console.WriteLine(new String('-', 50));
        }
    }

    // Factory design pattern
    abstract class Monster
    {
        public abstract string Title { get; }
    }

    class Guardian : Monster
    {
        public override string Title
        {
            get { return "Guardian"; }
        }
    }
    class Bandit : Monster
    {
        public override string Title
        {
            get { return "Bandit"; }
        }
    }
    class Spider : Monster
    {
        public override string Title
        {
            get { return "Spider"; }
        }
    }
    class MonsterFactory
    {
        public static Monster Get(int id)
        {
            switch (id)
            {
                case 0:
                    return new Guardian();
                case 1:
                case 2:
                    return new Bandit();
                default:
                    return new Spider();
            }
        }
    }

    // Interpreter design pattern
    class HitPower
    {
        public int Number;
    }
    abstract class AbstractInterpret
    {
        public abstract void Interpret(HitPower damage, bool undoFlag = false);
    }
    class HardHit : AbstractInterpret
    {
        public override void Interpret(HitPower damage, bool undoFlag = false)
        {
            int numberToAdd = 10;
            if (undoFlag)
                numberToAdd *= -1;
            Console.WriteLine("Hard Hit x10");
            damage.Number += numberToAdd;
        }
    }
    class SoftHit : AbstractInterpret
    {
        public override void Interpret(HitPower damage, bool undoFlag = false)
        {
            int numberToAdd = 2;
            if (undoFlag)
                numberToAdd *= -1;
            Console.WriteLine("Soft Hit x2");
            damage.Number += numberToAdd;
        }
    }


    class Program
    {
        static void Main()
        {
            // Call Singleton (based on class example)
            Log.GetInstance().WriteLog("Initial Save");

            // Separator
            Console.WriteLine(new String('=', 50));
            Console.WriteLine("==> Playable Characters <==");
            Console.WriteLine(new String('-', 50));

            // Call Prototype (based on class example, Borderlands characters, and a classmate)
            Berserker ber1 = new Berserker();
            ber1.Name = "Brick";
            ber1.Race = "Human";
            ber1.PreferredArmor = "Breast Plate";

            Berserker ber2 = (Berserker)ber1.Clone();
            ber2.Name = "Mordecai";
            ber2.PreferredArmor = "Chainmail";

            Console.WriteLine(ber1.GetDetails());
            Console.WriteLine(ber2.GetDetails());

            Siren siren1 = new Siren();
            siren1.Name = "Lilith";
            siren1.Race = "Siren";
            siren1.PreferredArmor = "Shield";
            siren1.HealthRechargePerMinute = 120;

            Siren siren2 = (Siren)siren1.Clone();
            siren2.Name = "Natasha";
            siren2.Race = "Super Siren";

            Console.WriteLine(siren1.GetDetails());
            Console.WriteLine(siren2.GetDetails());

            // Separator
            Console.WriteLine(new String('=',50));

            // Call Adapter (based on class example)
            IWeapons weaponWeapons = new WeaponAdapter();
            var weapons = new WeaponListingSystem(weaponWeapons);
            weapons.ShowWeaponList();

            // Call Singleton (based on class example)
            Log.GetInstance().WriteLog("Save Settings");

            // Separator
            Console.WriteLine(new String('=', 50));
            Console.WriteLine("==> Monsters <==");
            Console.WriteLine(new String('-', 50));

            // Call Factory (based on class example)
            for (int i = 0; i < 4; i++)
            {
                Monster monster = MonsterFactory.Get(i);
                Console.WriteLine($" #{i+1} - {monster.Title}");
            }

            // Separator
            Console.WriteLine(new String('=', 50));
            Console.WriteLine("==> Fight Results <==");
            Console.WriteLine(new String('-', 50));

            // Call Interpreter (based on class example)
            HitPower damage = new HitPower();
            int SelectedWeaponPower = 100;          // this would be extracted weapon selection

            List<AbstractInterpret> hits = new List<AbstractInterpret>();
            hits.Add(new HardHit());
            hits.Add(new HardHit());
            hits.Add(new SoftHit());
            hits.Add(new HardHit());
            hits.Add(new SoftHit());

            foreach (AbstractInterpret exp in hits)
                exp.Interpret(damage);

            Console.WriteLine($"Damage: {damage.Number * SelectedWeaponPower}");

            Console.WriteLine(new String('-', 50));
            Console.WriteLine("Calling Magic 'Undo Damage'");
            Console.WriteLine(new String('-', 50));

            hits.Reverse();

            foreach (AbstractInterpret exp in hits)
                exp.Interpret(damage, true);

            Console.WriteLine($"Damage: {damage.Number * SelectedWeaponPower}");

            // Call Singleton (based on class example)
            Console.WriteLine(new String('=', 50));
            Log.GetInstance().WriteLog("Save Fight Results");
            Console.WriteLine(new String('-', 50));
        }
    }
}
