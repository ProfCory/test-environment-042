# Another Life (SP): Design v0.4

> **v0.4:** character via **Freemode Identity + Menyoo** (no custom creator); the game wallet is your cash; ATM and payphone rules (§4).
>
> **v0.3:** addresses and postal codes (§11a), built into M0.
>
> **v0.2 changes:** character creator + ID photo, starting outfit, death/arrest/debt rules (§4a), dice overlay and practice-based skills (§8), crash pads as exterior pins (§3), phone ride/delivery services (§11), stolen-car dumps (§8). M0 skeleton built.

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
| 1. Character | Your GTA Online-style freemode character, built once in **Menyoo** and kept by **Freemode Identity** (look, weapons, wallet, vanilla shop access). Another Life adds only the camera **flash** that takes the photo for your fake ID. |
| 2. Arrival | Spawn at LSIA arrivals as that freemode character. Story characters are hidden. No bank account, no card. |
| 3. Inventory | **Fake ID** (with your photo), a burner phone, $40 in your pocket from "a friend". Clothes: grey sweats, a plain T-shirt and white canvas sneakers. Everything else you buy. |
| 4. First text | *Unknown number:* "Heard you made it. Got a place for you. Pick one, can't hold them long." Three crash pads are offered as map markers. |
| 5. Choose a crash pad | See §3. You get the keys and a beater on the lot. |
| 6. Install the gig app | Texted link → **GigNow** contact appears on the phone. First jobs are unlocked. |
| 7. Tutorial by text | Short texts for the first delivery, first fare, first meal and the first "lost my keys" moment. |

### Fake ID

- Lets you sign up for the gig apps and rent the crash pad.
- Police **ID check** during a stop: a d20 `bluff` check. Fail and your heat goes up.
- **Arrested** (or killed by cops): the fake ID is **flagged**. Every cop treats you worse (−25 cop rep) until you buy a replacement. See §4a.
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

**Interiors: none.** Every place is an exterior pin (a "rabbit hole"): walk to the marker for the bed, workbench, stash or car spot and press a key. That's settled for v1; no IPL interiors.

**Buying up.** Later you can rent or buy apartments, houses, a proper shop or a garage. Each is a JSON entry with a price, storage, garage slots, a heat modifier and the currency allowed (legal property needs cash or bank, not crypto).

---

## 4. Money: three currencies

| Currency | Used for | Earned from |
|---|---|---|
| **Cash** | Street purchases, food, rent, used cars, gig-app payouts (T1) | Gig jobs, selling legit items |
| **Bank / card** | Legal purchases: licenses, new cars, insurance, property, tune-ups at real shops | Deposits at ATMs (needs a bank account, which needs a real ID or T2+ approval) |
| **Crypto** | **Illegal only**: black-market guns, better fake IDs, illegal contracts, fences | Fences, chop shops, illegal contracts, loot |

- **Bank account gate:** you can't hold a bank balance until you have **T2 Verified** status. Until then it's cash only, with mugging risk.
- **Cash = the game wallet.** With Freemode Identity spoofing, the game's money *is* your cash, so vanilla clothing stores, Ammu-Nation, mod shops and job mods all spend and pay into it. Bank and crypto are tracked by Another Life. (Only active on the freemode character; Michael/Franklin/Trevor's money is never touched.)
- **ATMs** (every ATM prop on the map, once you have a bank account and card): cash → bank is free; bank → cash costs a **$3 fee** (the wallet is whole dollars, so not $2.50). No cash withdrawals from overdraft.
- **Payphones** (every payphone and phone booth): wire **crypto → bank** at today's rate minus **15–35%** (better rep, lower fee). Needs a bank account; no account, no wire.
- **Crypto rate** moves once a day (±8%, clamped $60–$900 per coin). Crypto purchases are priced in dollars and converted at the day's rate.

---

## 4a. Death, arrest and debt

### Death

1. **Hospital bill $250.** Paid from the cash on you if you have $250. If not, it goes on your **bank card** (if you have one). With neither, the hospital takes whatever cash you have and writes off the rest.
2. Then you **lose everything you're carrying** (cash, crypto, tools, food, loot) **except**: bank card, IDs, phone, and keys to things you own.
3. Respawn at the **nearest hospital**.

### Arrest, or killed by police

1. Respawn at the **nearest police station or jail**.
2. **Everything you carry is seized**, except bank card, IDs, phone and keys.
3. Your **fake ID is flagged** (−25 cop rep until replaced). Each arrest also costs −10 cop rep.
4. **$100 booking fee**: on your card if you have a bank account (it can go into overdraft). No account → a **bail bondsman** covers it.
5. If you were **driving your own vehicle**, it's impounded at that station: **$500** to get it out.
6. **Cop rep −60 or worse** (counting the flagged ID): you always have at least **one star** until you fix it.

### Bail bondsman

- Lends up to **$1,000 in total**.
- While you owe him, his **thugs** turn up and take whatever cash you're carrying until the bond is paid.
- Owe the full $1,000 and get arrested again: **no bail**. You serve time (a time skip) and the fee is written off.

### Bank overdraft and leg breakers

- Your bank balance can go down to **−$5,000**. Card purchases that would go lower are declined. Hospital bills and booking fees go through anyway.
- At **−$5,000 or lower**, **leg breakers** start coming. They **don't stop** until your balance is back to **$0**.

All of these numbers live in `rules.json`.

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

### The dice

- An on-screen **d20 overlay**: the **DC** is shown above, the numbers flick through 1–20 and land, then your modifier is added and the result shows pass/fail. Advantage and disadvantage show two dice.
- **Skills improve with practice.** Every check is tied to a skill: `lockpicking`, `brute`, `hotwire`, `search`, `hacking`, `mechanics`, `bluff`. Each attempt earns that skill XP (DC × 1 on success, a third of that on failure). Your modifier equals your level:

