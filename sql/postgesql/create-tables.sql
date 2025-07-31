CREATE TABLE "country"
(
    iso_2 citext COLLATE pg_catalog."default" NOT NULL,
    iso_3 citext COLLATE pg_catalog."default" NOT NULL,
    iso_number integer NOT NULL,
    name citext COLLATE pg_catalog."default" NOT NULL,
    calling_code citext COLLATE pg_catalog."default",
    CONSTRAINT "pk-country-iso_2" PRIMARY KEY (iso_2)
);

CREATE UNIQUE INDEX IF NOT EXISTS "ui-country-iso_3"
    ON public.country USING btree
    (iso_3 COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE UNIQUE INDEX IF NOT EXISTS "ui-country-iso_number"
    ON public.country USING btree
    (iso_number ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE UNIQUE INDEX IF NOT EXISTS "ui-country-name"
    ON public.country USING btree
    (name COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
