# Safe Chat

Safe Chat is a secure chat application designed to facilitate private and encrypted communication between users. It follows a client-server architecture, ensuring data privacy and security.

## Features
- Secure messaging with encryption
- User authentication
- Real-time communication
- Multi-client support
- Simple and lightweight design

## Project Structure
```
SafeChat/
├── Client/         # Client-side application
│   ├── ChatClient.cs
│   ├── LoginService.cs
│   ├── UserService.cs
│   ├── Program.cs
│   ├── Client.csproj
│
├── Server/         # Server-side application
│   ├── ChatServer.cs
│   ├── AuthenticationService.cs
│   ├── Server.csproj
│   ├── Program.cs
│
├── SafeChat.sln    # Solution file
├── LICENSE.txt     # License information
├── README.md       # Project documentation
```

## Prerequisites
- .NET SDK 6.0 or later
- A code editor (Visual Studio, VS Code, or Rider)

## Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/your-username/safe-chat.git
   ```
2. Navigate to the project directory:
   ```sh
   cd safe-chat
   ```
3. Build the solution:
   ```sh
   dotnet build
   ```

## Usage
### Running the Server
```sh
dotnet run --project Server/Server.csproj
```

### Running the Client
```sh
dotnet run --project Client/Client.csproj
```

## License
This project is licensed under the terms of the MIT License. See `LICENSE.txt` for more details.

## Contributing
Contributions are welcome! Feel free to fork the repository and submit a pull request.

## Contact
For issues and feature requests, please open an issue in the repository.
