# Technical Artist — Dashy Crashy Endless Runner

A Unity project built to explore and implement technical art systems inside an endless runner game inspired by *Dashy Crashy*.

---

## Gameplay

The player controls a car moving forward through an infinite road. Swipe left or right (mouse or touch) to switch lanes and dodge oncoming traffic. The world curves around the camera in a stylized way, and the sky shifts through a full day/night cycle as you play.

---

## Technical Art Systems

### World Bending (GlobalController)
A shader-driven world bend effect that curves the road and environment geometry around the camera. The bend origin is always locked to the main camera position, so the curve follows the player seamlessly in both edit and play mode.

- Horizontal and vertical bend controls
- Configurable flat distance before the bend kicks in
- Works on any material that implements the `_GlobalCurveAmount`, `_GlobalCurveOrigin`, and `_GlobalCurveDistance` shader properties

### Day / Night Cycle (DayNightManager)
A full 24-hour sky cycle built with layered mesh plane crossfades and smooth blending.

- Four sky phases: Dawn, Day, Sunset, Night
- Smooth ease-in/ease-out crossfades between phases using `SmoothStep`
- Separate scrolling cloud planes per phase
- Ambient light color driven by a `Gradient` evaluated against time of day
- Sun and moon pivots rotate on a correct arc — sun rises at dawn and sets at dusk, moon rises at night
- Directional light intensity transitions between day and night values
- Ground material swapping per phase

---

## Gameplay Systems

### Car Controller (CarController)
- Swipe left/right input for both mouse and touch via Unity's Input System
- Step-based lane system with configurable lane count and boundaries
- Velocity-driven lean (Z-axis roll) and steering (Y-axis yaw) rotation for a dynamic feel
- Trigger-based crash detection against traffic (tagged `Traffic`)

### Endless Road Loop (MovingBody + SpawnerLoop)
- Road and environment segments move backward to simulate forward motion
- `SpawnerLoop` recycles segments back to the start when they pass behind the camera, creating a seamless infinite road with no object pooling overhead

### Traffic System (TrafficSpawner + TrafficCar)
- Spawns random traffic prefabs ahead of the player at configurable lane positions
- Spawn rate and forward distance are fully configurable
- Traffic cars move backward toward the player and self-destruct after passing the despawn threshold

---

## Assets Used

- **Free Stylized Hand-Painted Skybox** — sky plane textures for each time of day
- **Generic Passenger Car Pack** — traffic vehicle prefabs
- **TextMesh Pro** — UI text rendering
- **Unity Input System** — cross-platform mouse and touch input

---

## Project Structure

```
Assets/
  _Assets/
    Atul/
      Scripts/         # Technical art scripts (DayNight, WorldBend, Traffic)
    _Scripts/          # Core gameplay scripts (Car, Road, Spawner)
    _Materials/        # Shared materials
    generic-passenger-car-pack/
  Scenes/
    Dashy-Crashy.unity
  Free Stylized Hand-Painted Skybox/
```

---

## Built With

- Unity (URP)
- C#
- Unity Input System
