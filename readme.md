
# Windows Credential Provider
_C#, .NET_

There was no implementation in .NET that could work as credential provider in windows, so I made this.
The code is totally free for any use.

## *Read this before you start*

Installing an untested credential provider might lock you out of the system,
as the code will run in process with winlogon.

Use a live distro to remove the dll if that happens.

Better yet, use a VM to do your experiments.

_Consider yourself warned._