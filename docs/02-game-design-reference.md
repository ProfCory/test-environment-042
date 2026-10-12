# METER RUNNING — Game Design Reference

Sections: **A. Character · B. Controls · C. Look & Feel · D. Eras · E. Customers · F. Dispatch · G. Economy**

---

## A. Character

### A1. Creation (one screen, under 60 seconds)
- **Name, look, hometown.** Choose 1 of 8 portraits and 6 skin/hair palettes. They age automatically per Era.
- **Background** (one choice, gives a starting perk and a starting debt):

| Background | Starting perk | Starting burden |
|---|---|---|
| Vet back from service | *Steady Hands*: −25% crash damage | VA paperwork, night terrors (fatigue +10% at night) |
| Immigrant, new in town | *Two Tongues*: speaks a second language, bonus tips from matching fares | Sends $50/wk home |
| Ex-mechanic | *Wrench*: repairs cost −40%, roadside fixes | Old shop debt $1,200 |
| Night-school student | *Book Smart*: learns new districts 2× faster | Tuition due each semester |
| Local kid, born here | *Knows Every Alley*: shortcuts visible from day 1 | Family nearby asks for rides (free) |
| Ex-con, second chance | *Read the Room*: spots robbers/fare-jumpers early | Cops stop you more often |

### A2. Stats (1–5, raised with XP from fares)
| Stat | Affects |
|---|---|
| **Streetsense** | Route knowledge, shortcut reveals, ETA accuracy, fewer wrong turns |
| **Charm** | Tips, ratings (Era III+), talking down angry or drunk fares |
| **Nerve** | Night/rough-district fares, robbery outcomes, holding speed in rain |
| **Wrench** | Breakdown chance, roadside repair, cab upkeep |
| **Stamina** | Shift length before fatigue penalties |

### A3. Needs (HUD meters)
- **Fatigue**: rises over a shift; at high levels the screen vignettes and steering lags slightly. Coffee helps briefly; sleep resets it.
- **Hunger / Bladder**: light-touch. Diner and gas-station stops are mini-events where you meet regulars and hear rumors.
- **Stress**: from crashes, rude fares, and debt. High stress cuts Charm.

### A4. Reputation tracks (−3 to +3)
**Dispatch** (better calls) · **Cops** (fewer stops, looser tickets) · **Garage** (repair priority) · **Nightlife** (clubs and bars call you) · **Uptown** (hotels, corporate → unlocks the Era IV high-end path) · **The Street** (neighborhood regulars).

### A5. Regulars
~20 named NPCs who show up across Eras and age with you (e.g., a club kid in 1982 becomes a tech VP booking your black car in 2023). Memory flags: `rodeWith`, `wasKind`, `overcharged`, `savedFrom`. This is the long-game hook.

---

## B. Controls

### B1. Desktop
| Action | Keys |
|---|---|
| Throttle / Brake-Reverse | `W` / `S` (or ↑ / ↓) |
| Steer | `A` / `D` (or ← / →) |
| Handbrake | `Space` |
| Horn | `H` |
| Pick up / drop off / interact | `E` (when stopped near a fare or marker) |
| Turn signal (left/right) | `Q` / `R` (cops and some fares notice) |
| Device (CB / MDT / Phone / Tablet) | `Tab`: opens the era's dispatch device |
| Accept / Decline call | `Y` / `N` (also clickable) |
| Map | `M` |
| Talk options with fare | `1` `2` `3` |
| Pause | `Esc` |
| Gamepad | Left stick steer, RT/LT throttle/brake, A interact, Y device, B horn |

### B2. Phone (landscape)
| Scheme | Layout | Notes |
|---|---|---|
| **Classic (default)** | Left thumb: floating virtual joystick (steer). Right thumb: big GAS and BRAKE pedals, small HORN | Interact button pops up contextually over the fare |
| **Assisted** | Tap a destination on the minimap → cab auto-follows the route; player controls only speed (gas/brake) and lane swerve with swipes | For one-handed / casual play. Small tip penalty ("you're a little robotic"), none in Era IV, where smooth > fast |
| **Tilt** | Phone tilt to steer, pedals on right | Optional; calibrate on first use |

- **Device panel:** swipe up from the bottom edge (Era III phone mount), or tap the dash icon.
- **Haptics:** short pulse on collision, long pulse on incoming dispatch call.
- **Accessibility:** colorblind-safe fare markers (shape + color), text scaling, "no fail" mode (no rent pressure), remappable keys, subtitles on all radio voice.

### B3. Feel targets
- 0→cruise in ~2 s, gentle oversteer on handbrake; the cab should feel *heavy* in Era I and *floaty-smooth* in Era IV.
- Boarding window: stopped within ~1.5 tiles of the fare for 0.5 s.
- Camera: slight look-ahead in the direction of travel; zooms out a little at speed.

