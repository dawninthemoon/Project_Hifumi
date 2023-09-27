using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthMana {
    private int _currentHealth;
    private int _currentMana;
    private EntityDecorator _entityDecorator;
    private EntityUIControl _uiControl;
    public int Health {
        get { return _currentHealth; }
        set { 
            _currentHealth = value;
            _uiControl.UpdateHealthBar(_currentHealth, _entityDecorator.Health);
        }
    }
    public int Mana { 
        get { return _currentMana; }
        set { 
            _currentMana = value;
            _uiControl.UpdateManaBar(_currentMana, _entityDecorator.Mana);
        }
    }

    public bool IsManaFull {
        get { return _currentMana >= _entityDecorator.Mana; }
    }

    public EntityHealthMana(EntityUIControl uiControl) {
        _uiControl = uiControl;
    }

    public void Initialize(EntityDecorator decorator) {
        _entityDecorator = decorator;
    }

    public void ReceiveDamage(int finalDamage) {
        ReduceHealth(finalDamage);
    }

    public void AddHealth(int amount) {
        Health = Mathf.Min(_currentHealth + amount, _entityDecorator.Health);
    }

    public void AddMana(int amount) {
        Mana = Mathf.Min(_currentMana + amount, _entityDecorator.Mana);
    }

    public void ReduceHealth(int amount) {
        Health = Mathf.Max(_currentHealth - amount, 0);
    }

    public void ReduceMana(int amount) {
        Health = Mathf.Max(_currentMana - amount, 0);
    }
}
