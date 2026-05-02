PRAGMA foreign_keys = ON;

-- Users
CREATE TABLE IF NOT EXISTS Users (
  UserID INTEGER PRIMARY KEY AUTOINCREMENT,
  Username TEXT NOT NULL UNIQUE,
  Password TEXT NOT NULL,
  Role TEXT NOT NULL DEFAULT 'User' CHECK (Role IN ('Admin', 'User')),
  EvaluatorID INTEGER,

  FOREIGN KEY (EvaluatorID) REFERENCES Evaluators(EvaluatorID)
);

-- Alternative
CREATE TABLE IF NOT EXISTS Cards (
  CardID          INTEGER PRIMARY KEY AUTOINCREMENT,
  CardName        TEXT    NOT NULL UNIQUE,
  CardDescription TEXT    NOT NULL DEFAULT ''
);

-- Criterion
CREATE TABLE IF NOT EXISTS Effects (
  EffectID          INTEGER PRIMARY KEY AUTOINCREMENT,
  EffectCode        TEXT    NOT NULL UNIQUE,
  EffectName        TEXT    NOT NULL,
  EffectDescription TEXT    NOT NULL DEFAULT ''
);

-- Vector
CREATE TABLE IF NOT EXISTS CardEffects (
  CardEffectID INTEGER PRIMARY KEY AUTOINCREMENT,
  CardID       INTEGER NOT NULL,
  EffectID     INTEGER NOT NULL,
  Value        REAL    NOT NULL,

  FOREIGN KEY (CardID)   REFERENCES Cards(CardID)   ON DELETE CASCADE,
  FOREIGN KEY (EffectID) REFERENCES Effects(EffectID) ON DELETE RESTRICT,

  UNIQUE (CardID, EffectID)
);

-- LPR
CREATE TABLE IF NOT EXISTS Evaluators (
  EvaluatorID    INTEGER PRIMARY KEY AUTOINCREMENT,
  EvaluatorName  TEXT    NOT NULL UNIQUE,
  CompetenceRank INTEGER NOT NULL CHECK (CompetenceRank >= 0)
);

-- Result
CREATE TABLE IF NOT EXISTS CardRankings (
  RankingID       INTEGER PRIMARY KEY AUTOINCREMENT,
  UserID          INTEGER NOT NULL,
  CardID          INTEGER NOT NULL,
  AlternativeRank INTEGER NOT NULL CHECK (AlternativeRank > 0),

  FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
  FOREIGN KEY (CardID) REFERENCES Cards(CardID) ON DELETE CASCADE,

  UNIQUE (UserID, CardID),
  UNIQUE (UserID, AlternativeRank)
);

-- Indexes
CREATE INDEX IF NOT EXISTS IX_CardEffects_CardID ON CardEffects(CardID);
CREATE INDEX IF NOT EXISTS IX_CardEffects_EffectID ON CardEffects(EffectID);

CREATE INDEX IF NOT EXISTS IX_CardRanking_UserID ON CardRankings(UserID);
CREATE INDEX IF NOT EXISTS IX_CardRanking_CardID ON CardRankings(CardID);

-- Data: Effects (Criteria)
INSERT OR IGNORE INTO Effects (EffectCode, EffectName, EffectDescription) VALUES
('DAMAGE',  'Damage',   'Amount of damage dealt to an enemy'),
('HEAL',    'Healing',  'Amount of health restored'),
('BLOCK',   'Block',    'Amount of damage prevented'),
('UTILITY', 'Utility',  'General usefulness in different situations'),
('CONTROL', 'Control',  'Ability to slow, stun or restrict an enemy'),
('COST',    'Mana Cost','Mana required to use the card');

-- Data: Cards (Alternatives)
INSERT OR IGNORE INTO Cards (CardName, CardDescription) VALUES
('Fireball',      'Deals high direct damage to one enemy.'),
('Ice Bolt',      'Deals damage and applies a control effect.'),
('Poison Strike', 'Applies poison and deals damage over time.'),
('Heal',          'Restores health to the player or ally.'),
('Shield',        'Provides block and defensive support.');

-- Data: CardEffects (Vector of scores)

-- Fireball
INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 10
FROM Cards c, Effects e
WHERE c.CardName = 'Fireball' AND e.EffectCode = 'DAMAGE';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 3
FROM Cards c, Effects e
WHERE c.CardName = 'Fireball' AND e.EffectCode = 'UTILITY';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 2
FROM Cards c, Effects e
WHERE c.CardName = 'Fireball' AND e.EffectCode = 'CONTROL';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 2
FROM Cards c, Effects e
WHERE c.CardName = 'Fireball' AND e.EffectCode = 'COST';

