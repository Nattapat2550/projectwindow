using System;
using System.Collections.Generic;
using System.Text;

namespace GTYApp.Data
{
    public static class Migrations
    {
        public static void RunAll()
        {
            using var conn = Db.GetOpenConnection();
            using var tx = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;

            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS users (
 id SERIAL PRIMARY KEY,
 username VARCHAR(50) UNIQUE,
 email VARCHAR(255) UNIQUE NOT NULL,
 password_hash VARCHAR(255),
 role VARCHAR(10) NOT NULL DEFAULT 'user',
 profile_picture_url TEXT DEFAULT 'images/user.png',
 is_email_verified BOOLEAN NOT NULL DEFAULT FALSE,
 oauth_provider VARCHAR(20),
 oauth_id VARCHAR(255),
 created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
 updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
 CONSTRAINT chk_role CHECK (role IN ('user','admin'))
);

CREATE TABLE IF NOT EXISTS verification_codes (
 id SERIAL PRIMARY KEY,
 user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
 code VARCHAR(6) NOT NULL,
 expires_at TIMESTAMPTZ NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_verif_user_exp ON verification_codes(user_id, expires_at);

CREATE TABLE IF NOT EXISTS password_reset_tokens (
 id SERIAL PRIMARY KEY,
 user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
 token VARCHAR(255) UNIQUE NOT NULL,
 expires_at TIMESTAMPTZ NOT NULL,
 is_used BOOLEAN NOT NULL DEFAULT FALSE
);
CREATE INDEX IF NOT EXISTS idx_reset_user_exp ON password_reset_tokens(user_id, expires_at);

CREATE TABLE IF NOT EXISTS homepage_content (
 id SERIAL PRIMARY KEY,
 section_name VARCHAR(100) UNIQUE NOT NULL,
 content TEXT
);

CREATE OR REPLACE FUNCTION set_updated_at() RETURNS TRIGGER AS $$
BEGIN
 NEW.updated_at = NOW();
 RETURN NEW;
END; $$ LANGUAGE plpgsql;

DO $$
BEGIN
 IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname='users_set_updated_at') THEN
   CREATE TRIGGER users_set_updated_at BEFORE UPDATE ON users
   FOR EACH ROW EXECUTE FUNCTION set_updated_at();
 END IF;
END$$;

CREATE TABLE IF NOT EXISTS carousel_items (
 id SERIAL PRIMARY KEY,
 item_index INTEGER NOT NULL DEFAULT 0,
 title VARCHAR(200),
 subtitle VARCHAR(200),
 description TEXT,
 image_dataurl TEXT NOT NULL,
 created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
 updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_carousel_item_index ON carousel_items(item_index ASC);

CREATE OR REPLACE FUNCTION set_updated_at_carousel() RETURNS TRIGGER AS $$
BEGIN
 NEW.updated_at = NOW();
 RETURN NEW;
END; $$ LANGUAGE plpgsql;

DO $$
BEGIN
 IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname='carousel_items_set_updated_at') THEN
   CREATE TRIGGER carousel_items_set_updated_at BEFORE UPDATE ON carousel_items
   FOR EACH ROW EXECUTE FUNCTION set_updated_at_carousel();
 END IF;
END$$;
";
            cmd.ExecuteNonQuery();

            // Seed: ใส่ section พื้นฐาน ถ้ายังไม่มี
            cmd.CommandText = @"
INSERT INTO homepage_content(section_name, content)
SELECT s, c FROM (VALUES
 ('hero', 'Welcome to TechStyle — build modern apps fast.'),
 ('features', '• Fast • Secure • Beautiful UI'),
 ('cta', 'Get started today!')
) AS v(s,c)
WHERE NOT EXISTS (SELECT 1 FROM homepage_content hc WHERE hc.section_name = v.s);";
            cmd.ExecuteNonQuery();

            // Seed: ผู้ใช้ตัวอย่าง (หากยังไม่มี)
            // รหัสผ่าน: P@ssw0rd123 แบบ SHA256
            var adminEmail = "admin@example.com";
            var adminPassHash = Utils.Security.HashPasswordSha256("P@ssw0rd123");
            cmd.CommandText = @"
INSERT INTO users(username, email, password_hash, role, is_email_verified)
SELECT 'admin', @email, @hash, 'admin', true
WHERE NOT EXISTS (SELECT 1 FROM users WHERE email=@email);";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@email", adminEmail);
            cmd.Parameters.AddWithValue("@hash", adminPassHash);
            cmd.ExecuteNonQuery();

            tx.Commit();
        }
    }
}
