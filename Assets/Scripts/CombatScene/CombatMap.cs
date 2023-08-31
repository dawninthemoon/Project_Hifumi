using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMap : MonoBehaviour {
    public static readonly float Width = 640;
    public static readonly float Height = 380f;
    public static Vector2 StageMinSize { get; private set; }
    public static Vector2 StageMaxSize { get; private set; }

    public static void SetMapView(Vector3 origin) {
        StageMinSize = (Vector2)origin + new Vector2(-Width / 2f, -Height / 2f);
        StageMaxSize = (Vector2)origin + new Vector2(Width / 2f, Height / 2f);
    }

    public static Vector2 GetBoarderNormal(Vector2 position, float radius) {
        Vector2 normal = Vector2.zero;

        if (position.y > StageMaxSize.y - radius) {
            normal = Vector2.down;
        }
        else if (position.x < StageMinSize.x + radius) {
            normal = Vector2.right;
        }
        else if (position.y < StageMinSize.y + radius) {
            normal = Vector2.up;
        }
        else if (position.x > StageMaxSize.x - radius) {
            normal = Vector2.left;
        }
        return normal;
    }

    public static bool IsInside(Vector2 position, float radius = 0f) {
        return 
            (position.y < StageMaxSize.y - radius) &&
            (position.x > StageMinSize.x + radius) &&
            (position.y > StageMinSize.y + radius) &&
            (position.x < StageMaxSize.x - radius);
    }

    public static Vector3 ClampPosition(Vector3 position, float radius) {
        position.x = Mathf.Clamp(position.x, StageMinSize.x + radius, StageMaxSize.x - radius);
        position.y = Mathf.Clamp(position.y, StageMinSize.y + radius, StageMaxSize.y - radius);
        return position;
    }
}
