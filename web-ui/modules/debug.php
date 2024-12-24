<?php

/**
 * Development environment
 */
function isDevEnv(): bool
{
    return in_array($_SERVER['SERVER_NAME'], ['botnet', 'botnet.localhost', 'localhost']);
}

if (isDevEnv()) {
    error_reporting(E_ALL);
    ini_set('display_errors', 'On');
}
