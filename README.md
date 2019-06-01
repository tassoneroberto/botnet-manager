# Botnet Manager

## Introduction
Malware capable of control many remote machines.
The repository contains the source code of the malware and the control panel as a web-ui.

## Legal disclaimer
Usage of the code from this repository for attacking targets without prior mutual consent is illegal. It's the end user's responsibility to obey all applicable local, state and federal laws. Developers assume no liability and are not responsible for any misuse or damage caused by this program.

## Screenshots
![](https://raw.githubusercontent.com/tassoneroberto/botnet-manager/master/screenshots/screenshot1.png)
![](https://raw.githubusercontent.com/tassoneroberto/botnet-manager/master/screenshots/screenshot2.png)
![](https://raw.githubusercontent.com/tassoneroberto/botnet-manager/master/screenshots/screenshot3.png)
![](https://raw.githubusercontent.com/tassoneroberto/botnet-manager/master/screenshots/screenshot4.png)
![](https://raw.githubusercontent.com/tassoneroberto/botnet-manager/master/screenshots/screenshot5.png)

## Installation
Clone
```
git clone https://github.com/tassoneroberto/botnet-manager.git
```

### Web UI
Upload the web-ui to a webserver (PHP and MySQL are required).

Create a MySQL schema and change the fields in ```connect.php``` file. Also, change the fields ```$validUrl``` in ```index.php``` with the web-ui address (used to communicate to the malware that the web-ui address changed).

Initialize the database running the ```db-init.sql``` SQL script.

### Malware

Open the project in Visual Studio and change the ```BASE_URL``` variable with the web-ui address.

Build the entire project.

Upload the output .exe files located in botnet/EXE to remote host (inside the software folder).

### Usage

Run the ```Installer.exe``` file and open the web-ui to check if the system is installed correctly.
