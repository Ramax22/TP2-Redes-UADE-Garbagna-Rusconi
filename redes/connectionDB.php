<?php
try
{
	$pdo=new PDO('mysql:host=localhost;dbname=redes', 'root', '1234');
	$pdo->setAttribute(PDO::ATTR_ERRMODE,PDO::ERRMODE_EXCEPTION);
	$pdo->exec('SET NAMES "utf8"');
}
catch (PDOException $e)
{
	echo "ERROR CONNECTION TO DATABASE ".$e->getMessage();
	exit();
}
echo "Server/";
?>