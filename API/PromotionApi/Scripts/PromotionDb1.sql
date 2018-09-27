CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE states (
    id bigint NOT NULL,
    name character varying(45) NULL,
    CONSTRAINT pk_states PRIMARY KEY (id)
);

CREATE TABLE stores (
    id bigserial NOT NULL,
    name character varying(45) NULL,
    register_date timestamp with time zone NOT NULL,
    token character varying(64) NULL,
    CONSTRAINT pk_stores PRIMARY KEY (id)
);

CREATE TABLE users (
    id bigserial NOT NULL,
    nickname character varying(45) NULL,
    name character varying(150) NULL,
    email character varying(255) NULL,
    password character varying(64) NULL,
    password_salt character varying(64) NULL,
    type integer NOT NULL,
    register_date timestamp with time zone NOT NULL,
    cpf character varying(11) NULL,
    credit double precision NOT NULL,
    image_url character varying(150) NULL,
    telephone character varying(11) NULL,
    cellphone character varying(11) NULL,
    token character varying(64) NULL,
    state_fk bigint NULL,
    CONSTRAINT pk_users PRIMARY KEY (id),
    CONSTRAINT fk_users_states_state_fk FOREIGN KEY (state_fk) REFERENCES states (id) ON DELETE RESTRICT
);

CREATE TABLE forgot_password_requests (
    id bigserial NOT NULL,
    ip character varying(45) NULL,
    code character varying(6) NULL,
    request_date timestamp with time zone NOT NULL,
    user_fk bigint NOT NULL,
    CONSTRAINT pk_forgot_password_requests PRIMARY KEY (id),
    CONSTRAINT fk_forgot_password_requests_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE promotions (
    id bigserial NOT NULL,
    name character varying(45) NULL,
    price double precision NOT NULL,
    active boolean NOT NULL,
    cashback_percentage double precision NULL,
    register_date timestamp with time zone NOT NULL,
    expire_date timestamp with time zone NOT NULL,
    image_url character varying(150) NULL,
    user_fk bigint NOT NULL,
    state_fk bigint NOT NULL,
    store_fk bigint NOT NULL,
    CONSTRAINT pk_promotions PRIMARY KEY (id),
    CONSTRAINT fk_promotions_states_state_fk FOREIGN KEY (state_fk) REFERENCES states (id) ON DELETE CASCADE,
    CONSTRAINT fk_promotions_stores_store_fk FOREIGN KEY (store_fk) REFERENCES stores (id) ON DELETE CASCADE,
    CONSTRAINT fk_promotions_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE orders (
    id bigserial NOT NULL,
    register_date timestamp with time zone NOT NULL,
    approved_by_user_fk bigint NULL,
    user_fk bigint NOT NULL,
    promotion_fk bigint NOT NULL,
    CONSTRAINT pk_orders PRIMARY KEY (id),
    CONSTRAINT fk_orders_users_approved_by_user_fk FOREIGN KEY (approved_by_user_fk) REFERENCES users (id) ON DELETE RESTRICT,
    CONSTRAINT fk_orders_promotions_promotion_fk FOREIGN KEY (promotion_fk) REFERENCES promotions (id) ON DELETE CASCADE,
    CONSTRAINT fk_orders_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

INSERT INTO states (id, name)
VALUES (0, 'Acre');
INSERT INTO states (id, name)
VALUES (24, 'Sergipe');
INSERT INTO states (id, name)
VALUES (23, 'Santa Catarina');
INSERT INTO states (id, name)
VALUES (22, 'Roraima');
INSERT INTO states (id, name)
VALUES (21, 'Rondônia');
INSERT INTO states (id, name)
VALUES (20, 'Rio de Janeiro');
INSERT INTO states (id, name)
VALUES (19, 'Rio Grande do Sul');
INSERT INTO states (id, name)
VALUES (18, 'Rio Grande do Norte');
INSERT INTO states (id, name)
VALUES (17, 'Piauí');
INSERT INTO states (id, name)
VALUES (16, 'Pernambuco');
INSERT INTO states (id, name)
VALUES (15, 'Pará');
INSERT INTO states (id, name)
VALUES (14, 'Paraíba');
INSERT INTO states (id, name)
VALUES (25, 'São Paulo');
INSERT INTO states (id, name)
VALUES (13, 'Paraná');
INSERT INTO states (id, name)
VALUES (11, 'Mato Grosso do Sul');
INSERT INTO states (id, name)
VALUES (10, 'Mato Grosso');
INSERT INTO states (id, name)
VALUES (9, 'Maranhão');
INSERT INTO states (id, name)
VALUES (8, 'Goiás');
INSERT INTO states (id, name)
VALUES (7, 'Espírito Santo');
INSERT INTO states (id, name)
VALUES (6, 'Distrito Federal');
INSERT INTO states (id, name)
VALUES (5, 'Ceará');
INSERT INTO states (id, name)
VALUES (4, 'Bahia');
INSERT INTO states (id, name)
VALUES (3, 'Amazonas');
INSERT INTO states (id, name)
VALUES (2, 'Amapá');
INSERT INTO states (id, name)
VALUES (1, 'Alagoas');
INSERT INTO states (id, name)
VALUES (12, 'Minas Gerais');
INSERT INTO states (id, name)
VALUES (26, 'Tocantins');

CREATE INDEX ix_forgot_password_requests_user_fk ON forgot_password_requests (user_fk);

CREATE INDEX ix_orders_approved_by_user_fk ON orders (approved_by_user_fk);

CREATE INDEX ix_orders_promotion_fk ON orders (promotion_fk);

CREATE INDEX ix_orders_user_fk ON orders (user_fk);

CREATE INDEX ix_promotions_state_fk ON promotions (state_fk);

CREATE INDEX ix_promotions_store_fk ON promotions (store_fk);

CREATE INDEX ix_promotions_user_fk ON promotions (user_fk);

CREATE INDEX ix_users_state_fk ON users (state_fk);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20180927111052_PromotionDb1', '2.1.2-rtm-30932');

