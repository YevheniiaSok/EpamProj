-- Table: article.articles

-- DROP TABLE IF EXISTS article.articles;
-- SEQUENCE: article.articles_id_seq

-- DROP SEQUENCE IF EXISTS article.articles_id_seq;

CREATE SEQUENCE IF NOT EXISTS article.articles_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE article.articles_id_seq
    OWNED BY article.articles.id;

ALTER SEQUENCE article.articles_id_seq
    OWNER TO postgres;

CREATE TABLE IF NOT EXISTS article.articles
(
    id integer NOT NULL DEFAULT nextval('article.articles_id_seq'::regclass),
    title character varying(120) COLLATE pg_catalog."default" NOT NULL,
    author character varying(50) COLLATE pg_catalog."default",
    category character varying(50) COLLATE pg_catalog."default" NOT NULL,
    description text COLLATE pg_catalog."default",
    filename character varying(200) COLLATE pg_catalog."default",
    date date,
    CONSTRAINT articles_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS article.articles
    OWNER to postgres;