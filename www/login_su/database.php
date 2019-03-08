<?php

include("config.php");

mysqli_report(MYSQLI_REPORT_STRICT);

function CreateDatabaseConnection($server, $username, $password, $database)
{
	try
	{
		$dataConnection = new mysqli($server, $username, $password);
	}
	catch(Exception $e)
	{
		die("Error while connecting to the database");
	}

	$dataConnection->select_db($database);
	$dataConnection->query("SET NAMES 'utf8'");
	
	return $dataConnection;
}

$g_databaseConnection = CreateDatabaseConnection($db_server, $db_username, $db_password, $db_database);

function GenerateRandomSha224()
{
	mt_srand(microtime(true) * 100000 + memory_get_usage(true));
	return hash("sha224", uniqid(mt_rand(), true));
}

function VerifyUser($dataConnection, $username, $password)
{
	$statement = $dataConnection->prepare("SELECT id, passhash, salt FROM users WHERE name = ?");
	if(!$statement)
	{
		throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	try
	{
		$statement->bind_param('s', $username);
		if(!$statement->execute())
		{
			throw new Exception(__FUNCTION__ . " failed.");
		}

		$statement->bind_result($id, $storedPasshash, $salt);
		if(!$statement->fetch())
		{
			throw new Exception("Incorrect username.");
		}
		
		$saltedPassword = $password . $salt;
		$hashedPassword = hash("sha224", $saltedPassword);
		
		if($hashedPassword !== $storedPasshash)
		{
			throw new Exception("Incorrect password.");
		}
		
		return $id;
	}
	finally
	{
		$statement->close();
	}
}

function InsertUser($dataConnection, $username, $passhash, $salt, $email)
{
	{
		$statement = $dataConnection->prepare("INSERT INTO users (name, passhash, salt, email) VALUES (?, ?, ?, ?)");
		if(!$statement)
		{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
		}

		try
		{
			$statement->bind_param('ssss', $username, $passhash, $salt, $email);

			if(!$statement->execute())
			{
				throw new Exception(__FUNCTION__ . " failed.");
			}
		}
		finally
		{
			$statement->close();
		}
	}	
}

function RefreshOrCreateSession($dataConnection, $userId)
{
	try
	{
		$sessionId = GetSessionFromUserId($dataConnection, $userId);
		RefreshSession($dataConnection, $sessionId);
	}
	catch(Exception $e)
	{
		$sessionId = CreateSession($dataConnection, $userId);
	}

	return $sessionId;
}

function CreateSession($dataConnection, $userId)
{
	//Delete any session that might be active
	{
		$statement = $dataConnection->prepare("DELETE FROM sessions WHERE userId = ?");
		if(!$statement)
		{
			throw new Exception("Failed to create session: " . $dataConnection->error);
		}
		
		try
		{
			$statement->bind_param('i', $userId);

			if(!$statement->execute())
			{
				throw new Exception("Failed to create session: " . $dataConnection->error);
			}
		}
		finally
		{
			$statement->close();
		}
	}
	
	//Create new session
	{
		$sessionId = GenerateRandomSha224();
		
		$statement = $dataConnection->prepare("INSERT INTO sessions (id, userid, expiration) VALUES (?, ?, NOW() + INTERVAL " . FFXIV_SESSION_LENGTH . " HOUR)");
		if(!$statement)
		{
			throw new Exception("Failed to create session: " . $dataConnection->error);
		}
		
		try
		{
			$statement->bind_param('si', $sessionId, $userId);

			if(!$statement->execute())
			{
				throw new Exception("Failed to create session: " . $dataConnection->error);
			}
		}
		finally
		{
			$statement->close();
		}
		
		return $sessionId;
	}
}

function GetSessionFromUserId($dataConnection, $userId)
{
	$statement = $dataConnection->prepare("SELECT id FROM sessions WHERE userId = ? AND expiration > NOW()");
	if(!$statement)
	{
		throw new Exception("Failed to get session id: " . $dataConnection->error);
	}
	
	try
	{
		$statement->bind_param('i', $userId);
		
		if(!$statement->execute())
		{
			throw new Exception("Failed to get session id: " . $dataConnection->error);
		}
		
		$statement->bind_result($sessionId);
		if(!$statement->fetch())
		{
			throw new Exception("Failed to get session id: " . $dataConnection->error);
		}
		
		return $sessionId;
	}
	finally
	{
		$statement->close();
	}
}

function RefreshSession($dataConnection, $sessionId)
{
	$statement = $dataConnection->prepare("UPDATE sessions SET expiration = NOW() + INTERVAL " . FFXIV_SESSION_LENGTH . " HOUR WHERE id = ?");
	if(!$statement)
	{
		throw new Exception("Failed to refresh session: " . $dataConnection->error);
	}
	
	try
	{
		$statement->bind_param('s', $sessionId);
		
		if(!$statement->execute())
		{
			throw new Exception("Failed to refresh session: " . $dataConnection->error);
		}
	}
	finally
	{
		$statement->close();
	}
}

function GetUserIdFromSession($dataConnection, $sessionId)
{
	$statement = $dataConnection->prepare("SELECT userId FROM sessions WHERE id = ? AND expiration > NOW()");
	if(!$statement)
	{
		throw new Exception("Could not get user id.");
	}

	try
	{
		$statement->bind_param('s', $sessionId);
		if(!$statement->execute())
		{
			throw new Exception("Could not get user id.");
		}
		
		$statement->bind_result($userId);
		if(!$statement->fetch())
		{
			throw new Exception("Could not get user id.");
		}
		
		return $userId;
	}
	finally
	{
		$statement->close();
	}
}

function GetUserInfo($dataConnection, $userId)
{
	$statement = $dataConnection->prepare("SELECT name FROM users WHERE id = ?");
	if(!$statement)
	{
		throw new Exception("Failed to get user information: " . $dataConnection->error);
	}
	
	try
	{
		$statement->bind_param('i', $userId);
		if(!$statement->execute())
		{
			throw new Exception("Failed to get user information: " . $dataConnection->error);
		}
		
		$result = $statement->get_result();
		if(!$result)
		{
			throw new Exception("Failed to get user information: " . $dataConnection->error);
		}

		$row = $result->fetch_assoc();
		if(!$row)
		{
			throw new Exception("Failed to get user information: " . $dataConnection->error);
		}
		
		return $row;
	}
	finally
	{
		$statement->close();
	}
}

function GetUserCharacters($dataConnection, $userId)
{
	$statement = $dataConnection->prepare("SELECT id, name FROM characters WHERE userId = ?");
	if(!$statement)
	{
		throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	try
	{
		$statement->bind_param('i', $userId);
		if(!$statement->execute())
		{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
		}
		
		$result = $statement->get_result();
		if(!$result)
		{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
		}

		$characters = array();
		
		while(1)
		{
			$row = $result->fetch_assoc();
			if(!$row)
			{
				break;
			}
			array_push($characters, $row);
		}
		
		return $characters;
	}
	finally
	{
		$statement->close();
	}
}

function GetCharacterInfo($dataConnection, $userId, $characterId)
{
	$query = sprintf("SELECT * FROM characters WHERE userId = '%d' AND id = '%d'", 
		$userId, $characterId);
	$result = $dataConnection->query($query);
	if(!$result)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	$row = $result->fetch_assoc();
	if(!$row)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	return $row;
}

function GetCharacterAppearance($dataConnection, $userId, $characterId)
{
	$query = sprintf("SELECT * FROM characters_appearance INNER JOIN characters ON characters_appearance.characterId = characters.id WHERE characters.userId = '%d' AND characters.Id='%d'", 
		$userId, $characterId);
	$result = $dataConnection->query($query);
	if(!$result)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	$row = $result->fetch_assoc();
	if(!$row)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	return $row;
}

function GetCharacterChocobo($dataConnection, $userId, $characterId)
{
	$query = sprintf("SELECT * FROM characters_chocobo INNER JOIN characters ON characters_chocobo.characterId = characters.id WHERE characters.userId = '%d' AND characters.Id='%d'", 
		$userId, $characterId);
	$result = $dataConnection->query($query);
	if(!$result)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	$row = $result->fetch_assoc();
	if(!$row)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	return $row;
}

function GetCharacterClassLevels($dataConnection, $userId, $characterId)
{
	$query = sprintf("SELECT * FROM characters_class_levels INNER JOIN characters ON characters_class_levels.characterId = characters.id WHERE characters.userId = '%d' AND characters.Id='%d'", 
		$userId, $characterId);
	$result = $dataConnection->query($query);
	if(!$result)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	$row = $result->fetch_assoc();
	if(!$row)
	{
			throw new Exception(__FUNCTION__ . " failed: " . $dataConnection->error);
	}
	
	return $row;
}

function UpdateCharacterInfo($dataConnection, $characterId, $characterInfo)
{
	$statement = $dataConnection->prepare("UPDATE ffxiv_characters SET
		name = ?, tribe = ?, size = ?, voice = ?, skinColor = ?, hairStyle = ?, hairColor = ?, hairOption = ?,
		eyeColor = ?, faceType = ?, faceBrow = ?, faceEye = ?, faceIris = ?, faceNose = ?, faceMouth = ?, faceJaw = ?,
		faceCheek = ?, faceOption1 = ?, faceOption2 = ?, guardian = ?, birthMonth = ?, birthDay = ?, allegiance = ?,
		weapon1 = ?, weapon2 = ?, headGear = ?, bodyGear = ?, legsGear = ?, handsGear = ?, feetGear = ?, 
		waistGear = ?, rightEarGear = ?, leftEarGear = ?, rightFingerGear = ?, leftFingerGear = ?
		WHERE id = ?");
	if(!$statement)
	{
		throw new Exception("Failed to update character information: " . $dataConnection->error);
	}
	
	try
	{
		if(!$statement->bind_param("siiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii",
			$characterInfo["name"], $characterInfo["tribe"], $characterInfo["size"], $characterInfo["voice"],
			$characterInfo["skinColor"], $characterInfo["hairStyle"], $characterInfo["hairColor"],
			$characterInfo["hairOption"], $characterInfo["eyeColor"], $characterInfo["faceType"],
			$characterInfo["faceBrow"], $characterInfo["faceEye"], $characterInfo["faceIris"],
			$characterInfo["faceNose"], $characterInfo["faceMouth"], $characterInfo["faceJaw"],
			$characterInfo["faceCheek"], $characterInfo["faceOption1"], $characterInfo["faceOption2"],
			$characterInfo["guardian"], $characterInfo["birthMonth"], $characterInfo["birthDay"], $characterInfo["allegiance"],
			$characterInfo["weapon1"], $characterInfo["weapon2"], $characterInfo["headGear"], $characterInfo["bodyGear"], 
			$characterInfo["legsGear"], $characterInfo["handsGear"], $characterInfo["feetGear"],
			$characterInfo["waistGear"], $characterInfo["rightEarGear"], $characterInfo["leftEarGear"], 
			$characterInfo["rightFingerGear"], $characterInfo["leftFingerGear"],
			$characterId))
		{
			throw new Exception("Failed to update character information: " . $dataConnection->error);
		}
		
		if(!$statement->execute())
		{
			throw new Exception("Failed to update character information: " . $dataConnection->error);
		}
	}
	finally
	{
		$statement->close();
	}
}

?>
