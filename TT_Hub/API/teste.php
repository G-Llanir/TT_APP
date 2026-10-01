<?php

	define("FILENAME", 'pushtest.xml');
	define("FOLDER", '');	
	define("SEPARATOR", '');
	define("STR_SUCCESS", 'set tg1=1');
	define("STR_ERROR", 'error');
	
	if($_SERVER['REQUEST_METHOD'] == 'POST'){
			$pathname = FOLDER.SEPARATOR.FILENAME;
			$postdata = file_get_contents("php://input");
			$handle = fopen($pathname, 'w+');
			$content = var_export($postdata, true);
			fwrite($handle, substr($content, 1, strlen($content)-2));
			fclose($handle);
			echo (($handle === false) ? STR_ERROR : STR_SUCCESS)."\r\n";
	}
	else {
			echo "php OK";
	}
	
?>