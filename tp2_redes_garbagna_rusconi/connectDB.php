<?php
try
{
	$pdo = new PDO('mysql:host=localhost;dbname=tp2_redes_garbagna_rusconi', 'root', '');
	$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
	$pdo->exec('SET NAMES "utf8"');
}
catch(PDOException $e)
{
	echo "ERROR CONECTION TO DATABASE" . $e->getMessage();
	exit();
}
echo "Server/";
?>