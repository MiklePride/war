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
    private List<Soldier> _platoonCountry1 = new List<Soldier>();
    private List<Soldier> _platoonCountry2 = new List<Soldier>();

    public Battlefield()
    {
        _platoonCountry1.Add(new Stormtrooper());
        _platoonCountry1.Add(new Juggernaut());
        _platoonCountry1.Add(new Sniper());

        _platoonCountry2.Add(new Marine());
        _platoonCountry2.Add(new Gunner());
        _platoonCountry2.Add(new SoldierOfFortuna());
    }

    public void StartBattle()
    {
        Console.WriteLine("Битва начинается!");

        while (_platoonCountry1.Count > 0 && _platoonCountry2.Count > 0)
        {
            Console.WriteLine("Атакуют солдаты страны 1:");

            MakeAttaсkPlatoon(_platoonCountry1, _platoonCountry2);

            if (_platoonCountry2.Count == 0)
                continue;

            Console.WriteLine("Атакуют солдаты страны 2:");

            MakeAttaсkPlatoon(_platoonCountry2, _platoonCountry1);
        }

        ShowWinner();
        Console.ReadLine();
    }

    private void MakeAttaсkPlatoon(List<Soldier> attackingPlatoon, List<Soldier> defendingPlatoon)
    {
        for (int i = 0; i < attackingPlatoon.Count; i++)
        {
            for (int j = 0; j < defendingPlatoon.Count; j++)
            {
                if (defendingPlatoon[j].GetAliveStatus())
                    attackingPlatoon[i].Attaсk(defendingPlatoon[j]);
            }
        }

        RemoveDeadSoldiers(defendingPlatoon);
    }

    private void RemoveDeadSoldiers(List<Soldier> soldiers)
    {
        for (int i = 0; i < soldiers.Count; i++)
        {
            if (soldiers[i].GetAliveStatus() == false)
            {
                soldiers.RemoveAt(i);
                i--;
            }
        }
    }

    private void ShowWinner()
    {
        if (_platoonCountry2.Count == 0)
        {
            Console.WriteLine("Победила страна 1!");
        }
        else
        {
            Console.WriteLine("Победила страна 2!");
        }
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
    protected int Damage;

    protected int HitChance;
    protected int ChanceAbilityTrigger;
    protected int MaximumRandomChance = 100;

    protected bool IsAlive = true;
    protected bool IsAbilityApplied = false;

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

        currentDamage = damage * (100 - Armor) / 100;
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

    public bool GetAliveStatus()
    {
        return IsAlive;
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
    }

    public override void Attaсk(Soldier solder)
    {
        if (IsAbilityTrigger())
            EnableAbility();

        if (IsHit())
        {
            Console.WriteLine($"{Name} стреляет.");

            if (IsAbilityApplied)
            {
                solder.TakeDamage(Damage);
                solder.TakeDamage(Damage);
                solder.TakeDamage(Damage);
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
    }

    public override void Attaсk(Soldier solder)
    {
        if (IsHit())
        {
            Console.WriteLine($"{Name} стреляет.");

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
    }

    public override void Attaсk(Soldier solder)
    {
        if (IsAbilityApplied == false)
        {
            if (IsAbilityTrigger())
                EnableAbility();
        }


        for (int i = 0; i < _numberOfShotsPerTurn; i++)
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