using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RieslingUtils;

public class BelongingsUIElement : EventTrigger {
    private Image _image;
    private Vector3 _mouseOffset;
    private int _targetLayer;
    private Belongings _belongingsData;

    private void Awake() {
        _image = GetComponent<Image>();
        _targetLayer = LayerMask.NameToLayer("Ally");
    }

    public void SetSprite(Sprite sprite) {
        _image.sprite = sprite;
    }

    public void SetBelongingsData(Belongings belongings) {
        _belongingsData = belongings;
        SetSprite(belongings.Sprite);
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        _mouseOffset = (transform.position - MouseUtils.GetMouseWorldPosition());
    }

    public override void OnDrag(PointerEventData eventData) {
        transform.position = MouseUtils.GetMouseWorldPosition() + _mouseOffset;
    }

    public override void OnEndDrag(PointerEventData eventData) {
        if (_belongingsData == null) {
            return;
        }

        var hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 100f, (1 << _targetLayer));
        if (hit.collider != null) {
            string targetEntityID = hit.collider.GetComponent<EntityBase>().ID;
            var playerData = GameMain.PlayerData;
            
            if (playerData.GetBelongingsList(targetEntityID).Count < 3) {
                GameMain.PlayerData.RemoveBelongingsInInventory(_belongingsData);
                GameMain.PlayerData.AddBelongings(targetEntityID, _belongingsData);

                SetSprite(null);
                _belongingsData = null;
            }
        }
        
        transform.localPosition = Vector3.zero;
    }
}
