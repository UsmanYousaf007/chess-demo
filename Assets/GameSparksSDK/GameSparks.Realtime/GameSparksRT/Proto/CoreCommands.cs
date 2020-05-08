﻿// Classes and structures being serialized

// Generated by ProtocolBuffer
// - a pure c# code generation implementation of protocol buffers
// Report bugs to: https://silentorbit.com/protobuf/

// DO NOT EDIT
// This file will be overwritten when CodeGenerator is run.
// To make custom modifications, edit the .proto file and add //:external before the message line
// then write the code and the changes in a separate file.
using System;
using System.Collections.Generic;

namespace Com.Gamesparks.Realtime.Proto
{
    internal partial class LoginCommand
    {
        public string Token { get; set; }

		public int? ClientVersion { get; set; }

    }

    internal partial class LoginResult
    {
        public bool Success { get; set; }

        public string ReconnectToken { get; set; }

        public int? PeerId { get; set; }

		public List<int> ActivePeers { get; set; }

		public int? FastPort { get; set; }

    }

    internal partial class PingCommand
    {
    }

    internal partial class PingResult
    {
    }

    internal partial class UDPConnectMessage
    {
    }

    internal partial class PlayerConnectMessage
    {
        public int PeerId { get; set; }

        public List<int> ActivePeers { get; set; }

    }

    internal partial class PlayerDisconnectMessage
    {
        public int PeerId { get; set; }

        public List<int> ActivePeers { get; set; }

    }

}
