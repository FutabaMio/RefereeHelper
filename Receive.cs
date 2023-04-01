using System;
using System.Net.Sockets;

public class UdpReceive
{
    public TimeOnly time;
    UdpClient udpClient = new UdpClient(27069);
    UdpReceiveResult result;
    byte[] datagram;
    string received;
    int secondOfDifference = 5;
    timeOfDifference = new (0, 0, secondOfDifference);
    
	public UdpReceive(int port)
	{
		
	}
    async Task<string> Receive()
    {
        result = await udpClient.ReceiveAsync();
        datagram = result.Buffer;
        received = Encoding.UTF8.GetString(datagram);
        time = TimeOnly.FromDateTime(DateTime.Now);
        received = received.Substring(received.IndexOf("Tag:") + 4);
        received = received.Substring(0, received.IndexOf(" "));
    }
}
public class Processing
{
    public Processing()
    {

    }
}