using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace Tests
{
    [TestClass]
    public class ServicesTest
    {
        public static int PORT_NO = 5000;
        public static string SERVER_IP = "127.0.0.1";

        public TcpClient tcpClient { get; set; } = new TcpClient();
        private static readonly List<User> _userTestData = new List<User>();

        public readonly IServiceRepository _repository;

        public ServicesTest()
        {
            _userTestData.Add(new User()
            {
                Client = tcpClient,
                Name = "Lucas",
                RoomName = "Take",
                Id = 1
            });

            _userTestData.Add(new User()
            {
                Client = tcpClient,
                Name = "John",
                RoomName = "Take",
                Id = 2
            });

            _userTestData.Add(new User()
            {
                Client = tcpClient,
                Name = "John",
                RoomName = "Take",
                Id = 3
            });

            _repository = new ServiceRepository();

        }

        [TestMethod]
        public void IntegrationTest()
        {
            TestConnection();
            CanICreateARoom();
            CanISendADirectMessage();
            CanISendAPrivateMessage();
            CanIGetListOfConnectedUsers();
            CanIGetRooms();
        }


        public void CanICreateARoom()
        {
            //Act
            _repository.InicialRoom();
        }

        public void TestConnection()
        {
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);
            ServerSocket.Start();
            tcpClient.Connect(SERVER_IP, PORT_NO);
            tcpClient = ServerSocket.AcceptTcpClient();

            NetworkStream stream = tcpClient.GetStream();

            Assert.IsTrue(stream != null, "Connected");

        }


        public void CanISendADirectMessage()
        {
            // Arrange
            string data = "to/ John teste";
            _userTestData[0].To = "John";

            //Act
            _repository.DirectionMessage(data, _userTestData[0], _userTestData.ToList());

            //Asset
            Assert.IsTrue(_userTestData[0].To != null, "Message Sent");
        }

        public void CanISendAPrivateMessage()
        {
            string message = "p/ John teste";
            _userTestData[0].To = "John";

            _repository.PrivateMessage(message, _userTestData[0], _userTestData.ToList());
        }

        public void CanIGetListOfConnectedUsers()
        {
            _repository.GetConnectedUsers(_userTestData[0].Client, _userTestData.ToList());
            Assert.IsTrue(true);
        }

        public void CanIGetRooms()
        {
            _repository.GetRooms(_userTestData[0].Client, _userTestData.ToList());
            Assert.IsTrue(true);
        }

    }
}
