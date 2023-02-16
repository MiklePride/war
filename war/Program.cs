using System;
using System.Collections.Generic;

namespace war
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Battlefield battlefield = new Battlefield();
            battlefield.StartBattle();
        }
    }
}

class Battlefield
{
    private Platoon _country1;
    private Platoon _country2;

    public Battlefield()
    {
        _country1 = new Platoon(new Stormtrooper(), new Juggernaut(), new Sniper());
        _country2 = new Platoon(new Marine(), new Gunner(), new SoldierOfFortuna());
    }

    public void StartBattle()
    {
        Console.WriteLine("Битва начинается!");

        while (_country1.IsAlive && _country2.IsAlive)
        {
            Console.WriteLine("Атакуют солдаты страны 1:");
            _country1.Attack(_country2);

            if (_country2.IsAlive == false)
                continue;

            Console.WriteLine("Атакуют солдаты страны 2:");
            _country2.Attack(_country1);
        }

        ShowWinner();
        Console.ReadLine();
    }

    private void ShowWinner()
    {
        if (_country1.IsAlive)
        {
            Console.WriteLine("Победила страна 1!");
        }
        else
        {
            Console.WriteLine("Победила страна 2!");
        }
    }
}

class Platoon
{
    private List<Soldier> _soldiers = new List<Soldier>();

    public Platoon(Soldier soldier1, Soldier soldier2, Soldier soldier3)
    {
        _soldiers.Add(soldier1);
        _soldiers.Add(soldier2);
        _soldiers.Add(soldier3);

        IsAlive = true;
    }

    public bool IsAlive { get; private set; }

    public void Attack(Platoon platoon)
    {
        int enemySoldiersCount = platoon.GetSoldierCount();

        for (int i = 0; i < _soldiers.Count; i++)
        {
            for (int j = 0; j < enemySoldiersCount; j++)
            {
                Soldier enemySoldier = platoon.GetSoldier(j);

                if (enemySoldier.IsAlive)
                    _soldiers[i].Attaсk(enemySoldier);
            }
        }

        platoon.RemoveDeadSoldiers();
    }

    private Soldier GetSoldier(int indexSoldier)
    {
        return _soldiers[indexSoldier];
    }

    private int GetSoldierCount()
    {
        return _soldiers.Count;
    }

    private void Die()
    {
        IsAlive = false;
    }

    private void RemoveDeadSoldiers()
    {
        for (int i = 0; i < _soldiers.Count; i++)
        {
            if (_soldiers[i].IsAlive == false)
            {
                _soldiers.RemoveAt(i);
                i--;
            }
        }

        if (_soldiers.Count == 0)
            Die();
    }
}

abstract class Soldier
{
    protected static Random Random = new Random();

    protected string Name;
    protected string Ability;

    protected int MaximumHealth;
    protected int Health;
    protected int Armor;

    protected int HitChance;
    protected int ChanceAbilityTrigger;
    protected int MaximumRandomChance = 100;

    protected bool IsAbilityApplied = false;

    public bool IsAlive { get; protected set; }
    public int Damage { get; protected set; }

    public virtual void Attaсk(Soldier solder)
    {
        if (IsAbilityTrigger())
            EnableAbility();

        Console.WriteLine($"{Name} стреляет.");

        if (IsHit())
        {
            solder.TakeDamage(Damage);
        }
        else
        {
            Console.WriteLine("Промах!");
        }

        if (IsAbilityApplied)
            DisableAbility();
    }

    public virtual void TakeDamage(int damage)
    {
        int currentDamage;
        int alphaArmor = 100;
        int alphaDamage = 100;

        currentDamage = damage * (alphaArmor - Armor) / alphaDamage;
        Health -= currentDamage;

        IsAlive = Health > 0;

        if (IsAlive)
        {
            Console.WriteLine($"{Name} получает урон в размере {currentDamage} единиц.");
        }
        else
        {
            Health = 0;
            IsAlive = false;

            Console.WriteLine($"{Name} убит!");
        }
    }

    protected virtual void EnableAbility()
    {
        IsAbilityApplied = true;
    }

    protected virtual void DisableAbility()
    {
        IsAbilityApplied = false;
    }

    protected bool IsAbilityTrigger()
    {
        return ChanceAbilityTrigger > Random.Next(MaximumRandomChance);
    }

    protected bool IsHit()
    {
        return HitChance > Random.Next(MaximumRandomChance);
    }
}
#region Solders country1 
class Stormtrooper : Soldier
{
    public Stormtrooper()
    {
        Name = "Штурмовик";
        Ability = "Стрельба очередью";

        MaximumHealth = 1800;
        Health = MaximumHealth;

        Armor = 30;
        Damage = 50;

        ChanceAbilityTrigger = 15;
        HitChance = 50;

        IsAlive = true;
    }

