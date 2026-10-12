# Another Life (SP): Design v0.1

> A single-player GTA V story-mode mod built on ScriptHookVDotNet. You arrive at LSIA with nothing but a fake ID and a phone, running from something back home. You take gig-app work, keep your head down, get legitimate and move up, or you don't.
>
> This is a port of the ALN3 FiveM design (dice checks, locations, items, loot) without the server, database and load-order plumbing.

---

## 1. Pillars

1. **Phone first.** Jobs, texts, contacts, banking and alerts all come through the phone. No HUD clutter and no audio required.
2. **Everything is a roll.** Risky actions use a d20 check against a difficulty number, the system carried over from ALN3. Your skills, tools and heat change the odds.
3. **Earn your way up.** Tier 1 is cash-in-hand gig work in a beater. Licenses and approvals unlock better vehicles, better contracts and better pay.
4. **The city remembers.** Reputation by district and with factions. Annoy people and work gets harder there.
5. **Data, not code.** Locations, items, jobs, vehicles, prices and loot all live in JSON files you edit; reload in-game.

---

## 2. Opening: fresh off the plane

| Step | What happens |
|---|---|
| 1. Arrival | Spawn at LSIA arrivals as a custom freemode character (male/female pick). Story characters are hidden. $0 cash, no bank card. |
| 2. Inventory | **Fake ID** (item), a burner phone, the clothes you're in, $40 in your pocket from "a friend". |
| 3. First text | *Unknown number:* "Heard you made it. Got a place for you. Pick one, can't hold them long." Three crash pads are offered as map markers. |
| 4. Choose a crash pad | See §3. You get the keys and a beater on the lot. |
| 5. Install the gig app | Texted link → **GigNow** contact appears on the phone. First jobs are unlocked. |
| 6. Tutorial by text | Short texts for the first delivery, first fare, first meal and the first "lost my keys" moment. |

### Fake ID

- Lets you sign up for the gig apps and rent the crash pad.
- Police **ID check** during a stop: a d20 roll against the ID's quality. Fail and your heat goes up and the fake ID is confiscated.
- Better fakes can be bought for crypto. The real path is legitimization (§5).

---

## 3. Crash pads and property

**Crash pad**: a run-down shop with a garage bay and a fix-it area, in the spirit of the biker clubhouse.

| Option | Area | Perk | Drawback |
|---|---|---|---|
| **Bay Shop** | East LS (La Mesa garages) | Close to city gig demand | Gang territory, higher theft risk |
| **Route 68 Garage** | Harmony / Sandy edge | Cheap rent, quiet, good for chop work | Long drives to city jobs |
| **Paleto Barn Shop** | Paleto Bay | Off the radar, lowest heat decay | Far from everything, few contracts |

Each crash pad has:
- **Bed**: sleep to save, pass time, recover.
- **Fridge and pantry stash**: food and drink storage.
- **Garage bay**: store 1–2 owned vehicles. They respawn here.
- **Workbench**: repair (costs parts), basic tune-up, strip stolen cars for parts.
- **Stash**: an item storage box.

**Interiors.** v1 uses exterior lots with marker interactions (bed, bench and stash as on-foot markers in the bay). Real interiors (biker clubhouse and garage IPLs from the online map data, loaded in story mode) come in a later milestone; story-mode IPL loading works but needs per-build testing.

**Buying up.** Later you can rent or buy apartments, houses, a proper shop or a garage. Each is a JSON entry with a price, storage, garage slots, a heat modifier and the currency allowed (legal property needs cash or bank, not crypto).

---

## 4. Money: three currencies

| Currency | Used for | Earned from |
|---|---|---|
| **Cash** | Street purchases, food, rent, used cars, gig-app payouts (T1) | Gig jobs, selling legit items |
| **Bank / card** | Legal purchases: licenses, new cars, insurance, property, tune-ups at real shops | Deposits at ATMs (needs a bank account, which needs a real ID or T2+ approval) |
| **Crypto** | **Illegal only**: black-market guns, better fake IDs, illegal contracts, fences | Fences, chop shops, illegal contracts, loot |

