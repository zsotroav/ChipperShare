# ChipperShare

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/fixed-bugs.svg)](https://forthebadge.com)

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Q5Q0M8XY)

This project is a a continuation of [ChipperUI](https://github.com/zsotroav/ChipperUI), a simple encryption/decryption application. It uses a symmetric key for encrypting and decrypting binary data. ChipperShare includes a Windows Forms-based UI.

The application is divided into server and client parts in the same executable. Both can run at the same time and either one can have multiple instances running if needed.

Keys are generated based on a user-provided passphrase, but they aren't stored on the drive. Keys are used on a connection basis and only appear as passphrases to the user.

## Table of Contents <!-- omit in toc -->

- [Compiling](#compiling)
- [Server-Client](#server-client)
  - [Server](#server)
  - [Client](#client)
- [Protocol](#protocol)
  - [Connecting](#connecting)
  - [Authentication](#authentication)
  - [Extra data](#extra-data)
  - [File](#file)
- [Algorithm](#algorithm)

## Compiling

Requirements:
- [.NET SDK 5.x.x](https://dotnet.microsoft.com/download)
- [.NET Desktop Runtime 5.x.x](https://dotnet.microsoft.com/download) (_if compiled without self containment_)

Compile to run with the .NET Desktop Runtime 5.x.x (_output size: ~190KB_): **RECOMMENDED**
>`dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained false`

Compile to run without the .NET Desktop Runtime 5.x.x (_output size: ~300MB_):
>`dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true`

## Server-Client

The server is the source of the file, and the client is requesting data from the server.

### Server

When clicked on the server button in the main form, the application asks for the file to be shared, the passphrase used for the encryption, and the IP address of the computer to connect to. (The the computer's valid IPs are offered in a dropdown menu)

The server starts listening to connections on its IP and the port `13000`. When a connection is requested, it accepts it and waits for the authentication of the client. More info in [authentication](#authentication).

### Client

When clicked on the client button in the main form, the client form is initialized. The user must enter the server's IP and the passphrase (and choose which IP (internet card) to use for the connection) to initialize the connection.

The client requests a login to the server and begins authentication, then sends/requests data as described in [protocol](#protocol).

## Protocol

The communication protocol is a custom-designed information exchanger. Not to be confused with the underlying network protocol, TCP on the port `13000`.

### Connecting

The client initializes a connection to the server with an IP. The server accepts the first incoming connection and will await authentication.

### Authentication

After the initial connection, the server will wait for the client to send an encrypted authentication string. This will make sure that both the client and server are using the same key and protocol version. The server handles the authentication and will close the connection if the authentication string doesn't match what is expected.

> `string expected = $"{serverIP}:{clientIP}:{protocolVersion}";`

If the server accepts the authentication attempt, it will respond (in UTF8 plain text) with `READY`, and the server will advance to the next step of sending extra data. If the connection is denied, `ABORT` will be sent instead.

### Extra data

The server will send the file's name and the sent data's size to ensure that the client is ready to accept it. The file name is encrypted, but the sent data's size is not.

### File

After all metadata and extra data are sent, the server will begin writing the encrypted file's data to the stream in 1024 byte chunks. Once all data is sent, the connection is closed, and the server is stopped. Similarly, the client will close the connection to the server once the file is saved.

On the client-side, the save file dialog's return values are what the received file is saved as. If the user cancels the dialog, a warning message box is displayed to ask if the user wants to discard the file or attempt to choose a location for it again.

## Algorithm

The algorithm is functionally the same as the one found in [ChipperUI](https://github.com/zsotroav/ChipperUI). The files can be found as a library in [lib/LibChipper](ChipperShare/lib/LibChipper/).
