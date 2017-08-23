# How to export

As a first step we need to add all the interfaces in library statement so the can be exported
(file: C:\Program Files (x86)\Windows Kits\10\Include\10.0.15063.0\um\credentialprovider.idl)

## 1. Midl compiler
midl "C:\Program Files (x86)\Windows Kits\10\Include\10.0.15063.0\um\credentialprovider.idl"

## 2. Interop library generation
TlbImp2.exe credentialprovider.tlb /out:CredentialProvider.Interop.dll /unsafe /verbose /preservesig