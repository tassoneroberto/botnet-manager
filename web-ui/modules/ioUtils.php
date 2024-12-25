<?php

function deleteDir(string $src): void
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

function recursiveMakeDir(string $currentDir, string $path): void
{
	foreach (explode("/", $path) as $folder) {
		$currentDir = $currentDir . $folder . "/";
		if (!is_dir($currentDir))
			mkdir($currentDir);
	}
}
