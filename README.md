# test-environment-042

## METER RUNNING: cab-driver life-sim

A top-down cab-driver life-sim for browser and phone. It's built with Phaser 4, React 19, TypeScript and Vite, starting from [phaserjs/template-react-ts](https://github.com/phaserjs/template-react-ts) (MIT, see `licenses/`).

### Run it

```bash
npm install
npm run dev        # http://localhost:8080
npm test           # unit tests (car model, test map)
npm run typecheck
npm run build      # static site in dist/
```

URL flags: `?touch` shows the phone controls on desktop; `?debug` draws physics bodies.

### Controls (M0)

| Action | Keyboard | Gamepad | Phone (landscape) |
|---|---|---|---|
| Gas | W / ↑ | RT | GAS pedal (right thumb) |
| Brake, reverse when stopped | S / ↓ | LT | BRAKE pedal |
| Steer | A D / ← → | Left stick | Floating joystick (left half of screen) |
| Handbrake | Space | RB | HB |
| Horn | H | B | HORN |

### Status: M0 skeleton ✅

- Era I checker cab drives on a 5×5-block test city (procedural tilemap; all art is drawn at runtime, so there are no binary assets)
- Arcade car model: speed-scaled steering, inverted steering in reverse, lateral grip, handbrake slides
- Buildings are solid; crashes shake the camera and vibrate on phones
- Camera follows with look-ahead and zooms out slightly at speed
- Keyboard, gamepad and touch inputs are merged, so you can switch devices mid-drive
- Era I amber HUD speedometer; portrait-phone "turn sideways" prompt; browser swipe/zoom gestures blocked

Next up: **M1 fare loop** (see the plan).

### Layout

```
src/
  game/
    scenes/   Boot (draws textures), City (driving)
    systems/  CarModel.ts (pure, unit-tested)
    world/    testMap.ts (pure, unit-tested), textures.ts
    input/    DriveControls.ts (keyboard + pad + touch), touchInput.ts
    audio/    horn.ts (Web Audio, no assets)
  ui/         Hud.tsx, TouchControls.tsx (React overlays)
```

### Planning docs

1. [Research & Build Plan](docs/01-research-and-plan.md): GitHub/template search, eras, architecture, milestones
2. [Game Design Reference](docs/02-game-design-reference.md): character, controls, look & feel, eras, customers, dispatch, economy
3. [Alpha Test Plan](docs/03-alpha-test-plan.md): checklists, scripted sessions, example results and bug reports
