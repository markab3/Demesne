# Economy Core

## Prestige

Prestige is a **continuous linear output metric** — a rate, not a tier system. The city produces X prestige per game-day.

**Sources:**
- Luxury goods available to citizens
- Cultural buildings (temple, theater, bardic hall)
- Trade volume passing through the market
- Vassal count (dominion prestige)

**Effects (continuous, no thresholds or gates):**
- **Trade range radius** — how far caravans and merchants can operate
- **Diplomatic weight** — influence in negotiations, reputation effects
- **Specialist attraction** — skilled craftsmen prefer high-prestige cities
- **Merchant range** — travel range of city-owned merchants is bounded by prestige

A city at 847 prestige reaches slightly further than one at 831.

## Authority

Authority governs territorial control and military projection.

**Income:**
- Base population × happiness modifier
- Happiness at 100% = full authority yield
- Happiness at 50% ≈ half yield
- Happiness at 0% = near-zero or negative if unrest is active

**Expenditure (upkeep):**
- Each claimed tile: flat upkeep (uninhabited tiles cheaper than inhabited)
- Each general: significant upkeep
- Each military unit operating outside territory: small upkeep per unit
- Each vassal: small upkeep per vassal managed

**Tile upkeep scaling:**
- Tiles 1–10: linear cost
- Tiles 11–20: cost × 1.5 per tile
- Tiles 21+: cost × 2.5 per tile

Large empires become structurally fragile — expensive to hold, slow to respond to crises.

### Deficit Cascade

When authority budget goes negative, consequences fire in priority order:

1. Outer uninhabited tiles drop first — low upkeep recovery, low drama
2. Outer inhabited tiles enter unrest timer — player has a window to correct
3. Unrest matures into NPC rebel spawns or crime surge if unaddressed
4. Crime accumulation drains gold and happiness simultaneously — accelerating the cascade

The cascade is self-amplifying if ignored but catchable early. A player who sees happiness dropping has warning before territory begins dissolving.

## Happiness

Happiness drives authority production and is the primary feedback mechanism for player decisions.

**Positive drivers:**
- Food variety and surplus
- Luxury goods access (citizens consuming luxuries boosts happiness and prestige simultaneously)
- Successful trade activity
- Cultural buildings

**Negative drivers:**
- Crime rate
- Tax rate (tunable by player)
- Recent military losses (temporary penalty)
- Food shortage

## Crime

Crime is a **pool metric** — it accumulates and decays rather than spiking instantly.

- Authority deficit causes the crime pool to fill
- Above threshold: gold drain per day + happiness penalty
- At critical levels: specific buildings become temporarily non-functional (market disrupted, port closed)
- Decays naturally with a positive authority budget; faster with garrison presence or civic buildings (constabulary, magistrate)

A player recovering from an authority crisis still has lingering crime to manage — the hole does not disappear the moment the budget is fixed.
