ALTER TABLE orders ADD answer character varying(400) NULL;

ALTER TABLE orders ADD answer_register_date timestamp with time zone NULL;

ALTER TABLE orders ADD comment character varying(400) NULL;

ALTER TABLE orders ADD comment_register_date timestamp with time zone NULL;

ALTER TABLE orders ADD is_vote_positive boolean NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20181024102812_PromotionDb3', '2.1.4-rtm-31024');

