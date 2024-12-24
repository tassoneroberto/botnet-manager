<?php

function checkKey($key): bool
{
	if ($key == hash("sha256", gmdate("Y-m-d") . "_d4mny0uf0undm3!!!") || $key == hash("sha256", gmdate("Y-m-d", strtotime("-1 days")) . "_d4mny0uf0undm3!!!"))
		return true;
	return false;
}
