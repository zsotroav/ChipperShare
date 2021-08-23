# ChipperShare Protocol History

## Table of Contents <!-- omit in toc --> 
- [Alpha](#alpha)
  - [Authentication](#authentication)
  - [Data exchange](#data-exchange)
- [Beta](#beta)
  - [Authentication](#authentication)
  - [Data exchange](#data-exchange)
- [Version 1 - Optimize](#version-1---optimize)
  - [Authentication](#authentication)
  - [Data exchange](#data-exchange)
- [Version 2 - cross-platform](#version-2---cross-platform)
  - [Authentication](#authentication)
  - [Data exchange](#data-exchange)

## Alpha 

**INT32 PROTOCOL VERSION: 1** 

The Alpha version was the first development version.

### Authentication

After the initial connection, the server will wait for the client to send an encrypted authentication string. This will make sure that both the client and server are using the same key and protocol version. The server handles the authentication and will close the connection if the authentication string doesn't match what is expected.

> `string expected = $"{serverIP}:{clientIP}:{protocolVersion}";`

If the server accepts the authentication attempt, it will respond (in UTF8 plain text) with `READY`, and the server will advance to the next step of sending extra data. If the connection is denied, `ABORT` will be sent instead.

### Data exchange

The alpha version of the protocol only sent the file's binary data encrypted without any extra data

## Beta

**INT32 PROTOCOL VERSION: 1** 

The Beta version was the second development version.

### Authentication

The authentication protocol remains the same.

### Data exchange

The Beta version was the first one to include file name communication. The server would send the encrypted file name to the client. The client will split the file name into the name and extension.

## Version 1 - Optimize

**INT32 PROTOCOL VERSION: 1** 

Version 1 is the first release version publically available on GitHub.

Code name `Optimize`

### Authentication

The authentication protocol remains the same.

### Data exchange

The server will now send the file's size before sending the file.

Order of sent data:
- Filename
- File size
- File data

## Version 2 - Cross-platform

**INT32 PROTOCOL VERSION: 2** 

Version 2 is the second release version publically available on GitHub. It was developed to ensure compatibility between different platforms and devices.

Code name `Cross-platform`

### Authentication

The authentication protocol remains the same, but the server will now notify the user on the log if the connection was denied due to protocol version mismatch.

### Data exchange

The server will now send the file's name's size before sending the file name.

Order of sent data:
- File size
- Filename size
- Filename
- File data