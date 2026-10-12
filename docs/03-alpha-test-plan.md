# METER RUNNING — Final Alpha Test Plan (with Example Results)

> The **checklists** are the real test plan. Anything marked **EXAMPLE** shows what a filled-in report should look like; those are illustrative results, not data from real tests.

---

## 1. Scope & exit criteria

**Build under test:** M10 Alpha. All 4 Eras playable start → finish.

**Alpha passes when:**
- [ ] A fresh character can play 1978 → 2026 without a blocker bug (≈ 6–8 hrs)
- [ ] 0 open **S1** (crash/save loss); ≤ 5 open **S2** (major feature broken)
- [ ] 60 fps on mid-range desktop; ≥ 45 fps on a 2021 mid-range Android / iPhone 12
- [ ] Save → close tab → reopen restores the exact state on every device
- [ ] ≥ 70% of testers rate "one more fare" pull ≥ 4/5
- [ ] Every checklist item below is ✅ or has a ticket

**Device matrix**
| Device | Browser | Input |
|---|---|---|
| Windows laptop | Chrome, Firefox | Keyboard, Xbox pad |
| MacBook | Safari, Chrome | Keyboard |
| iPhone 12 / 15 | Safari, installed PWA | Touch: Classic, Assisted, Tilt |
| Pixel 6a / Galaxy A54 | Chrome, installed PWA | Touch: Classic, Assisted, Tilt |
| iPad 9th gen | Safari | Touch + Bluetooth keyboard |

**Severity:** S1 crash/save loss · S2 feature broken, no workaround · S3 broken with workaround / balance · S4 cosmetic/text.

---

## 2. Checklists

### 2A. Character
- [ ] Creation completes in < 60 s; every field is optional except name
- [ ] All 6 backgrounds apply their perk *and* burden (verify in ledger)
- [ ] Portrait ages visibly at each Era Card
- [ ] Stat XP accrues per fare; level-up prompt appears and applies
- [ ] Fatigue: vignette + steering lag at > 80%; coffee and sleep work
- [ ] Hunger/bladder stops trigger diner/gas events
- [ ] All 6 reputation tracks change and show in the Ledger tab
- [ ] Rep gates work (e.g., Uptown ≥ +2 unlocks Black Car in Era IV)
- [ ] ≥ 5 regulars reappear in a later Era with correct memory flags
- [ ] "No fail" accessibility mode removes rent game-over

### 2B. Controls
- [ ] Keyboard: every binding in Design Ref §B1 works and is remappable
- [ ] Gamepad hot-plug mid-shift works
- [ ] Touch Classic: joystick floats to thumb position; pedals are ≥ 64 px targets
- [ ] Touch Assisted: tap-to-route follows roads; swipe lane-change works
- [ ] Tilt: calibration screen appears on first use; can be recalibrated
- [ ] Boarding window is consistent (≤ 1.5 tiles, 0.5 s stop)
- [ ] No accidental browser gestures (pull-to-refresh, swipe-back) on iOS/Android
- [ ] Device panel opens via `Tab`, dash icon, and swipe-up
- [ ] Haptics fire on collision and dispatch ping (Android; iOS where supported)
- [ ] Input latency feels < 100 ms on phones

### 2C. Look & Feel
- [ ] Each Era's palette, HUD font, meter, and device match Design Ref §C2
- [ ] Day/night cycle tint + headlights; rain reflections render on phones
- [ ] UI readable at 360 px tall landscape; text scaling 100–150% doesn't clip
- [ ] Colorblind mode: fare markers distinguishable by shape
- [ ] Each Era has ≥ 2 radio stations; volume sliders (music/SFX/voice) work
- [ ] All voice lines are subtitled
- [ ] Era Card headline + ledger render correctly for all 3 transitions
- [ ] Mood check (survey): testers name the era's mood with the intended keyword ≥ 50% of the time

### 2D. Era progression
- [ ] Era I: hails + radio work; shift lease charged; robbery event fires
- [ ] Era I → II: medallion purchase offer appears; loan tracked monthly
- [ ] Era II: MDT queue, airport stand queue, GPS unlock ~2003
- [ ] Era II → III: medallion crash event; all 3 exits (Hold / Sell / Convert) play through
- [ ] Era III: app pings, 15 s timer, acceptance rate, ratings, surge map, deactivation warning < 4.6★
- [ ] Era III → IV: path depends on prior choice (contractor vs. black car)
- [ ] Era IV: booking calendar conflicts, client dossier, cleanliness meter
- [ ] Fleet Owner endgame reachable; NPC drivers can be assigned
- [ ] Robotaxi-exec ending choice plays both branches
- [ ] Map overlays change per Era (new towers, bike lanes, chargers)

### 2E. Customers
- [ ] All archetypes in Design Ref §E2 spawn in their eras, districts, and times only
- [ ] `wants` scoring visibly changes tips (test: drive the same route smooth vs. reckless)
- [ ] Timer fares (bride, labor) fail/succeed correctly
- [ ] Fare-jumper: chase and let-go branches both resolve
- [ ] Robbery: hand over / talk / floor it, each tied to Nerve
- [ ] Multi-pax (disco, commuter pool) seat counts and split fares are correct
- [ ] Vomit/cleaning cost event works; Era IV cleanliness carries over
- [ ] Overcharge choice gives cash now and a Rep hit later
- [ ] Old-regular payoff fires in Era IV if flagged `wasKind`
- [ ] No repeated line from the same archetype within 3 fares

