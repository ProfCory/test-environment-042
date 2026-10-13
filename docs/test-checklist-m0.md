# M0 Test Checklist

Takes about 15 minutes. Tick each box, and for anything that fails, note what happened and attach `scripts/AnotherLife/log.txt`.

## Loads

- [ ] Story mode loads with no crash
- [ ] "Another Life loaded" notification appears
- [ ] `scripts/AnotherLife/log.txt` exists and has no errors

## Menu (F10)

- [ ] F10 opens the menu; F10 or Backspace closes it
- [ ] Status line shows `$40 cash | $0 bank | 0 crypto`
- [ ] `+$100 cash` → status shows $140
- [ ] `+$1,000 bank` → status shows $1000 bank

## Dice checks (F10 → Roll a check)

- [ ] Each check shows a line like `Bust the window: d20 13 +0 = 13 vs DC 11 SUCCESS`
- [ ] Roll `Bust the window` about 6 times → a "brute improved to +1" message appears, and the menu label now shows `brute +1`
- [ ] In a sports or super car, `Pick the lock` shows a higher DC than on foot
- [ ] With a wanted star, checks show `dis` (two dice, lower one counts)
- [ ] `Add lockpick + screwdriver`, then `Hotwire` and `Pick the lock` show `adv` (two dice, higher counts)
- [ ] Keep rolling `Hotwire` until a natural 1 → "Your screwdriver broke", and the next `Hotwire` is back to one die
- [ ] `Hack the panel` says `tool_required`

## Penalties (menu simulations)

- [ ] With $140 cash: `Simulate death penalty` → "Hospital: $250…" took what cash you had; cash is $0
- [ ] With the bank card and $0 cash: death → "$250 charged to your card"; bank goes down by 250
- [ ] `Simulate arrest penalty` → "Booked", "Your fake ID is flagged"
- [ ] With no bank account: arrest → "Bail bondsman covered $100"; status shows bond debt $100
- [ ] Arrest 10+ times with no bank → eventually "No bail"
- [ ] Arrest a few times → "Every cop in the county knows your face" → you always have 1 star
- [ ] `Clear fake ID flag` → the permanent star stops coming back (after cop rep allows)

## Real death and arrest

- [ ] Die for real (jump off something tall) → hospital messages appear; after respawn you're moved to the nearest hospital in `locations.json`
- [ ] Get busted for real → booking messages appear; after respawn you're moved to the nearest police station
- [ ] Get killed by a cop → treated as an arrest (booking messages, police-station respawn)

## Save

- [ ] `Save now` → "Saved."; `save/slot1.json` exists
- [ ] Quit to desktop, relaunch → status shows the same money
- [ ] Break `rules.json` on purpose (delete a comma), press F9 → a red "Data error" message appears and the game keeps running. Fix it and press F9 → "Data reloaded."

## Wallet, ATMs and payphones (as your freemode character, Freemode Identity on)

- [ ] New game (F10 → Start a new game): the Freemode Identity wallet shows **$40**
- [ ] Buy something cheap at a vanilla shop → F10 status cash drops by the same amount
- [ ] F10 `+$100 cash` → the Freemode Identity wallet goes up $100
- [ ] Switch to Franklin and back: Franklin's own money is never changed by Another Life
- [ ] Walk up to any ATM → "Press E to use the ATM". With no account → "You don't have an account."
- [ ] F10 `+$1,000 bank` (opens account, gives card) → ATM shows bank and cash; deposit $20 → cash −20, bank +20
- [ ] Withdraw $20 → cash +20, bank −23 ($3 fee)
- [ ] Withdraw more than your balance → "Insufficient funds"
- [ ] Any payphone → "Press E". With crypto (F10 `+0.1 crypto`) and an account → wire all → bank goes up by coins × rate × 85%
- [ ] Payphone with no bank account → refuses
- [ ] Die with a gun → after respawn you have no weapons, and they don't come back a few seconds later

## Addresses

- [ ] Beside the minimap: `<postal> | <street> | <zone>`, updating as you drive
- [ ] F10 → `Where am I?` shows the full address, e.g. `#4021 Elgin Ave, Downtown`
- [ ] F10 → `GPS to postal` → type a code (e.g. `2000`) → a waypoint appears; a bad code says "No postal"
- [ ] Set `"showAddress": false` under `hud` in `rules.json`, press F9 → the address disappears

## Coordinates (helps M1)

- [ ] F10 → Go to → each crash pad and location marked `*`: does it land somewhere sensible (not underground, not in the sea)?
- [ ] For any bad one, walk to where it *should* be and use `Log my position` with a label → send me `positions.txt` (each line now includes the address)
- [ ] Bonus: stand where you'd want the bed, workbench, stash and car spawn for each crash pad and log those too
