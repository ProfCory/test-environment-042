# METER RUNNING — Research & Build Plan

> Working title. A top-down life-sim about one cab driver's career. It runs in the browser and on phones, and it moves from 1978 radio-dispatch cabs, through the rideshare-app collapse, to high-end contract work.

---

## 1. Research: what's on GitHub

I searched GitHub and the web for an open-source top-down taxi or driving game to fork. **None is a real fit.** The taxi projects are small arcade toys with no career sim, dispatch, or economy, and most have **no license**, so you can read their code but not legally copy it.

### 1a. Taxi / driving references (read for ideas, don't fork)

| Repo | Stack | What's useful | License |
|---|---|---|---|
| [itsbirdo/hotwheels](https://github.com/itsbirdo/hotwheels) | Phaser, JS | GTA1-style top-down city, pixel art, AI cars. Closest *feel* reference | none listed |
| [koseoglumelih31-code/car-game](https://github.com/koseoglumelih31-code/car-game) | Phaser 3 + TS + Vite | Same stack as the plan. Tiny skeleton | MIT |
| [Daksh-Naik/Taxi-Sim-2D](https://github.com/Daksh-Naik/Taxi-Sim-2D) | JS | Arcade taxi sim, won a contest. Pickup/drop loop | none listed |
| [aekol99/Taxi-Driver-Beni-Mellal](https://github.com/aekol99/Taxi-Driver-Beni-Mellal) | Canvas API | Top-down taxi in a real city | none listed |
| [mateusdemboski/TapTapTaxi](https://github.com/mateusdemboski/TapTapTaxi) | Phaser 2 | One-tap mobile taxi controls | none listed |
| [filipef101/crazy-uber](https://github.com/filipef101/crazy-uber) | JS | Crazy Taxi meets rideshare | none listed |
| [SweetheartSquad/GameOff2023](https://github.com/SweetheartSquad/GameOff2023) | PixiJS | "Taxi Quest 66": narrative taxi game, good tone reference | none listed |
| [scottydelta/Crazy-Taxi](https://github.com/scottydelta/Crazy-Taxi) | HTML5 | Old Facebook-era clone | none listed |
| [mnursey/turbo_panzer](https://github.com/mnursey/turbo_panzer) | Phaser | Top-down car physics | none listed |

Non-GitHub inspiration found: *Whiplash Taxi Co* (browser, itch.io), *Bubux Simulator* (Phaser 3 top-down bus-route game), *TAXIN'T*.

### 1b. Templates (fork one of these)

| Template | Why | License |
|---|---|---|
| **[phaserjs/template-react-ts](https://github.com/phaserjs/template-react-ts)** ⭐ recommended | Official. Phaser 3 + **React** + TypeScript + Vite, with a React↔Phaser event bus already wired. React draws the phone, dispatch, and menus; Phaser draws the city | MIT, GitHub "template" repo |
| [phaserjs/template-vite-ts](https://github.com/phaserjs/template-vite-ts) | Same without React. Lighter, but every UI panel has to be built in Phaser | MIT |
| [ourcade/phaser3-typescript-vite-template](https://github.com/ourcade/phaser3-typescript-vite-template) | Minimal. Last updated 2023 | unlisted |
| [ASteinheiser/ts-online-game-template](https://github.com/ASteinheiser/ts-online-game-template) | React + Phaser + Colyseus + PWA + Electron. Overkill unless multiplayer is planned | check repo |

**Decision:** start from `phaserjs/template-react-ts` ("Use this template" on GitHub). This game's UI *is* the story: a CB radio mic, then a green-screen terminal, then a smartphone, then a concierge tablet. React suits UI-heavy overlays, and it's the stack you already use for your solo-RPG tools.

---

## 2. Core concept

**One driver, one city, ~48 years.** The career is played in four **Eras**. Between Eras the calendar jumps, the city's map and traffic change, the cab changes, and the way you *get* fares changes completely. The core loop stays the same: **find fare → drive → deliver → get paid → survive the month.**

| Era | Years | Vehicle | How you get fares | The pressure |
|---|---|---|---|---|
| **I. The Medallion** | 1978–1989 | Checker-style sedan, bench seat, mechanical meter | Street hails + **radio dispatcher** (voice, zone bids) | Shift lease, gas, mechanic, crime, rent |
| **II. The Terminal** | 1990–2008 | Crown Vic-style cruiser, digital meter, MDT screen, flip phone | Computer dispatch queue + hails + hotel/airport stands | Paying off a medallion, competition, GPS arriving |
| **III. The Disruption** | 2009–2019 | Your own hybrid hatchback | **Rideshare app**: algorithm pings, surge, star ratings | Medallion value collapses, lower per-trip pay, car payments, 1★ reviews |
| **IV. The Contract** | 2020–2026 | Leased luxury EV / black car | Multi-app contracting **or** high-end work: corporate accounts, VIP, concierge, events | Ratings, uniform standard, client NDAs, autonomous-fleet competition |

**The turning point (end of Era II → III):** the player has probably bought or financed a medallion. In Era III its value crashes. That's the emotional hinge of the campaign. Three exits:
- **Hold** and keep driving the medallion cab, hoping it recovers (hard mode).
- **Sell** at a loss and go app-only.
- **Convert**: use your reputation and old regulars to open a black-car service, which leads to the high-end Era IV path.

---

## 3. Architecture

```
src/
  game/                    # Phaser
    scenes/  Boot, Preload, City, Garage, EraTransition
    entities/ Cab.ts  TrafficCar.ts  Pedestrian.ts  Fare.ts
    systems/ DrivingPhysics.ts  Traffic.ts  Pathfinding.ts (grid A*)
             FareSpawner.ts  Meter.ts  DayClock.ts  Weather.ts
    maps/    era1.tmj  era2.tmj  era3.tmj  era4.tmj   (Tiled tilemaps)
  ui/                      # React overlays
    hud/     Speedo, Meter, Clock, CashFatigue
    devices/ CBRadio.tsx  MDTTerminal.tsx  Phone.tsx  ConciergeTablet.tsx
    screens/ CharacterCreate, Ledger, Garage, EraCard, Pause, Settings
    touch/   Joystick.tsx  PedalButtons.tsx
  data/    eras.json  customers.json  districts.json  dispatchers.json  vehicles.json
  state/   store.ts (zustand)  save.ts (IndexedDB, versioned)
  bus/     EventBus.ts (template's Phaser↔React bridge)
```

- **Physics:** Phaser Arcade for collisions. A custom arcade car model (velocity along heading, lateral grip, handbrake slide) instead of Matter.js, which keeps phone CPU low.
- **City:** Tiled tilemaps at 32px tiles. Road tiles carry lane-direction metadata for traffic AI and A* routing. One base city with per-era overlay layers (new towers, demolished blocks, bike lanes, EV chargers).
- **Data-driven content:** customers, districts, eras, and dispatch lines all live in JSON so they can be tuned without code changes.
- **Saves:** IndexedDB with a schema version and migration function, plus manual export/import JSON (useful for testers).
- **PWA:** installable on phones, works offline, locked to landscape.
- **Optional AI layer (stretch goal):** passenger small talk and dispatcher banter generated by an LLM from a persona card + trip context, with handwritten JSON lines as the fallback. Fits your two-model setup: a cheap model for chatter, a strong model for story beats. Not required for alpha.

---

## 4. Milestones (iterative skeleton → layers)

| # | Milestone | Done when | Est. |
|---|---|---|---|
| **M0** ✅ | Skeleton | Template forked; cab drives on a tiny test map; camera follows; desktop + touch input both work | 1 wk |
| **M1** | Fare loop | Hailing pedestrian spawns → stop next to them → board → destination marker → drop → meter pays | 1–2 wk |
| **M2** | City & traffic | 3×3-district Era I map; AI traffic follows lanes and lights; collisions damage the cab | 2 wk |
| **M3** | Shift & money | Day clock, shift lease, gas, fatigue, end-of-shift ledger, rent due weekly | 1–2 wk |
| **M4** | Radio dispatch | CB panel, dispatcher calls zones, player bids, wrong-address and no-show outcomes | 2 wk |
| **M5** | Customers | 12 Era I archetypes with patience, tip, mood, and events (robbery, birth, drunk, celebrity) | 2 wk |
| **M6** | Character | Creation screen, 5 stats, perks, reputation tracks, regulars who remember you | 1–2 wk |
| **M7** | Era II | MDT dispatch, airport/hotel stands, medallion purchase, map overlay | 2 wk |
| **M8** | Era III | Phone app UI, ratings, surge heatmap, medallion crash event and the three exits | 2–3 wk |
| **M9** | Era IV | Black-car contracts, VIP jobs, concierge tablet, autonomous-fleet rival | 2–3 wk |
| **M10** | **Alpha** | All 4 Eras playable start → finish; save/load; phone + desktop; telemetry on | — |

Each milestone ends in a playable build deployed to GitHub Pages / Netlify for quick testing on a phone.

---

## 5. Design pillars (the tie-breakers)

1. **The cab is the camera.** Everything happens from the driver's seat, top-down. No cutscenes; the story arrives through the radio, the screen, and the people in the back seat.
2. **The device is the era.** Changing how fares arrive (voice → text → algorithm → client) carries the theme more than any text dump.
3. **One more fare.** Shifts are 6–10 real minutes. Mobile sessions have to fit a coffee break.
4. **Money hurts, people stick.** Economy pressure is real, but the payoff is the regulars who follow you across decades.

Full system detail lives in `02-game-design-reference.md`; alpha test plan and example results are in `03-alpha-test-plan.md`.