---

## C. Look & Feel

### C1. Overall
**Top-down pixel art** (32px tiles, 2× scale), GTA1/Hotline-Miami angle, with modern lighting (day/night tint, headlight cones, wet-street reflections in rain). The city is fictional, **"Port Calder"**, a coastal East-Coast-ish city with 9 districts.

### C2. Per-era identity
| | Era I · 1978–89 | Era II · 1990–2008 | Era III · 2009–19 | Era IV · 2020–26 |
|---|---|---|---|---|
| **Palette** | Sodium orange, nicotine yellow, smog brown | Teal/magenta neon, then washed silver | Flat white, app-blue, LED cold | Matte black, champagne gold, soft violet night |
| **Dispatch device** | CB radio with a mic you "key" (hold to talk) | Green-on-black MDT terminal, beeping queue | Phone on a dash mount: pings, map pins, star rating | Concierge tablet: client dossier, itinerary, preferences |
| **HUD font** | Typewriter / meter dial | Segmented LCD / VGA | System sans | Thin luxury serif + sans |
| **Meter** | Mechanical drum with flag | Red LED digits | Hidden: app shows a fixed fare | "Contract" (hourly/flat) |
| **Music** | Disco, funk, early punk radio stations | Grunge, hip-hop, smooth jazz station | Lo-fi playlists, podcasts | Ambient, client-requested quiet |
| **Street life** | Payphones, porn-theater marquees, steam vents | Pagers, coffee chains appear, Y2K billboard | Scooters, bike lanes, food-delivery bikes | EV chargers, robotaxis, glass towers, drone deliveries |
| **Weather FX** | Heavy rain, garbage-strike haze | Snowstorm event | Heat wave, surge pricing | Clean, quiet, but robotaxis clog lanes |

### C3. Mood keywords
Era I *gritty, noisy, human* → Era II *busy, wired, ambitious* → Era III *cold, precarious, gamified* → Era IV *polished, quiet, lonely at the top.*

### C4. Audio
Engine note per vehicle; the radio dispatcher as a voiced character (barks + subtitles); passengers in Simlish-style gibberish with subtitled lines; rain on the roof; meter "ka-chunk" in Era I versus the app "ding" in Era III.

---

## D. Eras: rules that change

| Rule | I | II | III | IV |
|---|---|---|---|---|
| Street hails | ✔ main source | ✔ | ✖ rare (illegal pickups risk ticket) | ✖ |
| Dispatch | Radio voice, bid by zone | MDT queue, first-to-accept | Algorithm, 15 s accept timer, acceptance rate tracked | Booked contracts + app overflow |
| Fare calc | Meter (distance + wait) | Meter + flat airport rate | Platform sets fare, takes 25–40% cut | Hourly/flat, negotiated |
| Rating | Word of mouth (Rep tracks) | Rep + complaints to the commission | ⭐ 1–5; below 4.6 = deactivation warning | Client retention and referrals |
| Navigation | Paper map + Streetsense | Paper → early GPS (unlock ~2003) | Turn-by-turn in app | Turn-by-turn + client preferences |
| Car ownership | Lease a shift | Lease → **buy a medallion** | Own car (loan) | Lease a luxury EV or own the fleet |
| Threat | Robbery, breakdowns | Competition, cops | Deactivation, burnout | Losing clients to robotaxis |

### D1. Era transitions
An **Era Card** screen between eras: newspaper headline + ledger summary + "where are they now" for regulars + a choice that carries forward (e.g., 1989: *buy a medallion at $140k on a loan?*).

### D2. Era IV: the high-end ladder
1. **App Contractor**: juggle 2–3 apps, chase bonuses.
2. **Black Car**: licensed livery, airport/corporate accounts, dress code, an immaculate cab (cleanliness meter).
3. **Executive / VIP**: NDAs, security escorts, discretion choices (ignore what you overhear or sell it?).
4. **Fleet Owner** (endgame): hire NPC drivers and run dispatch *yourself*, flipping the first Era's power relationship.

---

## E. Customers

### E1. Fare model (`customers.json`)
```json
{
  "id": "nervous_bride",
  "eras": [1, 2],
  "districts": ["harbor_heights", "old_market"],
  "time": ["day"],
  "patience": 0.6,
  "tipBase": 0.25,
  "risk": 0,
  "wants": "fast",
  "event": "late_to_wedding",
  "lines": ["It's at noon. It's 11:40. Drive."]
}
```
`wants` ∈ `fast | smooth | quiet | chatty | scenic | discreet`, scored against how you actually drove.

