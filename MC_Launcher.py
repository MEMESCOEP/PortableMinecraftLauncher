#!/usr/bin/env python3

import json
import os
import platform
from pathlib import Path
import subprocess
import sys
import requests


"""
Debug output
"""
def debug(str):
    if os.getenv('DEBUG') != None:
        print(str)

"""
[Gets the natives_string toprepend to the jar if it exists. If there is nothing native specific, returns and empty string]
"""
def get_natives_string(lib):
    arch = "64"

    nativesFile=""
    if not "natives" in lib:
        return nativesFile

    if "windows" in lib["natives"] and platform.system() == 'Windows':
        nativesFile = lib["natives"]["windows"].replace("${arch}", arch)
    elif "osx" in lib["natives"] and platform.system() == 'Darwin':
        nativesFile = lib["natives"]["osx"].replace("${arch}", arch)
    elif "linux" in lib["natives"] and platform.system() == "Linux":
        nativesFile = lib["natives"]["linux"].replace("${arch}", arch)
    else:
        raise Exception("Platform not supported")

    return nativesFile


"""
[Parses "rule" subpropery of library object, testing to see if should be included]
"""
def should_use_library(lib):
    def rule_says_yes(rule):
        useLib = None

        if rule["action"] == "allow":
            useLib = False
        elif rule["action"] == "disallow":
            useLib = True

        if "os" in rule:
            for key, value in rule["os"].items():
                os = platform.system()
                if key == "name":
                    if value == "windows" and os != 'Windows':
                        return useLib
                    elif value == "osx" and os != 'Darwin':
                        return useLib
                    elif value == "linux" and os != 'Linux':
                        return useLib
                elif key == "arch":
                    if value == "x86" and platform.architecture()[0] != "32bit":
                        return useLib

        return not useLib

    if not "rules" in lib:
        return True

    shouldUseLibrary = False
    for i in lib["rules"]:
        if rule_says_yes(i):
            return True

    return shouldUseLibrary




"""
[Get string of all libraries to add to java classpath]
"""
def get_classpath(lib, mcDir):
    cp = []

    for i in lib["libraries"]:
        if not should_use_library(i):
            continue

        libDomain, libName, libVersion = i["name"].split(":")
        jarPath = os.path.join(mcDir, "libraries", *
                               libDomain.split('.'), libName, libVersion)

        native = get_natives_string(i)
        jarFile = libName + "-" + libVersion + ".jar"
        if native != "":
            jarFile = libName + "-" + libVersion + "-" + native + ".jar"

        cp.append(os.path.join(jarPath, jarFile))

    cp.append(os.path.join(mcDir, "versions", lib["id"], f'{lib["id"]}.jar'))

    return os.pathsep.join(cp)

version = '1.18.2'
username = sys.argv[1]
uuid = "NO_UUID"
try:
    url = f'https://api.mojang.com/users/profiles/minecraft/' + username
    response = requests.get(url)
    uuid = response.json()['id']
    print("[LAUNCHER / INFO] >> Got UUID \"{}\" for user \"{}\"".format(uuid, username))
except:
    print("[LAUNCHER / ERROR_NON_FATAL] >> Failed to get player UUID!")
accessToken = '{token}'
mc_PATH = "./mcdata/"

mcDir = os.path.join(mc_PATH, '.minecraft')
nativesDir = os.path.join(mc_PATH, 'versions', version, 'natives')
clientJson = json.loads(Path(os.path.join(mcDir, 'versions', version, f'{version}.json')).read_text())
classPath = get_classpath(clientJson, mcDir)
mainClass = clientJson['mainClass']
versionType = clientJson['type']
assetIndex = clientJson['assetIndex']['id']

debug(classPath)
debug(mainClass)
debug(versionType)
debug(assetIndex)

subprocess.call([
    './bin/JRE/bin/java.exe',
    f'-Djava.library.path={nativesDir}',
    '-Dminecraft.launcher.brand=custom-launcher',
    '-Dminecraft.launcher.version=2.1',
    '-cp',
    classPath,
    'net.minecraft.client.main.Main',
    '--username',
    username,
    '--version',
    version,
    '--gameDir',
    mcDir,
    '--assetsDir',
    os.path.join(mcDir, 'assets'),
    '--assetIndex',
    assetIndex,
    '--uuid',
    uuid,
    '--accessToken',
    accessToken,
    '--userType',
    'mojang',
    '--versionType',
    'release'
])