### 2F. Dispatch
- [ ] Era I: Marge calls zones; key-mic bid works on keyboard + touch
- [ ] Lying about your zone: win → late arrival → Dispatch Rep drop
- [ ] No-show and wrong-address outcomes occur at the tuned rate (~10% / ~5%)
- [ ] Era II: first-to-accept against NPC cabs; queue scrolls; beep audible
- [ ] Era III: destination hidden until pickup; accept/cancel rates tracked; throttling fires
- [ ] Streak bonus math correct; surge heatmap matches actual fare multiplier
- [ ] Era IV: overlapping bookings flagged; dossier preferences scored
- [ ] Fleet Owner: player-run dispatch assigns NPC drivers who earn and crash
- [ ] Dispatch pings never fire during Era Cards, pause, or open dialogue

---

## 3. Scripted test sessions

| ID | Script | Device | Time |
|---|---|---|---|
| **T1 First Shift** | New character (Immigrant) → finish one full Era I shift → read ledger | iPhone, Classic | 15 min |
| **T2 Radio Hustle** | Era I: win 5 radio bids, lie once, get robbed once | Windows, keyboard | 20 min |
| **T3 Medallion** | Era I → II transition, buy medallion, airport stand ×3 | Pixel, Assisted | 25 min |
| **T4 The Crash** | Era II → III, choose **Sell**, drive 10 app trips, hit surge | Mac, Safari | 30 min |
| **T5 Rating Spiral** | Era III: drive badly on purpose to trigger the deactivation warning | Android, Tilt | 15 min |
| **T6 Black Car** | Load the provided Era IV save → corporate + celebrity + gala bookings | iPad + keyboard | 30 min |
| **T7 Full Career** | 1978 → 2026, no debug skips, record total time + ending | Any | 6–8 hrs |
| **T8 Save Torture** | Save mid-fare, mid-dispatch, mid-Era-Card; kill the tab; reload each | All | 20 min |

---

## 4. EXAMPLE: filled-in alpha results summary

> **EXAMPLE ONLY.** A filled-in report should look like this. These numbers are made up.

**Build:** `alpha-0.10.3` · **Testers:** 12 · **Sessions:** 41 · **Total play:** 63 hrs

| Area | Pass | Fail | Notes |
|---|---|---|---|
| Character | 9/10 | 1 | Regular "Tino" forgets `wasKind` after the Era II → III load (BUG-112) |
| Controls | 8/10 | 2 | iOS swipe-back fires off the left joystick edge (BUG-097); tilt drifts after 10 min |
| Look & Feel | 7/8 | 1 | Era III MDT font clips at 150% text scale |
| Eras | 9/10 | 1 | Fleet Owner unreachable if the player picked **Hold** in Era III (BUG-120, S2) |
| Customers | 9/10 | 1 | Disco crowd repeats "Is this the Paradise Garage?" twice in a row |
| Dispatch | 8/9 | 1 | Algorithm ping fired during an Era Card (BUG-104) |

**Telemetry highlights (EXAMPLE)**
| Metric | Target | Result |
|---|---|---|
| Avg shift length | 6–10 min | 8.4 min ✅ |
| Era I first-shift quit rate | < 20% | 31% ⚠️ (rent shock on day 3) |
| % taking Convert exit | 25–40% | 22% |
| % reaching Era IV | > 60% | 58% ⚠️ |
| Avg app rating at end of Era III | 4.6–4.8 | 4.71 ✅ |
| Median phone fps (A54) | ≥ 45 | 52 ✅ |

**Tester quotes (EXAMPLE)**
- "Marge is the best character in the game. I felt real loss when she retired."
- "The Algorithm era made me anxious in a good way. The hidden destination is evil."
- "Assisted mode on phone is how I'll actually play this on the bus."
- "Era IV is pretty but it's a calendar game. Needed one more driving-skill challenge."

**Decisions (EXAMPLE)**
1. Delay first rent by 2 days and add a Marge tip about the cab stand → retest T1.
2. Fix BUG-120 before beta (S2, blocks an ending).
3. Era IV: add the "paparazzi tail" driving event to every 5th VIP booking.

---

## 5. EXAMPLE: bug reports

```
BUG-097  S2  Controls · iOS Safari 18 · iPhone 12
Title:   Left-edge swipe triggers browser Back mid-drive
Steps:   1. Era I shift  2. Steer hard left with joystick near screen edge
Result:  Safari navigates back, run state lost to last autosave
Expect:  Gesture suppressed while in City scene
Fix idea: overscroll-behavior: none; touch-action: none on canvas; 24px dead zone
```

```
BUG-120  S2  Eras · All
Title:   Fleet Owner tier never unlocks after "Hold" exit
Steps:   Era III choose Hold → reach Era IV with Uptown +3, $50k
Result:  Fleet Owner option missing from ladder
Expect:  Hold path reaches Fleet Owner via medallion buyback event
Cause:   eras.json gate checks `exit == "convert"` only
```

```
BUG-104  S3  Dispatch · Desktop Chrome
Title:   Algorithm ping fires during Era Card
Expect:  Dispatch system paused while any modal screen is open
```

---

## 6. Tester survey (end of session, 1–5 scale + free text)

1. How strong was the "one more fare" pull?
2. Did driving feel different between the Era I cab and the Era IV EV?
3. Which era's dispatch was most fun? Most stressful?
4. Name one regular you remember. What did they want?
5. On phone: which control scheme did you settle on, and why?
6. Did the medallion crash feel fair, or like a cheap shot?
7. One word for each era's mood.
8. What made you quit the session?