- **Laundering:** crypto → cash at a crypto exchange or shady ATM. The fee is 15–35% depending on rep, and every transaction has a heat roll.
- **Bank account gate:** you can't hold a bank balance until you have **T2 Verified** status. Until then it's cash only, with mugging risk.

---

## 5. Career tiers and licensing

| Tier | Status | Unlock | Vehicles allowed for contracts | Jobs | Pay |
|---|---|---|---|---|---|
| **T1** | Off the books | Start | Any beater, scooter, bike | GigNow: food/parcel delivery, cash rides (unlicensed cab) | $ low, cash only |
| **T2** | Verified | 25 gig rep + no active warrants + $500 "verification" | Sedans, vans | Licensed rideshare, larger deliveries, courier | $$, bank unlocked |
| **T3** | Licensed | Driver's license (written test + driving test minigame) + $2,000 | Commercial vans, box trucks, nicer sedans | Airport/hotel fares, freight, tow | $$$ |
| **T4** | Professional | Chauffeur/commercial permit + clean 7 days + insurance | Luxury cars, limos, armored | Black-car VIP, corporate contracts, executive courier | $$$$ |

- Licenses are items with **flags**: `license.driver`, `permit.chauffeur`, `permit.commercial`, `insurance.active`.
- A gig app **refuses** a contract if your vehicle class is above your tier, or if the car is reported stolen (stolen cars can't do legal jobs).
- Driving without a license at a traffic stop adds heat and a fine; T1 players avoid cops.

---

## 6. Jobs: contract apps on the phone

Each app is a phone contact (iFruitAddon2). Open it to see a list of offers. Each offer shows pay, distance, a risk rating and a timer.

| App | Type | T1 | T2+ | Legal? |
|---|---|---|---|---|
| **GigNow** | Food and parcel delivery | ✓ | ✓ | Legal |
| **RideLine** | Passenger fares (cab/rideshare) | Cash fares only, unlicensed (cops notice) | ✓ Licensed | Legal |
| **HaulIt** | Vans, box trucks, freight | — | T3 | Legal |
| **Blackline** | VIP chauffeur, corporate | — | T4 | Legal |
| **Unknown Number** | "Pick up this car", "drop this bag", chop orders | ✓ | ✓ | **Illegal**, pays crypto |

### Job loop (the same engine for every app)

1. Offer arrives by text, or you open the app.
2. Accept. A waypoint is set to the pickup.
3. **Pickup**: a passenger walks to the car and gets in (the door is forced shut after they're seated), or you collect a package prop.
4. Drive to the drop-off. **Grading**: time vs. ETA, smoothness (hard braking, collisions), vehicle condition and class, and the passenger type's "wants".
5. Pay plus tip. Gig rep up or down, district rep up or down.
6. **Events** (by chance): a passenger skips the fare, a package is a drug drop (moral choice), a passenger is drunk, a cop tail, a "take the long way" request.

### Passenger types (data-driven)

`customers.json`: each type has spawn areas, time of day, patience, tip range, wants (`fast`, `smooth`, `quiet`, `discreet`), the tiers it appears in, and event chances.

---

## 7. Vehicles: ownership, condition and handling

### Ownership

- An owned vehicle record holds plate, model, color, mods, **condition**, mileage, fuel (optional), location, the garage it's stored at and an insurance flag.
- Owned vehicles are saved and **respawn** at their garage or at their last parked spot on load.
- Keys are an item per plate. No key means it must be hotwired.

### Condition and handling (Legacy build)

| Condition | How you get it | Effect |
|---|---|---|
| **Beater** (0–40) | Starter car, cheap used lots, stolen junkers | ~75% torque, lower top speed, soft brakes, a slight steering pull, random **stall** chance at low speed, engine health starts low |
| **Used** (40–75) | Used dealers, private sales | ~90% torque, normal brakes, occasional pull |
| **Tuned** (75–95) | Pay a shop or use your workbench + parts | 100% performance, slight upgrades available |
| **New** (95–100) | Dealership (bank only, license required) | Full factory handling, warranty (free first repair) |

- Condition wears with mileage and with every crash.
- Done per vehicle through natives (torque multiplier, top-speed modifier, engine health, brake force via mods), so two cars of the same model can feel different.
- **Tune-up** at a shop (bank) or at your workbench (parts + time + a d20 Mechanics check).

---

## 8. Crime: carjacking, lockpicking and searching

### Getting in (choose a method at the driver's door)

| Method | Input | Roll | Noise |
|---|---|---|---|
| **Lockpick** | Timing minigame: hit the key when the pin is in the sweet spot, 3–5 pins | d20 sets the **number of pins and the zone size** | Quiet. A failed pin can jam (DC+2) |
| **Bust it** | **Tap repeatedly** to fill a bar before the timer runs out | d20 sets **taps needed and the timer** | **Alarm on start**, nearby peds may call cops |
| **Keys** | Already have them | — | — |

- **Difficulty** carries over from ALN3: base DC by action (`lockpick 12`, `smash 11`, `hotwire 14`), adjusted by vehicle class (sports +3, super +6) and by value (+1/+2/+3 at $20k/$60k/$150k).
- **Advantage**: a good tool, or a natural 20 on your last check (momentum). **Disadvantage**: being wanted.
- **Natural 1**: the tool breaks, the alarm goes off and you get +2 wanted stars.

### Searching

- **Search vehicle** (after entry): d20 check → **keys in the visor** (skip hotwire, and the car isn't flagged stolen for 10 minutes), plus a glovebox/trunk loot table roll (cash, items, rarely a gun).
- **Search body** (knocked out or dead peds): every ped drops *something* from a loot table chosen by ped type (business, gang, cop, homeless, tourist). Guns are rare; ammo is scarce.
- **Hotwire**: d20 check plus a short hold-and-release minigame.

### Fencing

- Stolen cars → **chop shop** (crypto) or **strip at your workbench** (parts and loot).
- Stolen items → fence or pawn (cash, at a lower rate).

---

## 9. Needs: light but real

| Need | Drains | At 0 |
|---|---|---|
| **Food** | Fully empty in ~1 in-game day (48 real minutes) | **Shakes**: aim sway and slight camera shake. After 6 more in-game hours, slow health drain |
| **Drink** | Fully empty in ~1 in-game day | Same, slightly faster |

- Eat and drink from shops, vending machines, food trucks or your fridge.
- Sleep is optional (no fatigue meter in v1); the bed is for saving and skipping time.
- The needs bars show only when low, or when you open the phone's status app.

---

## 10. Reputation and heat

Three separate tracks:

| Track | Scope | Goes down when you… | Low-rep effects | High-rep effects |
|---|---|---|---|---|
| **District rep** (−100 to +100) | Per district (about 10) | Assault, steal, crash into people, run people over, rob shops, fail jobs there | Fewer offers, shops charge more or refuse, people call cops faster, passengers refuse rides | Better offers, discounts, tips up |
| **Faction rep** | Gangs, cops, gig companies, fences | Attack members, snitch, steal their cars, fail their contracts | Hostile on sight (gangs), deactivated (apps), bad prices (fences) | Protection, exclusive jobs, better rates |
| **Heat** | Global, decays over time | Witnessed crimes, failed ID checks, warrants | Cops stop you more, bigger response | — |

- Rep changes are **witness-based**: if no one sees it, it doesn't count (simple line-of-sight check on nearby peds).
- Heat decays faster at your crash pad (Paleto fastest).

---

## 11. Phone

Built on **iFruitAddon2** contacts plus picture-message notifications that look like texts.

| Contact / app | Purpose |
|---|---|
| **GigNow / RideLine / HaulIt / Blackline** | Job lists, accept, active job, earnings |
| **Unknown Number** | Illegal contracts, story texts |
| **Bank** | Balance, transfers, pay rent (T2+) |
| **Wallet** | Crypto balance, launder via exchange contacts |
| **Status** | Food, drink, health, heat, district rep, licenses |
| **Garage** | Your vehicles, conditions, locations; mark a car on the map |
| **Landlord** | Rent due, renew, upgrade |
| **Mechanic** | Book a tune-up, tow your broken car (costs money) |

Text examples: rent reminders, job offers, "your package is ready", gang threats when rep drops, a fence offering a deal.

---

## 12. Tech stack

| Piece | Choice | Why |
|---|---|---|
| Game | **GTA V Legacy**, story mode | Most mod-compatible |
| Script loader | Script Hook V + **ScriptHookVDotNet 3** | Maintained, C# |
| Menus | **LemonUI** (MIT) | Shop, garage, workbench, job list menus |
| Phone | **iFruitAddon2** | Contacts in the in-game phone |
| Data | JSON in `scripts/AnotherLife/data/` | Edit without code; `F9` hot-reloads |
| Save | `scripts/AnotherLife/save/slot1.json` | Human-readable, backed up on each save |
| Language | C# (.NET Framework 4.8) | What SHVDN runs |

### Module map (ALN3 → SP)

| ALN3 resource | SP module | Notes |
|---|---|---|
| aln-core, aln-db, aln-persistent-data | `Core`, `SaveSystem` | One JSON save; no DB |
| aln-economy, aln-atm, aln-launder | `Economy` | Cash / bank / crypto (replaces "dirty") |
| aln-items, aln-inventory | `Items`, `Inventory` | Same item keys and icons |
| aln-locations | `Locations` | Same IDs and tags → `locations.json` |
| aln-loot | `Loot` | Same deterministic pools → `loot.json` |
| aln-minigame, aln-criminal-xp/tools/unlocks | `Checks` (d20), `Skills` | Same DCs and rules |
| aln-carjack | `Carjack` | + tap-to-bust and lockpick minigames + search |
| aln-world-population | `WorldTuning` | Density settings |
| aln-services | `Services` | Call a taxi, tow, mechanic |
| aln-spawn, aln-respawn | `Onboarding`, `Respawn` | Airport intro; hospital or jail respawn |
| aln-pawn | `Fence` | Pawn = cash, fence = crypto |
| *(new)* | `Jobs`, `Vehicles`, `Needs`, `Reputation`, `Property`, `Phone`, `Licenses` | |
| bg3_dice | 2D roll overlay | The paid NUI dice can't run in story mode |

---

## 13. Milestones

| # | Milestone | Done when |
|---|---|---|
| **M0** | Skeleton | The DLL loads in story mode; data JSON loads; F9 reload; save/load works; a debug menu |
| **M1** | Arrival + crash pad | Airport spawn as a freemode ped, fake ID, choose 1 of 3 crash pads, beater on the lot, bed saves |
| **M2** | Economy + phone | Cash/bank/crypto, phone contacts, texts, the status app |
| **M3** | Gig jobs | GigNow delivery + RideLine fares end-to-end, grading, pay, gig rep |
| **M4** | Vehicles | Ownership, keys, respawn at garage, condition-based handling, tune-up |
| **M5** | Carjack | Lockpick + tap-to-bust, alarms, hotwire, search for keys/loot, d20 overlay |
| **M6** | Needs + loot | Food/drink cycle, shakes, health drain, ped drops, body search |
| **M7** | Reputation + heat | District/faction rep, witness checks, ID checks at stops |
| **M8** | Tiers + licenses | T2–T4 gates, license tests, new apps, property purchases |
| **M9** | Illegal side | Unknown Number contracts, chop shop, fence, laundering |

Each milestone ships as a build you drop into `GTA V/scripts/`, with a **test checklist**, since the game can't run in the cloud workspace.

---

## 14. Open questions

1. **Character**: freemode character with a simple creator (face presets, hair, clothes), or start from a fixed model and add a creator later?
2. **Death**: hospital respawn with a bill (cash/bank) and lose carried crypto/items? Or lose only cash?
3. **Arrest**: jail time skip + fine + confiscated illegal items + lose the fake ID?
4. **Story mode leftovers**: hide Michael/Franklin/Trevor entirely (yes by default), and block story missions from triggering?
5. **Difficulty knobs**: should the d20 checks be visible (show DC and roll) or hidden (just success/fail)?
