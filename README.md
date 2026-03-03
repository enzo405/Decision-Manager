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

Random events can occur each turn, simulating the unpredictability of the professional world. Difficulty scales with the player's level — thresholds tighten and negative events become more frequent and impactful.

---

## Pedagogical Goals

- Understand the human consequences of managerial decisions
- Learn to manage risk and uncertainty
- Identify the fragile balance between performance and well-being
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
4. A random event may occur
5. Feedback popup explains what happened and why
6. Player clicks Continue — next turn begins

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
Thresholds tighten as the player levels up — the same decisions become riskier at higher levels. Negative effects on failed cards are amplified by 5% per level.

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

---

## Backend Integration

Decision Manager connects to the [DecisionManager API](https://github.com/your-username/DecisionManagerAPI) to :
- Persist player progression across devices reinstalls
- Dynamically configure cards and game settings remotely
- Roll random events server-side

Player identity is based on `SystemInfo.deviceUniqueIdentifier` — no login required.

---

## Known Limitations

- `deviceUniqueIdentifier` resets on app reinstall on Android 10+ — progression may be lost
