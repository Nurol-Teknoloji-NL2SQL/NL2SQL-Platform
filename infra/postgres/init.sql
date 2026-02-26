-- ============================================================
-- PostgreSQL DB Seeding Script
-- users, products, orders, order_items
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    phone VARCHAR(30),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS products (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    sku VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    unit_price NUMERIC(12,2) NOT NULL CHECK (unit_price >= 0),
    stock_quantity INTEGER NOT NULL DEFAULT 0 CHECK (stock_quantity >= 0),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS orders (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    order_number VARCHAR(40) NOT NULL UNIQUE,
    status VARCHAR(20) NOT NULL DEFAULT 'pending',
    order_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    total_amount NUMERIC(12,2) NOT NULL DEFAULT 0 CHECK (total_amount >= 0),
    shipping_address TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    CONSTRAINT fk_orders_user
        FOREIGN KEY (user_id) REFERENCES users(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS order_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    order_id UUID NOT NULL,
    product_id UUID NOT NULL,
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    unit_price NUMERIC(12,2) NOT NULL CHECK (unit_price >= 0),
    line_total NUMERIC(12,2) GENERATED ALWAYS AS (quantity * unit_price) STORED,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    CONSTRAINT fk_order_items_order
        FOREIGN KEY (order_id) REFERENCES orders(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_order_items_product
        FOREIGN KEY (product_id) REFERENCES products(id)
        ON DELETE RESTRICT,
    CONSTRAINT uq_order_product UNIQUE (order_id, product_id)
);

CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_products_sku ON products(sku);
CREATE INDEX IF NOT EXISTS idx_orders_user_id ON orders(user_id);
CREATE INDEX IF NOT EXISTS idx_orders_order_date ON orders(order_date DESC);
CREATE INDEX IF NOT EXISTS idx_order_items_order_id ON order_items(order_id);
CREATE INDEX IF NOT EXISTS idx_order_items_product_id ON order_items(product_id);

INSERT INTO users (id, first_name, last_name, email, phone)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Ayse', 'Yilmaz', 'ayse.yilmaz@example.com', '+90-532-111-2233'),
    ('22222222-2222-2222-2222-222222222222', 'Mehmet', 'Kaya', 'mehmet.kaya@example.com', '+90-533-222-3344'),
    ('33333333-3333-3333-3333-333333333333', 'Elif', 'Demir', 'elif.demir@example.com', '+90-534-333-4455'),
    ('44444444-4444-4444-4444-444444444444', 'Can', 'Acar', 'can.acar@example.com', '+90-535-444-5566')
ON CONFLICT (email) DO NOTHING;

INSERT INTO products (id, sku, name, description, unit_price, stock_quantity, is_active)
VALUES
    ('aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 'LTP-14-PRO', 'Laptop Pro 14"', '14 inch developer laptop', 45999.00, 25, TRUE),
    ('aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 'MON-27-4K', 'Monitor 27" 4K', '27 inch IPS 4K monitor', 12999.00, 40, TRUE),
    ('aaaaaaa3-aaaa-aaaa-aaaa-aaaaaaaaaaa3', 'KEY-MECH-TR', 'Mechanical Keyboard TR', 'Mechanical keyboard with TR layout', 3499.00, 80, TRUE),
    ('aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4', 'MOU-WL-ERG', 'Wireless Ergonomic Mouse', 'Ergonomic wireless mouse', 1799.00, 120, TRUE),
    ('aaaaaaa5-aaaa-aaaa-aaaa-aaaaaaaaaaa5', 'DOC-USB-C', 'USB-C Dock Station', 'Dock station with HDMI and LAN', 4999.00, 35, TRUE)
ON CONFLICT (sku) DO NOTHING;

INSERT INTO orders (id, user_id, order_number, status, order_date, total_amount, shipping_address)
VALUES
    ('bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '11111111-1111-1111-1111-111111111111', 'ORD-2026-0001', 'completed', NOW() - INTERVAL '10 day', 58998.00, 'Ankara, Cankaya, Turkiye'),
    ('bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2', '22222222-2222-2222-2222-222222222222', 'ORD-2026-0002', 'processing', NOW() - INTERVAL '6 day', 14798.00, 'Istanbul, Kadikoy, Turkiye'),
    ('bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3', '33333333-3333-3333-3333-333333333333', 'ORD-2026-0003', 'shipped', NOW() - INTERVAL '3 day', 22998.00, 'Izmir, Bornova, Turkiye'),
    ('bbbbbbb4-bbbb-bbbb-bbbb-bbbbbbbbbbb4', '11111111-1111-1111-1111-111111111111', 'ORD-2026-0004', 'pending', NOW() - INTERVAL '1 day', 4999.00, 'Ankara, Cankaya, Turkiye')
ON CONFLICT (order_number) DO NOTHING;

INSERT INTO order_items (id, order_id, product_id, quantity, unit_price)
VALUES
    ('ccccccc1-cccc-cccc-cccc-ccccccccccc1', 'bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1', 'aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 1, 45999.00),
    ('ccccccc2-cccc-cccc-cccc-ccccccccccc2', 'bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1', 'aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 1, 12999.00),

    ('ccccccc3-cccc-cccc-cccc-ccccccccccc3', 'bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 'aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 1, 12999.00),
    ('ccccccc4-cccc-cccc-cccc-ccccccccccc4', 'bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2', 'aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4', 1, 1799.00),

    ('ccccccc5-cccc-cccc-cccc-ccccccccccc5', 'bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3', 'aaaaaaa5-aaaa-aaaa-aaaa-aaaaaaaaaaa5', 1, 4999.00),
    ('ccccccc6-cccc-cccc-cccc-ccccccccccc6', 'bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3', 'aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 1, 12999.00),
    ('ccccccc7-cccc-cccc-cccc-ccccccccccc7', 'bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3', 'aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4', 3, 1666.67),

    ('ccccccc8-cccc-cccc-cccc-ccccccccccc8', 'bbbbbbb4-bbbb-bbbb-bbbb-bbbbbbbbbbb4', 'aaaaaaa5-aaaa-aaaa-aaaa-aaaaaaaaaaa5', 1, 4999.00)
ON CONFLICT (order_id, product_id) DO NOTHING;
