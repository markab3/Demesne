-- Demesne initial schema
-- Apply with: psql -U postgres -d demesne -f migrations/001_initial_schema.sql

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================
-- Enums
-- ============================================================

DO $$ BEGIN
    CREATE TYPE terrain_type AS ENUM (
        'Forest', 'Pasture', 'Meadow', 'Mineral', 'Coastal', 'River', 'Desert'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE specialist_trade AS ENUM (
        'Blacksmith', 'Brewer', 'Weaver', 'Carpenter', 'Tanner',
        'Miller', 'Baker', 'Alchemist', 'Cartographer'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE retainer_type AS ENUM (
        'ProfessionalSoldier', 'General', 'Knight', 'Spy', 'Diplomat', 'Herald'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE messenger_status AS ENUM (
        'InTransit', 'Captured', 'Delivered', 'Lost'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE agreement_type AS ENUM (
        'TradeLeague', 'DefensePact', 'Suzerainty'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE agreement_status AS ENUM (
        'Active', 'Broken', 'Expired'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
    CREATE TYPE order_status AS ENUM (
        'Pending', 'Executing', 'Completed', 'Failed', 'Cancelled'
    );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

-- ============================================================
-- Players
-- ============================================================

CREATE TABLE IF NOT EXISTS players (
    player_id      UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    username       VARCHAR(100) UNIQUE NOT NULL,
    password_hash  VARCHAR(255) NOT NULL,
    created_at     TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

-- ============================================================
-- Cities
-- ============================================================

CREATE TABLE IF NOT EXISTS cities (
    city_id          UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id         UUID             REFERENCES players(player_id),
    name             VARCHAR(100)     NOT NULL,
    prestige         DOUBLE PRECISION NOT NULL DEFAULT 0,
    authority        DOUBLE PRECISION NOT NULL DEFAULT 100,
    authority_budget DOUBLE PRECISION NOT NULL DEFAULT 0,
    treasury         INT              NOT NULL DEFAULT 0,
    happiness        DOUBLE PRECISION NOT NULL DEFAULT 100,
    crime_pool       DOUBLE PRECISION NOT NULL DEFAULT 0
);

-- Per-industry craft quality scores (1–100)
CREATE TABLE IF NOT EXISTS city_craft_ratings (
    city_id  UUID             NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    trade    specialist_trade NOT NULL,
    rating   DOUBLE PRECISION NOT NULL DEFAULT 1 CHECK (rating BETWEEN 1 AND 100),
    PRIMARY KEY (city_id, trade)
);

-- ============================================================
-- Tiles (hex grid, axial q/r coordinates)
-- ============================================================

CREATE TABLE IF NOT EXISTS tiles (
    tile_id              UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    q                    INT              NOT NULL,
    r                    INT              NOT NULL,
    altitude             DOUBLE PRECISION NOT NULL,
    temperature          DOUBLE PRECISION NOT NULL,
    precipitation        DOUBLE PRECISION NOT NULL,
    arable_percentage    DOUBLE PRECISION NOT NULL CHECK (arable_percentage BETWEEN 0 AND 1),
    terrain_type         terrain_type     NOT NULL,
    controlling_city_id  UUID             REFERENCES cities(city_id),
    UNIQUE (q, r)
);

-- Resource deposits per tile (good_type is text; full enum added once production chains are finalised)
CREATE TABLE IF NOT EXISTS tile_resources (
    tile_id   UUID             NOT NULL REFERENCES tiles(tile_id) ON DELETE CASCADE,
    good_type TEXT             NOT NULL,
    quantity  DOUBLE PRECISION NOT NULL,
    PRIMARY KEY (tile_id, good_type)
);

-- ============================================================
-- Peasant Pools (one per inhabited tile)
-- ============================================================

CREATE TABLE IF NOT EXISTS peasant_pools (
    tile_id          UUID             PRIMARY KEY REFERENCES tiles(tile_id) ON DELETE CASCADE,
    population       INT              NOT NULL DEFAULT 0,
    happiness        DOUBLE PRECISION NOT NULL DEFAULT 100,
    inventory        JSONB            NOT NULL DEFAULT '{}', -- { good_type: quantity }
    goods_tax_rate   DOUBLE PRECISION NOT NULL DEFAULT 0.1 CHECK (goods_tax_rate BETWEEN 0 AND 1),
    corvee_days_owed INT              NOT NULL DEFAULT 0
);

-- ============================================================
-- Nobles / Manors
-- ============================================================

CREATE TABLE IF NOT EXISTS nobles (
    noble_id                UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    city_id                 UUID         NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    name                    VARCHAR(100) NOT NULL,
    loyalty                 DOUBLE PRECISION NOT NULL DEFAULT 100,
    feudal_levy_obligation  INT          NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS noble_tiles (
    noble_id  UUID NOT NULL REFERENCES nobles(noble_id) ON DELETE CASCADE,
    tile_id   UUID NOT NULL REFERENCES tiles(tile_id)   ON DELETE CASCADE,
    PRIMARY KEY (noble_id, tile_id)
);

-- ============================================================
-- Stockpiles
-- ============================================================

CREATE TABLE IF NOT EXISTS stockpiles (
    stockpile_id  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    city_id       UUID UNIQUE NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS stockpile_goods (
    stockpile_id  UUID             NOT NULL REFERENCES stockpiles(stockpile_id) ON DELETE CASCADE,
    good_type     TEXT             NOT NULL,
    quantity      INT              NOT NULL DEFAULT 0,
    quality       DOUBLE PRECISION NOT NULL DEFAULT 50 CHECK (quality BETWEEN 1 AND 100),
    PRIMARY KEY (stockpile_id, good_type)
);

-- Per-good buy parameters: minimum threshold, maximum ceiling, maximum price
CREATE TABLE IF NOT EXISTS stockpile_buy_orders (
    buy_order_id  UUID  PRIMARY KEY DEFAULT gen_random_uuid(),
    stockpile_id  UUID  NOT NULL REFERENCES stockpiles(stockpile_id) ON DELETE CASCADE,
    good_type     TEXT  NOT NULL,
    min_threshold INT   NOT NULL DEFAULT 0,
    max_ceiling   INT   NOT NULL DEFAULT 100,
    max_price     INT   NOT NULL DEFAULT 0,
    UNIQUE (stockpile_id, good_type)
);

-- ============================================================
-- Markets
-- ============================================================

CREATE TABLE IF NOT EXISTS markets (
    market_id  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    city_id    UUID UNIQUE NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS market_listings (
    listing_id  UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    market_id   UUID             NOT NULL REFERENCES markets(market_id) ON DELETE CASCADE,
    good_type   TEXT             NOT NULL,
    quantity    INT              NOT NULL,
    quality     DOUBLE PRECISION NOT NULL CHECK (quality BETWEEN 1 AND 100),
    price       INT              NOT NULL,
    seller_id   UUID             -- city_id or specialist_id; nullable (lord listings have no named seller)
);

CREATE TABLE IF NOT EXISTS market_price_history (
    market_id        UUID             NOT NULL REFERENCES markets(market_id) ON DELETE CASCADE,
    good_type        TEXT             NOT NULL,
    tick_timestamp   TIMESTAMPTZ      NOT NULL,
    price            DOUBLE PRECISION NOT NULL,
    PRIMARY KEY (market_id, good_type, tick_timestamp)
);

-- ============================================================
-- Specialists
-- ============================================================

CREATE TABLE IF NOT EXISTS specialists (
    specialist_id       UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    home_city_id        UUID             NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    name                VARCHAR(100)     NOT NULL,
    primary_trade       specialist_trade NOT NULL,
    skill_rating        DOUBLE PRECISION NOT NULL DEFAULT 10 CHECK (skill_rating BETWEEN 1 AND 100),
    secondary_skills    TEXT[]           NOT NULL DEFAULT '{}',
    inventory           JSONB            NOT NULL DEFAULT '{}', -- { good_type: { quantity, quality } }
    gold                INT              NOT NULL DEFAULT 0,
    happiness           DOUBLE PRECISION NOT NULL DEFAULT 80,
    commission_capacity DOUBLE PRECISION NOT NULL DEFAULT 0.3 CHECK (commission_capacity BETWEEN 0 AND 1),
    loyalty             DOUBLE PRECISION NOT NULL DEFAULT 80
);

-- ============================================================
-- Retainers
-- ============================================================

CREATE TABLE IF NOT EXISTS retainers (
    retainer_id          UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    city_id              UUID             NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    name                 VARCHAR(100)     NOT NULL,
    type                 retainer_type    NOT NULL,
    salary               INT              NOT NULL DEFAULT 0,
    happiness            DOUBLE PRECISION NOT NULL DEFAULT 80,
    inventory            JSONB            NOT NULL DEFAULT '{}',
    loyalty              DOUBLE PRECISION NOT NULL DEFAULT 80,
    equipment_condition  DOUBLE PRECISION NOT NULL DEFAULT 100 CHECK (equipment_condition BETWEEN 0 AND 100),
    -- General-specific fields; null for non-generals
    authority_upkeep     INT,
    command_radius       INT
);

-- ============================================================
-- Merchants
-- ============================================================

CREATE TABLE IF NOT EXISTS merchants (
    merchant_id                UUID  PRIMARY KEY DEFAULT gen_random_uuid(),
    home_city_id               UUID  NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    current_tile_id            UUID  REFERENCES tiles(tile_id),
    current_city_id            UUID  REFERENCES cities(city_id),
    ticks_remaining_in_transit INT   NOT NULL DEFAULT 0,
    inventory                  JSONB NOT NULL DEFAULT '{}',
    trade_ledger               JSONB NOT NULL DEFAULT '{}', -- { "city_id:good_type": LedgerEntry }
    gold                       INT   NOT NULL DEFAULT 0,
    standing_instructions      JSONB NOT NULL DEFAULT '{}'
);

-- Ordered journey legs
CREATE TABLE IF NOT EXISTS merchant_itinerary (
    merchant_id     UUID NOT NULL REFERENCES merchants(merchant_id) ON DELETE CASCADE,
    sequence_order  INT  NOT NULL,
    city_id         UUID NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    PRIMARY KEY (merchant_id, sequence_order)
);

-- ============================================================
-- Messengers
-- ============================================================

CREATE TABLE IF NOT EXISTS messengers (
    messenger_id        UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    sender_city_id      UUID             NOT NULL REFERENCES cities(city_id),
    recipient_city_id   UUID             NOT NULL REFERENCES cities(city_id),
    contents_encrypted  BYTEA,
    current_tile_id     UUID             REFERENCES tiles(tile_id),
    status              messenger_status NOT NULL DEFAULT 'InTransit',
    travel_speed_per_tick INT            NOT NULL DEFAULT 1
);

-- Path calculated at send time; fixed for the journey
CREATE TABLE IF NOT EXISTS messenger_path (
    messenger_id    UUID NOT NULL REFERENCES messengers(messenger_id) ON DELETE CASCADE,
    sequence_order  INT  NOT NULL,
    tile_id         UUID NOT NULL REFERENCES tiles(tile_id),
    PRIMARY KEY (messenger_id, sequence_order)
);

-- ============================================================
-- Diplomatic Agreements
-- ============================================================

CREATE TABLE IF NOT EXISTS diplomatic_agreements (
    agreement_id               UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    type                       agreement_type   NOT NULL,
    terms                      JSONB            NOT NULL DEFAULT '{}',
    status                     agreement_status NOT NULL DEFAULT 'Active',
    reputation_cost_on_break   DOUBLE PRECISION NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS diplomatic_agreement_parties (
    agreement_id  UUID NOT NULL REFERENCES diplomatic_agreements(agreement_id) ON DELETE CASCADE,
    city_id       UUID NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    PRIMARY KEY (agreement_id, city_id)
);

-- ============================================================
-- Buildings
-- ============================================================

CREATE TABLE IF NOT EXISTS buildings (
    building_id    UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    city_id        UUID             NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    type           TEXT             NOT NULL,
    condition      DOUBLE PRECISION NOT NULL DEFAULT 100 CHECK (condition BETWEEN 0 AND 100),
    constructed_at TIMESTAMPTZ      NOT NULL DEFAULT NOW()
);

-- ============================================================
-- Order Queue
-- ============================================================

CREATE TABLE IF NOT EXISTS orders (
    order_id       UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    city_id        UUID         NOT NULL REFERENCES cities(city_id) ON DELETE CASCADE,
    player_id      UUID         NOT NULL REFERENCES players(player_id),
    type           TEXT         NOT NULL,
    parameters     JSONB        NOT NULL DEFAULT '{}',
    priority       INT          NOT NULL DEFAULT 0,
    status         order_status NOT NULL DEFAULT 'Pending',
    created_at     TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    failure_reason TEXT
);
