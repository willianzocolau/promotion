DROP TABLE promotions;
DROP TABLE credit_cards;
DROP TABLE partner_stores;
DROP TABLE users;
DROP TABLE estados;

CREATE TABLE estados(
	idestado INTEGER NOT NULL,
	name VARCHAR(45) NOT NULL,
	PRIMARY KEY (idestado)
);
CREATE TABLE users(
	iduser INTEGER NOT NULL,
	nickname VARCHAR(45) NOT NULL,
	email VARCHAR(255) NOT NULL,
	password VARCHAR(64) NOT NULL,
	type INTEGER NOT NULL,
	register_date TIMESTAMP NOT NULL,
	cpf VARCHAR(11) NOT NULL,
	credit DOUBLE PRECISION NOT NULL,
	name VARCHAR(150) NOT NULL,
	image_url VARCHAR(150),
	telephone VARCHAR(11),
	cellphone VARCHAR(11),
	token VARCHAR(64),
	estados_idestado INTEGER NOT NULL REFERENCES estados(idestado),
	PRIMARY KEY (iduser)
);
CREATE TABLE credit_cards(
	idcredit_card INTEGER NOT NULL,
	number VARCHAR(16) NOT NULL,
	valid_date VARCHAR(4) NOT NULL,
	name VARCHAR(100) NOT NULL,
	users_iduser INTEGER NOT NULL REFERENCES users(iduser),
	PRIMARY KEY (idcredit_card)
);
CREATE TABLE partner_stores(
	idstore INTEGER NOT NULL,
	name VARCHAR(45) NOT NULL,
	register_date TIMESTAMP NOT NULL,
	PRIMARY KEY (idstore)
);
CREATE TABLE promotions(
	idpromotion INTEGER NOT NULL,
	name VARCHAR(45) NOT NULL,
	price DOUBLE PRECISION NOT NULL,
	register_date TIMESTAMP NOT NULL,
	expire_date TIMESTAMP NOT NULL,
	image_url VARCHAR(150),
	stores_idstore INTEGER NOT NULL REFERENCES partner_stores(idstore),
	users_iduser INTEGER NOT NULL REFERENCES users(iduser),
	estados_idestado INTEGER NOT NULL REFERENCES estados(idestado),
	PRIMARY KEY (idpromotion)
);
