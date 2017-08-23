
# Windows Credential Provider
_C#, .NET_

There was no implementation in .NET that could work as credential provider in windows,
so I started this. I hope to have a full example at some point.

The code is totally free for any use.

## *Read this before you start*

Installing an untested credential provider might lock you out of the system,
as the code will run in process with winlogon.

Use a live distro to remove the dll if that happens.

Better yet, use a VM to do your experiments.

_Consider yourself warned._

## Installation
To start a setup to develop your own Windows Credential Provider:
- Install the COM component by building the project
- Merge the registry to install the cred. provider

## How to
I have included the official doc on how to use the credential provider - note that you have to have some knowledge about COM and the examples are in C++.

I have also included the guide on how to (re)export Interop typelib from IDL in windows SDK. You can use that to export almost any component.