<?php

function getFontAwesomeIconNameByOS(string $os): string
{
    switch ($os) {
        case OS_WINDOWS:
            return "windows";
        case OS_MACOS:
            return "apple";
        case OS_LINUX:
            return "linux";
        case OS_ANDROID:
            return "android";
        case OS_IOS:
            return "mobile";
        default:
            return "question";
    }
}
