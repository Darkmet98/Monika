![Monika](https://github.com/Darkmet98/Monika/blob/master/MonikaBanner.jpg?raw=true)
# Monika [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0) [![Build Status](https://travis-ci.com/Darkmet98/Monika.svg?branch=master)](https://travis-ci.com/Darkmet98/Monika)
A simple exporter and importer for RenPy Translations to work with CAT Tools.

# Version

## 1.0
* Initial Release.

## 1.1
* First public release
* Added a custom dictionary to work with all character name games.
* WIP DDLC LovePotion Port languages.
* Ported to Net Core 3.

# Usage

## Renpy Files
* Export Rpy to Po: Monika -export "script-ch0.rpy"
* Export Po to Rpy: Monika -import "script-ch0.po"
* Fix Po import if the translation program (Like PoEdit) broke the Po: Monika -fix_import "script-ch0.po" "script-ch0.rpy"

## DDLC LovePotion (Alpha, do not use for now)
* Port Po to Luke DDLC's Lua file: Monika -port "script-ch0.po" "script-ch0.lua"

## How to fully use this program with RenPy SDK translation generator
Read the wiki


# Tested games
* Doki Doki Literature Club
* Sakura Sadist
* Hiveswap Friendsim
* Pesterquest
* Souda Love Revolution

# Credits
* Thanks to Pleonex for Yarhl libraries.
* Team Salvato for Monika sprite.
