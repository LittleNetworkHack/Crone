-- Procedure parameters
-- Used for testing
--DECLARE @FirstName	varchar(max)	= 'Amy%';

-- Procedure variables
DECLARE @PersonType nchar(2);
DECLARE	@NameStyle	bit;

-- Calculate variables
SET		@PersonType		= 'IN';
SET		@NameStyle		= 0;

-- Procedure
SELECT	* 
FROM	Person.Person 
WHERE	FirstName LIKE @FirstName
AND		PersonType = @PersonType