using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelongingObject : MonoBehaviour {
    public Belongings BelongingData { get; set; }
    public InteractiveEntity Interactive { get; private set; }
    private SpriteRenderer _renderer;

    private void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        Interactive = GetComponent<InteractiveEntity>();
    }

    public void SetSprite(Sprite sprite) {
        _renderer.sprite = sprite;
    }
}
