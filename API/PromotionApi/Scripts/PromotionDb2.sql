CREATE TABLE matchs (
    id bigserial NOT NULL,
    register_date timestamp with time zone NOT NULL,
    is_active boolean NOT NULL,
    user_fk bigint NOT NULL,
    promotion_fk bigint NOT NULL,
    CONSTRAINT pk_matchs PRIMARY KEY (id),
    CONSTRAINT fk_matchs_promotions_promotion_fk FOREIGN KEY (promotion_fk) REFERENCES promotions (id) ON DELETE CASCADE,
    CONSTRAINT fk_matchs_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE wishlist (
    id bigserial NOT NULL,
    name character varying(45) NULL,
    register_date timestamp with time zone NOT NULL,
    user_fk bigint NOT NULL,
    CONSTRAINT pk_wishlist PRIMARY KEY (id),
    CONSTRAINT fk_wishlist_users_user_fk FOREIGN KEY (user_fk) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_matchs_promotion_fk ON matchs (promotion_fk);

CREATE INDEX ix_matchs_user_fk ON matchs (user_fk);

CREATE INDEX ix_wishlist_user_fk ON wishlist (user_fk);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20181022002812_PromotionDb2', '2.1.4-rtm-31024');

