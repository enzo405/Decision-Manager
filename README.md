# Decision Manager

A mobile serious game about managerial decision-making and human resource management under uncertainty.

Built with Unity for Android, Decision Manager puts you in the role of a newly appointed manager responsible for balancing your team's performance and well-being over 12 weeks.

---

## Screenshots

> _Screenshots coming soon_

<!-- 
| Main Game | Feedback Popup | Game Over |
|---|---|---|
| ![Main Game](screenshots/main_game.png) | ![Feedback](screenshots/feedback.png) | ![Game Over](screenshots/game_over.png) |
-->

---

## Concept

Each turn, the player chooses one of three decision cards — such as organizing a team meeting, setting ambitious goals, or launching a training program. Every decision has a success probability and affects four key statistics :

- **Motivation** — team engagement
- **Stress** — pressure felt by the team
- **Performance** — overall productivity
- **Turnover** — risk of team members leaving

Each card may also carry **deferred events** — consequences that trigger in the turns following the decision, simulating the delayed and unpredictable nature of managerial choices.

Random events can occur each turn. Difficulty scales with the player's level — thresholds tighten and negative effects become more impactful.

---

## Pedagogical Goals

- Understand the human consequences of managerial decisions
- Learn to manage risk and uncertainty
- Identify the fragile balance between performance and well-being
- Experience the delayed consequences of decisions
- Develop a medium to long-term strategic vision

---

## Tech Stack

- **Engine** : Unity 6 (2D)
- **Language** : C#
- **Platform** : Android
- **Orientation** : Portrait
- **Backend** : [DecisionManager API](https://github.com/enzo405/Decision-Manager-API) (ASP.NET Core 9 + PostgreSQL)

---

## Gameplay

### Core Loop
1. Player selects one of three decision cards
2. A probability roll determines success or failure
3. Stats update immediately
4. Deferred events from previously played cards are evaluated
5. A random event may trigger based on cards played
6. Feedback popup explains what happened and why
7. Player clicks Continue — next turn begins

### Win / Loss Conditions

**Victory** — survive 12 weeks while keeping :
- Stress below the threshold
- Turnover under control
- Performance above the minimum

**Defeat** triggered by :
- **Burnout** — stress too high
- **Massive departures** — turnover too high
- **Poor performance** — performance too low

### Difficulty Scaling
Thresholds tighten as the player levels up — the same decisions become riskier at higher levels. Negative effects on failed cards and triggered events are amplified by 2% per level, up to a maximum of 20%.

---

## Event System

Each card can carry one or more **deferred events** — consequences that may trigger in a defined week range after the card is played.

For example, playing *Transformation Agile* at week 3 might trigger *"La résistance au changement refait surface"* between weeks 2 and 7.

Each event has :
- A **week range** (relative to when the card was played)
- A **chance** of triggering
- **Stat effects** (amplified by player level)

This system simulates the delayed and compounding consequences of managerial decisions.

---

## Progression System

- Players earn XP each turn (base + bonus for good decisions)
- Level up unlocks new, more complex decision cards
- Progression persists across games via the backend API
- 20 levels — from **Manager Junior** to **Directeur Exécutif**

| Level | Title |
|---|---|
| 1 | Manager Junior |
| 2-3 | Manager |
| 4-5 | Manager Confirmé |
| 6-7 | Manager Senior |
| 8-10 | Directeur |
| 11+ | Directeur Exécutif |

---

## Cards

30 decision cards spread across 5 unlock levels :

| Level | Examples |
|---|---|
| 1 | Réunion d'équipe, Formation professionnelle, Gestion de crise |
| 2 | Session de mentorat, Programme bien-être, Entretien annuel |
| 3 | Transformation Agile, Séminaire stratégique, Développement du leadership |
| 4 | Intégration post-fusion, Laboratoire d'innovation, Plan de succession |
| 5 | Coaching exécutif, Analyse prédictive RH, Stratégie orientée sens |

Each card has :
- A success probability
- Primary effects on success
- Secondary (often negative) effects on failure
- A risk level (Low / Medium / High)
- A pedagogical feedback message
- Deferred events with delayed stat consequences

---

## Backend Integration

Decision Manager connects to the [DecisionManager API](https://github.com/enzo405/Decision-Manager-API) to :
- Persist player progression across game sessions
- Dynamically fetch cards and their associated events
- Remotely configure game settings and defeat conditions without a game update

Player identity is based on `SystemInfo.deviceUniqueIdentifier` — no login required.

---

## Known Limitations

- `deviceUniqueIdentifier` resets on app reinstall on Android 10+ — progression may be lost