-- Ice Bolt
INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 7
FROM Cards c, Effects e
WHERE c.CardName = 'Ice Bolt' AND e.EffectCode = 'DAMAGE';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 5
FROM Cards c, Effects e
WHERE c.CardName = 'Ice Bolt' AND e.EffectCode = 'UTILITY';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 8
FROM Cards c, Effects e
WHERE c.CardName = 'Ice Bolt' AND e.EffectCode = 'CONTROL';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 2
FROM Cards c, Effects e
WHERE c.CardName = 'Ice Bolt' AND e.EffectCode = 'COST';

-- Poison Strike
INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 6
FROM Cards c, Effects e
WHERE c.CardName = 'Poison Strike' AND e.EffectCode = 'DAMAGE';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 7
FROM Cards c, Effects e
WHERE c.CardName = 'Poison Strike' AND e.EffectCode = 'UTILITY';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 6
FROM Cards c, Effects e
WHERE c.CardName = 'Poison Strike' AND e.EffectCode = 'CONTROL';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 1
FROM Cards c, Effects e
WHERE c.CardName = 'Poison Strike' AND e.EffectCode = 'COST';

-- Heal
INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 9
FROM Cards c, Effects e
WHERE c.CardName = 'Heal' AND e.EffectCode = 'HEAL';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 6
FROM Cards c, Effects e
WHERE c.CardName = 'Heal' AND e.EffectCode = 'UTILITY';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 2
FROM Cards c, Effects e
WHERE c.CardName = 'Heal' AND e.EffectCode = 'CONTROL';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 1
FROM Cards c, Effects e
WHERE c.CardName = 'Heal' AND e.EffectCode = 'COST';

-- Shield
INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 10
FROM Cards c, Effects e
WHERE c.CardName = 'Shield' AND e.EffectCode = 'BLOCK';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 5
FROM Cards c, Effects e
WHERE c.CardName = 'Shield' AND e.EffectCode = 'UTILITY';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 3
FROM Cards c, Effects e
WHERE c.CardName = 'Shield' AND e.EffectCode = 'CONTROL';

INSERT OR IGNORE INTO CardEffects (CardID, EffectID, Value)
SELECT c.CardID, e.EffectID, 1
FROM Cards c, Effects e
WHERE c.CardName = 'Shield' AND e.EffectCode = 'COST';

-- Data: Evaluators (LPR)
INSERT OR IGNORE INTO Evaluators (EvaluatorName, CompetenceRank) VALUES
('Player', 1),
('Game Designer', 2);

-- Data: Users

-- Admin
INSERT OR IGNORE INTO Users (Username, Password, Role, EvaluatorID)
VALUES ('admin', 'admin123', 'Admin', NULL);

-- Player
INSERT OR IGNORE INTO Users (Username, Password, Role, EvaluatorID)
SELECT 'player_user', 'password123', 'User', EvaluatorID
FROM Evaluators
WHERE EvaluatorName = 'Player';

-- Designer
INSERT OR IGNORE INTO Users (Username, Password, Role, EvaluatorID)
SELECT 'designer_user', 'password123', 'User', EvaluatorID
FROM Evaluators
WHERE EvaluatorName = 'Game Designer';

-- Data: Result (manual ranking)

-- Ranking for player_user
INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 1
FROM Users u, Cards c
WHERE u.Username = 'player_user' AND c.CardName = 'Fireball';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 2
FROM Users u, Cards c
WHERE u.Username = 'player_user' AND c.CardName = 'Ice Bolt';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 3
FROM Users u, Cards c
WHERE u.Username = 'player_user' AND c.CardName = 'Shield';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 4
FROM Users u, Cards c
WHERE u.Username = 'player_user' AND c.CardName = 'Poison Strike';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 5
FROM Users u, Cards c
WHERE u.Username = 'player_user' AND c.CardName = 'Heal';

-- Ranking for Game Designer
INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 1
FROM Users u, Cards c
WHERE u.Username = 'designer_user' AND c.CardName = 'Ice Bolt';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 2
FROM Users u, Cards c
WHERE u.Username = 'designer_user' AND c.CardName = 'Shield';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 3
FROM Users u, Cards c
WHERE u.Username = 'designer_user' AND c.CardName = 'Fireball';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 4
FROM Users u, Cards c
WHERE u.Username = 'designer_user' AND c.CardName = 'Heal';

INSERT OR IGNORE INTO CardRankings (UserID, CardID, AlternativeRank)
SELECT u.UserID, c.CardID, 5
FROM Users u, Cards c
WHERE u.Username = 'designer_user' AND c.CardName = 'Poison Strike';