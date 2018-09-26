CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "States" (
    "Id" bigint NOT NULL,
    "Name" character varying(45) NULL,
    CONSTRAINT "PK_States" PRIMARY KEY ("Id")
);

CREATE TABLE "Stores" (
    "Id" bigserial NOT NULL,
    "Name" character varying(45) NULL,
    "RegisterDate" timestamp with time zone NOT NULL,
    "Token" character varying(64) NULL,
    CONSTRAINT "PK_Stores" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" bigserial NOT NULL,
    "Nickname" character varying(45) NULL,
    "Name" character varying(150) NULL,
    "Email" character varying(255) NULL,
    "Password" character varying(64) NULL,
    "PasswordSalt" character varying(64) NULL,
    "Type" integer NOT NULL,
    "RegisterDate" timestamp with time zone NOT NULL,
    "Cpf" character varying(11) NULL,
    "Credit" double precision NOT NULL,
    "ImageUrl" character varying(150) NULL,
    "Telephone" character varying(11) NULL,
    "Cellphone" character varying(11) NULL,
    "Token" character varying(64) NULL,
    "StateFK" bigint NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Users_States_StateFK" FOREIGN KEY ("StateFK") REFERENCES "States" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ForgotPasswordRequests" (
    "Id" bigserial NOT NULL,
    "Ip" character varying(45) NULL,
    "Code" character varying(6) NULL,
    "RequestDate" timestamp with time zone NOT NULL,
    "UserFK" bigint NOT NULL,
    CONSTRAINT "PK_ForgotPasswordRequests" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ForgotPasswordRequests_Users_UserFK" FOREIGN KEY ("UserFK") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Promotions" (
    "Id" bigserial NOT NULL,
    "Name" character varying(45) NULL,
    "Price" double precision NOT NULL,
    "Active" boolean NOT NULL,
    "CashbackPercentage" double precision NULL,
    "RegisterDate" timestamp with time zone NOT NULL,
    "ExpireDate" timestamp with time zone NOT NULL,
    "ImageUrl" character varying(150) NULL,
    "UserFK" bigint NOT NULL,
    "StateFK" bigint NOT NULL,
    "StoreFK" bigint NOT NULL,
    CONSTRAINT "PK_Promotions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Promotions_States_StateFK" FOREIGN KEY ("StateFK") REFERENCES "States" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Promotions_Stores_StoreFK" FOREIGN KEY ("StoreFK") REFERENCES "Stores" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Promotions_Users_UserFK" FOREIGN KEY ("UserFK") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Orders" (
    "Id" bigserial NOT NULL,
    "Date" timestamp with time zone NOT NULL,
    "ApprovedByUserFK" bigint NULL,
    "UserFK" bigint NOT NULL,
    "PromotionFK" bigint NOT NULL,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Orders_Users_ApprovedByUserFK" FOREIGN KEY ("ApprovedByUserFK") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_Promotions_PromotionFK" FOREIGN KEY ("PromotionFK") REFERENCES "Promotions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Orders_Users_UserFK" FOREIGN KEY ("UserFK") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

INSERT INTO "States" ("Id", "Name")
VALUES (0, 'Acre');
INSERT INTO "States" ("Id", "Name")
VALUES (24, 'Sergipe');
INSERT INTO "States" ("Id", "Name")
VALUES (23, 'Santa Catarina');
INSERT INTO "States" ("Id", "Name")
VALUES (22, 'Roraima');
INSERT INTO "States" ("Id", "Name")
VALUES (21, 'Rondônia');
INSERT INTO "States" ("Id", "Name")
VALUES (20, 'Rio de Janeiro');
INSERT INTO "States" ("Id", "Name")
VALUES (19, 'Rio Grande do Sul');
INSERT INTO "States" ("Id", "Name")
VALUES (18, 'Rio Grande do Norte');
INSERT INTO "States" ("Id", "Name")
VALUES (17, 'Piauí');
INSERT INTO "States" ("Id", "Name")
VALUES (16, 'Pernambuco');
INSERT INTO "States" ("Id", "Name")
VALUES (15, 'Pará');
INSERT INTO "States" ("Id", "Name")
VALUES (14, 'Paraíba');
INSERT INTO "States" ("Id", "Name")
VALUES (25, 'São Paulo');
INSERT INTO "States" ("Id", "Name")
VALUES (13, 'Paraná');
INSERT INTO "States" ("Id", "Name")
VALUES (11, 'Mato Grosso do Sul');
INSERT INTO "States" ("Id", "Name")
VALUES (10, 'Mato Grosso');
INSERT INTO "States" ("Id", "Name")
VALUES (9, 'Maranhão');
INSERT INTO "States" ("Id", "Name")
VALUES (8, 'Goiás');
INSERT INTO "States" ("Id", "Name")
VALUES (7, 'Espírito Santo');
INSERT INTO "States" ("Id", "Name")
VALUES (6, 'Distrito Federal');
INSERT INTO "States" ("Id", "Name")
VALUES (5, 'Ceará');
INSERT INTO "States" ("Id", "Name")
VALUES (4, 'Bahia');
INSERT INTO "States" ("Id", "Name")
VALUES (3, 'Amazonas');
INSERT INTO "States" ("Id", "Name")
VALUES (2, 'Amapá');
INSERT INTO "States" ("Id", "Name")
VALUES (1, 'Alagoas');
INSERT INTO "States" ("Id", "Name")
VALUES (12, 'Minas Gerais');
INSERT INTO "States" ("Id", "Name")
VALUES (26, 'Tocantins');

CREATE INDEX "IX_ForgotPasswordRequests_UserFK" ON "ForgotPasswordRequests" ("UserFK");

CREATE INDEX "IX_Orders_ApprovedByUserFK" ON "Orders" ("ApprovedByUserFK");

CREATE INDEX "IX_Orders_PromotionFK" ON "Orders" ("PromotionFK");

CREATE INDEX "IX_Orders_UserFK" ON "Orders" ("UserFK");

CREATE INDEX "IX_Promotions_StateFK" ON "Promotions" ("StateFK");

CREATE INDEX "IX_Promotions_StoreFK" ON "Promotions" ("StoreFK");

CREATE INDEX "IX_Promotions_UserFK" ON "Promotions" ("UserFK");

CREATE INDEX "IX_Users_StateFK" ON "Users" ("StateFK");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20180926191140_PromotionDb1', '2.1.2-rtm-30932');