    public override void Attaсk(Soldier solder)
    {
        int shotsCount = 3;

        if (IsAbilityTrigger())
            EnableAbility();

        Console.WriteLine($"{Name} стреляет.");

        if (IsHit())
        {
            if (IsAbilityApplied)
            {
                for (int i = 0; i < shotsCount; i++)
                {
                    solder.TakeDamage(Damage);
                }
            }
            else
            {
                solder.TakeDamage(Damage);
            }
        }
        else
        {
            Console.WriteLine("Промах!");
        }

        if (IsAbilityApplied)
            DisableAbility();
    }
}

class Juggernaut : Soldier
{
    public Juggernaut()
    {
        Name = "Джаггернаут";
        Ability = "Силовое поле";

        MaximumHealth = 2000;
        Health = MaximumHealth;

        Armor = 40;
        Damage = 75;

        ChanceAbilityTrigger = 20;
        HitChance = 35;

        IsAlive = true;
    }

    public override void Attaсk(Soldier solder)
    {
        Console.WriteLine($"{Name} стреляет.");

        if (IsHit())
        {
            solder.TakeDamage(Damage);
        }
        else
        {
            Console.WriteLine("Промах!");
        }
    }

    public override void TakeDamage(int damage)
    {
        if (IsAbilityTrigger())
            EnableAbility();

        if (IsAbilityApplied)
        {
            Console.WriteLine($"{Name} использовал {Ability} и отразил атаку.");

            DisableAbility();
        }
        else
        {
            base.TakeDamage(damage);
        }
    }
}

class Sniper : Soldier
{
    private int _damageMultiplier = 4;

    public Sniper()
    {
        Name = "Снайпер";
        Ability = "Выстрел в голову";

        MaximumHealth = 1500;
        Health = MaximumHealth;

        Armor = 25;
        Damage = 100;

        ChanceAbilityTrigger = 30;
        HitChance = 75;

        IsAlive = true;
    }

    protected override void DisableAbility()
    {
        base.DisableAbility();

        Damage /= _damageMultiplier;
    }

    protected override void EnableAbility()
    {
        base.EnableAbility();

        Damage *= _damageMultiplier;
    }
}
#endregion

#region Solders Country2
class Marine : Soldier
{
    private int _healing = 50;

    public Marine()
    {
        Name = "Морпех";
        Ability = "Самолечение";

        MaximumHealth = 1300;
        Health = MaximumHealth;

        Armor = 25;
        Damage = 50;

        ChanceAbilityTrigger = 30;
        HitChance = 50;

        IsAlive = true;
    }

    protected override void EnableAbility()
    {
        base.EnableAbility();

        Health += _healing;

        if (Health > MaximumHealth)
            Health = MaximumHealth;
    }
}

class Gunner : Soldier
{
    private int _numberOfShotsPerTurn = 5;
    private int _armorMultiplier = 2;
    private int _numberOfMoves = 5;
    private int _movesCount = 0;

    public Gunner()
    {
        Name = "Пулеметчик";
        Ability = "Укрепление брони";

        MaximumHealth = 1900;
        Health = MaximumHealth;

        Armor = 20;
        Damage = 20;

        ChanceAbilityTrigger = 25;
        HitChance = 75;

        IsAlive = true;
    }

    public override void Attaсk(Soldier solder)
    {
        if (IsAbilityApplied == false)
        {
            if (IsAbilityTrigger())
                EnableAbility();
        }

        Console.WriteLine($"{Name} стреляет.");

        for (int i = 0; i < _numberOfShotsPerTurn; i++)
        {
            if (IsHit())
            {
                solder.TakeDamage(Damage);
            }
            else
            {
                Console.WriteLine("Промах!");
            }
        }

        if (IsAbilityApplied == true && _movesCount == _numberOfMoves)
        {
            DisableAbility();
            _movesCount = 0;
        }
        else
        {
            _movesCount++;
        }
    }

    protected override void DisableAbility()
    {
        IsAbilityApplied = false;
        Armor /= _armorMultiplier;
    }

    protected override void EnableAbility()
    {
        IsAbilityApplied = true;
        Armor *= _armorMultiplier;
    }
}

class SoldierOfFortuna : Soldier
{
    public SoldierOfFortuna()
    {
        Name = "Солдат удачи";
        Ability = "Фортуна";

        MaximumHealth = 1000;
        Health = MaximumHealth;

        Armor = 25;
        Damage = 150;

        ChanceAbilityTrigger = 70;
        HitChance = 40;

        IsAlive = true;
    }

    public override void Attaсk(Soldier solder)
    {
        Console.WriteLine($"{Name} стреляет.");

        if (IsHit())
        {
            solder.TakeDamage(Damage);
        }
        else
        {
            Console.WriteLine("Промах!");
        }
    }

    public override void TakeDamage(int damage)
    {
        if (IsAbilityTrigger())
            EnableAbility();

        if (IsAbilityApplied)
        {
            Console.WriteLine($"{Name}: Сработала {Ability}, урон не получен.");
            DisableAbility();
        }
        else
        {
            base.TakeDamage(damage);
        }

    }
}
#endregion