| Level | XP needed | Modifier | Roughly |
|---|---|---|---|
| 0 | 0 | +0 | Fresh off the plane |
| 1 | 60 | +1 | ~5 successful break-ins |
| 2 | 150 | +2 | |
| 3 | 300 | +3 | |
| 4 | 550 | +4 | |
| 5 | 900 | +5 | A pro |

- **Tools** listed per action in `checks.json` (e.g. a slim jim beats a lockpick set). Carrying one gives advantage; a natural 1 can break it.
- **Natural 20** always succeeds and gives advantage on your next check. **Natural 1** always fails.

### Searching

- **Search vehicle** (after entry): d20 check → **keys in the visor** (skip hotwire, and the car isn't flagged stolen for 10 minutes), plus a glovebox/trunk loot table roll (cash, items, rarely a gun).
- **Search body** (knocked out or dead peds): every ped drops *something* from a loot table chosen by ped type (business, gang, cop, homeless, tourist). Guns are rare; ammo is scarce.
- **Hotwire**: d20 check plus a short hold-and-release minigame.

### Fencing

- Stolen cars → **chop shop** (crypto) or **strip at your workbench** (parts and loot).
- Stolen items → fence or pawn (cash, at a lower rate).
- **Dump spots**: 3 scrapyards where you leave a stolen car for next to nothing (0.02–0.08 crypto) with a **45 in-game-minute cooldown**. A way to get rid of the evidence, not a living.

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
| **Call a ride** | A cab drives to you and takes you to your waypoint; the fare is charged on arrival (cash or card) |
| **Order** | Delivery of shop items (food, drink, tools, clothes) to where you are or to your crash pad. Shop price **+10% + $15**. Illegal goods are paid in crypto at the day's rate. Fewer shops need to exist on the map. |
| **Mail** | A saved inbox you can reread: the controls guide, receipts (hospital bills, booking fees, rent, orders), job briefs, licence paperwork, landlord notices. Texts are quick pings; anything you'd want to look up later also lands here. |

Text examples: rent reminders, job offers, "your package is ready", gang threats when rep drops, a fence offering a deal.

Until M2, texts are feed notifications (rereadable in Pause > Brief > Notifications). New saves get the controls as four texts; F10 > Text me the controls resends them.

**Mail is built early (M0)** as F10 > Mail, so notes and test steps can be sent to the player during development: drop a `.txt` into `scripts/AnotherLife/mail/inbox/` and it's delivered with a text ping. The mailbox (`mail/mailbox.json`) is separate from the save. In M2 it moves onto the phone and the game starts sending its own mail (receipts, notices) through `Mailbox.Send`.

### 11a. Addresses

Every spot on the map has an address: **nearest postal + street + zone**, e.g. **#4021 Elgin Ave, Downtown**.

- **Postals:** 1,687 points from [DevBlocky/nearest-postal](https://github.com/DevBlocky/nearest-postal) (`new-postals.json`, MIT) in `data/postals.json`. Your postal is the code of the closest point.
- **Street and zone** come from the game (`World.GetStreetName`, `World.GetZoneLocalizedName`).
- Shown beside the minimap: `4021 | Elgin Ave | Downtown` (turn it off with `hud.showAddress` in `rules.json`).
- **Used by:** job offers ("Pickup #3150 → Drop #1204"), GPS to a postal from the phone, deliveries ("to #2210"), property listings, police texts ("shots near #4021"), and zones rolled up into reputation districts.
- **Optional:** a pause-map texture with the postal numbers printed on it (the "New & Improved Postal Map" art) can be installed separately; the codes match `new-postals.json`.

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
| **M0** ✅ built | Skeleton | The DLL loads in story mode; data JSON loads; F9 reload; save/load works; a debug menu; d20 checks with practice; death/arrest/debt rules; real death and arrest detection |
| **M1** | Arrival + crash pad | Works with Freemode Identity + Menyoo: ID-photo flash, LSIA arrival scene, "pick a crash pad" text, pins for bed/workbench/stash/car, beater on the lot, bed saves |
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

## 14. Decisions log

| Question | Decision |
|---|---|
| Character | Freemode Identity + Menyoo build and keep the character; Another Life adds the ID-photo flash. Start in sweats, T-shirt, white sneakers; buy clothes at vanilla stores. |
| Money | Cash = game wallet (Freemode Identity). ATMs: cash ↔ bank, $3 withdraw fee. Payphones: crypto → bank minus 15–35%. |
| Death | §4a: $250 bill (cash, else card, else what you have); lose everything carried except card, IDs, phone, keys. |
| Arrest | §4a: station/jail respawn, seizure, flagged ID, $100 fee, $500 impound, bondsman up to $1k, permanent star at bad cop rep. |
| Dice | DC shown, numbers flick and land; skills improve with practice. |
| Interiors | None; exterior pins only. |
| Extras | Call-a-ride and delivery orders on the phone; 3 stolen-car dumps with a cooldown. |

### Assumptions to confirm

1. **The phone survives death and arrest.** Everything runs through it, so losing it would lock you out of the game.
2. **"Killed by cops"** = the killer is a police, SWAT or army ped.
3. **Hospital bill with no card and less than $250**: they take what cash you have and drop the rest (no debt).
4. **Coordinates** for the crash pads, Bolingbroke, LSIA arrivals and the dump spots are estimates (marked `verify`). Use F10 → Go to, and Log my position, to fix them.
