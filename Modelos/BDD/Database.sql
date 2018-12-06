CREATE TABLE states (
    id bigint NOT NULL,
    name character varying(45) NULL,
    CONSTRAINT pk_states PRIMARY KEY (id)
);

CREATE TABLE stores (
    id bigint NOT NULL,
    name character varying(45) NULL,
    register_date timestamp with time zone NOT NULL,
    token character varying(64) NULL,
    CONSTRAINT pk_stores PRIMARY KEY (id)
);

CREATE TABLE users (
    id bigint NOT NULL,
    nickname character varying(45) NULL,
    name character varying(150) NULL,
    email character varying(255) NULL,
    password character varying(64) NULL,
    password_salt character varying(64) NULL,
    type integer NOT NULL,
    register_date timestamp NOT NULL,
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
    id bigint NOT NULL,
    ip character varying(45) NULL,
    code character varying(6) NULL,
    request_date timestamp NOT NULL,
    user_fk bigint NOT NULL,
    CONSTRAINT pk_forgot_password_requests PRIMARY KEY (id),
    CONSTRAINT fk_forgot_password_requests_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE promotions (
    id bigint NOT NULL,
    name character varying(45) NULL,
    price double precision NOT NULL,
    active boolean NOT NULL,
    cashback_percentage double precision NULL,
    register_date timestamp NOT NULL,
    expire_date timestamp NOT NULL,
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
    id bigint NOT NULL,
    register_date timestamp NOT NULL,
    approved_by_user_fk bigint NULL,
    user_fk bigint NOT NULL,
    promotion_fk bigint NOT NULL,
    CONSTRAINT pk_orders PRIMARY KEY (id),
    CONSTRAINT fk_orders_users_approved_by_user_fk FOREIGN KEY (approved_by_user_fk) REFERENCES users (id) ON DELETE RESTRICT,
    CONSTRAINT fk_orders_promotions_promotion_fk FOREIGN KEY (promotion_fk) REFERENCES promotions (id) ON DELETE CASCADE,
    CONSTRAINT fk_orders_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_forgot_password_requests_user_fk ON forgot_password_requests (user_fk);

CREATE INDEX ix_orders_approved_by_user_fk ON orders (approved_by_user_fk);

CREATE INDEX ix_orders_promotion_fk ON orders (promotion_fk);

CREATE INDEX ix_orders_user_fk ON orders (user_fk);

CREATE INDEX ix_promotions_state_fk ON promotions (state_fk);

CREATE INDEX ix_promotions_store_fk ON promotions (store_fk);

CREATE INDEX ix_promotions_user_fk ON promotions (user_fk);

CREATE INDEX ix_users_state_fk ON users (state_fk);

CREATE TABLE matchs (
    id bigint NOT NULL,
    register_date timestamp NOT NULL,
    is_active boolean NOT NULL,
    user_fk bigint NOT NULL,
    promotion_fk bigint NOT NULL,
    CONSTRAINT pk_matchs PRIMARY KEY (id),
    CONSTRAINT fk_matchs_promotions_promotion_fk FOREIGN KEY (promotion_fk) REFERENCES promotions (id) ON DELETE CASCADE,
    CONSTRAINT fk_matchs_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE wishlist (
    id bigint NOT NULL,
    name character varying(45) NULL,
    register_date timestamp NOT NULL,
    user_fk bigint NOT NULL,
    CONSTRAINT pk_wishlist PRIMARY KEY (id),
    CONSTRAINT fk_wishlist_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_matchs_promotion_fk ON matchs (promotion_fk);

CREATE INDEX ix_matchs_user_fk ON matchs (user_fk);

CREATE INDEX ix_wishlist_user_fk ON wishlist (user_fk);

ALTER TABLE orders ADD answer character varying(400) NULL;

ALTER TABLE orders ADD answer_register_date timestamp NULL;

ALTER TABLE orders ADD comment character varying(400) NULL;

ALTER TABLE orders ADD comment_register_date timestamp NULL;

ALTER TABLE orders ADD is_vote_positive boolean NULL;