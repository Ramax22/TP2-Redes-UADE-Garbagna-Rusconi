<?php
#Incluyo el codigo de conexion
include "connectionDB.php";

#Agarro el user y la pass enviados en el request
$username = $_POST['username'];
$password = $_POST['password'];

#Creo query para la db donde consultará si encuentra al usuario y la ejecuto
$sql = "SELECT username FROM users WHERE username = '$username' AND password = '$password'";
$result = $pdo->query($sql);

#Si encontro el usuario lo informo, en caso contrario digo que no se pudo encontrar
if ($result->rowCount() > 0) {
	echo "Success";
} else {
	echo "User not found";
}
?>