# Prefab Setup Guide — Page-Turn Animation

This is the manual Unity Editor setup required to get the page-turn animation working. The code is already in place — this just wires up the scene.

---

## Step 1 — Duplicate the Material

1. In the **Project** window, go to `Assets/Materials/`
2. Right-click `BookPage.mat` → **Duplicate** → rename to `TurningFrontPage`
3. Right-click `BookPage.mat` → **Duplicate** again → rename to `TurningBackPage`

---

## Step 2 — Open the Book Prefab

In the **Project** window, go to `Assets/Prefabs/` and double-click `Book.prefab` to open it in Prefab Mode.

The current hierarchy should look like:
```
Book
├── LeftPage
│   └── StoryText
└── RightPage
    └── StoryText
```

---

## Step 3 — Add TurningPagePivot

1. Right-click the **Book** root → **Create Empty** → rename to `TurningPagePivot`
2. In the **Inspector**, set its **Transform**:
   - Position: `X: 2.35  Y: 0.5  Z: 0`
   - Rotation: `X: 0  Y: 0  Z: 0`
   - Scale: `X: 1  Y: 1  Z: 1`
3. **Uncheck the active checkbox** at the top of the Inspector (the object should be inactive by default)

---

## Step 4 — Add TurningFront

1. Right-click **TurningPagePivot** → **3D Object → Quad** → rename to `TurningFront`
2. Set its **Transform**:
   - Position: `X: 0.35  Y: 0  Z: 0`
   - Rotation: `X: 90  Y: 180  Z: 0`
   - Scale: `X: 0.07  Y: 0.1  Z: 0.1`
3. In the **MeshRenderer** component, assign `TurningFrontPage` to the material slot
4. **Remove the MeshCollider** component (right-click it → Remove Component)
5. Add a StoryText child (see Step 6)

> **Note:** If in Play Mode the turning page appears on the wrong side of the spine, change Position X to `-0.35`.

---

## Step 5 — Add TurningBack

1. Right-click **TurningPagePivot** → **3D Object → Quad** → rename to `TurningBack`
2. Set its **Transform**:
   - Position: `X: 0.35  Y: 0  Z: 0`
   - Rotation: `X: 90  Y: 0  Z: 0`
   - Scale: `X: 0.07  Y: 0.1  Z: 0.1`
3. In the **MeshRenderer** component, assign `TurningBackPage` to the material slot
4. **Remove the MeshCollider** component
5. Add a StoryText child (see Step 6)

---

## Step 6 — Add StoryText to Both Turning Pages

Do this for both **TurningFront** and **TurningBack**:

1. In the hierarchy, select the StoryText child under **RightPage**
2. **Ctrl+D** (or Cmd+D) to duplicate it
3. Drag the duplicate onto **TurningFront** (or **TurningBack**) to re-parent it
4. Leave all its Transform values exactly as they are (copied from RightPage's StoryText)

---

## Step 7 — Final Prefab Hierarchy Check

The hierarchy should now look like:
```
Book
├── LeftPage
│   └── StoryText
├── RightPage
│   └── StoryText
└── TurningPagePivot  ← inactive
    ├── TurningFront
    │   └── StoryText
    └── TurningBack
        └── StoryText
```

Click **Save** (or hit Ctrl+S) to save the prefab, then exit Prefab Mode.

---

## Step 8 — Wire the BookManager Inspector Fields

1. In the **Hierarchy**, select the **BookManager** GameObject (or whichever object has the BookManager script)
2. In the **Inspector**, under the **Turning Page** header, you'll see four new fields:
   - `Turning Page Pivot`
   - `Turning Front`
   - `Turning Back`
   - `Turn Duration`
3. Drag the GameObjects from the Hierarchy into their matching slots:
   - `Turning Page Pivot` ← drag **TurningPagePivot**
   - `Turning Front` ← drag **TurningFront**
   - `Turning Back` ← drag **TurningBack**
4. Leave `Turn Duration` at `0.6` — increase for a slower turn, decrease for faster

---

## Step 9 — Test in Play Mode

1. Press **Play**
2. Click a page — the animation should play over ~0.6s
3. If the page appears on the wrong side during the turn, exit Play Mode and flip TurningFront/TurningBack Position X from `0.35` to `-0.35`

---

← [Back to Overview](overview.md)
