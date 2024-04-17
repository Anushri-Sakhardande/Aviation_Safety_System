LOAD DATA
	INFILE 'C:\Users\anush\OneDrive\Documents\Aviation_Safety_System\aircraft.csv'
	INTO TABLE AIRCRAFT
	FIELDS TERMINATED BY ',' optionally enclosed by '"'
	(
		admin_id,
		aircraft_id,
		type,
		Manufacturer,
		Mil_Com
		
	)
