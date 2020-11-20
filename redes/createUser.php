<?php
#Incluyo el codigo de conexion
include "connectionDB.php";

#Agarro el user y la pass enviados en el request
$username = $_POST['username'];
$password = $_POST['password'];

#Creo query para la db donde consultará si ya hay un usuario con el mismo nombre
#Y la ejecuto
$sql = "SELECT username FROM users WHERE username = '$username'";
$result = $pdo->query($sql);

#Chequeo si ya hay un usuario con ese nombre, si esta cortamos la ejecucion
if ($result->rowCount() > 0) {
	echo "User already exist";
	exit();
}

#Creamos usuario
$sql = "INSERT INTO users SET username = '$username', password = '$password'";
$pdo->query($sql);
echo "Success"
?>