### E2. Archetypes
| Era | Customer | Wants | Tip / Pay | Twist |
|---|---|---|---|---|
| I | Dockworker after shift | cheap, direct | low | Becomes a regular, gives union rumors |
| I | Disco crowd (3–4 pax) | fast, chatty | high | Loud; may vomit (cleaning cost) |
| I | Nervous bride | fast | very high if on time | Timer event |
| I | Pregnant woman in labor | fast + smooth | gratitude, Rep +2 | Hospital run, crash = disaster |
| I | Stick-up man | — | takes your cash | Nerve check: hand it over / talk / floor it |
| I | Fare-jumper | — | 0 | Bolts at a red light; chase or let go |
| II | Business traveler (airport) | fast, quiet | flat rate + tip | Airport-stand queue |
| II | Stockbroker 1987/2008 crash night | discreet | huge tip or none | Mood depends on the market event |
| II | Tourist family | scenic | medium | Overcharge option: money now, Rep hit later |
| II | Off-duty cop | smooth | small tip | Cops Rep +1 |
| III | Bar-close crowd, surge | fast | surge multiplier | 1★ risk; drunk events |
| III | Commuter pool (2 strangers) | direct | split low | Route-optimization puzzle |
| III | Influencer filming | scenic, chatty | rating bump/crash | Viral moment |
| III | Gig-worker nurse | quiet | low pay, high rating | Talks about burnout, mirroring yours |
| IV | Corporate client (account) | quiet, on-time | contract | Cleanliness + punctuality scored |
| IV | Celebrity incognito | discreet | big + NDA | Paparazzi chase event |
| IV | Wedding / gala booking | smooth | hourly | Multi-stop itinerary |
| IV | Old regular, now rich | chatty | very high | Payoff for Era I kindness |
| IV | Autonomous-fleet exec | — | — | Offers you a "safety driver" job: end the career or keep driving? |

### E3. Fare outcomes
**Satisfaction** = 0.4·time vs. ETA + 0.3·smoothness (hard brakes, collisions) + 0.2·`wants` match + 0.1·dialogue choices. Drives tip %, Rep, rating, and regular-unlock chance.

---

## F. Dispatch

### F1. Era I: "Marge" on the radio
- Marge (voiced, tired, funny) calls out jobs by **zone**: *"Car in Zone Four, I got a pickup at 1200 Baxter, going to the airport."*
- Player **keys the mic** (hold `Tab`/button) to bid with your zone and car number. Nearest bid usually wins; Dispatch Rep breaks ties.
- Lie about your zone to win a fare: you get it, but arriving late hurts Dispatch Rep. Catch Marge a coffee and she slips you the good jobs.
- Outcomes: real fare, no-show (wasted gas), wrong address, a "Code" (robbery warning, cop request).

### F2. Era II: "The Box" (MDT)
- Green-screen queue: `#4471 HARBOR HTS → INTL AIRPORT  ETA 6  [ACCEPT]`. First to accept wins.
- Dispatchers are now faceless; Marge retires (Era Card beat) and her nephew Dez runs the night desk.
- Airport and hotel **stands**: physical queue lanes; wait time versus guaranteed fares.

### F3. Era III: "The Algorithm"
- Ping pops with a 15 s timer, pickup distance, and **no destination shown** (classic gig tension).
- **Acceptance rate** and **cancellation rate** tracked; drop too low and you get throttled (fewer pings).
- **Surge heatmap** on the phone: chase red zones and arrive as the surge ends.
- Streak bonuses ("3 more trips for $15") are a deliberate trap the player learns to read.

### F4. Era IV: Contracts & Concierge
- **Booking calendar**: accept jobs days ahead; overlapping bookings are a scheduling puzzle.
- **Client dossier**: preferences (temperature, music, water brand, *don't speak first*).
- Run your own dispatch at the Fleet Owner tier: assign NPC drivers to calls. Marge's voice lines return as tutorial callbacks if you hire her granddaughter.

---

## G. Economy (alpha tuning baseline)

| | Era I (1980) | Era II (1998) | Era III (2015) | Era IV (2024) |
|---|---|---|---|---|
| Avg fare (gross) | $6 | $14 | $16 (driver nets ~$11) | $90/hr contract |
| Shift lease / car cost | $45/shift | $110/shift or medallion loan $1,200/mo | Car loan $380/mo + insurance | EV lease $900/mo |
| Gas/charge per shift | $8 | $20 | $25 | $12 |
| Rent per week | $60 | $180 | $450 | $700 |
| Target shift net | $40–70 | $90–160 | $80–140 | $250–500 |
| Medallion value | $60k | $140k → $600k peak | crashes to ~$150k | — |

Dollar figures are game-balance values inspired by the real NYC medallion arc, not historical data.
