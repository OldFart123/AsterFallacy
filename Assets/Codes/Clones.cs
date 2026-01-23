using System;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Cinemachine.AxisState;
using static Unity.Collections.Unicode;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Clones : MonoBehaviour
{
    Ooo nice choice — afterimages feel *way* better for sprinting than trails 😈
Let’s do this clean, performant, and easy to tune.

You’ll get:

* 👻 Ghost clones while **SPRINTING**
* ❌ No clones while idle / walking
* 🔥 Dash can stay separate (trail or nothing)
* Zero GC spikes

---

## 🧱 Step 1 — Create the Afterimage prefab

1. **Duplicate your Player sprite GameObject**
2. Remove:

   * Rigidbody2D
   * Collider
   * Animator
3. Keep:

   * SpriteRenderer only
4. Add this script → **`Afterimage.cs`**

```csharp
using UnityEngine;

public class Afterimage : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.15f;
    [SerializeField] private float fadeSpeed = 8f;

    private SpriteRenderer sr;
    private Color color;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    public void Init(Sprite sprite, Vector3 scale, bool facingRight, Color tint)
    {
        sr.sprite = sprite;
        transform.localScale = scale;
        color = tint;
        sr.color = color;
    }

    void Update()
    {
        color.a = Mathf.MoveTowards(color.a, 0f, fadeSpeed * Time.deltaTime);
        sr.color = color;

        if (color.a <= 0.01f)
            Destroy(gameObject);
    }
}
```

➡️ Save this as **`AfterimagePrefab`**

---

## 🧠 Step 2 — Add sprint afterimage settings to your player

Add these to `Player_Movement`:

```csharp
[Header("Sprint Afterimages")]
[SerializeField] private Afterimage afterimagePrefab;
[SerializeField] private float afterimageSpawnRate = 0.05f;
[SerializeField] private Color afterimageColor = new Color(1f, 1f, 1f, 0.6f);

private float afterimageTimer;
```

---

## 🏃 Step 3 — Spawn afterimages ONLY while sprinting

Add this method to `Player_Movement`:

```csharp
private void HandleSprintAfterimages()
{
    bool isSprinting = isGrounded && SpeedMove > baseMoveSpeed && Mathf.Abs(rigid_bod.linearVelocity.x) > 0.1f;

    if (!isSprinting)
    {
        afterimageTimer = 0f;
        return;
    }

    afterimageTimer -= Time.deltaTime;

    if (afterimageTimer <= 0f)
    {
        SpawnAfterimage();
        afterimageTimer = afterimageSpawnRate;
    }
}
```

---

### Call it in `Update()` (near the end is perfect):

```csharp
HandleSprintAfterimages();
```

---

## 👻 Step 4 — Spawn logic (this is the magic)

```csharp
private void SpawnAfterimage()
{
    Afterimage img = Instantiate(
        afterimagePrefab,
        transform.position,
        Quaternion.identity
    );

    SpriteRenderer playerSR = sprite_renderer;

    img.Init(
        playerSR.sprite,
        transform.localScale,
        facingRight,
        afterimageColor
    );
}
```

---

## 🎨 Optional polish (highly recommended)

### ✨ Color shift with speed

```csharp
float t = Mathf.InverseLerp(baseMoveSpeed, SprintSpeed, Mathf.Abs(rigid_bod.linearVelocity.x));
Color dynamicColor = Color.Lerp(Color.white, Color.cyan, t);
```

### 🧼 Object pooling (later)

If you sprint a LOT, I can give you a pool version — zero Instantiate/Destroy.

---

## 🎯 Final result

✔ Clean ghost clones
✔ Only during sprint
✔ No dash interference
✔ Looks sick at high speed

If you want:

*Directional skew / stretch
* Motion blur feel
* Dash uses *brighter* afterimages
* Kill clones instantly on stop

Just say the style 😎

}
