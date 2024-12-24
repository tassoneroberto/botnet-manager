<?php

function deleteDir($src)
{
	$dir = opendir($src);
	while (false !== ($file = readdir($dir))) {
		if (($file != '.') && ($file != '..')) {
			$full = $src . '/' . $file;
			if (is_dir($full)) {
				deleteDir($full);
			} else {
				unlink($full);
			}
		}
	}
	closedir($dir);
	rmdir($src);
}

function recursiveMakeDir($currentDir, $path)
{
	foreach (explode("/", $path) as $folder) {
		$currentDir = $currentDir . $folder . "/";
		if (!is_dir($currentDir))
			mkdir($currentDir);
	}
}
