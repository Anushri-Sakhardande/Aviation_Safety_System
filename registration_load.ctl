LOAD DATA
	INFILE 'C:\Users\anush\OneDrive\Documents\Aviation_Safety_System\registration_1.csv'
	INTO TABLE REGISTRATION
	FIELDS TERMINATED BY ',' optionally enclosed by '"'
	(
		admin_id,
		registration_id,
		Manufacturer,
		registration,
		aircraft_id
	)
